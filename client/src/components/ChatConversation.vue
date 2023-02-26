<template>
  
  <v-card color="#E3F2FD" height="100%" width="100%" tile class="d-flex flex-column align">
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

<style lang="scss">
.chat-message-container {
  height: calc(100vh - 100px - 64px);

  &__date {
    // width: 100%;
    margin: auto;
  }
}
</style>