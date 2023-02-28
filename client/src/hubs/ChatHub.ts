import { HubConnection, HubConnectionBuilder, IHttpConnectionOptions, LogLevel } from '@microsoft/signalr'
import { msalPluginInstance } from '@/plugins/msalPlugin';
import AuthStore from '@/store/AuthStore';
import { chatHubMethodHandlers } from '@/store/ChatStore';
import { HubResponse } from '@/interfaces/HubResponse';
import { firebaseAuthPluginInstance } from '@/plugins/firebaseAuthPlugin';

declare module "vue/types/vue" {
  interface Vue {
    $chatHub: any;
  }
}

export default {

  async acquireToken() {
    let token = ""

    if (AuthStore.googleAuthenticated) {
      token = await firebaseAuthPluginInstance.getUserIdToken()
    }
    if (AuthStore.msalAuthenticated) {
        const msalTokenResult = await msalPluginInstance.acquireToken();
        token = msalTokenResult ? msalTokenResult : ""
    }

    return token
  },

  install (vue: any) {
    const hubUrl = window.location.origin + "/hubs/chat";

    const options: IHttpConnectionOptions = {
      accessTokenFactory: this.acquireToken,
    }

    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, options)
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    connection.serverTimeoutInMilliseconds = 120000;

    const chatHub = new vue({ data: {connection}});
    // Vue components call this.$chatHub to access this hub
    vue.prototype.$chatHub = chatHub;

    // Set up handlers for ChatHub methods defined in the ChatStore
    for(const method of chatHubMethodHandlers) {
        connection.on(method.name, (data: HubResponse<any>) => {
          method.handler.call(this, data); 
        })
    }

    connection.onclose(() => {
      console.log("ChatHub connection onclose")
      this.start(connection)
    })

    connection.onreconnecting(error => {
      console.log('Reconnecting interval', error);
    });
    
    vue.prototype.$projectHubInstance = this;

    (window as any).signalR = connection;
    
    // TODO: Decide if start connection will be called later, 
    // due to the fact that the authentication can take longer, and this.start should wait until after user has authenticated
    this.start(connection);
  },

  start (connection: HubConnection) {
    if (connection.state === "Connected") {
        return
    }
    let startedPromise: any = null;
    startedPromise = connection.start().catch((err: any) => {
        console.error('Failed to connect with hub', err)
        return new Promise((resolve, reject) => 
        setTimeout(() => this.start(connection).then(resolve).catch(reject), 5000))
    })
    return startedPromise
  } 

}