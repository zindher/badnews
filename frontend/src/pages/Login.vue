<template>
  <div class="auth-container">
    <div class="auth-card">
      <div class="logo-section">
        <Logo variant="icon" :isCompact="true" />
        <span class="brand-text">Gritalo</span>
      </div>

      <div class="social-login">
        <h2>Bienvenido</h2>
        <p class="subtitle">Inicia sesión para continuar</p>

        <div v-if="userStore.isLoading" class="flex justify-center mb-4">
          <Loader message="Iniciando sesión..." />
        </div>

        <!-- Google Sign-In -->
        <GoogleSignInButton :showDivider="false" />

        <!-- Apple Sign-In -->
        <AppleSignInButton :showDivider="true" />
      </div>
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
import { useUserStore } from '../stores/userStore'
import Logo from '../components/Logo.vue'
import Loader from '../components/Loader.vue'
import GoogleSignInButton from '../components/GoogleSignInButton.vue'
import AppleSignInButton from '../components/AppleSignInButton.vue'
import TermsAndConditionsModal from '../components/TermsAndConditionsModal.vue'

const userStore = useUserStore()

const showTermsModal = ref(false)

const handleTermsAccepted = () => {
  showTermsModal.value = false
}

const handleTermsRejected = () => {
  showTermsModal.value = false
}
</script>

<style>
/* Component-specific styles can go here if needed */
</style>

