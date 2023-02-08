import Vue from 'vue'
import Vuex from 'vuex'
import { AuthStoreModule } from './AuthStore'
import { UserStoreModule } from './UserStore'

Vue.use(Vuex)

export interface IAppState {
  auth: AuthStoreModule,
  user: UserStoreModule

}

export default new Vuex.Store<IAppState>({
})
