import AuthenticationRequest from "@/models/AuthenticationRequest";
import AuthenticationResponse from "@/models/AuthenticationResponse";
import RegisterPasswordUserRequest from "@/models/RegisterPasswordUserRequest";
import { ApiService } from "@/services/ApiService";
import { Module, VuexModule, Mutation, Action, getModule } from "vuex-module-decorators";
import store from ".";

export interface IAuthStoreState {
  isAuthenticated: boolean;
}

// Hack due to issues
// https://github.com/championswimmer/vuex-module-decorators/issues/131
// https://github.com/championswimmer/vuex-module-decorators/issues/189
const name = 'auth';
if (store && store.state[name]) {
  try{
    store.unregisterModule(name);
  } catch (e){
    console.warn("Unregister store module workaround error, ignoring ...")
  }
}

@Module({ dynamic: true, store: store, name: 'auth' })
export class AuthStoreModule extends VuexModule implements IAuthStoreState {
  static apiPath = "/auth"
  msalAuthenticated: boolean = false;
  msalRedirectInProgress: boolean = false;
  googleAuthenticated: boolean = false;
  googleRedirectInProgress: boolean = false;

  get isAuthenticated(): boolean {
    return this.msalAuthenticated || this.googleAuthenticated;
  }

  get isAuthenticationRedirectInProgress(): boolean {
    return this.msalRedirectInProgress || this.googleRedirectInProgress;
  }

  @Mutation
  private SET_MSAL_AUTHENTICATED(val: boolean) {
    this.msalAuthenticated = val;
  }

  @Action
  public setMsalAuthenticated(val: boolean) {
    this.SET_MSAL_AUTHENTICATED(val);
  }

  @Mutation
  private SET_MSAL_REDIRECT_PROGRESS(val: boolean) {
    this.msalRedirectInProgress = val;
  }

  @Action
  public setMsalRedirectProgress(val: boolean) {
    this.SET_MSAL_REDIRECT_PROGRESS(val)
  }

  @Mutation
  private SET_GOOGLE_AUTHENTICATED(val: boolean) {
    this.googleAuthenticated = val;
  }

  @Action
  public setGoogleAuthenticated(val: boolean) {
    this.SET_GOOGLE_AUTHENTICATED(val);
  }

  @Mutation
  private SET_GOOGLE_REDIRECT_PROGRESS(val: boolean) {
    this.googleRedirectInProgress = val;
  }

  @Action
  public setGoogleRedirectProgress(val: boolean) {
    this.SET_GOOGLE_REDIRECT_PROGRESS(val)
  }

  @Mutation
  private SAVE_ACCESS_TOKEN(authenticationResponse: AuthenticationResponse) {
    console.log("SAVE_ACCESS_TOKEN", authenticationResponse);
  }

  @Action
  public async signIn(authenticateRequest: AuthenticationRequest) {
    try {
      const response = await ApiService.post<AuthenticationResponse>("/auth/signIn", authenticateRequest);
      this.SAVE_ACCESS_TOKEN(response.data);
    } catch (error) {
      console.error("Error when signing in", error);
      alert("Error when signing in: " + error);
    } 
  }

  @Action
  public async register(registerRequest: RegisterPasswordUserRequest) {
    try {
      const response = await ApiService.post<AuthenticationResponse>("/auth/register", registerRequest);
      this.SAVE_ACCESS_TOKEN(response.data);
    } catch (error) {
      console.error("Error when registering", error);
      alert("Error when registering: " + error);
    } 
  }
  
}

const AuthStore = getModule(AuthStoreModule);
export default AuthStore;