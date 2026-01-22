<template>
  <div class="logo" :class="{ 'logo-dark': isDark, 'logo-compact': isCompact }">
    <!-- Full variant with logo and text -->
    <img 
      v-if="variant === 'full'"
      src="/Logo.png" 
      alt="Gritalo Logo" 
      class="logo-image"
      :class="{ 'logo-compact': isCompact }"
    />
    
    <!-- Icon variant (logo without text) -->
    <img 
      v-else-if="variant === 'icon'"
      src="/iconoLogo.png" 
      alt="Gritalo" 
      class="logo-icon"
      :class="{ 'logo-compact': isCompact }"
    />
    
    <!-- Text variant -->
    <div v-else-if="variant === 'text'" class="logo-text-only">
      <span class="brand-name">Gritalo</span>
      <span class="brand-tagline">📢</span>
    </div>
  </div>
</template>

<script setup>
defineProps({
  variant: {
    type: String,
    default: 'icon', // 'icon', 'full', 'text'
    validator: (value) => ['icon', 'full', 'text'].includes(value)
  },
  isDark: {
    type: Boolean,
    default: false
  },
  isCompact: {
    type: Boolean,
    default: false
  }
});
</script>

<style scoped>
/* Specific Logo component styles only */
.logo {
  display: flex;
  align-items: center;
  justify-content: center;
}

.logo-image {
  height: 280px;
  width: auto;
  object-fit: contain;
  filter: drop-shadow(0 4px 12px rgba(231, 76, 60, 0.2));
}

.logo-image.logo-compact {
  height: 50px;
  width: auto;
}

.logo-icon {
  height: 60px;
  width: auto;
  object-fit: contain;
  filter: drop-shadow(0 2px 4px rgba(231, 76, 60, 0.15));
}

.logo-icon.logo-compact {
  height: 55px;
  width: auto;
}

/* Text Only Variant */
.logo-text-only {
  display: flex;
  align-items: center;
  gap: 8px;
}

.brand-name {
  color: #E74C3C;
  font-family: 'Poppins', 'Segoe UI', sans-serif;
  letter-spacing: -0.5px;
  font-weight: 800;
}

@media (max-width: 768px) {
  .logo-text-only {
    gap: 6px;
  }
}
</style>
