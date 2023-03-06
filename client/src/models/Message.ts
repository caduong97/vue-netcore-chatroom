import UserStore from "@/store/UserStore";
import User from "@/models/User";
import moment from "moment";

export enum MessageSavingStatusEnum
{
  Success = 0,
  Pending,
  Failed
} 
export default class Message {
  id: number = 0;
  text: string = "";
  sentAt: Date = new Date();
  archivedAt: Date | null = null;
  sentByUserId: string | null = null;
  sentToChatId: string | null = "";
  
  pendingId: string | null = null;
  savingStatus: MessageSavingStatusEnum = MessageSavingStatusEnum.Success;

  public static fromApi(data: Message) {
    const message = new Message();
    message.id = data.id;
    message.text = data.text;
    message.sentAt = new Date(data.sentAt);
    message.archivedAt = data.archivedAt ? new Date(data.archivedAt) : null;
    message.sentByUserId = data.sentByUserId;
    message.sentToChatId = data.sentToChatId;
    message.pendingId = data.pendingId;
    message.savingStatus = data.savingStatus;

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

  get pending(): boolean {
    return this.pendingId !== null 
  }
  
  get failedToSave(): boolean {
    return this.savingStatus === MessageSavingStatusEnum.Failed
  }

  get sent(): boolean {
    return this.savingStatus === MessageSavingStatusEnum.Success
  }

  get sentDateFormatted() {
    return this.sentAt ? moment(this.sentAt).format('ddd Do MMM hh:mm:ss') : ""
  }

  get sentDateAsDay() {
    return this.sentAt ? moment(this.sentAt).format('ddd Do MMM') : ""
  }
 }