import UserStore from "@/store/UserStore";
import User from "@/models/User";
import moment from "moment";
import Chat from "./Chat";
import ChatStore from "@/store/ChatStore";

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
  sentByUserName: string | null = null;
  sentToChatId: string | null = "";
  seenByUserIds: string[] = [];
  
  pendingId: string | null = null;
  savingStatus: MessageSavingStatusEnum = MessageSavingStatusEnum.Success;

  public static fromApi(data: Message) {
    const message = new Message();
    message.id = data.id;
    message.text = data.text;
    message.sentAt = new Date(data.sentAt);
    message.archivedAt = data.archivedAt ? new Date(data.archivedAt) : null;
    message.sentByUserId = data.sentByUserId;
    message.sentByUserName = data.sentByUserName;
    message.sentToChatId = data.sentToChatId;
    message.seenByUserIds = [...data.seenByUserIds]
    message.pendingId = data.pendingId;
    message.savingStatus = data.savingStatus;

    return message;
  }

  get me(): User | null {
    return UserStore.me;
  }

  get chat(): Chat | null {
    return ChatStore.chats.find(c => c.id === this.sentToChatId) ?? null;
  }

  get incoming(): boolean {
    return this.me === null || this.sentByUserId !== this.me.id;
  }

  get outgoing(): boolean {
    return !this.incoming;
  }

  get seen(): boolean {
    return this.outgoing || (this.me !== null && this.sentByUserId !== this.me.id && this.seenByUserIds.includes(this.me.id))
  }

  get seenByOtherUser(): boolean {
    return this.outgoing && this.seenByUserIds.length > 0
  }

  get seenByEveryOne(): boolean {
    return (this.outgoing && this.seenByUserIds.length === this.chat?.chatUserIds.filter(cuid => cuid !== this.me?.id).length) ||
    (this.incoming && this.seenByUserIds.length === this.chat?.chatUserIds.filter(cuid => cuid !== this.sentByUserId).length)
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