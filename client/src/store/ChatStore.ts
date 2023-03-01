import AuthenticationRequest from "@/models/AuthenticationRequest";
import AuthenticationResponse from "@/models/AuthenticationResponse";
import Chat from "@/models/Chat";
import { HubResponse } from "@/interfaces/HubResponse";
import HubMethodHandler from "@/interfaces/HubMethodHandler";
import RegisterPasswordUserRequest from "@/models/RegisterPasswordUserRequest";
import { ApiService } from "@/services/ApiService";
import { Module, VuexModule, Mutation, Action, getModule } from "vuex-module-decorators";
import store from ".";
import Message from "@/models/Message";
import ChatGroupConnectionMapping from "@/models/ChatGroupConnectionMapping";

export interface IChatStoreState {
}

// Hack due to issues
// https://github.com/championswimmer/vuex-module-decorators/issues/131
// https://github.com/championswimmer/vuex-module-decorators/issues/189
const name = 'chat';
if (store && store.state[name]) {
  try{
    store.unregisterModule(name);
  } catch (e){
    console.warn("Unregister store module workaround error, ignoring ...")
  }
}

@Module({ dynamic: true, store: store, name: 'chat' })
export class ChatStoreModule extends VuexModule implements IChatStoreState {
  static apiPath = "/chat"
  chats: Chat[] = [];
  chatGroupConnectionMappings: ChatGroupConnectionMapping[] = [];

  @Mutation
  private GET_CHATS(payload: any) {
    this.chats = payload.map((d: any) => Chat.fromApi(d));
  }

  @Action
  async getChats() {
    const path = ChatStoreModule.apiPath;
    try {
      const response = await ApiService.get(path);
      // console.log("getchats", response.data)
      this.GET_CHATS(response.data)
    } catch (error) {
      console.error("Error when fetching chats:", error)
    }

  }

  @Mutation
  CREATE_OR_UPDATE_CHAT(payload: Chat) {
    const chatIndex = this.chats.findIndex(c => c.id === payload.id) 
    if (chatIndex > -1) {
      this.chats.splice(chatIndex, 1, Chat.fromApi(payload));
    } else {
      this.chats.push(Chat.fromApi(payload));
    }
  }

  @Action
  async createOrUpdateChat(payload: Chat) {
    const path = ChatStoreModule.apiPath + "/createOrUpdate";
    // console.log("createOrUpdateChat",path, payload)
    try {
      const response = await ApiService.post<Chat>(path, payload);
      this.CREATE_OR_UPDATE_CHAT(response.data);
    } catch (error) {
      
    }
  }

  @Action
  async createMessage(payload: Message) {
    const path = ChatStoreModule.apiPath + "/createMessage";

    try {
      // TODO: add to pending message list
      const response = await ApiService.post<Chat>(path, payload);

    } catch (error) {
      console.error(error);
    }
  }

  @Action
  broadcastChatMessage(hubResponse: HubResponse<Message>) {
    console.log("broadcastChatMessage", hubResponse.data)
    this.BOARDCAST_CHAT_MESSAGE(hubResponse.data);
  }

  @Mutation
  BOARDCAST_CHAT_MESSAGE(payload: Message) {
    const chat = this.chats.find(c => c.id === payload.sentToChatId);
    if (!chat) {
      return
    }
    chat.messages.push(Message.fromApi(payload));
  }

  @Action
  addChatGroupConnectionMappings(hubResponse: HubResponse<ChatGroupConnectionMapping[]>) {
    this.ADD_CHAT_GROUP_CONNECTION_MAPPINGS(hubResponse.data);
  }

  @Mutation
  ADD_CHAT_GROUP_CONNECTION_MAPPINGS(payload: ChatGroupConnectionMapping[]) {
    // console.log("ADD_CHAT_GROUP_CONNECTION_MAPPINGS", payload)
    payload.forEach(cm => {
      const exisingConnectionMapping = this.chatGroupConnectionMappings
        .find(cgcm => cgcm.chatId === cm.chatId && cgcm.connectionId === cm.connectionId && cm.email);
      if (!exisingConnectionMapping) {
        this.chatGroupConnectionMappings.push(cm);
      }
    })
  }

  @Action
  removeChatGroupConnectionMappings(hubResponse: HubResponse<ChatGroupConnectionMapping[]>) {
    this.REMOVE_CHAT_GROUP_CONNECTION_MAPPINGS(hubResponse.data)
  }

  @Mutation
  REMOVE_CHAT_GROUP_CONNECTION_MAPPINGS(payload: ChatGroupConnectionMapping[]) {
    // console.log("REMOVE_CHAT_GROUP_CONNECTION_MAPPINGS", payload)
    payload.forEach(cm => {
      const exisingConnectionMappingIndex = this.chatGroupConnectionMappings
        .findIndex(cgcm => cgcm.chatId === cm.chatId && cgcm.connectionId === cm.connectionId && cm.email);
      if (exisingConnectionMappingIndex > -1) {
        this.chatGroupConnectionMappings.splice(exisingConnectionMappingIndex, 1)
      }
    })
  }
  
  @Action
  receiveMessage(hubResponse: HubResponse<string>) {
    console.log(hubResponse)
  }
}

const ChatStore = getModule(ChatStoreModule);

export const chatHubMethodHandlers: HubMethodHandler[] = [
  {name: "ReceiveMessage", handler: ChatStore.receiveMessage},
  {name: "BroadcastChatMessage", handler: ChatStore.broadcastChatMessage},
  {name: "AddChatGroupConnectionMappings", handler: ChatStore.addChatGroupConnectionMappings},
  {name: "RemoveChatGroupConnectionMappings", handler: ChatStore.removeChatGroupConnectionMappings}

]

export default ChatStore;