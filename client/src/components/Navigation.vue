<template>
  <div>
    <v-app-bar color="primary" elevation="0" app>
      <v-app-bar-nav-icon @click="drawer = !drawer" dark></v-app-bar-nav-icon>

      <div v-if="chat && chatOnlineUsers.length > 0">
        <ChatOnlineUser v-if="chatOnlineUsers.length === 1" :onlineUser="chatOnlineUsers[0]"></ChatOnlineUser>
        <ChatOnlineUser v-else :onlineUserCount="chatOnlineUsers.length" :allOnlineUsers="chatOnlineUsers" ></ChatOnlineUser>
      </div>


      <v-spacer></v-spacer>
      <v-menu offset-y left close-on-content-click transition="slide-y-transition" >
        <template v-slot:activator="{ on, attrs }">
          <v-avatar
            color="#fff"
            size="40"
            v-bind="attrs"
            v-on="on"
          >
            <span v-if="me">{{ me.initials }}</span>
          </v-avatar>
        </template>
        <v-list>
          <v-list-item @click="onSignOutClick">
            <v-list-item-title>Sign out</v-list-item-title>  
          </v-list-item>
          <v-list-item link to="/my-profile">
            <v-list-item-title>Your profile</v-list-item-title>  
          </v-list-item>
        </v-list>
      </v-menu>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" app :temporary="$vuetify.breakpoint.mdAndDown">
      <v-list-item link to="/" :style="$vuetify.breakpoint.smAndDown ? 'height: 56px;': 'height: 64px;'">
        <v-list-item-icon>
          <v-icon size="28">mdi-home</v-icon>
        </v-list-item-icon>
        <v-list-item-content  class="text-left">
          <v-list-item-title class="text-body-1">
            Home
          </v-list-item-title>
          <!-- <v-list-item-subtitle>
            subtext
          </v-list-item-subtitle> -->
        </v-list-item-content>
      </v-list-item>

      <v-divider></v-divider>

      <ChatNavigation></ChatNavigation>

    </v-navigation-drawer>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import User from '@/models/User';
import AuthStore from '@/store/AuthStore';
import UserStore from '@/store/UserStore';
import ChatNavigation from "@/components/ChatNavigation.vue"
import ChatStore from "@/store/ChatStore";
import Chat from "@/models/Chat";
import HubConnectionMapping from "@/interfaces/HubConnectionMapping";
import ChatOnlineUser from "./ChatOnlineUser.vue";

@Component({
  name: "Navigation",
  components: {
    ChatNavigation,
    ChatOnlineUser
  }
})
export default class Navigation extends Vue {
  drawer: boolean = false;

  get me(): User | null {
    return UserStore.me;
  }

  get chatHubConnectionMappings(): HubConnectionMapping[] {
    return ChatStore.chatHubConnectionMappings
      .filter((cm, index, arr) => arr.findIndex(chcm => chcm.email === cm.email) === index)
      .filter(cm => cm.email !== this.me?.email);
  }

  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get chatIdFromRouteParam() {
    return this.$route.params.chatId ?? null
  }
  
  get chat(): Chat | null  {
    return this.chats.find(c => c.id == this.chatIdFromRouteParam) ?? null
  }

  get chatOnlineUsers(): User[] {
    return this.chat
      ? Chat.GetChatOnlineUsers(this.chatHubConnectionMappings, this.chat?.chatUserIds)
      : []
  }

  async onSignOutClick() {
    if (AuthStore.msalAuthenticated) {
      try {
        await this.$msal.signOut();
      } catch (error) {
        console.error(error);
        alert("Error signing out with O365: " + error);
      }
    }
    if (AuthStore.googleAuthenticated) {
      try {
        await this.$firebaseAuth.signOutWithGoogle();
      } catch (error) {
        console.error(error);
        alert("Error signing out with Google: " + error);
      }
    }

    (window as any).location.href = window.location.origin;
  }

  created() {
    if (this.$vuetify.breakpoint.lgAndUp) {
      this.drawer = true;
    }
  }
}
</script>

<style>

</style>