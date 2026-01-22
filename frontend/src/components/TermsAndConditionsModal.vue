<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeWithoutAccept">
    <div class="modal-content terms-modal">
      <!-- Header -->
      <div class="modal-header">
        <h2>Términos y Condiciones</h2>
        <button v-if="!isRequired" class="close-btn" @click="closeWithoutAccept">×</button>
      </div>

      <!-- Terms Content -->
      <div class="modal-body">
        <div class="terms-scroll">
          <!-- 1. Servicio -->
          <section class="terms-section">
            <h3>1. Descripción del Servicio</h3>
            <p>
              BadNews es una plataforma que conecta a compradores con mensajeros profesionales para realizar llamadas personalizadas de entrega de noticias. El servicio utiliza un sistema de reintento automático y fallback a SMS cuando sea necesario.
            </p>
          </section>

          <!-- 2. Sistema de Reintento 3x3 -->
          <section class="terms-section">
            <h3>2. Sistema de Reintento (3x3)</h3>
            <p>
              Nuestro sistema utiliza una estrategia probada de reintento:
            </p>
            <ul>
              <li><strong>Periodo:</strong> 3 días consecutivos</li>
              <li><strong>Intentos:</strong> Máximo 3 llamadas por día</li>
              <li><strong>Total:</strong> Hasta 9 intentos de contacto en 3 días</li>
              <li><strong>Horarios:</strong> Respetando zonas horarias y horarios preferidos del destinatario</li>
            </ul>
            <p class="emphasis">
              BadNews se compromete a realizar todos los intentos posibles dentro del periodo establecido. Sin embargo, circunstancias fuera de nuestro control (números inactivos, bloqueados, conexiones fallidas, dispositivos apagados) pueden impedir el contacto.
            </p>
          </section>

          <!-- 3. Fallback a SMS -->
          <section class="terms-section">
            <h3>3. Sistema de Fallback a SMS</h3>
            <p>
              Si después de completar los 9 intentos de llamada no se logra contacto, el sistema automáticamente:
            </p>
            <ul>
              <li>Envía un SMS con el mensaje a través de nuestros proveedores de SMS</li>
              <li>El SMS incluye el contenido del mensaje de forma resumida</li>
              <li>Se considera un fallback exitoso como entrega de información</li>
            </ul>
            <p class="emphasis">
              El SMS se envía una única vez como método de entrega final. BadNews no es responsable de entregas fallidas de SMS por parte del proveedor de telefonía.
            </p>
          </section>

          <!-- 4. Política de Reembolsos -->
          <section class="terms-section highlight-section">
            <h3>4. Política de Reembolsos</h3>
            <p class="important">
              <strong>BadNews completa su ciclo de servicio</strong> cuando:
            </p>
            <ul>
              <li>✓ Realiza los 3 intentos de llamada por cada día durante 3 días</li>
              <li>✓ Respeta horarios y zonas horarias configuradas</li>
              <li>✓ Intenta contacto a través de SMS como método final de entrega</li>
            </ul>
            <p class="important">
              <strong>NO se otorgan reembolsos</strong> en los siguientes casos:
            </p>
            <ul>
              <li>✗ Número de teléfono incorrecto o inactivo proporcionado por el comprador</li>
              <li>✗ Número bloqueado, ocupado permanentemente o no disponible</li>
              <li>✗ Dispositivo apagado, sin cobertura o fuera de rango del servicio</li>
              <li>✗ Problemas técnicos de la operadora de telefonía (ajenos a BadNews)</li>
              <li>✗ El destinatario rechaza la llamada intencionalmente</li>
              <li>✗ Mensaje entregado por SMS exitosamente</li>
            </ul>
            <p class="emphasis">
              <strong>BadNews actúa como intermediario de entrega de información.</strong> Una vez completado el ciclo 3x3 + SMS, BadNews ha cumplido su servicio contractual. La responsabilidad de proporcionar números correctos y válidos es del comprador.
            </p>
          </section>

          <!-- 5. Responsabilidades del Usuario -->
          <section class="terms-section">
            <h3>5. Responsabilidades del Usuario (Comprador)</h3>
            <p>Como comprador, usted es responsable de:</p>
            <ul>
              <li>Proporcionar número(s) de teléfono <strong>correctos y válidos</strong></li>
              <li>Asegurar que el destinatario pueda recibir llamadas internacionales (si aplica)</li>
              <li>Informar sobre zonas horarias y horarios preferidos de forma precisa</li>
              <li>Proporcionar mensajes claros, legales y no discriminatorios</li>
              <li>No utilizar el servicio para spam, acoso o contenido ilegal</li>
              <li>Entender que puede cambiar el horario de llamada hasta 24 horas antes del servicio</li>
            </ul>
          </section>

          <!-- 6. Responsabilidades de BadNews -->
          <section class="terms-section">
            <h3>6. Responsabilidades de BadNews</h3>
            <p>BadNews se compromete a:</p>
            <ul>
              <li>Realizar todos los intentos programados dentro del periodo 3x3</li>
              <li>Respetar zonas horarias y horarios configurados</li>
              <li>Mantener confidencialidad de datos de llamadas</li>
              <li>Grabar llamadas para control de calidad (cuando sea legal)</li>
              <li>Enviar SMS como fallback después de completar reintentos</li>
              <li>Proporcionar reporte detallado de intentos de entrega</li>
              <li>Mantener servicio disponible durante el periodo contratado</li>
            </ul>
          </section>

          <!-- 7. Limitaciones de Responsabilidad -->
          <section class="terms-section">
            <h3>7. Limitaciones de Responsabilidad</h3>
            <p>
              BadNews no es responsable por:
            </p>
            <ul>
              <li>Fallos en redes de telefonía o proveedores de telecomunicaciones</li>
              <li>Números de teléfono inactivos o incorrectos proporcionados por el usuario</li>
              <li>Restricciones técnicas de operadoras (bloqueos, límites de llamadas)</li>
              <li>Cambios de política de operadoras durante el servicio</li>
              <li>Daño emocional o psicológico causado por el contenido del mensaje</li>
              <li>Consecuencias de la entrega del mensaje (reacciones del destinatario)</li>
            </ul>
          </section>

          <!-- 8. Modificaciones de Servicios -->
          <section class="terms-section">
            <h3>8. Modificaciones de Servicio</h3>
            <p>
              Una vez iniciado el ciclo de 3 días:
            </p>
            <ul>
              <li>Puede cambiar <strong>horarios</strong> hasta 24 horas antes</li>
              <li>Puede cambiar <strong>mensaje</strong> hasta 24 horas antes</li>
              <li>NO puede cambiar número de teléfono (debe crear nuevo encargo)</li>
              <li>NO puede cancelar después de iniciados los intentos (sin refund)</li>
            </ul>
          </section>

          <!-- 9. Privacidad y Datos -->
          <section class="terms-section">
            <h3>9. Privacidad y Datos</h3>
            <p>
              BadNews maneja datos sensibles. Usted acepta que:
            </p>
            <ul>
              <li>Números de teléfono serán almacenados por el periodo del servicio</li>
              <li>Se realizarán grabaciones de audio para control de calidad</li>
              <li>Datos se eliminan después de 90 días del servicio (según normativas locales)</li>
              <li>BadNews puede usar estadísticas anonimizadas para mejorar servicio</li>
            </ul>
          </section>

          <!-- 10. Aceptación y Vigencia -->
          <section class="terms-section">
            <h3>10. Aceptación de Términos</h3>
            <p>
              Al crear su cuenta y hacer clic en "Acepto Términos y Condiciones", usted:
            </p>
            <ul>
              <li>✓ Ha leído estos términos completos</li>
              <li>✓ Entiende la política de reembolsos</li>
              <li>✓ Acepta el sistema 3x3 como mecanismo de entrega</li>
              <li>✓ Acepta que NO hay reembolso una vez completado el ciclo</li>
              <li>✓ Asume responsabilidad de números correctos</li>
              <li>✓ Acepta las limitaciones de responsabilidad de BadNews</li>
            </ul>
            <p class="emphasis">
              Estos términos tienen validez indefinida y aplican a todos los servicios contratados en BadNews. Cambios futuros serán notificados 30 días antes.
            </p>
          </section>

          <!-- Última actualización -->
          <section class="terms-section">
            <p class="last-updated">
              <small>Última actualización: 21 de Enero de 2026</small>
            </p>
          </section>
        </div>
      </div>

      <!-- Acceptance Checkbox and Buttons -->
      <div class="modal-footer">
        <label class="acceptance-checkbox">
          <input 
            v-model="hasAccepted" 
            type="checkbox" 
            :disabled="!termsRead"
          />
          <span>He leído y acepto los Términos y Condiciones</span>
        </label>

        <div class="action-buttons">
          <button 
            v-if="!isRequired"
            class="btn btn-secondary" 
            @click="closeWithoutAccept"
          >
            Cancelar
          </button>
          <button 
            class="btn btn-primary" 
            :disabled="!hasAccepted"
            @click="acceptTerms"
          >
            Acepto y Continuar
          </button>
        </div>
      </div>

      <!-- Scroll Indicator -->
      <div v-if="!termsRead" class="scroll-indicator">
        ↓ Scroll para leer los términos completos
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TermsAndConditionsModal',
  props: {
    isOpen: {
      type: Boolean,
      required: true
    },
    isRequired: {
      type: Boolean,
      default: false // true si es requisito (en registro), false si es opcional
    }
  },
  emits: ['accept', 'reject', 'close'],
  data() {
    return {
      hasAccepted: false,
      termsRead: false,
      scrollPercentage: 0
    };
  },
  methods: {
    handleScroll(event) {
      const element = event.target;
      const scrollPercentage = (element.scrollTop / (element.scrollHeight - element.clientHeight)) * 100;
      this.scrollPercentage = scrollPercentage;
      
      // Marcar como leído si scrollea 90% o más
      if (scrollPercentage >= 90) {
        this.termsRead = true;
      }
    },
    acceptTerms() {
      if (this.hasAccepted) {
        this.$emit('accept', {
          accepted: true,
          timestamp: new Date().toISOString()
        });
      }
    },
    closeWithoutAccept() {
      if (!this.isRequired) {
        this.hasAccepted = false;
        this.termsRead = false;
        this.$emit('close');
      } else {
        this.$emit('reject');
      }
    }
  },
  watch: {
    isOpen(newVal) {
      if (!newVal) {
        this.hasAccepted = false;
        this.termsRead = false;
        this.scrollPercentage = 0;
      }
    }
  }
};
</script>

