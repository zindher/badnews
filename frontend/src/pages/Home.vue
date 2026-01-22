<template>
  <div class="home">
    <!-- HERO SECTION -->
    <section class="hero">
      <div class="hero-content">
        <h1>Gritalo 📢</h1>
        <h2>Encargos de Llamadas Personalizadas</h2>
        <p class="hero-subtitle">Contrata a un profesional para entregar tus bromas, confesiones y mensajes especiales. ¡Hazlo memorable!</p>
        <div class="cta-buttons">
          <router-link to="/orders/new" class="btn btn-primary">Crear Encargo Ahora</router-link>
          <a href="#features" class="btn btn-secondary">Conocer Más</a>
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
      <h2>3 Pasos Sencillos</h2>
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
            <li>✓ Dinero devuelto si no se logra</li>
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
          <p>Nuestro sistema reintenta automáticamente hasta 3 veces al día durante 3 días. Si sigue sin contestar, te devolvemos el dinero.</p>
        </details>
        <details class="faq-item">
          <summary>¿Es seguro mi pago?</summary>
          <p>Totalmente. Usamos Mercado Pago, el procesador de pagos más seguro en México con certificación PCI.</p>
        </details>
        <details class="faq-item">
          <summary>¿Cuánto tarda en completarse?</summary>
          <p>Típicamente 24-48 horas. Depende de la disponibilidad del mensajero y horarios.</p>
        </details>
        <details class="faq-item">
          <summary>¿Puedo cambiar mi encargo?</summary>
          <p>Sí, mientras no haya sido iniciado. Una vez que comience el intento, el cambio tiene costo.</p>
        </details>
        <details class="faq-item">
          <summary>¿Hay reembolso si algo sale mal?</summary>
          <p>100% garantizado. Si algo sale mal, reembolso total o re-intento gratuito. Tu satisfacción es nuestro éxito.</p>
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
</script>

<style scoped>
.home {
  width: 100%;
}

/* HERO SECTION */
.hero {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 3rem 1rem;
  text-align: center;
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
  align-items: center;
}

.hero-content h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  font-weight: 700;
}

.hero-content h2 {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  font-weight: 500;
  opacity: 0.95;
}

.hero-subtitle {
  font-size: 1rem;
  line-height: 1.5;
  margin-bottom: 1.5rem;
  opacity: 0.9;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.cta-buttons {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.3s;
  border: none;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 44px;
  font-size: 1rem;
}

.btn-primary {
  background: white;
  color: #667eea;
}

.btn-primary:active {
  transform: scale(0.98);
}

.btn-secondary {
  background: transparent;
  color: white;
  border: 2px solid white;
}

.btn-secondary:active {
  background: rgba(255, 255, 255, 0.1);
}

.btn-large {
  padding: 1rem 2rem;
  font-size: 1.1rem;
  min-height: 48px;
}

.hero-footer {
  font-size: 0.9rem;
  opacity: 0.8;
}

.hero-visual {
  display: none;
}

.phone-mockup {
  width: 200px;
  height: 400px;
  background: white;
  border-radius: 40px;
  padding: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
}

.phone-screen {
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 4rem;
}

/* STATS SECTION */
.stats {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
  padding: 2rem 1rem;
  background: white;
}

.stat-item {
  text-align: center;
}

.stat-number {
  font-size: 2rem;
  font-weight: 700;
  color: #667eea;
  margin-bottom: 0.5rem;
}

.stat-label {
  color: #666;
  font-size: 0.95rem;
}

/* FEATURES SECTION */
.features {
  padding: 3rem 1rem;
  background: #f9f9f9;
}

.features h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
}

.feature-card {
  background: white;
  padding: 2rem 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
  transition: all 0.3s;
}

.feature-card:active {
  transform: scale(0.98);
}

.feature-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.feature-card h3 {
  color: #667eea;
  margin-bottom: 0.75rem;
  font-size: 1.2rem;
}

.feature-card p {
  color: #666;
  line-height: 1.5;
  font-size: 0.95rem;
}

/* HOW IT WORKS SECTION */
.how-it-works {
  padding: 3rem 1rem;
  background: white;
}

.how-it-works h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.steps {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  grid-auto-rows: 1fr;
  gap: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
  align-items: stretch;
}

.step {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem 1.5rem;
  border-radius: 8px;
  text-align: center;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.step-number {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: 1rem;
  display: block;
}

.step h3 {
  margin-bottom: 0.75rem;
  font-size: 1.2rem;
}

.step p {
  font-size: 0.95rem;
  opacity: 0.9;
}

.step-arrow {
  display: none;
  text-align: center;
  font-size: 2rem;
  color: #667eea;
  font-weight: bold;
}

/* TESTIMONIALS SECTION */
.testimonials {
  padding: 3rem 1rem;
  background: #f9f9f9;
}

.testimonials h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.testimonials-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  max-width: 1000px;
  margin: 0 auto;
}

