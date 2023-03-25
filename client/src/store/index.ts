import Vue from 'vue'
import Vuex from 'vuex'
import { AuthStoreModule } from './AuthStore'
import { ChatStoreModule } from './ChatStore'
import { MainStoreModule } from './MainStore'
import { UserStoreModule } from './UserStore'

Vue.use(Vuex)

export interface IAppState {
  main: MainStoreModule,
  auth: AuthStoreModule,
  user: UserStoreModule,
  chat: ChatStoreModule,
}

export default new Vuex.Store<IAppState>({
})
