<template>
  <v-dialog
      v-model="dialog"
      width="500"
    >
      <v-card>
        <v-card-title class="text-h5">
          New chat
        </v-card-title>

        <v-card-text>
          <v-form ref="chatCreationForm">
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="chat.name"
                  label="Name"
                  :rules="[formRules.nameRequired]"
                  :disabled="loading"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-btn
            color="grey"
            text
            @click="closeDialog"
          >
            Cancel
          </v-btn>
          <v-spacer></v-spacer>
          <v-btn
            color="primary"
            text
            @click="createNewChat"
          >
            Create
          </v-btn>
        </v-card-actions>
      </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Chat from "@/models/Chat";
import User from "@/models/User";
import AuthStore from "@/store/AuthStore";
import ChatStore from "@/store/ChatStore";
import UserStore from "@/store/UserStore";
import { Vue, Component } from "vue-property-decorator"

@Component({
  name: "ChatCreationDialog"
})
export default class ChatCreationDialog extends Vue {
  dialog: boolean = false;
  chat: Chat = new Chat();
  formRules: any = {
    nameRequired: (v: any) => !!v || 'Required',
  }
  loading: boolean = false;

  get me(): User | null {
    return UserStore.me;
  }

  async createNewChat() {
    if (!(this.$refs.chatCreationForm as any).validate()) {
      return
    }

    if (this.me) 
    {
      this.chat.chatUserIds.push(this.me.id);
    }
    try {
      await ChatStore.createOrUpdateChat(this.chat)
    } catch (error) {
      
    }

    this.chat = new Chat();
    // this.chat.chatUserIds = []
    this.closeDialog();
  }

  openDialog() {
    this.dialog = true;
  }

  closeDialog() {
    (this.$refs.chatCreationForm as any).reset();
    this.chat = new Chat();
    this.dialog = false;
  }

  created() {
    this.$root.$on("openCreateChatDialog", this.openDialog)
  }

  beforeDestroy() {
    this.$root.$on("openCreateChatDialog", this.closeDialog)
  }
}
</script>

<style>

</style>