.testimonial-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #667eea;
}

.stars {
  margin-bottom: 1rem;
}

.testimonial-card p {
  color: #666;
  line-height: 1.6;
  margin-bottom: 1rem;
  font-style: italic;
}

.testimonial-author {
  color: #667eea;
  font-weight: 600;
  font-size: 0.9rem;
}

/* PRICING SECTION */
.pricing {
  padding: 3rem 1rem;
  background: white;
}

.pricing h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.pricing-container {
  display: flex;
  justify-content: center;
  max-width: 600px;
  margin: 0 auto;
}

.pricing-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem 1.5rem;
  border-radius: 12px;
  position: relative;
  transition: all 0.3s;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
}

.pricing-card:active {
  transform: scale(0.98);
}

.pricing-card h3 {
  color: white;
  margin-bottom: 0.5rem;
  font-size: 1.5rem;
}

.pricing-subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.95rem;
  margin-bottom: 1.5rem;
  font-weight: 500;
}

.price {
  font-size: 2.5rem;
  color: white;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.price span {
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 400;
}

.pricing-features {
  list-style: none;
  margin-bottom: 1.5rem;
  text-align: left;
}

.pricing-features li {
  padding: 0.7rem 0;
  color: rgba(255, 255, 255, 0.95);
  font-size: 0.95rem;
}

.pricing-card .btn {
  width: 100%;
}

/* FAQ SECTION */
.faq {
  padding: 3rem 1rem;
  background: #f9f9f9;
}

.faq h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.faq-grid {
  display: grid;
  gap: 1rem;
  max-width: 800px;
  margin: 0 auto;
}

.faq-item {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  cursor: pointer;
  border: 1px solid #eee;
}

.faq-item summary {
  font-weight: 600;
  color: #333;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.faq-item summary:hover {
  color: #667eea;
}

.faq-item p {
  margin-top: 1rem;
  color: #666;
  line-height: 1.6;
}

/* CTA FINAL SECTION */
.cta-final {
  padding: 3rem 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-align: center;
}

.cta-final h2 {
  font-size: 2rem;
  margin-bottom: 1rem;
}

.cta-final > p:nth-of-type(1) {
  font-size: 1.1rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.disclaimer {
  font-size: 0.85rem;
  margin-top: 1.5rem;
  opacity: 0.8;
}

.disclaimer a {
  color: white;
  text-decoration: underline;
}

/* TABLET */
@media (min-width: 640px) {
  .hero {
    padding: 4rem 2rem;
  }

  .hero-content h1 {
    font-size: 2.5rem;
  }

  .hero-content h2 {
    font-size: 1.8rem;
  }

  .cta-buttons {
    flex-direction: row;
    justify-content: center;
  }

  .stats {
    grid-template-columns: repeat(4, 1fr);
    padding: 3rem 2rem;
  }

  .features {
    padding: 4rem 2rem;
  }

  .features-grid {
    grid-template-columns: repeat(3, 1fr);
  }

  .how-it-works {
    padding: 4rem 2rem;
  }

  .steps {
    grid-template-columns: repeat(4, 1fr);
    gap: 2rem;
  }

  .step-arrow {
    display: none;
  }

  .testimonials {
    padding: 4rem 2rem;
  }

  .pricing {
    padding: 4rem 2rem;
  }

  .cta-final {
    padding: 4rem 2rem;
  }
}

/* DESKTOP */
@media (max-width: 768px) {
  .steps {
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
  }

  .step {
    min-height: auto;
    padding: 1.5rem 1rem;
  }
}

@media (max-width: 480px) {
  .steps {
    grid-template-columns: 1fr;
  }

  .how-it-works h2 {
    font-size: 1.5rem;
  }
}

@media (min-width: 1024px) {
  .hero {
    grid-template-columns: 1fr 1fr;
  }

  .hero-visual {
    display: flex;
    justify-content: center;
  }

  .hero-content h1 {
    font-size: 3.5rem;
  }

  .hero-content h2 {
    font-size: 2.2rem;
  }

  .btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(255, 255, 255, 0.3);
  }

  .btn-secondary:hover {
    background: rgba(255, 255, 255, 0.2);
  }

  .feature-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
  }

  .pricing-card:hover {
    transform: translateY(-4px);
  }

  .faq-item:hover {
    border-color: #667eea;
  }
}
</style>
