<template>
  <v-list dense nav >
    <v-hover
      v-slot="{ hover }"
      open-delay="200"
      v-for="chat in chats" :key="chat.id" 
    >
    <v-list-item 
      @click.stop="onChatClick(chat.id)"
      :class="{'v-list-item--active': chatIdFromRouteParam === chat.id}"
    >
      <v-list-item-icon>
        <v-icon>mdi-chat</v-icon>
      </v-list-item-icon>
      <v-list-item-content class="text-left">
        <v-list-item-title>{{ chat.name }}</v-list-item-title>

      </v-list-item-content>
      <MoreAction
        :items="moreActionItems"
        :target="chat.id"
        :nudges="[6,17,0,0]"
        :hovered="hover ? true : false"
        @moreActionToggled="onMoreActionToggled"
        @moreActionItemClicked="onMoreActionItemClicked"
      ></MoreAction>
    </v-list-item>
    </v-hover>
  </v-list>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import ChatStore from "@/store/ChatStore";
import { Vue, Component } from "vue-property-decorator";
import MoreAction, { IMoreActionItem } from "@/components/MoreAction.vue"
import { ChatEditOnlyEnum } from "./ChatCreationDialog.vue";

@Component({
  name: "ChatNavigation",
  components: {
    MoreAction
  }
})
export default class ChatNavigation extends Vue {
  moreActionItems: IMoreActionItem[] = [
    {id: "manageChatUsers", text: "Manage people", disabled: false, dangerous: false},
    {id: "manageChatName", text: "Rename", disabled: false, dangerous: false}
  ]
  hover!: boolean;


  get chats(): Chat[] {
    return ChatStore.chats;
  }

  get chatIdFromRouteParam(): string | null {
    return this.$route.params ? this.$route.params.chatId : null
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