<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Gritalo 📢</h1>
      
      <!-- TAB SELECTOR -->
      <div class="tab-selector">
        <button 
          :class="{ active: isLogin }" 
          @click="isLogin = true"
          class="tab-btn"
        >
          Iniciar Sesión
        </button>
        <button 
          :class="{ active: !isLogin }" 
          @click="isLogin = false"
          class="tab-btn"
        >
          Registrarse
        </button>
      </div>

      <!-- LOGIN FORM -->
      <form v-if="isLogin" @submit.prevent="handleLogin" class="form">
        <h2>Bienvenido de vuelta</h2>
        <p class="subtitle">Inicia sesión en tu cuenta</p>
        
        <div class="form-group">
          <label>Email</label>
          <input 
            v-model="loginForm.email" 
            type="email" 
            placeholder="tu@email.com"
            required
          />
        </div>

        <div class="form-group">
          <label>Contraseña</label>
          <input 
            v-model="loginForm.password" 
            type="password" 
            placeholder="••••••••"
            required
          />
        </div>

        <button 
          type="submit" 
          class="btn btn-primary"
          :disabled="loading"
        >
          {{ loading ? 'Iniciando sesión...' : 'Iniciar Sesión' }}
        </button>

        <p v-if="error" class="error-message">{{ error }}</p>
      </form>

      <!-- REGISTER FORM -->
      <form v-else @submit.prevent="handleRegister" class="form">
        <h2>Únete a Gritalo</h2>
        <p class="subtitle">Crea tu cuenta en segundos</p>

        <div class="form-group">
          <label>Nombre Completo</label>
          <input 
            v-model="registerForm.name" 
            type="text" 
            placeholder="Juan Pérez"
            required
          />
        </div>

        <div class="form-group">
          <label>Email</label>
          <input 
            v-model="registerForm.email" 
            type="email" 
            placeholder="tu@email.com"
            required
          />
        </div>

        <div class="form-group">
          <label>Contraseña</label>
          <input 
            v-model="registerForm.password" 
            type="password" 
            placeholder="••••••••"
            required
          />
        </div>

        <div class="form-group">
          <label>¿Eres...?</label>
          <select v-model="registerForm.userType" required>
            <option value="">Selecciona un rol</option>
            <option value="buyer">Comprador (Quiero enviar encargos)</option>
            <option value="messenger">Mensajero (Quiero ganar dinero)</option>
          </select>
        </div>

        <button 
          type="submit" 
          class="btn btn-primary"
          :disabled="loading"
        >
          {{ loading ? 'Registrando...' : 'Registrarse' }}
        </button>

        <p v-if="error" class="error-message">{{ error }}</p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/userStore'

const router = useRouter()
const userStore = useUserStore()

const isLogin = ref(true)
const loading = ref(false)
const error = ref('')

const loginForm = ref({
  email: 'buyer@test.com',
  password: 'password123'
})

const registerForm = ref({
  name: '',
  email: '',
  password: '',
  userType: 'buyer'
})

const handleLogin = async () => {
  error.value = ''
  loading.value = true

  try {
    const success = await userStore.login(loginForm.value.email, loginForm.value.password)
    
    if (success) {
      router.push(userStore.user?.role === 'messenger' ? '/messenger/home' : '/orders')
    } else {
      error.value = 'Email o contraseña incorrectos'
    }
  } catch (err) {
    error.value = 'Error al iniciar sesión. Intenta más tarde.'
    console.error(err)
  } finally {
    loading.value = false
  }
}

const handleRegister = async () => {
  error.value = ''
  loading.value = true

  try {
    const success = await userStore.register(
      registerForm.value.email,
      registerForm.value.password,
      registerForm.value.name,
      registerForm.value.userType
    )

    if (success) {
      router.push(userStore.user?.role === 'messenger' ? '/messenger/home' : '/orders')
    } else {
      error.value = 'Error al registrarse. Intenta nuevamente.'
    }
  } catch (err) {
    error.value = 'Error al registrarse. Intenta más tarde.'
    console.error(err)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 1rem;
}

.auth-card {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
}

h1 {
  text-align: center;
  color: #667eea;
  margin-bottom: 2rem;
  font-size: 2rem;
}

.tab-selector {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  background: #f5f5f5;
  padding: 0.5rem;
  border-radius: 8px;
}

.tab-btn {
  flex: 1;
  padding: 0.75rem;
  border: none;
  background: transparent;
  color: #666;
  font-weight: 600;
  cursor: pointer;
  border-radius: 6px;
  transition: all 0.3s;
}

.tab-btn.active {
  background: white;
  color: #667eea;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.1);
}

.form h2 {
  color: #333;
  margin-bottom: 0.5rem;
  font-size: 1.5rem;
}

.subtitle {
  color: #999;
  margin-bottom: 1.5rem;
  font-size: 0.9rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
  font-size: 0.9rem;
}

input,
select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.3s;
  font-family: inherit;
}

input:focus,
select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.btn {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 1rem;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  margin-top: 1rem;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 20px rgba(102, 126, 234, 0.3);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  color: #e74c3c;
  margin-top: 1rem;
  text-align: center;
  font-size: 0.9rem;
}

@media (max-width: 480px) {
  .auth-card {
    padding: 1.5rem;
  }

  h1 {
    font-size: 1.5rem;
    margin-bottom: 1.5rem;
  }

  .form h2 {
    font-size: 1.3rem;
  }
}
</style>
