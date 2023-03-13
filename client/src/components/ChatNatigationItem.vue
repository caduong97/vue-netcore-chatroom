<template>
  <v-hover
    v-slot="{ hover }"
    open-delay="200"
  >
    <v-list-item 
      @click.stop="onChatClick(chat.id)"
      :class="{'v-list-item--active': chatIdFromRouteParam === chat.id}"
    >
      <v-row no-gutters>
        <v-col cols="2" class="d-flex justify-start align-center">
          <v-list-item-icon>
            <v-icon>mdi-chat</v-icon>
          </v-list-item-icon>
        </v-col>
        
        <v-list-item-content class="text-left">
          <v-list-item-title class="d-flex align-center">{{ chat.name }}</v-list-item-title>
          <v-list-item-subtitle class="mt-3" v-if="chatSubtitleText">{{ chatSubtitleText }}</v-list-item-subtitle>
        </v-list-item-content>

        <v-col cols="2" class="d-flex justify-end align-center">
          <MoreAction
            :items="moreActionItems"
            :target="chat.id"
            :nudges="[-10,17,0,0]"
            :hovered="hover || $vuetify.breakpoint.mdAndDown ? true : false"
            @moreActionToggled="onMoreActionToggled"
            @moreActionItemClicked="onMoreActionItemClicked"
          ></MoreAction>
        </v-col>
      </v-row>
  </v-list-item>
  </v-hover>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import { Vue, Component, Prop } from "vue-property-decorator";
import { ChatEditOnlyEnum } from "@/components/ChatCreationDialog.vue";
import MoreAction, { IMoreActionItem } from "@/components/MoreAction.vue";
import UserStore from "@/store/UserStore";
import User from "@/models/User";

@Component({
  name: "ChatNavigationItem",
  components: {
    MoreAction
  }
})
export default class ChatNavigationItem extends Vue {
  moreActionItems: IMoreActionItem[] = [
    {id: "manageChatUsers", text: "Manage people", disabled: false, dangerous: false},
    {id: "manageChatName", text: "Rename", disabled: false, dangerous: false}
  ]
  
  @Prop({ required: true })
  chat!: Chat;

  @Prop({ required: false, default: null })
  hovered!: boolean;

  get me(): User | null {
    return UserStore.me;
  }

  get chatIdFromRouteParam(): string | null {
    return this.$route.params ? this.$route.params.chatId : null
  }

  get chatSubtitleText(): string | null {
    if (this.chat.latestMessage) {
      const sender = this.chat.latestMessage.sentByUserId === this.me?.id
        ? "You"
        : this.chat.latestMessage.sentByUserName !== null
          ? this.chat.latestMessage.sentByUserName
          : "Other"

      return `${sender}: ${this.chat.latestMessage.text}`
    }
    return null
  }

  getMoreActionItemById(id: string): IMoreActionItem | null {
    return this.moreActionItems.find(i => i.id == id) ?? null;
  }

  onChatClick(chatId: string) {
    if (this.$route.name !== 'Chat' || this.$route.params.chatId !== chatId) {
      this.$router.push({name: 'Chat', params: { chatId: chatId} })
    }
  }

  onMoreActionToggled(opened: boolean) {
  }

  onMoreActionItemClicked(payload: {moreActionItemId: string, moreActionItemTarget: any}) {
    const clickedItem = this.getMoreActionItemById(payload.moreActionItemId);
    if (clickedItem == null) return

    switch (clickedItem.id) {
      case "manageChatUsers":
        this.onManageChatUsersClick(payload.moreActionItemTarget)
        break;
      case "manageChatName":
        this.onManageChatName(payload.moreActionItemTarget)
        break;
      default:
        break;
    }

  }

  onManageChatUsersClick(chatId: string) {
    // console.log("onManageChatUsersClick", chatId)
    this.$root.$emit("openCreateChatDialog", {chatId, editOnly: ChatEditOnlyEnum.ChatUsers})

  }

  onManageChatName(chatId: string) {
    // console.log("onManageChatName", chatId)
    this.$root.$emit("openCreateChatDialog", {chatId, editOnly: ChatEditOnlyEnum.ChatName})
  }
}
</script>

<style>

</style>