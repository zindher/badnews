import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUIStore = defineStore('ui', () => {
  const sidebarOpen = ref(false)
  const notificationMessage = ref('')
  const notificationType = ref('success') // success, error, warning, info
  const showNotification = ref(false)

  const toggleSidebar = () => {
    sidebarOpen.value = !sidebarOpen.value
  }

  const showNotify = (message, type = 'success', duration = 3000) => {
    notificationMessage.value = message
    notificationType.value = type
    showNotification.value = true

    setTimeout(() => {
      showNotification.value = false
    }, duration)
  }

  const showSuccess = (message) => showNotify(message, 'success')
  const showError = (message) => showNotify(message, 'error')
  const showWarning = (message) => showNotify(message, 'warning')
  const showInfo = (message) => showNotify(message, 'info')

  const closeNotification = () => {
    showNotification.value = false
  }

  return {
    sidebarOpen,
    notificationMessage,
    notificationType,
    showNotification,
    toggleSidebar,
    showNotify,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    closeNotification
  }
})
