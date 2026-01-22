<template>
  <div v-if="visible" class="rounded-md bg-green-50 p-4 border border-green-200">
    <div class="flex">
      <div class="flex-shrink-0">
        <svg class="h-5 w-5 text-green-400" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
        </svg>
      </div>
      <div class="ml-3">
        <h3 class="text-sm font-medium text-green-800">Success</h3>
        <div class="mt-2 text-sm text-green-700">
          <p>{{ message }}</p>
        </div>
        <div v-if="showClose" class="mt-4">
          <button 
            @click="close"
            class="text-sm font-medium text-green-600 hover:text-green-500"
          >
            Dismiss
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  message: {
    type: String,
    required: true
  },
  visible: {
    type: Boolean,
    default: true
  },
  showClose: {
    type: Boolean,
    default: true
  },
  autoDismiss: {
    type: Number,
    default: 0 // 0 = no auto-dismiss
  }
})

const emit = defineEmits(['close'])
const isVisible = ref(props.visible)

watch(() => props.visible, (newVal) => {
  isVisible.value = newVal
  if (newVal && props.autoDismiss > 0) {
    setTimeout(() => {
      close()
    }, props.autoDismiss)
  }
})

const close = () => {
  isVisible.value = false
  emit('close')
}
</script>