<style scoped>
/* Modal styling - all global styles are in global.css */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.terms-modal {
  width: 90vw;
  max-width: 700px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
  border-bottom: 1px solid #e2e8f0;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.75rem;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: background 0.2s;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 0;
}

.terms-scroll {
  padding: 1.5rem;
}

.terms-section {
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e2e8f0;
}

.terms-section:last-of-type {
  border-bottom: none;
}

.terms-section h3 {
  margin: 0 0 0.75rem 0;
  color: var(--primary-color);
  font-size: 1.1rem;
}

.terms-section p {
  margin: 0.5rem 0;
  color: #333;
  line-height: 1.6;
  font-size: 0.95rem;
}

.terms-section ul {
  margin: 0.75rem 0;
  padding-left: 1.5rem;
}

.terms-section li {
  margin-bottom: 0.5rem;
  color: #333;
  line-height: 1.6;
  font-size: 0.95rem;
}

.emphasis {
  font-style: italic;
  color: #666;
}

.important {
  font-weight: 700;
  color: var(--primary-color);
}

.highlight-section {
  background: #f0fdf4;
  border-left: 4px solid #10b981;
  padding: 1rem;
  border-radius: 4px;
  margin: 1rem 0;
}

.last-updated {
  color: #999;
  font-size: 0.85rem;
  display: block;
  text-align: center;
}

.modal-footer {
  padding: 1.5rem;
  background: #f9fafb;
  border-top: 1px solid #e2e8f0;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.acceptance-checkbox {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
}

.acceptance-checkbox input {
  width: 20px;
  height: 20px;
  cursor: pointer;
  accent-color: var(--primary-color);
}

.acceptance-checkbox input:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.action-buttons {
  display: flex;
  gap: 1rem;
}

.btn {
  flex: 1;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: linear-gradient(135deg, var(--primary-color) 0%, #4a3d7a 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(231, 76, 60, 0.3);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  background: white;
  color: #666;
  border: 1px solid #e2e8f0;
}

.btn-secondary:hover {
  background: #f0f0f0;
}

.scroll-indicator {
  text-align: center;
  padding: 0.75rem;
  background: #fff3cd;
  color: #856404;
  font-size: 0.85rem;
  border-top: 1px solid #ffc107;
}

@media (max-width: 640px) {
  .terms-modal {
    width: 95vw;
    max-height: 90vh;
  }

  .modal-header h2 {
    font-size: 1.25rem;
  }

  .action-buttons {
    flex-direction: column;
  }
}
</style>

