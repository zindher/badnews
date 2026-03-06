<template>
  <div class="apple-login-container">
    <div v-if="!appleClientIdConfigured" class="warning">
      ⚠️ Apple Sign-In not configured. Set VITE_APPLE_CLIENT_ID and VITE_APPLE_REDIRECT_URI in .env
    </div>

    <div v-else>
      <!-- Divider -->
      <div class="divider" v-if="showDivider">
        <span>O</span>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <div class="spinner"></div>
        <p>Iniciando sesión con Apple...</p>
      </div>

      <!-- Apple Sign-In Button -->
      <button
        v-if="!isLoading"
        class="apple-signin-btn"
        @click="handleAppleSignIn"
        :disabled="isLoading"
        type="button"
      >
        <svg class="apple-icon" viewBox="0 0 814 1000" xmlns="http://www.w3.org/2000/svg">
          <path d="M788.1 340.9c-5.8 4.5-108.2 62.2-108.2 190.5 0 148.4 130.3 200.9 134.2 202.2-.6 3.2-20.7 71.9-68.7 141.9-42.8 61.6-87.5 123.1-155.5 123.1s-85.5-39.5-164-39.5c-76 0-103.7 40.8-165.9 40.8s-105-57.8-155.5-127.4C46 790.5 0 663 0 541.8c0-207.5 135.4-317.3 269-317.3 70.1 0 128.4 46.4 172.5 46.4 42.8 0 109.6-49.1 189.2-49.1 30.4 0 135.5 2.6 205.3 108.4zm-244-181.7c32.1-38.2 55.5-91.1 55.5-144 0-7.4-.6-14.9-1.9-21.1-52.6 2-115 35.2-152.8 75.8-28.5 31.4-56.3 83.6-56.3 137.4 0 8.1 1.3 16.2 1.9 18.8 3.2.6 8.4 1.3 13.6 1.3 47.4 0 106.5-32.1 140-68.2z"/>
        </svg>
        Continuar con Apple
      </button>

      <!-- Error Message -->
      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
        <button @click="clearError" class="close-btn">×</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useUserStore } from '../stores/userStore'
import { useRouter } from 'vue-router'
import { useUIStore } from '../stores/uiStore'
import { authService } from '../services/authService'

const props = defineProps({
  showDivider: {
    type: Boolean,
    default: true,
  },
})

const userStore = useUserStore()
const uiStore = useUIStore()
const router = useRouter()

const isLoading = ref(false)
const errorMessage = ref('')
const appleClientIdConfigured = ref(false)

const APPLE_CLIENT_ID = import.meta.env.VITE_APPLE_CLIENT_ID
const APPLE_REDIRECT_URI = import.meta.env.VITE_APPLE_REDIRECT_URI

onMounted(() => {
  if (!APPLE_CLIENT_ID || !APPLE_REDIRECT_URI) {
    console.warn('Apple Sign-In not configured')
    return
  }
  appleClientIdConfigured.value = true

  // Load Apple Sign-In JS SDK
  if (!document.getElementById('apple-signin-script')) {
    const script = document.createElement('script')
    script.id = 'apple-signin-script'
    script.src = 'https://appleid.cdn-apple.com/appleauth/static/jsapi/appleid/1/en_US/appleid.auth.js'
    script.async = true
    script.onload = initializeApple
    document.head.appendChild(script)
  } else {
    initializeApple()
  }
})

const initializeApple = () => {
  if (!window.AppleID) return
  window.AppleID.auth.init({
    clientId: APPLE_CLIENT_ID,
    scope: 'name email',
    redirectURI: APPLE_REDIRECT_URI,
    usePopup: true,
  })
}

const handleAppleSignIn = async () => {
  if (!window.AppleID) {
    errorMessage.value = 'Apple Sign-In no está disponible'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const response = await window.AppleID.auth.signIn()
    const identityToken = response.authorization?.id_token
    const firstName = response.user?.name?.firstName || null
    const lastName = response.user?.name?.lastName || null

    if (!identityToken) throw new Error('No identity token received from Apple')

    const result = await authService.loginWithApple(identityToken, firstName, lastName)

    if (result.data?.success && result.data?.data) {
      const { token, user } = result.data.data
      authService.setAuthData(token, user)
      userStore.setAuthData(token, user)
      uiStore.addSuccess(`¡Bienvenido, ${user.firstName}!`)
      const redirectPath = user.role === 'Messenger' ? '/messenger/home' : '/orders'
      await router.push(redirectPath)
    } else {
      throw new Error(result.data?.message || 'Login failed')
    }
  } catch (error) {
    // Apple SDK sets error.error = 'popup_closed_by_user' when user dismisses
    if (error?.error === 'popup_closed_by_user') {
      return
    }
    console.error('Apple login error:', error)
    const message =
      error?.response?.data?.message ||
      error?.message ||
      'Error al iniciar sesión con Apple'
    errorMessage.value = message
    uiStore.addError(message)
  } finally {
    isLoading.value = false
  }
}

const clearError = () => {
  errorMessage.value = ''
}
</script>

<style scoped>
.apple-login-container {
  width: 100%;
}

.apple-signin-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.6rem;
  width: 100%;
  padding: 0.75rem 1rem;
  background: #000;
  color: #fff;
  border: none;
  border-radius: 6px;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
  margin: 1rem 0;
}

.apple-signin-btn:hover:not(:disabled) {
  background: #1a1a1a;
}

.apple-signin-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.apple-icon {
  width: 20px;
  height: 20px;
  fill: #fff;
  flex-shrink: 0;
}

.divider {
  display: flex;
  align-items: center;
  margin: 1.5rem 0;
  color: #999;
  font-size: 0.9rem;
  font-weight: 500;
}

.divider span {
  background: white;
  padding: 0 0.5rem;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background: #e0e0e0;
}

.warning {
  padding: 1rem;
  background: #fff3cd;
  border: 1px solid #ffc107;
  border-radius: 4px;
  color: #856404;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.loading-state {
  text-align: center;
  padding: 2rem 0;
}

.spinner {
  display: inline-block;
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid var(--primary-color, #000);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  background: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 4px;
  color: #721c24;
  font-size: 0.95rem;
  margin-bottom: 1rem;
}

.close-btn {
  background: none;
  border: none;
  color: #721c24;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0;
  margin-left: 1rem;
}

.close-btn:hover {
  opacity: 0.7;
}
</style>
