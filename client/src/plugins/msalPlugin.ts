import AuthStore from "@/store/AuthStore";
import UserStore from "@/store/UserStore";
import * as msal from "@azure/msal-browser";
import Vue, { PluginObject, VueConstructor } from "vue";

declare module "vue/types/vue" {
  interface Vue {
    $msal: MsalPlugin;
  }
}



export interface IMsalPluginOptions {
  clientId: string;
  authority: string;
  redirectUri: string; // Full redirect URL, in form of http://localhost:3000
  postLogoutRedirectUri: string;
  cacheLocation: string;
}

export class MsalPluginOptions implements IMsalPluginOptions {
  clientId!: string;
  authority!: string;
  redirectUri!: string;
  postLogoutRedirectUri!: string;
  cacheLocation!: string;

  constructor(data: IMsalPluginOptions) {
    this.clientId = data.clientId;
    this.authority = data.authority;
    this.redirectUri = data.redirectUri;
    this.postLogoutRedirectUri = data.postLogoutRedirectUri;
    this.cacheLocation = data.cacheLocation;
  }
}

export let msalPluginInstance: MsalPlugin;

export class MsalPlugin implements PluginObject<IMsalPluginOptions> {
  private msalInstance!: msal.PublicClientApplication; // Will be used to execute other tasks like sign in, acquire token, etc
  private options!: MsalPluginOptions;

  public isAuthenticated = false;

  public install(vue: VueConstructor<Vue>, options?: IMsalPluginOptions) : void {
    if (!options) {
      throw new Error("MsalPlugin options must be specified");
    }

    this.options = new MsalPluginOptions(options);
    this.initialize(this.options);
    msalPluginInstance = this;

    vue.prototype.$msal = Vue.observable(msalPluginInstance);
  }

  private initialize(options: MsalPluginOptions) {
    const msalConfig: msal.Configuration  = {
      auth: {
        clientId: options.clientId,
        authority: options.authority,
        redirectUri: options.redirectUri,
        postLogoutRedirectUri: options.postLogoutRedirectUri,
      },
      cache: {
        cacheLocation: options.cacheLocation, // This configures where your cache will be stored
        storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
      },
      system: {	
        loggerOptions: {	
          loggerCallback: (level, message, containsPii) => {	
            if (containsPii) {		
              return;		
            }		
            switch (level) {		
              case msal.LogLevel.Error:		
                console.error(message);		
                return;		
              case msal.LogLevel.Info:		
                console.info(message);		
                return;		
              case msal.LogLevel.Verbose:		
                console.debug(message);		
                return;		
              case msal.LogLevel.Warning:		
                console.warn(message);		
                return;		
            }	
          }	
        }	
      }
    };

    this.msalInstance = new msal.PublicClientApplication(msalConfig);
    this.isAuthenticated = this.getIsAuthenticated();
    AuthStore.setMsalAuthenticated(this.isAuthenticated);

    AuthStore.setMsalRedirectProgress(true);
    this.msalInstance.handleRedirectPromise()
      .then(redirectPromiseResponse => {
        if (redirectPromiseResponse !== null) {
          this.isAuthenticated = !!redirectPromiseResponse.account;
          AuthStore.setMsalAuthenticated(this.isAuthenticated);
          UserStore.signInSso();
        }
      })
      .catch(e => console.warn(e))
      .finally(() => {
        AuthStore.setMsalRedirectProgress(false)
        if (this.isAuthenticated) {
          UserStore.getMe()
        }
      })
  }

  public async signIn() {
    try {
      const loginRequest: msal.RedirectRequest = {
        scopes: ["user.read"],
        redirectUri: this.options.redirectUri
      };
      console.log('login request', loginRequest)
      return this.msalInstance.loginRedirect(loginRequest);
    } catch (err) {
      console.error('login request error', err)
    }
  }

  /**
  * Gets a token silently, or falls back to interactive redirect. 
  */
  public async acquireToken() {
    // console.log("Calling MSALplugin acquireToken");
    const apiAccessScope = "api://" + process.env.VUE_APP_MSAL_CLIENT_ID + "/access_as_user"
    const request = {
      account: this.msalInstance.getAllAccounts()[0],
      scopes: [apiAccessScope],
    };
    try {
      const response = await this.msalInstance.acquireTokenSilent(request);
      // console.log("acquireToken response", response)
      return response.accessToken;
    } catch (error) {
      console.log("catch acquireToken", error);
      if (error instanceof msal.InteractionRequiredAuthError) {
        return this.msalInstance.acquireTokenRedirect(request);
      } else {
        console.warn(error);   
      }
    }
  }

  public async signOut() {
    const currentAccount = this.msalInstance.getActiveAccount();
    const request: msal.EndSessionRequest = {
      account: currentAccount,
      postLogoutRedirectUri: this.options.postLogoutRedirectUri,
      onRedirectNavigate: (url: string) => {
        document.location.href = this.options.postLogoutRedirectUri;
        return false;
      },
    }
    return this.msalInstance.logoutRedirect(request);
  }

  public getIsAuthenticated() {
    const accounts: msal.AccountInfo[] = this.msalInstance.getAllAccounts();
    return accounts && accounts.length > 0;
  }
}