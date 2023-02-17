import AuthenticationRequest from "@/models/AuthenticationRequest";
import AuthenticationResponse from "@/models/AuthenticationResponse";
import Chat from "@/models/Chat";
import RegisterPasswordUserRequest from "@/models/RegisterPasswordUserRequest";
import { ApiService } from "@/services/ApiService";
import { Module, VuexModule, Mutation, Action, getModule } from "vuex-module-decorators";
import store from ".";

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

  @Mutation
  private GET_CHATS(data: any) {
    this.chats = data.map((d: any) => Chat.fromApi(d));
  }

  @Action
  async getChats() {
    const path = ChatStoreModule.apiPath;
    try {
      const response = await ApiService.get(path);
      console.log("getchats", response.data)
      this.GET_CHATS(response.data)
    } catch (error) {
      console.error("Error when fetching chats:", error)
    }

  }

  @Mutation
  CREATE_OR_UPDATE_CHAT(data: Chat) {
    const chatIndex = this.chats.findIndex(c => c.id === data.id) 
    if (chatIndex > -1) {
      this.chats.splice(chatIndex, 1, Chat.fromApi(data));
    } else {
      this.chats.push(Chat.fromApi(data));
    }
  }

  @Action
  async createOrUpdateChat(payload: Chat) {
    const path = ChatStoreModule.apiPath + "/createOrUpdate";
    console.log("createOrUpdateChat",path, payload)
    try {
      const response = await ApiService.post<Chat>(path, payload);
      this.CREATE_OR_UPDATE_CHAT(response.data);
    } catch (error) {
      
    }
  }
  
}

const ChatStore = getModule(ChatStoreModule);
export default ChatStore;