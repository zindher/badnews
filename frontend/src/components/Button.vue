<template>
  <button 
    :class="[
      'btn',
      `btn-${variant}`,
      { 'btn-disabled': disabled },
      customClass
    ]"
    :disabled="disabled"
    @click="$emit('click')"
  >
    <span v-if="loading" class="spinner"></span>
    <slot />
  </button>
</template>

<script setup>
defineProps({
  variant: {
    type: String,
    default: 'primary',
    validator: (value) => ['primary', 'secondary', 'danger', 'ghost'].includes(value)
  },
  disabled: {
    type: Boolean,
    default: false
  },
  loading: {
    type: Boolean,
    default: false
  },
  customClass: {
    type: String,
    default: ''
  }
})

defineEmits(['click'])
</script>

<style scoped>
/* Specific Button component variants and states only */
.btn:active {
  transform: scale(0.98);
}

/* PRIMARY VARIANT */
.btn-primary {
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(91, 75, 159, 0.3);
}

.btn-primary:active:not(:disabled) {
  box-shadow: 0 1px 4px rgba(91, 75, 159, 0.3);
}

/* SECONDARY VARIANT */
.btn-secondary {
  background: #f0f0f0;
  color: #333;
  border: 1px solid #ddd;
}

.btn-secondary:active:not(:disabled) {
  background: #e0e0e0;
}

/* DANGER VARIANT */
.btn-danger {
  background: #f44336;
  color: white;
  box-shadow: 0 2px 8px rgba(244, 67, 54, 0.3);
}

.btn-danger:active:not(:disabled) {
  box-shadow: 0 1px 4px rgba(244, 67, 54, 0.3);
}

/* GHOST VARIANT */
.btn-ghost {
  background: transparent;
  color: var(--primary-color);
  border: 2px solid var(--primary-color);
}

.btn-ghost:active:not(:disabled) {
  background: rgba(91, 75, 159, 0.1);
}

/* LOADING STATE */
.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

.btn-secondary .spinner {
  border-color: rgba(51, 51, 51, 0.3);
  border-top-color: #333;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* DESKTOP HOVER EFFECTS */
@media (min-width: 1024px) {
  .btn:hover:not(:disabled) {
    transform: translateY(-2px);
  }

  .btn-primary:hover:not(:disabled) {
    box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
  }

  .btn-secondary:hover:not(:disabled) {
    background: #e0e0e0;
  }

  .btn-danger:hover:not(:disabled) {
    box-shadow: 0 4px 12px rgba(244, 67, 54, 0.4);
  }

  .btn-ghost:hover:not(:disabled) {
    background: rgba(102, 126, 234, 0.1);
  }

  .btn:active {
    transform: scale(1);
  }
}
</style>
