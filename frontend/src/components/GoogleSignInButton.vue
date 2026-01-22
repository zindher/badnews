<template>
  <div class="google-login-container">
    <div v-if="!googleClientIdConfigured" class="warning">
      ⚠️ Google Sign-In not configured. Set VITE_GOOGLE_CLIENT_ID in .env
    </div>

    <div v-else>
      <!-- Google Sign-In Button Container -->
      <div id="google-signin-button"></div>

      <!-- Divider -->
      <div class="divider" v-if="showDivider">
        <span>O</span>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <div class="spinner"></div>
        <p>Iniciando sesión con Google...</p>
      </div>

      <!-- Error Message -->
      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
        <button @click="clearError" class="close-btn">×</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useUserStore } from '../stores/userStore'
import { useRouter } from 'vue-router'
import { useUIStore } from '../stores/uiStore'
import { authService } from '../services/authService'
import { loadGoogleScript, initializeGoogleSignIn, googleAuthService } from '../services/googleAuthService'

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
const googleClientIdConfigured = ref(false)

onMounted(async () => {
  try {
    // Validate Google configuration
    if (!googleAuthService.validateConfiguration()) {
      console.warn('Google OAuth not configured')
      return
    }

    googleClientIdConfigured.value = true

    // Load Google Sign-In script
    await loadGoogleScript()

    // Initialize Google Sign-In button
    const clientId = googleAuthService.getClientId()
    initializeGoogleSignIn(
      clientId,
      'google-signin-button',
      handleGoogleSignInSuccess,
      handleGoogleSignInError
    )
  } catch (error) {
    console.error('Error setting up Google Sign-In:', error)
    errorMessage.value = 'Error loading Google Sign-In'
  }
})

/**
 * Handle successful Google Sign-In
 */
const handleGoogleSignInSuccess = async (credential) => {
  isLoading.value = true
  errorMessage.value = ''

  try {
    // Send token to backend
    const response = await authService.loginWithGoogle(credential)

    if (response.data?.success && response.data?.data) {
      const { token, user } = response.data.data

      // Store auth data
      authService.setAuthData(token, user)
      userStore.setAuthData(token, user)

      // Show success notification
      uiStore.addSuccess(`¡Bienvenido, ${user.firstName}!`)

      // Redirect based on role
      const redirectPath = user.role === 'Messenger' ? '/messenger/home' : '/orders'
      await router.push(redirectPath)
    } else {
      throw new Error(response.data?.message || 'Login failed')
    }
  } catch (error) {
    console.error('Google login error:', error)
    errorMessage.value = error.response?.data?.message || error.message || 'Error logging in with Google'
    uiStore.addError(errorMessage.value)
  } finally {
    isLoading.value = false
  }
}

/**
 * Handle Google Sign-In error
 */
const handleGoogleSignInError = (error) => {
  console.error('Google Sign-In error:', error)
  errorMessage.value = 'Error with Google Sign-In'
  uiStore.addError('Error logging in with Google')
}

/**
 * Clear error message
 */
const clearError = () => {
  errorMessage.value = ''
}
</script>

<style scoped>
/* Specific GoogleSignInButton component styles only */
.google-login-container {
  width: 100%;
}

#google-signin-button {
  display: flex;
  justify-content: center;
  margin: 1rem 0;
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
  border-top: 4px solid var(--primary-color);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
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
