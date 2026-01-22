<template>
  <div v-if="notifications.length > 0" class="fixed top-4 right-4 z-50 space-y-2 max-w-md">
    <transition-group name="toast" tag="div">
      <div
        v-for="notification in notifications"
        :key="notification.id"
        :class="[
          'p-4 rounded-lg shadow-lg text-white flex items-start justify-between gap-3 animate-slide-in',
          getBackgroundColor(notification.type)
        ]"
      >
        <div class="flex items-start gap-3 flex-1">
          <component :is="getIcon(notification.type)" class="h-5 w-5 flex-shrink-0 mt-0.5" />
          <p class="text-sm font-medium">{{ notification.message }}</p>
        </div>
        <button
          @click="removeNotification(notification.id)"
          class="flex-shrink-0 text-white hover:opacity-80 transition-opacity"
        >
          <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useUIStore } from '../stores/uiStore'

const uiStore = useUIStore()

const notifications = computed(() => uiStore.notifications)

const removeNotification = (id) => {
  uiStore.removeNotification(id)
}

const getBackgroundColor = (type) => {
  const colors = {
    success: 'bg-green-500',
    error: 'bg-red-500',
    warning: 'bg-yellow-500',
    info: 'bg-blue-500'
  }
  return colors[type] || colors.info
}

const getIcon = (type) => {
  const icons = {
    success: 'IconCheckCircle',
    error: 'IconExclamation',
    warning: 'IconWarning',
    info: 'IconInfo'
  }
  // Using inline SVG instead
  return 'svg'
}
</script>

<style scoped>
/* Specific NotificationToast component animations only */
.animate-slide-in {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    transform: translateX(400px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  transform: translateX(400px);
  opacity: 0;
}

.toast-leave-to {
  transform: translateX(400px);
  opacity: 0;
}
</style>
