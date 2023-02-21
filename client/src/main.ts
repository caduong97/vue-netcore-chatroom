import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'
import { ApiService } from './services/ApiService'
import { IMsalPluginOptions, MsalPlugin } from './plugins/msalPlugin'
import { FirebaseAuthPlugin, IFirebaseAuthPluginOptions } from './plugins/firebaseAuthPlugin'
import ChatHub from './hubs/ChatHub'

Vue.config.productionTip = false

const msalPluginOptions: IMsalPluginOptions = {
  clientId: process.env.VUE_APP_MSAL_CLIENT_ID,
  authority: "https://login.microsoftonline.com/" + process.env.VUE_APP_MSAL_TENANT_ID,
  cacheLocation: "localStorage",
  redirectUri: window.location.origin,
  postLogoutRedirectUri: window.location.origin
}
Vue.use(new MsalPlugin(), msalPluginOptions)

const firebaseConfig: IFirebaseAuthPluginOptions = {
  apiKey: process.env.VUE_APP_FIREBASE_API_KEY,
  authDomain: process.env.VUE_APP_FIREBASE_AUTH_DOMAIN,
  projectId: process.env.VUE_APP_FIREBASE_PROJECT_ID,
  storageBucket: process.env.VUE_APP_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: process.env.VUE_APP_FIREBASE_MESSAGING_SENDER_ID,
  appId: process.env.VUE_APP_FIREBASE_APP_ID,
};

Vue.use(new FirebaseAuthPlugin(), firebaseConfig)

ApiService.ConfigureInterceptors();

Vue.use(ChatHub);

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
