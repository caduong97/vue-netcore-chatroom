import User, { IUser } from "@/models/User";
import { ApiService } from "@/services/ApiService";
import { Module, VuexModule, Mutation, Action, getModule } from "vuex-module-decorators";
import store from ".";

export interface IUserStoreState {
  me: User | null;
}

// Hack due to issues
// https://github.com/championswimmer/vuex-module-decorators/issues/131
// https://github.com/championswimmer/vuex-module-decorators/issues/189
const name = 'user';
if (store && store.state[name]) {
  try{
    store.unregisterModule(name);
  } catch (e){
    console.warn("Unregister store module workaround error, ignoring ...")
  }
}

@Module({ dynamic: true, store: store, name: 'user' })
export class UserStoreModule extends VuexModule implements IUserStoreState {
  static apiPath: string = "/user"
  me: User | null = null;
  
  @Mutation
  private SIGN_IN_SSO(data: IUser) {
    this.me = User.fromApi(data);
  }

  @Action
  public async signInSso() {
    const path = UserStoreModule.apiPath + "/signInSso";

    try {
      const response = await ApiService.get(path);
      // console.log("signInSso response:", response);
      this.SIGN_IN_SSO(response.data);
    } catch (error) {
      console.error(error);
      alert("Error in registerSso: " + error)
    }
  }

  @Mutation
  private GET_ME(data: IUser | null) {
    if (data) {
      this.me = User.fromApi(data);
    }
  }

  @Action
  public async getMe() {
    const path = UserStoreModule.apiPath + "/me";

    try {
      const response = await ApiService.get(path);
      this.GET_ME(response.data);
    } catch (error) {
      console.error(error);
      alert("Error in getMe: " + error)
    }
  }

  @Mutation
  private UPDATE_USER(data: User) {
    this.me = User.fromApi(data);
  }

  @Action
  public async updateUser(user: User) {
    const path = UserStoreModule.apiPath + "/update";

    try {
      const response = await ApiService.post<User>(path, user);
      console.log("updateUser response", response)
      this.UPDATE_USER(response.data);
    } catch (error) {
      console.error(error);
      alert("Error in getMe: " + error)
    }

  }
}

const UserStore = getModule(UserStoreModule);
export default UserStore;