<template>
  <div class="form-group">
    <label v-if="label" :for="id" :class="{ 'label-required': required }">
      {{ label }}
    </label>
    
    <div v-if="type === 'textarea'" class="input-wrapper">
      <textarea
        :id="id"
        :value="modelValue"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        :rows="rows"
        @input="$emit('update:modelValue', $event.target.value)"
        @blur="$emit('blur')"
      />
    </div>

    <div v-else-if="type === 'checkbox'" class="checkbox-wrapper">
      <input
        :id="id"
        type="checkbox"
        :checked="modelValue"
        :required="required"
        :disabled="disabled"
        @change="$emit('update:modelValue', $event.target.checked)"
      />
      <label :for="id" class="checkbox-label">{{ label }}</label>
    </div>

    <div v-else-if="type === 'select'" class="select-wrapper">
      <select
        :id="id"
        :value="modelValue"
        :required="required"
        :disabled="disabled"
        @change="$emit('update:modelValue', $event.target.value)"
      >
        <option value="">{{ placeholder || 'Selecciona...' }}</option>
        <option v-for="option in options" :key="option.value" :value="option.value">
          {{ option.label }}
        </option>
      </select>
    </div>

    <div v-else class="input-wrapper" :class="{ 'input-has-addon': type === 'currency' }">
      <span v-if="type === 'currency'" class="currency-symbol">$</span>
      <input
        :id="id"
        :type="type"
        :value="modelValue"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        :min="min"
        :max="max"
        :step="step"
        @input="$emit('update:modelValue', $event.target.value)"
        @blur="$emit('blur')"
      />
      <span v-if="type === 'currency'" class="currency-code">MXN</span>
    </div>

    <p v-if="helpText" class="help-text">{{ helpText }}</p>
    <p v-if="error" class="error-text">{{ error }}</p>
  </div>
</template>

<script setup>
defineProps({
  id: {
    type: String,
    required: true
  },
  label: String,
  type: {
    type: String,
    default: 'text',
    validator: (value) => [
      'text', 'email', 'tel', 'number', 'password', 
      'textarea', 'checkbox', 'select', 'currency'
    ].includes(value)
  },
  modelValue: {
    type: [String, Number, Boolean],
    default: ''
  },
  placeholder: String,
  helpText: String,
  error: String,
  required: Boolean,
  disabled: Boolean,
  rows: {
    type: Number,
    default: 5
  },
  min: [String, Number],
  max: [String, Number],
  step: [String, Number],
  options: Array
})

defineEmits(['update:modelValue', 'blur'])
</script>

<style scoped>
/* Specific FormField component styles only */
.form-group {
  margin-bottom: 1.5rem;
}

.label-required::after {
  content: ' *';
  color: #f44336;
}

.input-wrapper,
.select-wrapper,
.checkbox-wrapper {
  display: flex;
  align-items: center;
  position: relative;
}

.input-has-addon {
  border: 1px solid #ddd;
  border-radius: 6px;
  overflow: hidden;
  background: white;
  gap: 0;
}

.input-has-addon input {
  border: none;
  padding-left: 2rem;
  flex: 1;
}

.input-has-addon input:focus {
  box-shadow: none;
}

.currency-symbol,
.currency-code {
  color: #666;
  font-weight: 500;
  padding: 0 0.75rem;
  white-space: nowrap;
  user-select: none;
}

.currency-symbol {
  order: -1;
}

/* CHECKBOX STYLE */
.checkbox-wrapper {
  align-items: flex-start;
  gap: 0.75rem;
}

.checkbox-label {
  margin: 0;
  cursor: pointer;
  font-weight: normal;
  color: #333;
  padding: 2px 0;
}
</style>
