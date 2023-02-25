<template>
  <v-container fluid class="d-flex align-center pa-0">
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
    </template>

    <template v-else>
      <ChatConversation
        :chat="chat"
      />
    </template>
  </v-container>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import ChatStore from "@/store/ChatStore";
import { Vue, Component, Watch } from "vue-property-decorator"
import ChatCreationDialog, { ChatEditOnlyEnum } from "@/components/ChatCreationDialog.vue";
import ChatConversation from "@/components/ChatConversation.vue"

@Component({
  name: "ChatView",
  components: {
    ChatCreationDialog,
    ChatConversation
  }
})
export default class ChatView extends Vue {
  loading: boolean = false;


  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get chatIdFromRouteParam() {
    return this.$route.params.chatId ?? null
  }

  @Watch("chatIdFromRouteParam") 
  onChatIdFromRouteChange() {
    console.log("onChatIdFromRouteChange")
  }

  get chat(): Chat | null {
    return this.chatIdFromRouteParam
      ? this.chats.find(c => c.id == this.chatIdFromRouteParam) ?? null
      : null;
  }

  addPeopleToChat() {
    this.$root.$emit("openCreateChatDialog", {chatId: this.chatIdFromRouteParam, editOnly: ChatEditOnlyEnum.ChatUsers})
  }

  initChat() {
    this.loading = true;

    this.loading = false;
  }

  created() {
    console.log("created")
  }
}
</script>

<style>

</style>