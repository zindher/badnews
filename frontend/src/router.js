import { createRouter, createWebHistory } from 'vue-router'
import Home from './pages/Home.vue'
import Orders from './pages/Orders.vue'
import Profile from './pages/Profile.vue'
import CreateOrder from './pages/CreateOrder.vue'
import Terms from './pages/Terms.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/orders', component: Orders },
  { path: '/orders/new', component: CreateOrder },
  { path: '/profile', component: Profile },
  { path: '/terms', component: Terms }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
