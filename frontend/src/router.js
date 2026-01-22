import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from './stores/userStore'

// Auth Pages
import Login from './pages/Login.vue'
import TermsAndConditions from './pages/TermsAndConditions.vue'

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
  { path: '/terms-conditions', component: TermsAndConditions },
  { path: '/', component: Home },
  { path: '/orders', component: Orders, meta: { requiresAuth: true } },
  { path: '/orders/new', component: CreateOrder, meta: { requiresAuth: true } },
  { path: '/orders/:id', component: Orders, meta: { requiresAuth: true } },
  { path: '/profile', component: Profile, meta: { requiresAuth: true } },
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
  
  // Si la ruta requiere autenticación y no está autenticado
  if (to.meta.requiresAuth && !userStore.isAuthenticated) {
    // Redirige a login con la ruta destino como redirect
    next({ path: '/login', query: { redirect: to.fullPath } })
  } 
  // Si la ruta requiere admin y el usuario no es admin
  else if (to.meta.requiresAdmin && userStore.user?.role !== 'Admin') {
    next('/')
  } 
  // Si todo está bien, continúa
  else {
    next()
  }
})

export default router
