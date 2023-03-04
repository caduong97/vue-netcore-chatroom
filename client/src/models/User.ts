import HubConnectionMapping from "@/interfaces/HubConnectionMapping";
import ChatStore from "@/store/ChatStore";

export interface IUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  location: string;
  bio: string;
  archived: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export default class User {
  id: string = "";
  email: string = "";
  firstName: string = "";
  lastName: string = "";
  location: string = "";
  bio: string = "";
  archived: boolean = false
  createdAt: Date = new Date();
  updatedAt: Date = new Date;

  public static fromApi (data: IUser): User {
    const user = new User();

    user.id = data.id;
    user.email = data.email;
    user.firstName = data.firstName;
    user.lastName = data.lastName;
    user.location = data.location;
    user.bio = data.bio;
    user.archived = data.archived;
    user.createdAt = data.createdAt;
    user.updatedAt = data.updatedAt;
    
    return user;
  }

  public static copyFrom(data: User): User {
    const user = new User();

    user.id = data.id;
    user.email = data.email;
    user.firstName = data.firstName;
    user.lastName = data.lastName;
    user.location = data.location;
    user.bio = data.bio;
    user.archived = data.archived;
    user.createdAt = data.createdAt;
    user.updatedAt = data.updatedAt;
    
    return user;
  }

  get initials(): string {
    const firstNameInitial = this.firstName ? this.firstName.slice(0,1) : "-";
    const lastNameInitial = this.lastName ? this.lastName.slice(0,1) : "-";
    return firstNameInitial + lastNameInitial;
  }

  get fullName(): string {
    const firstName = this.firstName ?? "";
    const lastName = this.lastName ?? "";

    return `${firstName} ${lastName}`
  }

  get userChatHubConnectionMappings(): HubConnectionMapping[] {
    return ChatStore.chatHubConnectionMappings.filter(cm => cm.email === this.email);
  }

  get userIsOnline(): boolean {
    return this.userChatHubConnectionMappings.length > 0;
  }
}