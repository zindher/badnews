<template>
  <div class="home">
    <!-- HERO SECTION -->
    <section class="hero">
      <div class="hero-content">
        <div class="hero-logo">
          <Logo variant="full" />
        </div>
        <h2>Encargos de Llamadas Personalizadas</h2>
        <p class="hero-subtitle">Contrata a un profesional para entregar tus bromas, confesiones y mensajes especiales. ¡Hazlo memorable!</p>
        <div class="cta-buttons">
          <router-link to="/orders/new" class="btn btn-primary">Crear Encargo Ahora</router-link>
          <a href="#features" class="btn btn-secondary">Conocer Más</a>
          <a :href="apkUrl" class="btn btn-download" download="gritalo.apk">
            <span class="btn-download-icon">📲</span> Descargar App (Android)
          </a>
        </div>
        <p class="hero-footer">✨ Profesionales verificados • 🔒 100% Anónimo • 📱 Disponible en móvil</p>
      </div>
      <div class="hero-visual">
        <div class="phone-mockup">
          <div class="phone-screen">📞</div>
        </div>
      </div>
    </section>

    <!-- STATS SECTION -->
    <section class="stats">
      <div class="stat-item">
        <div class="stat-number">5000+</div>
        <div class="stat-label">Encargos Completados</div>
      </div>
      <div class="stat-item">
        <div class="stat-number">4.9★</div>
        <div class="stat-label">Calificación Promedio</div>
      </div>
      <div class="stat-item">
        <div class="stat-number">24/7</div>
        <div class="stat-label">Disponible Siempre</div>
      </div>
      <div class="stat-item">
        <div class="stat-number">100%</div>
        <div class="stat-label">Satisfacción Garantizada</div>
      </div>
    </section>

    <!-- FEATURES SECTION -->
    <section class="features" id="features">
      <h2>¿Por qué Gritalo?</h2>
      <div class="features-grid">
        <div class="feature-card">
          <div class="feature-icon">📞</div>
          <h3>Llamadas Profesionales</h3>
          <p>Mensajeros entrenados que entregan tu mensaje con la mejor actitud y profesionalismo</p>
        </div>
        <div class="feature-card">
          <div class="feature-icon">🎥</div>
          <h3>Grabación Incluida</h3>
          <p>Recibe la grabación completa de la llamada para que revivas el momento una y otra vez</p>
        </div>
        <div class="feature-card">
          <div class="feature-icon">🔒</div>
          <h3>100% Anónimo</h3>
          <p>El receptor no sabrá quién pagó por la llamada si así lo deseas. Total confidencialidad</p>
        </div>
        <div class="feature-card">
          <div class="feature-icon">🔄</div>
          <h3>Reintentos Automáticos</h3>
          <p>Si no contestan, intentamos 3 veces al día durante 3 días hasta conseguirlo</p>
        </div>
        <div class="feature-card">
          <div class="feature-icon">💳</div>
          <h3>Pago Seguro</h3>
          <p>Transacciones 100% seguras con Mercado Pago. Tu dinero protegido garantizado</p>
        </div>
        <div class="feature-card">
          <div class="feature-icon">⭐</div>
          <h3>Mensajeros Verificados</h3>
          <p>Todos nuestros profesionales pasan filtros de seguridad y verificación exhaustiva</p>
        </div>
      </div>
    </section>

    <!-- HOW IT WORKS SECTION -->
    <section class="how-it-works">
      <h2>4 Pasos Sencillos</h2>
      <div class="steps">
        <div class="step">
          <div class="step-number">1</div>
          <h3>Crea tu Encargo</h3>
          <p>Cuéntale al mensajero qué quieres que diga, a quién y cuándo</p>
        </div>
        <div class="step-arrow">→</div>
        <div class="step">
          <div class="step-number">2</div>
          <h3>Pago & Asignación</h3>
          <p>Realiza el pago y nuestro sistema asigna un mensajero verificado</p>
        </div>
        <div class="step-arrow">→</div>
        <div class="step">
          <div class="step-number">3</div>
          <h3>Mensajero Llamará</h3>
          <p>El mensajero realiza la llamada en el horario que especificaste</p>
        </div>
        <div class="step-arrow">→</div>
        <div class="step">
          <div class="step-number">4</div>
          <h3>Recibe la Grabación</h3>
          <p>Descarga el video y revive el momento especial</p>
        </div>
      </div>
    </section>

    <!-- TESTIMONIALS SECTION -->
    <section class="testimonials">
      <h2>Lo que Dicen Nuestros Clientes</h2>
      <div class="testimonials-grid">
        <div class="testimonial-card">
          <div class="stars">⭐⭐⭐⭐⭐</div>
          <p>"¡Fue lo más divertido que hemos hecho! Mi novio cayó redondo con la broma."</p>
          <div class="testimonial-author">- María, CDMX</div>
        </div>
        <div class="testimonial-card">
          <div class="stars">⭐⭐⭐⭐⭐</div>
          <p>"Super profesionales y al primer intento. La grabación quedó perfecta para mostrar."</p>
          <div class="testimonial-author">- Carlos, Guadalajara</div>
        </div>
        <div class="testimonial-card">
          <div class="stars">⭐⭐⭐⭐⭐</div>
          <p>"Confié mis datos sin preocupación. Gritalo es de verdad anónimo y seguro."</p>
          <div class="testimonial-author">- Ana, Monterrey</div>
        </div>
      </div>
    </section>

    <!-- PRICING SECTION -->
    <section class="pricing">
      <h2>Un Precio Simple y Justo</h2>
      <div class="pricing-container">
        <div class="pricing-card featured">
          <h3>Plan Gritalo</h3>
          <div class="price">$199<span>/llamada</span></div>
          <p class="pricing-subtitle">Incluye el 3 de 3: 3 intentos, 3 días, garantizado</p>
          <ul class="pricing-features">
            <li>✓ Hasta 5 minutos de llamada</li>
            <li>✓ Grabación en HD incluida</li>
            <li>✓ 3 intentos de llamada</li>
            <li>✓ 3 días para lograr la conexión</li>
            <li>✓ Mensajero verificado</li>
            <li>✓ Fallback a SMS si no contactamos</li>
          </ul>
          <router-link to="/terms" class="btn btn-primary">Comenzar Ahora</router-link>
        </div>
      </div>
    </section>

    <!-- FAQ SECTION -->
    <section class="faq">
      <h2>Preguntas Frecuentes</h2>
      <div class="faq-grid">
        <details class="faq-item">
          <summary>¿Es realmente anónimo?</summary>
          <p>Sí, 100%. El receptor ve un número de Gritalo, no el tuyo. Tu identidad está protegida completamente.</p>
        </details>
        <details class="faq-item">
          <summary>¿Qué pasa si no contesta?</summary>
          <p>Nuestro sistema reintenta automáticamente 3 veces al día durante 3 días (máximo 9 intentos). Si no hay contacto después, enviamos un SMS automático como último intento. Esto completa nuestro ciclo de servicio.</p>
        </details>
        <details class="faq-item">
          <summary>¿Es seguro mi pago?</summary>
          <p>Totalmente. Usamos Mercado Pago, el procesador de pagos más seguro en México con certificación PCI.</p>
        </details>
        <details class="faq-item">
          <summary>¿Cuánto tarda en completarse?</summary>
          <p>El ciclo completo toma 3 días (9 reintentos máximo). Si el receptor contesta antes, se completa inmediatamente. Si no contesta después de 3 días, se envía SMS automático como fallback final.</p>
        </details>
        <details class="faq-item">
          <summary>¿Puedo cambiar o cancelar mi encargo?</summary>
          <p>Sí, puedes cambiar horarios y mensaje hasta 24 horas antes de que inicie. No puedes cambiar el número de teléfono (debes crear un nuevo encargo). Cancelaciones post-inicio no tienen reembolso.</p>
        </details>
        <details class="faq-item">
          <summary>¿Hay reembolso si algo sale mal?</summary>
          <p>BadNews completa el ciclo 3x3 + SMS. Si el número es incorrecto o está inactivo, BadNews no es responsable. Lee nuestros <router-link to="/terms-conditions">Términos y Condiciones</router-link> para detalles.</p>
        </details>
      </div>
    </section>

    <!-- CTA FINAL SECTION -->
    <section class="cta-final">
      <h2>¿Listo para Gritar tu Mensaje?</h2>
      <p>Únete a miles de mexicanos que ya han creado momentos inolvidables</p>
      <router-link to="/orders/new" class="btn btn-primary btn-large">Crear mi Primer Encargo</router-link>
      <p class="disclaimer">Al crear un encargo, aceptas nuestros <router-link to="/terms">Términos y Condiciones</router-link></p>
    </section>
  </div>
</template>

<script setup>
import Logo from '../components/Logo.vue'

const apkUrl = import.meta.env.VITE_APK_DOWNLOAD_URL || 'https://github.com/zindher/badnews/releases/latest/download/app-release.apk'
</script>

<style scoped>
/* Styles specific to Home component only */
.hero-logo :deep(.logo-full) {
  width: 240px;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.2));
}

.btn-download {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: #27ae60;
  color: #fff;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.2s ease, transform 0.1s ease;
}

.btn-download:hover {
  background: #219a52;
  transform: translateY(-1px);
}

.btn-download-icon {
  font-size: 1.2rem;
}
</style>