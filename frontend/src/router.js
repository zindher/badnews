import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from './stores/userStore'

// Auth Pages
import Login from './pages/Login.vue'

// Buyer Pages
import Home from './pages/Home.vue'
import Orders from './pages/Orders.vue'
import Profile from './pages/Profile.vue'
import CreateOrder from './pages/CreateOrder.vue'
import Terms from './pages/Terms.vue'

// Messenger Pages
import MessengerHome from './pages/MessengerHome.vue'
import PaymentSuccess from './pages/PaymentSuccess.vue'
import PaymentFailed from './pages/PaymentFailed.vue'
import Earnings from './pages/Earnings.vue'
import History from './pages/History.vue'

// Admin Pages
import AdminDashboard from './pages/AdminDashboard.vue'
import Analytics from './pages/Analytics.vue'

const routes = [
  { path: '/login', component: Login },
  { path: '/', component: Home },
  { path: '/orders', component: Orders },
  { path: '/orders/new', component: CreateOrder },
  { path: '/orders/:id', component: Orders },
  { path: '/profile', component: Profile },
  { path: '/terms', component: Terms },
  
  // Messenger routes
  { path: '/messenger/home', component: MessengerHome, meta: { requiresAuth: true } },
  { path: '/messenger/earnings', component: Earnings, meta: { requiresAuth: true } },
  { path: '/messenger/history', component: History, meta: { requiresAuth: true } },
  
  // Payment routes
  { path: '/payment/success', component: PaymentSuccess },
  { path: '/payment/failed', component: PaymentFailed },
  
  // Admin routes
  { path: '/admin/dashboard', component: AdminDashboard, meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/admin/analytics', component: Analytics, meta: { requiresAuth: true, requiresAdmin: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  const userStore = useUserStore()
  
  if (to.meta.requiresAuth && !userStore.isAuthenticated) {
    next('/')
  } else if (to.meta.requiresAdmin && userStore.user?.role !== 'admin') {
    next('/')
  } else {
    next()
  }
})

export default router
