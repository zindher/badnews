import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { transactionService } from '../services/transactionService'

export const useUIStore = defineStore('ui', () => {
  // Sidebar and UI state
  const sidebarOpen = ref(false)
  const showNotification = ref(false)
  const notificationMessage = ref('')
  const notificationType = ref('success') // success, error, warning, info
  const notificationDuration = ref(3000)
  
  // Notifications stack for multiple toasts
  const notifications = ref([])
  let notificationId = 0

  // Loading state
  const isGlobalLoading = ref(false)
  const loadingMessage = ref('')

  // Modal states
  const showModal = ref(false)
  const modalType = ref('') // confirm, alert, custom
  const modalTitle = ref('')
  const modalMessage = ref('')
  const modalData = ref(null)

  // Computed
  const notificationsCount = computed(() => notifications.value.length)

  // Sidebar actions
  const toggleSidebar = () => {
    sidebarOpen.value = !sidebarOpen.value
  }

  const closeSidebar = () => {
    sidebarOpen.value = false
  }

  const openSidebar = () => {
    sidebarOpen.value = true
  }

  // Single notification (legacy support)
  const showNotify = (message, type = 'success', duration = 3000) => {
    notificationMessage.value = message
    notificationType.value = type
    notificationDuration.value = duration
    showNotification.value = true

    setTimeout(() => {
      showNotification.value = false
    }, duration)
  }

  // Stack-based notifications
  const addNotification = (message, type = 'success', duration = 3000) => {
    const id = notificationId++
    const notification = { id, message, type, duration }
    notifications.value.push(notification)

    if (duration > 0) {
      setTimeout(() => {
        removeNotification(id)
      }, duration)
    }

    return id
  }

  const removeNotification = (id) => {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index !== -1) {
      notifications.value.splice(index, 1)
    }
  }

  const clearNotifications = () => {
    notifications.value = []
  }

  // Convenience methods for single notification
  const showSuccess = (message) => showNotify(message, 'success')
  const showError = (message) => showNotify(message, 'error')
  const showWarning = (message) => showNotify(message, 'warning')
  const showInfo = (message) => showNotify(message, 'info')

  // Convenience methods for stacked notifications
  const addSuccess = (message, duration) => addNotification(message, 'success', duration)
  const addError = (message, duration) => addNotification(message, 'error', duration)
  const addWarning = (message, duration) => addNotification(message, 'warning', duration)
  const addInfo = (message, duration) => addNotification(message, 'info', duration)

  const closeNotification = () => {
    showNotification.value = false
  }

  // Global loading state
  const setGlobalLoading = (loading, message = '') => {
    isGlobalLoading.value = loading
    loadingMessage.value = message
  }

  // Modal actions
  const openModal = (type, title, message, data = null) => {
    modalType.value = type
    modalTitle.value = title
    modalMessage.value = message
    modalData.value = data
    showModal.value = true
  }

  const closeModal = () => {
    showModal.value = false
    modalType.value = ''
    modalTitle.value = ''
    modalMessage.value = ''
    modalData.value = null
  }

  const openConfirmModal = (title, message, data = null) => {
    openModal('confirm', title, message, data)
  }

  const openAlertModal = (title, message) => {
    openModal('alert', title, message, null)
  }

  return {
    // Sidebar
    sidebarOpen,
    toggleSidebar,
    closeSidebar,
    openSidebar,

    // Single notification
    showNotification,
    notificationMessage,
    notificationType,
    notificationDuration,
    showNotify,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    closeNotification,

    // Stacked notifications
    notifications,
    notificationsCount,
    addNotification,
    removeNotification,
    clearNotifications,
    addSuccess,
    addError,
    addWarning,
    addInfo,

    // Global loading
    isGlobalLoading,
    loadingMessage,
    setGlobalLoading,

    // Modals
    showModal,
    modalType,
    modalTitle,
    modalMessage,
    modalData,
    openModal,
    closeModal,
    openConfirmModal,
    openAlertModal
  }
})
