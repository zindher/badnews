<template>
  <teleport to="body">
    <transition name="modal">
      <div v-if="show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <div class="bg-white rounded-lg shadow-xl max-w-sm w-full mx-4 animate-modal-enter">
          <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-lg font-semibold text-gray-900">{{ title }}</h2>
          </div>
          
          <div class="px-6 py-4">
            <p class="text-gray-600">{{ message }}</p>
          </div>
          
          <div class="px-6 py-4 border-t border-gray-200 flex justify-end gap-3">
            <button
              v-if="type === 'confirm'"
              @click="cancel"
              class="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
            >
              Cancel
            </button>
            <button
              @click="confirm"
              :class="[
                'px-4 py-2 text-white rounded-lg transition-colors',
                type === 'confirm' ? 'bg-blue-600 hover:bg-blue-700' : 'bg-gray-600 hover:bg-gray-700'
              ]"
            >
              {{ confirmText }}
            </button>
          </div>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<script setup>
import { computed } from 'vue'
import { useUIStore } from '../stores/uiStore'

const props = defineProps({
  confirmText: {
    type: String,
    default: 'OK'
  },
  cancelText: {
    type: String,
    default: 'Cancel'
  }
})

const emit = defineEmits(['confirm', 'cancel'])
const uiStore = useUIStore()

const show = computed(() => uiStore.showModal)
const type = computed(() => uiStore.modalType)
const title = computed(() => uiStore.modalTitle)
const message = computed(() => uiStore.modalMessage)

const confirm = () => {
  emit('confirm', uiStore.modalData)
  uiStore.closeModal()
}

const cancel = () => {
  emit('cancel')
  uiStore.closeModal()
}
</script>

<style scoped>
/* Specific ModalDialog component animations only */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.animate-modal-enter {
  animation: modalEnter 0.3s ease-out;
}

@keyframes modalEnter {
  from {
    transform: scale(0.95);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}
</style>
