import AuthStore from "@/store/AuthStore";
import UserStore from "@/store/UserStore";
import { initializeApp } from "@firebase/app";
import * as firebase from "firebase/auth";
import Vue, { PluginObject, VueConstructor } from "vue";

declare module "vue/types/vue" {
  interface Vue {
    $firebaseAuth: FirebaseAuthPlugin;
  }
}

export interface IFirebaseAuthPluginOptions {
  apiKey: string;
  authDomain: string;
  projectId: string;
  storageBucket: string;
  messagingSenderId: string;
  appId: string;
}

export class FirebaseAuthPluginOptions implements IFirebaseAuthPluginOptions {
  apiKey: string;
  authDomain: string;
  projectId: string;
  storageBucket: string;
  messagingSenderId: string;
  appId: string;

  constructor(data: IFirebaseAuthPluginOptions) {
    this.apiKey = data.apiKey;
    this.authDomain = data.authDomain;
    this.projectId = data.projectId;
    this.storageBucket = data.storageBucket;
    this.messagingSenderId = data.messagingSenderId;
    this.appId = data.appId;
  }
}

export let firebaseAuthPluginInstance: FirebaseAuthPlugin;


export class FirebaseAuthPlugin implements PluginObject<FirebaseAuthPluginOptions> {
  private options!: FirebaseAuthPluginOptions;
  public isAuthenticated = false;
  
  public install(vue: VueConstructor<Vue>, options?: IFirebaseAuthPluginOptions) : void {
    if (!options) {
      throw new Error("FirebaseAuthPluginOptions must be specified");
    }

    this.options = new FirebaseAuthPluginOptions(options);
    initializeApp(this.options);

    this.setIsAuthenticated();
    AuthStore.setGoogleRedirectProgress(true);

    const auth = firebase.getAuth();
    firebase.getRedirectResult(auth).then(redirectPromiseResponse => {
      if (redirectPromiseResponse !== null) {
        UserStore.signInSso();
        AuthStore.setGoogleAuthenticated(this.isAuthenticated);
      }
    })
    .catch(e => console.warn(e))
    .finally(() => {
      AuthStore.setGoogleRedirectProgress(false)
      if (this.isAuthenticated) {
        UserStore.getMe()
      }
    })

    firebaseAuthPluginInstance = this;

    vue.prototype.$firebaseAuth = Vue.observable(firebaseAuthPluginInstance);

  }

  public async signInWithGoogle() {
    const provider = new firebase.GoogleAuthProvider();
    provider.setCustomParameters({
      prompt: 'select_account'
    });

    const auth = firebase.getAuth();

    try {
      await firebase.signInWithRedirect(auth, provider)
    } catch (error: any) {
      const errorMessage = error.message;
      console.error("Error signing in with Google:", errorMessage)
      alert("Error signing in with Google: " + errorMessage)
      return false
    }
  }

  public async  signOutWithGoogle() {
    const auth = firebase.getAuth();
    try {
      await firebase.signOut(auth)
    } catch (err: any) {
      console.error("Error signout Google account:", err.message)
      alert("Error signout Google account: " + err.message)
    } 
  }

  public async getUserIdToken () {
    let token = null;
    const user = firebase.getAuth().currentUser;
    if (user) {
      token = await user.getIdToken();
    }
    return token ? token : ""
  }

  public setIsAuthenticated() {
    firebase.getAuth().onAuthStateChanged((user: any) => {
      if (user) {
        this.isAuthenticated = true;
      }
      else {
        this.isAuthenticated = false
      }
      AuthStore.setGoogleAuthenticated(this.isAuthenticated); 
    })
  }
}