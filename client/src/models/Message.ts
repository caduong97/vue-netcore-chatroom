import UserStore from "@/store/UserStore";
import User from "@/models/User";
import moment from "moment";

export default class Message {
  id: number = 0;
  text: string = "";
  sentAt: Date = new Date();
  archivedAt: Date | null = null;
  sentByUserId: string | null = null;
  sentToChatId: string = "";

  public static fromApi(data: Message) {
    const message = new Message();
    message.id = data.id;
    message.text = data.text;
    message.sentAt = new Date(data.sentAt);
    message.archivedAt = data.archivedAt ? new Date(data.archivedAt) : null;
    message.sentByUserId = data.sentByUserId;
    message.sentToChatId = data.sentToChatId;

    return message;
  }

  get me(): User | null {
    return UserStore.me;
  }

  get incoming(): boolean {
    return this.me === null || this.sentByUserId !== this.me.id;
  }

  get outgoing(): boolean {
    return !this.incoming;
  }

  get sentDateFormatted() {
    return moment(this.sentAt).format('ddd Do MMM hh:mm:ss')
  }

  get sentDateAsDay() {
    return moment(this.sentAt).format('ddd Do MMM')
  }
 }