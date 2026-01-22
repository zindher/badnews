<template>
  <div class="terms-page">
    <header class="terms-header">
      <router-link to="/" class="back-link">← Volver</router-link>
      <h1>Términos y Condiciones</h1>
      <p class="subtitle">Gritalo - Política de servicio</p>
    </header>

    <main class="terms-main">
      <div class="terms-container">
        <article class="terms-article">
          <!-- 1. Servicio -->
          <section class="terms-section">
            <h2>1. Descripción del Servicio</h2>
            <p>
              BadNews es una plataforma que conecta a compradores con mensajeros profesionales para realizar llamadas personalizadas de entrega de noticias. El servicio utiliza un sistema de reintento automático y fallback a SMS cuando sea necesario.
            </p>
          </section>

          <!-- 2. Sistema 3x3 -->
          <section class="terms-section">
            <h2>2. Sistema de Reintento (3x3)</h2>
            <p>Nuestro sistema utiliza una estrategia probada de reintento:</p>
            <ul>
              <li><strong>Periodo:</strong> 3 días consecutivos</li>
              <li><strong>Intentos:</strong> Máximo 3 llamadas por día</li>
              <li><strong>Total:</strong> Hasta 9 intentos de contacto en 3 días</li>
              <li><strong>Horarios:</strong> Respetando zonas horarias y horarios preferidos</li>
            </ul>
            <div class="emphasis-box">
              <p>BadNews se compromete a realizar todos los intentos posibles dentro del periodo establecido.</p>
            </div>
          </section>

          <!-- 3. Fallback a SMS -->
          <section class="terms-section">
            <h2>3. Sistema de Fallback a SMS</h2>
            <p>Si no se logra contacto en los intentos de llamada:</p>
            <ul>
              <li>Envía un SMS con el mensaje automáticamente</li>
              <li>El SMS incluye el contenido de forma resumida</li>
              <li>Se considera entrega exitosa de información</li>
            </ul>
            <div class="emphasis-box">
              <p>BadNews no es responsable de entregas fallidas de SMS por parte del proveedor.</p>
            </div>
          </section>

          <!-- 4. Política de Reembolsos -->
          <section class="terms-section highlight-section">
            <h2>4. Política de Reembolsos</h2>
            <p class="important">BadNews completa su ciclo de servicio cuando:</p>
            <ul>
              <li>✓ Realiza los 3 intentos de llamada por cada día durante 3 días</li>
              <li>✓ Respeta horarios y zonas horarias configuradas</li>
              <li>✓ Intenta contacto a través de SMS como método final</li>
            </ul>
            <p class="important">NO se otorgan reembolsos en estos casos:</p>
            <ul>
              <li>✗ Número de teléfono incorrecto o inactivo</li>
              <li>✗ Número bloqueado u ocupado permanentemente</li>
              <li>✗ Dispositivo apagado o sin cobertura</li>
              <li>✗ Problemas técnicos de la operadora</li>
              <li>✗ El destinatario rechaza la llamada</li>
              <li>✗ Mensaje entregado por SMS</li>
            </ul>
          </section>

          <!-- 5. Responsabilidades Usuario -->
          <section class="terms-section">
            <h2>5. Responsabilidades del Usuario</h2>
            <p>Como comprador, usted es responsable de:</p>
            <ul>
              <li>Proporcionar números de teléfono correctos y válidos</li>
              <li>Asegurar capacidad de recibir llamadas internacionales</li>
              <li>Informar zonas horarias precisamente</li>
              <li>Proporcionar mensajes claros y legales</li>
              <li>No usar el servicio para spam o acoso</li>
              <li>Cambiar horarios hasta 24 horas antes del servicio</li>
            </ul>
          </section>

          <!-- 6. Responsabilidades BadNews -->
          <section class="terms-section">
            <h2>6. Responsabilidades de BadNews</h2>
            <p>BadNews se compromete a:</p>
            <ul>
              <li>Realizar todos los intentos programados dentro del periodo 3x3</li>
              <li>Respetar zonas horarias configuradas</li>
              <li>Mantener confidencialidad de datos</li>
              <li>Grabar llamadas para control de calidad</li>
              <li>Enviar SMS como fallback después de reintentos</li>
              <li>Proporcionar reporte detallado de intentos</li>
              <li>Mantener servicio disponible durante el periodo contratado</li>
            </ul>
          </section>

          <!-- 7. Limitaciones -->
          <section class="terms-section">
            <h2>7. Limitaciones de Responsabilidad</h2>
            <p>BadNews no es responsable por:</p>
            <ul>
              <li>Fallos en redes de telefonía</li>
              <li>Números inactivos o incorrectos proporcionados</li>
              <li>Restricciones técnicas de operadoras</li>
              <li>Cambios de política de operadoras</li>
              <li>Daño emocional causado por el mensaje</li>
              <li>Consecuencias de la entrega</li>
            </ul>
          </section>

          <!-- 8. Aceptación -->
          <section class="terms-section">
            <h2>8. Aceptación de Términos</h2>
            <p>Al crear su cuenta, usted:</p>
            <ul>
              <li>✓ Ha leído estos términos completos</li>
              <li>✓ Entiende la política de reembolsos</li>
              <li>✓ Acepta el sistema 3x3 de entrega</li>
              <li>✓ Acepta que NO hay reembolso después del ciclo</li>
              <li>✓ Asume responsabilidad de números correctos</li>
              <li>✓ Acepta las limitaciones de responsabilidad</li>
            </ul>
            <div class="emphasis-box">
              <p>Estos términos aplican a todos los servicios. Cambios futuros serán notificados 30 días antes.</p>
            </div>
          </section>

          <section class="terms-section">
            <p class="last-updated">Última actualización: 22 de Enero de 2026</p>
          </section>
        </article>

        <div class="terms-footer-btn">
          <router-link to="/login" class="btn-primary">Volver a Inicio de Sesión</router-link>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
export default {
  name: 'TermsAndConditionsPage',
  meta: {
    title: 'Términos y Condiciones - BadNews'
  }
};
</script>

<style scoped>
/* All styles in global.css */
</style>
