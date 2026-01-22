using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace BadNews.Services;

public class EmailService : IEmailService
{
        private readonly SendGridClient _sendGridClient;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(string apiKey, string fromEmail, string fromName)
        {
            _sendGridClient = new SendGridClient(apiKey);
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendOrderConfirmationAsync(string toEmail, string toName, string orderId, decimal amount)
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, toName);
            var subject = $"Orden #{orderId} - Confirmación";

            var htmlContent = $@"
                <h2>¡Tu orden ha sido confirmada!</h2>
                <p>Hola {toName},</p>
                <p>Tu orden <strong>#{orderId}</strong> ha sido registrada exitosamente.</p>
                <p><strong>Monto: ${amount:F2}</strong></p>
                <p>Un mensajero seleccionado te contactará pronto para realizar la llamada personalizada.</p>
                <p>Agradecemos tu preferencia.</p>
                <p>Saludos,<br/>El equipo de BadNews</p>
            ";

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(to);

            await _sendGridClient.SendEmailAsync(msg);
        }

        public async Task SendOrderAcceptedAsync(string buyerEmail, string buyerName, string messengerName, string orderId)
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(buyerEmail, buyerName);
            var subject = $"¡Un mensajero aceptó tu orden #{orderId}!";

            var htmlContent = $@"
                <h2>¡Tu orden fue aceptada!</h2>
                <p>Hola {buyerName},</p>
                <p>El mensajero <strong>{messengerName}</strong> ha aceptado tu orden <strong>#{orderId}</strong>.</p>
                <p>Te contactaremos pronto para coordinar la llamada personalizada.</p>
                <p>Saludos,<br/>El equipo de BadNews</p>
            ";

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(to);

            await _sendGridClient.SendEmailAsync(msg);
        }

        public async Task SendPaymentSuccessAsync(string email, string name, string orderId, decimal amount, string paymentId)
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(email, name);
            var subject = $"Pago confirmado - Orden #{orderId}";

            var htmlContent = $@"
                <h2>Tu pago ha sido procesado correctamente</h2>
                <p>Hola {name},</p>
                <p>Tu pago para la orden <strong>#{orderId}</strong> ha sido confirmado.</p>
                <p><strong>Monto pagado: ${amount:F2}</strong></p>
                <p><strong>ID de transacción: {paymentId}</strong></p>
                <p>Pronto un mensajero seleccionado realizará la llamada personalizada.</p>
                <p>Saludos,<br/>El equipo de BadNews</p>
            ";

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(to);

            await _sendGridClient.SendEmailAsync(msg);
        }

        public async Task SendPaymentFailedAsync(string email, string name, string orderId, string reason)
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(email, name);
            var subject = $"Error en el pago - Orden #{orderId}";

            var htmlContent = $@"
                <h2>Hubo un problema con tu pago</h2>
                <p>Hola {name},</p>
                <p>No pudimos procesar tu pago para la orden <strong>#{orderId}</strong>.</p>
                <p><strong>Motivo:</strong> {reason}</p>
                <p>Por favor intenta nuevamente o contacta a nuestro equipo de soporte.</p>
                <p>Saludos,<br/>El equipo de BadNews</p>
            ";

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(to);

            await _sendGridClient.SendEmailAsync(msg);
        }

        public async Task SendCallReminderAsync(string email, string name, string messengerName, string orderId)
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(email, name);
            var subject = $"Recordatorio: Tu llamada personalizada llegará pronto";

            var htmlContent = $@"
                <h2>Tu mensajero está en camino</h2>
                <p>Hola {name},</p>
                <p>Este es un recordatorio de que {messengerName} hará tu llamada personalizada pronto (Orden #{orderId}).</p>
                <p>Asegúrate de estar disponible en el número registrado.</p>
                <p>Saludos,<br/>El equipo de BadNews</p>
            ";

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(to);

            await _sendGridClient.SendEmailAsync(msg);
        }

        public async Task SendEarningsNotificationAsync(string email, string name, decimal earnings, string period)
        {
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(email, name);
            var subject = $"Tus ganancias de {period} están listas";

            var htmlContent = $@"
                <h2>Tus ganancias han sido calculadas</h2>
                <p>Hola {name},</p>
                <p>Tus ganancias del {period} son <strong>${earnings:F2}</strong>.</p>
                <p>Inicia sesión en tu dashboard para ver los detalles y solicitar un retiro.</p>
                <p>Saludos,<br/>El equipo de BadNews</p>
            ";

            var msg = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(to);

            await _sendGridClient.SendEmailAsync(msg);
        }
    }