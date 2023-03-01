import HubConnectionMapping from "@/interfaces/HubConnectionMapping";

export default class ChatGroupConnectionMapping implements HubConnectionMapping {
  connectionId: string = "";
  email: string = "";
  chatId: string = "";

  constructor(cm: {connectionId: string, email: string, chatId: string}) {
    this.connectionId = cm.connectionId;
    this.email = cm.email;
    this.chatId = cm.chatId;
  }
}