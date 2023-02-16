<template>
  <v-container fluid class="d-flex align-center">
    <template v-if="chat && chat.chatHasOnlyOneUser">
      <v-row>
        <v-col cols="12">
          <div class="text-p1 text-center">There are no people in this chat</div>
        </v-col>
        <v-btn 
          color="#1A237E"
          elevation="4"
          class="mx-auto"
          dark
          @click="addPeopleToChat"
        >Add people to chat</v-btn>
      </v-row>

      <ChatCreationDialog></ChatCreationDialog>
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
import ChatCreationDialog from "@/components/ChatCreationDialog.vue";

@Component({
  name: "ChatView",
  components: {
    ChatCreationDialog
  }
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
    this.$root.$emit("openCreateChatDialog", this.chatIdFromRouteParam)
  }
}
</script>

<style>

</style>