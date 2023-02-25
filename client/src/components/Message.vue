<template>
  <v-sheet
    :min-height="50" 
    :min-width="80"
    elevation="2"
    rounded
    :class="{
      'message-item__incoming': message.incoming,
      'message-item__outgoing': message.outgoing
    }"
    class="message-item d-flex align-center pa-3 mb-1"
  >
    {{message.text}}
  </v-sheet>
</template>

<script lang="ts">
import Message from "@/models/Message";
import { Vue, Component, Prop } from "vue-property-decorator"

@Component({
  name: "MessageItem"
})
export default class MessageItem extends Vue {
  @Prop({ required: true })
  message!: Message;


}
</script>

<style lang="scss">
.message-item {
  &__outgoing {
    margin-left: auto;
  }
  &__incoming {
    margin-right: auto;
  }
}
</style>