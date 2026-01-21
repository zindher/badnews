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
.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
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

input[type="text"],
input[type="email"],
input[type="tel"],
input[type="number"],
input[type="password"],
textarea,
select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  font-family: inherit;
  transition: all 0.3s ease;
  box-sizing: border-box;
  -webkit-appearance: none;
  appearance: none;
  background-color: white;
}

input::placeholder,
textarea::placeholder {
  color: #999;
}

input:focus,
textarea:focus,
select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

input:disabled,
textarea:disabled,
select:disabled {
  background-color: #f5f5f5;
  color: #999;
  cursor: not-allowed;
}

textarea {
  resize: vertical;
  min-height: 120px;
}

select {
  cursor: pointer;
  padding-right: 2rem;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='8' viewBox='0 0 12 8'%3E%3Cpath fill='%23667eea' d='M1 1l5 5 5-5'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  padding-right: 2.5rem;
}

/* CURRENCY INPUT */
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

input[type="checkbox"] {
  width: 20px;
  height: 20px;
  cursor: pointer;
  margin-top: 2px;
  accent-color: #667eea;
  flex-shrink: 0;
}

.checkbox-label {
  margin: 0;
  cursor: pointer;
  font-weight: normal;
  color: #333;
  padding: 2px 0;
}

/* HELP & ERROR TEXT */
.help-text {
  margin: 0.5rem 0 0 0;
  color: #666;
  font-size: 0.85rem;
  line-height: 1.4;
}

.error-text {
  margin: 0.5rem 0 0 0;
  color: #f44336;
  font-size: 0.85rem;
  font-weight: 500;
}

/* RESPONSIVE */
@media (max-width: 480px) {
  .form-group {
    margin-bottom: 1.25rem;
  }

  input[type="text"],
  input[type="email"],
  input[type="tel"],
  input[type="number"],
  input[type="password"],
  textarea,
  select {
    font-size: 16px; /* Evita zoom automático en iOS */
  }

  label {
    font-size: 0.9rem;
  }
}
</style>
