<template>
  
  <v-card color="#E3F2FD" height="100%" width="100%" tile class="d-flex flex-column align">
    <v-card-text class="mt-auto d-flex flex-column-reverse" style="overflow-y: auto;">
      <MessageItem v-for="(message, index) in sortedMessages" :key="index" :message="message"/>
    </v-card-text>
    
    <v-card-actions style="background: #fff; height: 100px;" class="align-self-stretch">
      <v-row class="pl-3" >
        <v-col cols="11">
          <v-text-field
            placeholder="Type something"
            hide-details
            filled
            outlined
            v-model="message.text"
            @keyup.enter="createMessage"
          ></v-text-field>
        </v-col>
        <v-col >
          <v-btn icon fab color="primary" @click="createMessage">
            <v-icon size="30">
              mdi-send
            </v-icon>
            </v-btn>
        </v-col>
      </v-row>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import Message from "@/models/Message";
import User from "@/models/User";
import ChatStore from "@/store/ChatStore";
import UserStore from "@/store/UserStore";
import { Vue, Component, Prop } from "vue-property-decorator"
import MessageItem from "@/components/Message.vue"

@Component({
  name: "ChatConversation",
  components: {
    MessageItem
  }
})
export default class ChatConversation extends Vue {
  textMessage: string = "";
  message: Message = new Message();

  @Prop({ required: true })
  chat!: Chat;

  get me(): User | null {
    return UserStore.me;
  }

  get sortedMessages(): Message[] {
    return this.chat.messages.sort((a: Message, b: Message) => a.sentAt > b.sentAt ? -1 : 1);
  }

  async createMessage() {
    this.message.sentAt = new Date();

    try {
      await ChatStore.createMessage(this.message)
    } catch (error) {
      
    }

    this.initMessage();
  }

  initMessage() {
    this.message = new Message();
    this.message.sentToChatId = this.chat.id;
    this.message.sentByUserId = this.me?.id ?? null;
  }

  created() {
    this.initMessage();
  }
}
</script>

<style>

</style>