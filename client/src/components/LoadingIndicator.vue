<template>
  <v-container v-if="isAuthenticationRedirectInProgress" class="loading-indicator">
    <v-progress-circular
      class="loading-indicator__progress-circular"
      :size="50"
      color="primary"
      indeterminate
    ></v-progress-circular>
  </v-container>
</template>

<script lang="ts">
import AuthStore from "@/store/AuthStore";
import { Vue, Component, Prop } from "vue-property-decorator"

@Component({
  name: "LoadingIndicator"
})
export default class LoadingIndicator extends Vue {

  @Prop({ required: false, default: false})
  loading!: false;

  get show() {
    return this.isAuthenticationRedirectInProgress || this.loading;
  }

  get isAuthenticationRedirectInProgress(): boolean {
    return AuthStore.isAuthenticationRedirectInProgress;
  }
}
</script>

<style lang="scss">
.loading-indicator {
  width: 100%;
  height: 100%;
  min-width: 100vw;
  min-height: 100vh;
  position: fixed;
  z-index: 200;
  background-color: rgba(#fff,0.8);

  &__progress-circular {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 100;
  }
}
</style>