<template>
  <div class="profile">
    <h2>Mi Perfil</h2>
    <div class="profile-card">
      <p>Perfil de usuario - Funcionalidad próximamente</p>
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
.profile {
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
}

h2 {
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.profile-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  color: #666;
}

@media (min-width: 640px) {
  h2 {
    font-size: 1.8rem;
  }

  .profile-card {
    padding: 2rem;
  }
}

@media (min-width: 1024px) {
  h2 {
    font-size: 2rem;
  }

  .profile-card:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  }
}
</style>
