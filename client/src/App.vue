<template>
  <v-app id="app">
    <LoadingIndicator></LoadingIndicator>
    <router-view/>
  </v-app>
</template>

<script lang="ts">
import { Vue, Component, Watch } from "vue-property-decorator"
import User from "./models/User";
import { Routes } from "./router/Routes";
import AuthStore from "./store/AuthStore";
import UserStore from "./store/UserStore";
import LoadingIndicator from "@/components/LoadingIndicator.vue";

@Component({
  name: "App",
  components: {
    LoadingIndicator
  }
})
export default class App extends Vue {
  @Watch("isAuthenticated") 
  async onIsAuthenticatedChange(newVal: boolean) {
    if (newVal) {
      console.log("Signin redirecting....")
      this.$router.replace({name: Routes.Home.name})
      await this.fetchAllData();
    }
  }

  get isAuthenticated(): boolean {
    return AuthStore.isAuthenticated;
  }

  get isAuthenticationRedirectInProgress(): boolean {
    return AuthStore.isAuthenticationRedirectInProgress;
  }

  async fetchAllData() {
    if (AuthStore.isAuthenticated) {
      this.$chatHubInstance.start(this.$chatHub.connection)
      // await UserStore.getMe();
      await UserStore.getAllUsers();
    }
  }

  async created() {
    (window as any).__app = this;
    await this.fetchAllData();
  }
}
</script>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  margin: 0;
  padding: 0;
  overflow: auto;

}

#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}


</style>
