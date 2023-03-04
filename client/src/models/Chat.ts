import Message from "./Message";
import { Guid } from "guid-typescript";
import ChatGroupConnectionMapping from "./ChatGroupConnectionMapping";
import ChatStore from "@/store/ChatStore";
import UserStore from "@/store/UserStore";
import User from "./User";

export default class Chat {;
  id: string = Guid.createEmpty().toString();
  name: string = "";
  createdAt: Date = new Date();
  updatedAt: Date = new Date();
  chatUserIds: string[] = [];
  
  // Messages are included inside Chat model instead of being loaded independently
  // to avoid the need to map messages to chats on the frontend, which can lead to perf issue.
  messages: Message[] = [];

  messageIncoming: boolean = false;

  public static fromApi(data: Chat) {
    const chat = new Chat();
    chat.id = data.id;
    chat.name = data.name;
    chat.createdAt = new Date(data.createdAt);
    chat.updatedAt = new Date(data.updatedAt);
    chat.chatUserIds = data.chatUserIds;
    chat.messages = data.messages.map(m => Message.fromApi(m));

    return chat;
  }

  get chatHasOnlyOneUser() {
    return this.chatUserIds.length === 1;
  }
}