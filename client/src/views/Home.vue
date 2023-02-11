<template>
  <div>
    <Navigation></Navigation>    
    <v-main>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-main>
  </div>
</template>

<script lang="ts">
import User from '@/models/User';
import UserStore from '@/store/UserStore';
import { Component, Vue } from 'vue-property-decorator';
import Navigation from "@/components/Navigation.vue";

@Component({
  components: {
    Navigation
  },
})
export default class Home extends Vue {
  loading: boolean = false;

  get me(): User | null {
    return UserStore.me;
  }  

  mounted() {
  
  }

}
</script>

<style lang="scss">
</style>
