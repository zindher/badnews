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
.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 44px;
  gap: 0.5rem;
  touch-action: manipulation;
  position: relative;
  font-family: inherit;
  -webkit-user-select: none;
  user-select: none;
}

.btn:active {
  transform: scale(0.98);
}

/* PRIMARY VARIANT */
.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.btn-primary:active:not(:disabled) {
  box-shadow: 0 1px 4px rgba(102, 126, 234, 0.3);
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
  color: #667eea;
  border: 2px solid #667eea;
}

.btn-ghost:active:not(:disabled) {
  background: rgba(102, 126, 234, 0.1);
}

/* DISABLED STATE */
.btn-disabled,
.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none !important;
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

/* RESPONSIVE SIZING */
@media (max-width: 480px) {
  .btn {
    font-size: 0.95rem;
    padding: 0.65rem 1.25rem;
    min-height: 40px;
  }
}
</style>
