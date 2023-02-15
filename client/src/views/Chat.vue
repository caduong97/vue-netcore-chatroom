<template>
  <v-container fluid class="d-flex align-center">
    <template v-if="chat && chat.chatHasOnlyOneUser">
      <v-row>
        <v-btn 
          color="primary"
          elevation="4"
          class="mx-auto"
          @click="addPeopleToChat"
        >Add people to chat</v-btn>
      </v-row>
    </template>

    <template v-else></template>
  </v-container>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import User from "@/models/User";
import AuthStore from "@/store/AuthStore";
import ChatStore from "@/store/ChatStore";
import UserStore from "@/store/UserStore";
import { Vue, Component } from "vue-property-decorator"

@Component({
  name: "ChatView"
})
export default class ChatView extends Vue {
  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get chatIdFromRouteParam() {
    return this.$route.params.chatId ?? null
  }

  get chat(): Chat | null {
    return this.chatIdFromRouteParam
      ? this.chats.find(c => c.id == this.chatIdFromRouteParam) ?? null
      : null;
  }

  addPeopleToChat() {

  }
}
</script>

<style>

</style>