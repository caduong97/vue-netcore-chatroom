export default class Message {
  id: number = 0;
  text: string = "";
  sentAt: Date = new Date();
  archivedAt: Date | null = null;
  sentByUserId: string | null = null;

  public static fromApi(data: Message) {
    const message = new Message();
    message.id = data.id;
    message.text = data.text;
    message.sentAt = new Date(data.sentAt);
    message.archivedAt = data.archivedAt ? new Date(data.archivedAt) : null;
    message.sentByUserId = data.sentByUserId;

    return message;
  }
}