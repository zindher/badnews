<template>
  <div id="app">
    <header>
      <nav class="navbar">
        <div class="nav-container">
          <h1 class="logo">Gritalo 📢</h1>
          <button class="menu-toggle" @click="mobileMenuOpen = !mobileMenuOpen" :aria-expanded="mobileMenuOpen">
            <span></span>
            <span></span>
            <span></span>
          </button>
          <div class="nav-links" :class="{ active: mobileMenuOpen }">
            <router-link to="/" class="nav-link" @click="mobileMenuOpen = false">Home</router-link>
            <router-link to="/orders" class="nav-link" @click="mobileMenuOpen = false">Mis Encargos</router-link>
            <router-link to="/profile" class="nav-link" @click="mobileMenuOpen = false">Perfil</router-link>
          </div>
        </div>
      </nav>
    </header>
    <main class="main-content">
      <router-view></router-view>
    </main>
    <footer class="footer">
      <div class="footer-content">
        <p>&copy; 2026 Gritalo. Todos los derechos reservados.</p>
        <nav class="footer-links">
          <router-link to="/terms">Términos y Condiciones</router-link>
          <a href="mailto:soporte@gritalo.com">Soporte</a>
        </nav>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const mobileMenuOpen = ref(false)
</script>

<style scoped>
#app {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #333;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.navbar {
  padding: 1rem 0;
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1rem;
}

.logo {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  white-space: nowrap;
}

.menu-toggle {
  display: none;
  flex-direction: column;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
}

.menu-toggle span {
  width: 25px;
  height: 3px;
  background: white;
  margin: 4px 0;
  transition: 0.3s;
  border-radius: 2px;
}

.nav-links {
  display: flex;
  gap: 2rem;
  align-items: center;
}

.nav-link {
  color: white;
  text-decoration: none;
  transition: opacity 0.3s;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-weight: 500;
}

.nav-link:hover {
  opacity: 0.9;
  background: rgba(255, 255, 255, 0.1);
}

.nav-link.router-link-active {
  background: rgba(255, 255, 255, 0.2);
  opacity: 1;
}

.main-content {
  flex: 1;
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  padding: 1rem;
}

/* RESPONSIVE MOBILE */
@media (max-width: 768px) {
  .nav-container {
    padding: 0 0.75rem;
  }

  .logo {
    font-size: 1.2rem;
  }

  .menu-toggle {
    display: flex;
  }

  .nav-links {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    flex-direction: column;
    gap: 0;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    padding: 1rem 0;
    max-height: 0;
    overflow: hidden;
    transition: max-height 0.3s ease-in-out;
  }

  .nav-links.active {
    max-height: 300px;
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
  }

  .nav-link {
    width: 100%;
    padding: 1rem 1.5rem;
    border-radius: 0;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  .main-content {
    padding: 1rem 1rem;
  }
}

@media (max-width: 480px) {
  .logo {
    font-size: 1rem;
  }

  .nav-container {
    padding: 0 0.5rem;
  }

  .main-content {
    padding: 0.75rem 0.75rem;
  }
}

.footer {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem 1rem;
  margin-top: auto;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
  text-align: center;
}

.footer-content p {
  margin: 0;
  font-size: 0.95rem;
  opacity: 0.9;
}

.footer-links {
  display: flex;
  flex-direction: row;
  gap: 2rem;
  flex-wrap: wrap;
  justify-content: center;
}

.footer-links a,
.footer-links router-link {
  color: white;
  text-decoration: none;
  font-size: 0.95rem;
  transition: opacity 0.3s ease;
  border-bottom: 2px solid transparent;
  padding-bottom: 0.25rem;
}

.footer-links a:hover,
.footer-links router-link:hover {
  opacity: 0.8;
  border-bottom-color: white;
}

@media (min-width: 768px) {
  .footer {
    padding: 2.5rem 2rem;
  }

  .footer-content {
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
  }

  .footer-content p {
    margin: 0;
  }

  .footer-links {
    gap: 2rem;
  }
}

@media (max-width: 480px) {
  .footer {
    padding: 1.5rem 1rem;
  }

  .footer-content {
    gap: 1rem;
  }

  .footer-links {
    gap: 1rem;
    font-size: 0.9rem;
  }
}
</style>
