using SendGrid;
using SendGrid.Helpers.Mail;

namespace BadNews.Services;

public class SendGridServiceImpl : ISendGridService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridServiceImpl> _logger;

    public SendGridServiceImpl(
        IConfiguration configuration,
        ILogger<SendGridServiceImpl> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private SendGridClient GetClient()
    {
        var apiKey = _configuration["SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid API key not configured");
        return new SendGridClient(apiKey);
    }

    /// <summary>
    /// Sends a generic email
    /// </summary>
    public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
    {
        try
        {
            var fromEmail = _configuration["SendGrid:FromEmail"]
                ?? throw new InvalidOperationException("SendGrid FromEmail not configured");

            var client = GetClient();
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, "BadNews"),
                Subject = subject,
                HtmlContent = htmlContent
            };

            msg.AddTo(new EmailAddress(to));

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation($"Email sent to {to}: {subject}");
                return true;
            }

            _logger.LogWarning($"Email send failed for {to}. Status: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {to}");
            return false;
        }
    }

    /// <summary>
    /// Sends order confirmation email
    /// </summary>
    public async Task<bool> SendOrderConfirmationAsync(
        string buyerEmail,
        string buyerName,
        int orderId,
        decimal amount)
    {
        var htmlContent = $@"
            <html>
                <body style=""font-family: Arial, sans-serif;"">
                    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                        <h1>Orden Confirmada</h1>
                        <p>Hola {buyerName},</p>
                        <p>Tu orden ha sido confirmada exitosamente.</p>
                        
                        <div style=""background-color: #f5f5f5; padding: 15px; margin: 20px 0;"">
                            <h2>Detalles de la Orden</h2>
                            <p><strong>Número de Orden:</strong> #{orderId}</p>
                            <p><strong>Monto:</strong> ${amount:F2} MXN</p>
                            <p><strong>Estado:</strong> Pendiente de Pago</p>
                        </div>
                        
                        <p>Pronto recibirás la grabación de la llamada una vez que sea completada.</p>
                        <p>Si tienes preguntas, no dudes en contactarnos.</p>
                        
                        <p>Saludos,<br/>BadNews Team</p>
                    </div>
                </body>
            </html>";

        return await SendEmailAsync(buyerEmail, $"Orden Confirmada #{orderId}", htmlContent);
    }

    /// <summary>
    /// Sends payment receipt email
    /// </summary>
    public async Task<bool> SendPaymentReceiptAsync(
        string buyerEmail,
        string buyerName,
        int orderId,
        decimal amount,
        string paymentId)
    {
        var htmlContent = $@"
            <html>
                <body style=""font-family: Arial, sans-serif;"">
                    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                        <h1>Pago Recibido</h1>
                        <p>Hola {buyerName},</p>
                        <p>Tu pago ha sido procesado exitosamente.</p>
                        
                        <div style=""background-color: #f5f5f5; padding: 15px; margin: 20px 0;"">
                            <h2>Recibo de Pago</h2>
                            <p><strong>Número de Orden:</strong> #{orderId}</p>
                            <p><strong>Monto:</strong> ${amount:F2} MXN</p>
                            <p><strong>Número de Transacción:</strong> {paymentId}</p>
                            <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>
                        
                        <p>Tu mensaje será entregado pronto. Te notificaremos cuando la llamada sea completada.</p>
                        
                        <p>Saludos,<br/>BadNews Team</p>
                    </div>
                </body>
            </html>";

        return await SendEmailAsync(buyerEmail, $"Recibo de Pago - Orden #{orderId}", htmlContent);
    }

    /// <summary>
    /// Sends call completion notification with recording link
    /// </summary>
    public async Task<bool> SendCallCompletionAsync(
        string buyerEmail,
        string buyerName,
        int orderId,
        string recordingUrl)
    {
        var htmlContent = $@"
            <html>
                <body style=""font-family: Arial, sans-serif;"">
                    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                        <h1>Llamada Completada</h1>
                        <p>Hola {buyerName},</p>
                        <p>Tu mensaje ha sido entregado exitosamente!</p>
                        
                        <div style=""background-color: #f5f5f5; padding: 15px; margin: 20px 0;"">
                            <h2>Detalles de la Llamada</h2>
                            <p><strong>Número de Orden:</strong> #{orderId}</p>
                            <p><strong>Estado:</strong> Completada</p>
                            <p><strong>Fecha de Entrega:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>
                        
                        <p style=""text-align: center; margin: 30px 0;"">
                            <a href=""{recordingUrl}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">
                                Escuchar Grabación
                            </a>
                        </p>
                        
                        <p>Por favor, sé honesto en tu calificación para ayudarnos a mejorar el servicio.</p>
                        
                        <p>Saludos,<br/>BadNews Team</p>
                    </div>
                </body>
            </html>";

        return await SendEmailAsync(buyerEmail, $"Llamada Completada - Orden #{orderId}", htmlContent);
    }

    /// <summary>
    /// Sends refund notification
    /// </summary>
    public async Task<bool> SendRefundNotificationAsync(
        string buyerEmail,
        string buyerName,
        int orderId,
        decimal amount,
        string reason)
    {
        var htmlContent = $@"
            <html>
                <body style=""font-family: Arial, sans-serif;"">
                    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                        <h1>Reembolso Procesado</h1>
                        <p>Hola {buyerName},</p>
                        <p>Tu reembolso ha sido procesado exitosamente.</p>
                        
                        <div style=""background-color: #f5f5f5; padding: 15px; margin: 20px 0;"">
                            <h2>Detalles del Reembolso</h2>
                            <p><strong>Número de Orden:</strong> #{orderId}</p>
                            <p><strong>Monto del Reembolso:</strong> ${amount:F2} MXN</p>
                            <p><strong>Razón:</strong> {reason}</p>
                            <p><strong>Fecha de Proceso:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>
                        
                        <p>El reembolso será reflejado en tu cuenta bancaria dentro de 3-5 días hábiles.</p>
                        
                        <p>Si tienes preguntas, no dudes en contactarnos.</p>
                        
                        <p>Saludos,<br/>BadNews Team</p>
                    </div>
                </body>
            </html>";

        return await SendEmailAsync(buyerEmail, $"Reembolso Procesado - Orden #{orderId}", htmlContent);
    }

    /// <summary>
    /// Sends messenger payment notification
    /// </summary>
    public async Task<bool> SendMessengerPaymentAsync(
        string messengerEmail,
        string messengerName,
        decimal amount,
        int callsCompleted)
    {
        var htmlContent = $@"
            <html>
                <body style=""font-family: Arial, sans-serif;"">
                    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                        <h1>Pago de Ganancias</h1>
                        <p>Hola {messengerName},</p>
                        <p>¡Felicidades! Has ganado dinero en BadNews.</p>
                        
                        <div style=""background-color: #f5f5f5; padding: 15px; margin: 20px 0;"">
                            <h2>Resumen de Ganancias</h2>
                            <p><strong>Llamadas Completadas:</strong> {callsCompleted}</p>
                            <p><strong>Monto Total:</strong> ${amount:F2} MXN</p>
                            <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy}</p>
                        </div>
                        
                        <p>Tu pago ha sido procesado y será transferido a tu cuenta bancaria.</p>
                        <p>Gracias por ser parte del equipo de BadNews!</p>
                        
                        <p>Saludos,<br/>BadNews Team</p>
                    </div>
                </body>
            </html>";

        return await SendEmailAsync(messengerEmail, "Pago de Ganancias Recibido", htmlContent);
    }

    /// <summary>
    /// Sends new order notification to messenger
    /// </summary>
    public async Task<bool> SendOrderNotificationAsync(
        string messengerEmail,
        string messengerName,
        int orderId,
        string recipientPhone,
        decimal amount)
    {
        var htmlContent = $@"
            <html>
                <body style=""font-family: Arial, sans-serif;"">
                    <div style=""max-width: 600px; margin: 0 auto; padding: 20px;"">
                        <h1>Nueva Orden Disponible</h1>
                        <p>Hola {messengerName},</p>
                        <p>¡Hay una nueva orden disponible para ti!</p>
                        
                        <div style=""background-color: #f5f5f5; padding: 15px; margin: 20px 0;"">
                            <h2>Detalles de la Orden</h2>
                            <p><strong>Número de Orden:</strong> #{orderId}</p>
                            <p><strong>Teléfono Destino:</strong> {recipientPhone}</p>
                            <p><strong>Pago:</strong> ${amount:F2} MXN</p>
                        </div>
                        
                        <p style=""text-align: center; margin: 30px 0;"">
                            <a href=""https://badnews.mx/orders/{orderId}"" style=""background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">
                                Ver Orden
                            </a>
                        </p>
                        
                        <p>Abre tu app para aceptar esta orden.</p>
                        
                        <p>Saludos,<br/>BadNews Team</p>
                    </div>
                </body>
            </html>";

        return await SendEmailAsync(messengerEmail, $"Nueva Orden Disponible - ${amount:F2} MXN", htmlContent);
    }
}
