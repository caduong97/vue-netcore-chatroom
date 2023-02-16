<template>
  <div>
    <v-app-bar color="primary" elevation="0" app>
      <v-app-bar-nav-icon @click="drawer = !drawer" dark></v-app-bar-nav-icon>
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

    <v-navigation-drawer v-model="drawer" absolute app :temporary="$vuetify.breakpoint.mdAndDown">
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

      <v-list dense nav >
        <v-list-item 
          v-for="chat in chats" :key="chat.id" 
          link
          :to="{name: 'Chat', params: { chatId: chat.id} }"  
        >
          <v-list-item-icon>
            <v-icon>mdi-chat</v-icon>
          </v-list-item-icon>
          <v-list-item-content class="text-left">
            <v-list-item-title>{{ chat.name }}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>

    </v-navigation-drawer>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import User from '@/models/User';
import AuthStore from '@/store/AuthStore';
import UserStore from '@/store/UserStore';
import ChatStore from "@/store/ChatStore";
import Chat from "@/models/Chat";

@Component({
  name: "Navigation"
})
export default class Navigation extends Vue {
  drawer: boolean = true;

  get me(): User | null {
    return UserStore.me;
  }

  get chats(): Chat[] {
    return ChatStore.chats;
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
}
</script>

<style>

</style>