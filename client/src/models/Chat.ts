import Message from "./Message";

export default class Chat {
  id: string = "";
  name: string = "";
  createdAt: Date = new Date();
  updatedAt: Date = new Date();
  chatUserIds: string[] = [];
  
  // Messages are included inside Chat model instead of being loaded independently
  // to avoid the need to map messages to chats on the frontend, which can lead to perf issue.
  messages: Message[] = [];

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
}