<template>
  <v-container fluid class="d-flex align-center pa-0">
      <v-card color="#E3F2FD" height="100%" width="100%" tile class="d-flex flex-column align">
        <LoadingIndicator :loading="loading"/>
        <v-card-text class="mt-auto d-flex flex-column-reverse chat-message-container" style="overflow-y: auto;">
          <div v-for="key in groupedSortedMessagesByDayKeys" :key="key" class="d-flex flex-column-reverse">
            <MessageItem 
              v-for="(message, index) in groupedSortedMessagesByDay[key]" 
              :key="index" 
              :message="message"
              />
            <span class="chat-message-container__date">{{ key }}</span>

          </div>

        </v-card-text>
        
        <v-card-actions style="background: #fff; height: 100px;" class="align-self-stretch">
          <v-row class="pl-3" >
            <v-col sm="11">
              <v-text-field
                placeholder="Type something"
                hide-details
                filled
                outlined
                v-model="message.text"
                @keyup.enter="createMessage"
              ></v-text-field>
            </v-col>
            
            <v-col sm="1" class="d-flex">
              <v-spacer></v-spacer>
              <v-btn icon fab color="primary" @click="createMessage">
                <v-icon size="30">
                  mdi-send
                </v-icon>
                </v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
  </v-container>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import ChatStore from "@/store/ChatStore";
import { Vue, Component, Watch } from "vue-property-decorator"
import ChatCreationDialog, { ChatEditOnlyEnum } from "@/components/ChatCreationDialog.vue";
import { NavigationGuardNext, Route } from "vue-router";
import UserStore from "@/store/UserStore";
import Message from "@/models/Message";
import User from "@/models/User";
import MessageItem from "@/components/Message.vue"
import LoadingIndicator from "@/components/LoadingIndicator.vue"

Component.registerHooks([
  "beforeRouteLeave",
  "beforeRouteUpdate",
  "beforeRouteEnter"
])

@Component({
  name: "ChatView",
  components: {
    ChatCreationDialog,
    MessageItem,
    LoadingIndicator
  }
})
export default class ChatView extends Vue {
  loading: boolean = false;
  message: Message = new Message();

  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get chatIdFromRouteParam() {
    return this.$route.params.chatId ?? null
  }

  @Watch("chatIdFromRouteParam", { immediate: true }) 
  async onChatIdFromRouteChange(newVal: string | null, oldVal: string | null) {
    if (newVal !== null && newVal !== oldVal) {
      await this.initChat();
      this.initMessage();
    }
    
  }
  
  get chat(): Chat | null {
    return this.chatIdFromRouteParam
      ? this.chats.find(c => c.id == this.chatIdFromRouteParam) ?? null
      : null;
  }

  get chatGroupConnectionMappings() {
    return this.chat ? this.chat.chatGroupConnectionMappings : []
  }

  get availableInChatUsers(): User[] {
    return this.chat ? this.chat.availableInChatUsers : []
  }

  get me(): User | null {
    return UserStore.me;
  }

  get sortedMessages(): Message[] {
    return this.chat
      ? this.chat.messages.sort((a: Message, b: Message) => a.sentAt > b.sentAt ? -1 : 1)
      : [];
  }

  get groupedSortedMessagesByDay() {
    return this.sortedMessages.reduce((group: any, message: Message) => {
      const { sentDateAsDay } = message;
      group[sentDateAsDay] = group[sentDateAsDay] ?? [];
      group[sentDateAsDay].push(message);
      return group;
    }, {})
  }

  get groupedSortedMessagesByDayKeys() {
    return Object.keys(this.groupedSortedMessagesByDay);
  }

  async createMessage() {
    this.message.sentAt = new Date();

    console.log("create message", this.message)
    await ChatStore.createMessage(this.message)

    this.initMessage();
  }

  initMessage() {
    this.message = new Message();
    this.message.sentToChatId = this.chat!.id;
    this.message.sentByUserId = this.me?.id ?? null;
  }

  async initChat() {
    this.loading = true;

    if (this.chat && this.chat.messages.length === 0) {
      await ChatStore.getChatMessages({chatId: this.chat.id, startingIndex: 0})
    }

    this.loading = false;
  }

  joinChat(chatId: string) {
    console.log("Joining chat...", chatId)
    this.$chatHub.connection.invoke("JoinChat", chatId)
  }

  leaveChat(chatId: string) {
    if (this.chat && !this.chat.chatHasOnlyOneUser) {
      console.log("Leaving chat...", chatId)
      this.$chatHub.connection.invoke("LeaveChat", chatId)
    }
  }

  beforeRouteEnter(to: Route, from: Route, next: NavigationGuardNext) {
    next(vm => {
      (vm as any).joinChat(to.params.chatId)
    })

  }

  async beforeRouteLeave(to: Route, from: Route, next: NavigationGuardNext) {
    this.leaveChat(from.params.chatId)
    next()
  }

  beforeRouteUpdate(to: Route, from: Route, next: NavigationGuardNext) {
    this.leaveChat(from.params.chatId);
    this.joinChat(to.params.chatId)
    next()
  }

  async created() {
  }
}
</script>

<style lang="scss">
.chat-message-container {
  height: calc(100vh - 100px - 64px);

  &__date {
    // width: 100%;
    margin: auto;
  }
}
</style>