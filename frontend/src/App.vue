<template>
  <div id="app">
    <!-- Global Notification Toast -->
    <NotificationToast />
    
    <!-- Global Modal Dialog -->
    <ModalDialog @confirm="handleModalConfirm" @cancel="handleModalCancel" />

    <header>
      <nav class="navbar">
        <div class="nav-container">
        <router-link to="/" class="nav-brand">
            <Logo variant="icon" :isCompact="true" />
            <span class="brand-text">Gritalo</span>
          </router-link>
          <button class="menu-toggle" @click="mobileMenuOpen = !mobileMenuOpen" :aria-expanded="mobileMenuOpen">
            <span></span>
            <span></span>
            <span></span>
          </button>
          <div class="nav-links" :class="{ active: mobileMenuOpen }">
            <router-link v-if="userStore.isAuthenticated" to="/orders" class="nav-link" @click="mobileMenuOpen = false">Mis Encargos</router-link>
            <router-link v-if="userStore.isAuthenticated" to="/profile" class="nav-link" @click="mobileMenuOpen = false">Perfil</router-link>
            <button v-if="userStore.isAuthenticated" @click="logout" class="nav-link logout-btn">Salir</button>
            <router-link v-else to="/login" class="nav-link" @click="mobileMenuOpen = false">Iniciar Sesión</router-link>
          </div>
        </div>
      </nav>
    </header>
    <main class="main-content">
      <router-view></router-view>
    </main>
    <footer class="footer">
      <div class="footer-content">
        <div class="footer-brand">
          <Logo variant="icon" />
          <span style="color: white; font-weight: bold; font-size: 1.1rem;">Gritalo</span>
        </div>
        <p>&copy; 2026 Gritalo. Todos los derechos reservados.</p>
        <nav class="footer-links">
          <router-link to="/terms">Términos y Condiciones</router-link>
          <a href="mailto:soporte@gritalo.com">Soporte</a>
        </nav>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from './stores/userStore'
import NotificationToast from './components/NotificationToast.vue'
import ModalDialog from './components/ModalDialog.vue'
import Logo from './components/Logo.vue'

const router = useRouter()
const userStore = useUserStore()
const mobileMenuOpen = ref(false)

const logout = async () => {
  await userStore.logout()
  mobileMenuOpen.value = false
  router.push('/')
}

const handleModalConfirm = (data) => {
  console.log('Modal confirmed with data:', data)
}

const handleModalCancel = () => {
  console.log('Modal cancelled')
}
</script>


