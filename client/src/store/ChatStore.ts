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
import HubConnectionMapping from "@/interfaces/HubConnectionMapping";
import UserStore from "@/store/UserStore"
import Vue from "vue"

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
  defaultMessageAmount = 13;
  chats: Chat[] = [];
  chatHubConnectionMappings: HubConnectionMapping[] = [];


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
  private GET_CHATS(payload: any) {
    this.chats = payload.map((d: any) => Chat.fromApi(d));
  }

  @Action
  async getChatMessages(payload: { chatId: string, startingIndex: number}) {
    const path = `${ChatStoreModule.apiPath}/messages?chatId=${payload.chatId}&startingIndex=${payload.startingIndex}&amount=${ChatStore.defaultMessageAmount}`;

    try {
      const response = await ApiService.get(path);
      this.GET_CHAT_MESSAGES(response.data);
    } catch (error) {
      console.error(error);
    }
  }

  @Mutation
  GET_CHAT_MESSAGES(payload: Message[]) {
    const chatId = payload.length > 0 ? payload[0].sentToChatId : null;
    const chatToUpdate = this.chats.find(c => c.id === chatId) ?? null;
    if (chatToUpdate) {
      payload.forEach(mess => {
        if (!chatToUpdate.messages.find(m => m.id === mess.id)) {
          chatToUpdate.messages.push(Message.fromApi(mess))
        }
      });
    }
  }


  @Action
  async createOrUpdateChat(payload: Chat) {
    const path = ChatStoreModule.apiPath + "/createOrUpdate";
    try {
      const response = await ApiService.post<Chat>(path, payload);
      // this.CREATE_OR_UPDATE_CHAT(response.data);
    } catch (error) {
      
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
  updateChatUsers(hubResponse: HubResponse<Chat>) {
    this.UPDATE_CHAT_USERS(hubResponse.data);
  }

  @Mutation
  UPDATE_CHAT_USERS(payload: Chat) {
    const chat = this.chats.find(c => c.id === payload.id)
    if (!chat) {
      this.chats.push(Chat.fromApi(payload));
      Vue.prototype.$chatHub.connection.invoke("JoinChat", payload.id)
      return
    } 

    const userId = UserStore.me?.id
    const chatIndex = this.chats.findIndex(c => c.id === payload.id) 
    if (userId && payload.chatUserIds.includes(userId)) {
      this.chats.splice(chatIndex, 1, Chat.fromApi(payload));
    }
    if (userId && !payload.chatUserIds.includes(userId)) {
      this.chats.splice(chatIndex, 1)
      Vue.prototype.$chatHub.connection.invoke("LeaveChat", payload.id)
    }

  }

  @Action
  async createMessage(payload: Message) {
    const path = ChatStoreModule.apiPath + "/createMessage";

    try {
      const chat = this.chats.find(c => c.id === payload.sentToChatId);
      if (!chat) return
      // Push the payload to the chat messages array as pending message 
      const messageIndex = chat.messages.findIndex(m => m.pendingId === payload.pendingId)
      if (messageIndex > -1) {
        chat.messages.splice(messageIndex, 1, payload)
      } else {
        chat.messages.push(payload);
      }

      const response = await ApiService.post<Chat>(path, payload);

    } catch (error) {
      console.error(error);
    }
  }

  @Action
  broadcastChatMessage(hubResponse: HubResponse<Message>) {
    // console.log("broadcastChatMessage", hubResponse.data)
    this.BOARDCAST_CHAT_MESSAGE(hubResponse.data);
  }

  @Mutation
  BOARDCAST_CHAT_MESSAGE(payload: Message) {
    const chat = this.chats.find(c => c.id === payload.sentToChatId);
    if (!chat) {
      return
    }

    const pendingMessageIndex = chat.messages.findIndex(m => m.pendingId === payload.pendingId) 

    const updatedMessage = Message.fromApi(payload)
    updatedMessage.pendingId = null // The message has been succesfully, set pending id to null here
    if (pendingMessageIndex > -1) {
      chat.messages.splice(pendingMessageIndex, 1, updatedMessage)
    } else {
      chat.messages.push(updatedMessage);
    }
  }

  @Action
  showFailedChatMessageToSender(hubResponse: HubResponse<Message>) {
    this.SHOW_FAILED_CHAT_MESSAGE(hubResponse.data);
  }

  @Mutation
  SHOW_FAILED_CHAT_MESSAGE(payload: Message) {
    const chat = this.chats.find(c => c.id === payload.sentToChatId);
    if (!chat) {
      return
    }

    // TODO: save failed message to somewhere that can be resent later, 
    // right now failed messages are only preserved in Vuex store, meaning if if user refresh they won't see them can cannot "Retry" anymore
    const failedMessageIndex = chat.messages.findIndex(m => m.pendingId === payload.pendingId) 
    if (failedMessageIndex > -1) {
      chat.messages.splice(failedMessageIndex, 1, Message.fromApi(payload))
    } 
  }

  @Action
  addConnectionMappings(hubResponse: HubResponse<HubConnectionMapping[]>) {
    this.ADD_CONNECTION_MAPPINGS(hubResponse.data);
  }

  @Mutation
  ADD_CONNECTION_MAPPINGS(payload: HubConnectionMapping[]) {
    console.log("ADD_CONNECTION_MAPPINGS", payload)
    payload.forEach(connectionMapping => {
      const exisingConnectionMapping = this.chatHubConnectionMappings
        .find(cm => cm.connectionId === connectionMapping.connectionId && cm.email === connectionMapping.email);
      if (!exisingConnectionMapping) {
        this.chatHubConnectionMappings.push(connectionMapping);
      }
    })
  }

  @Action
  removeConnectionMappings(hubResponse: HubResponse<HubConnectionMapping[]>) {
    this.REMOVE_CONNECTION_MAPPINGS(hubResponse.data)
  }

  @Mutation
  REMOVE_CONNECTION_MAPPINGS(payload: HubConnectionMapping[]) {
    console.log("REMOVE_CONNECTION_MAPPINGS", payload)
    payload.forEach(connectionMapping => {
      const exisingConnectionMappingIndex = this.chatHubConnectionMappings
        .findIndex(cm => cm.connectionId === connectionMapping.connectionId && cm.email === connectionMapping.email);
      if (exisingConnectionMappingIndex > -1) {
        this.chatHubConnectionMappings.splice(exisingConnectionMappingIndex, 1)
      }
    })
  }

  @Action
  updateMessagingStatus(hubResponse: HubResponse<{chatId: string, incoming: boolean}>) {
    this.UPDATE_MESSAGING_STATUS(hubResponse.data);
  }

  @Mutation
  UPDATE_MESSAGING_STATUS(payload: {chatId: string, incoming: boolean}) {
    // console.log("UPDATE_MESSAGING_STATUS", payload)
    const chat = this.chats.find(c => c.id === payload.chatId);
    if (chat) {
      chat.messageIncoming = payload.incoming
    }
  }

  @Action
  async markMessagesAsSeen(payload: {chatId: string, messagesIds: number[]}) {
    const path = `${ChatStoreModule.apiPath}/markMessagesAsSeen/${payload.chatId}`;

    try {
      const response = await ApiService.post<Message[]>(path, payload.messagesIds)
    } catch (error) {
      console.error(error);
    }
  }

  @Action
  updateMessagesSeenByUsers(hubResponse: HubResponse<Message[]>) {
    this.UPDATE_MESSAGES_SEEN_BY_USERS(hubResponse.data);
  }

  @Mutation
  UPDATE_MESSAGES_SEEN_BY_USERS(payload: Message[]) {
    // console.log("UPDATE_MESSAGES_SEEN_BY_USERS", payload)
    const chat = this.chats.find(c => c.id === payload[0].sentToChatId);
    if (!chat) {
      return
    }

    payload.forEach(m => {
      const message = chat.messages.find(mes => m.id === mes.id)
      if (!message) return
      message.seenByUserIds.splice(0, message.seenByUserIds.length);
      message.seenByUserIds = [...m.seenByUserIds];
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
  {name: "UpdateChatUsers", handler: ChatStore.updateChatUsers},
  {name: "BroadcastChatMessage", handler: ChatStore.broadcastChatMessage},
  {name: "ShowFailedChatMessageToSender", handler: ChatStore.showFailedChatMessageToSender},
  {name: "AddConnectionMappings", handler: ChatStore.addConnectionMappings},
  {name: "RemoveConnectionMappings", handler: ChatStore.removeConnectionMappings},
  {name: "UpdateMessagingStatus", handler: ChatStore.updateMessagingStatus},
  {name: "UpdateMessagesSeenByUsers", handler: ChatStore.updateMessagesSeenByUsers}
]

export default ChatStore;