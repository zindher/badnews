<template>
  <div class="auth-container">
    <div class="auth-card">
      <div class="logo-section">
        <Logo variant="icon" :isCompact="true" />
        <span class="brand-text">Gritalo</span>
      </div>
      
      <!-- TAB SELECTOR -->
      <div class="tab-selector">
        <button 
          :class="{ active: isLogin }" 
          @click="isLogin = true"
          class="tab-btn"
        >
          Iniciar Sesión
        </button>
        <button 
          :class="{ active: !isLogin }" 
          @click="isLogin = false"
          class="tab-btn"
        >
          Registrarse
        </button>
      </div>

      <!-- LOGIN FORM -->
      <form v-if="isLogin" @submit.prevent="handleLogin" class="form">
        <h2>Bienvenido de vuelta</h2>
        <p class="subtitle">Inicia sesión en tu cuenta</p>
        
        <div v-if="userStore.isLoading" class="flex justify-center mb-4">
          <Loader message="Iniciando sesión..." />
        </div>
        
        <div class="form-group">
          <label>Email</label>
          <input 
            v-model="loginForm.email" 
            type="email" 
            placeholder="tu@email.com"
            :disabled="userStore.isLoading"
            required
          />
        </div>

        <div class="form-group">
          <label>Contraseña</label>
          <input 
            v-model="loginForm.password" 
            type="password" 
            placeholder="••••••••"
            :disabled="userStore.isLoading"
            required
          />
        </div>

        <button 
          type="submit" 
          class="btn btn-primary"
          :disabled="userStore.isLoading"
        >
          {{ userStore.isLoading ? 'Iniciando sesión...' : 'Iniciar Sesión' }}
        </button>

        <!-- Google Sign-In -->
        <GoogleSignInButton :showDivider="true" />
      </form>

      <!-- REGISTER FORM -->
      <form v-else @submit.prevent="handleRegister" class="form">
        <h2>Únete a Gritalo</h2>
        <p class="subtitle">Crea tu cuenta en segundos</p>

        <div v-if="userStore.isLoading" class="flex justify-center mb-4">
          <Loader message="Registrando..." />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Nombre</label>
            <input 
              v-model="registerForm.firstName" 
              type="text" 
              placeholder="Juan"
              :disabled="userStore.isLoading"
              required
            />
          </div>

          <div class="form-group">
            <label>Apellido</label>
            <input 
              v-model="registerForm.lastName" 
              type="text" 
              placeholder="Pérez"
              :disabled="userStore.isLoading"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label>Email</label>
          <input 
            v-model="registerForm.email" 
            type="email" 
            placeholder="tu@email.com"
            :disabled="userStore.isLoading"
            required
          />
        </div>

        <div class="form-group">
          <label>Contraseña</label>
          <input 
            v-model="registerForm.password" 
            type="password" 
            placeholder="••••••••"
            :disabled="userStore.isLoading"
            required
          />
        </div>

        <div class="form-group">
          <label>Confirmar Contraseña</label>
          <input 
            v-model="registerForm.confirmPassword" 
            type="password" 
            placeholder="••••••••"
            :disabled="userStore.isLoading"
            required
          />
        </div>

        <button 
          type="submit" 
          class="btn btn-primary"
          :disabled="userStore.isLoading"
        >
          {{ userStore.isLoading ? 'Registrando...' : 'Registrarse' }}
        </button>
      </form>
    </div>

    <!-- Terms and Conditions Modal -->
    <TermsAndConditionsModal 
      :isOpen="showTermsModal"
      :isRequired="true"
      @accept="handleTermsAccepted"
      @reject="handleTermsRejected"
      @close="showTermsModal = false"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/userStore'
import { useUIStore } from '../stores/uiStore'
import Logo from '../components/Logo.vue'
import Loader from '../components/Loader.vue'
import ErrorMessage from '../components/ErrorMessage.vue'
import GoogleSignInButton from '../components/GoogleSignInButton.vue'
import TermsAndConditionsModal from '../components/TermsAndConditionsModal.vue'

const router = useRouter()
const userStore = useUserStore()
const uiStore = useUIStore()

const isLogin = ref(true)
const showErrorMessage = ref(false)
const showTermsModal = ref(false)
const termsAccepted = ref(false)

const loginForm = ref({
  email: 'buyer@test.com',
  password: 'password123'
})

const registerForm = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
  role: 'Buyer' // Frontend es solo para Compradores
})

// Form validation
const validateLoginForm = () => {
  if (!loginForm.value.email || !loginForm.value.password) {
    uiStore.addError('Por favor completa todos los campos')
    return false
  }
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(loginForm.value.email)) {
    uiStore.addError('Email inválido')
    return false
  }
  return true
}

const validateRegisterForm = () => {
  if (!registerForm.value.firstName || !registerForm.value.lastName || 
      !registerForm.value.email || !registerForm.value.password || 
      !registerForm.value.confirmPassword) {
    uiStore.addError('Por favor completa todos los campos')
    return false
  }
  if (registerForm.value.password !== registerForm.value.confirmPassword) {
    uiStore.addError('Las contraseñas no coinciden')
    return false
  }
  if (registerForm.value.password.length < 6) {
    uiStore.addError('La contraseña debe tener al menos 6 caracteres')
    return false
  }
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(registerForm.value.email)) {
    uiStore.addError('Email inválido')
    return false
  }
  return true
}

const handleLogin = async () => {
  if (!validateLoginForm()) return

  try {
    const success = await userStore.login(
      loginForm.value.email, 
      loginForm.value.password
    )
    
    if (success) {
      uiStore.addSuccess('¡Bienvenido de vuelta!')
      setTimeout(() => {
        const redirectPath = userStore.isMessenger ? '/messenger/home' : '/orders'
        router.push(redirectPath)
      }, 1000)
    } else {
      uiStore.addError(userStore.error || 'Email o contraseña incorrectos')
    }
  } catch (err) {
    uiStore.addError(err.message || 'Error al iniciar sesión')
    console.error('Login error:', err)
  }
}

const handleRegister = async () => {
  if (!validateRegisterForm()) return

  // Mostrar modal de T&C
  if (!termsAccepted.value) {
    showTermsModal.value = true
    return
  }

  try {
    const userData = {
      firstName: registerForm.value.firstName,
      lastName: registerForm.value.lastName,
      email: registerForm.value.email,
      password: registerForm.value.password,
      role: registerForm.value.role,
      termsAcceptedAt: new Date().toISOString()
    }

    const success = await userStore.register(userData)
    
    if (success) {
      uiStore.addSuccess('¡Cuenta creada exitosamente!')
      setTimeout(() => {
        const redirectPath = userStore.isMessenger ? '/messenger/home' : '/orders'
        router.push(redirectPath)
      }, 1000)
    } else {
      uiStore.addError(userStore.error || 'Error al registrarse')
    }
  } catch (err) {
    uiStore.addError(err.message || 'Error al registrarse')
    console.error('Register error:', err)
  }
}

const handleTermsAccepted = (data) => {
  termsAccepted.value = true
  showTermsModal.value = false
  // Continuar con el registro
  handleRegister()
}

const handleTermsRejected = () => {
  termsAccepted.value = false
  showTermsModal.value = false
  uiStore.addError('Debes aceptar los Términos y Condiciones para registrarte')
}
</script>

<style>
/* Component-specific styles can go here if needed */
</style>

