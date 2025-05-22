namespace ExemploMiddleware.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestTime = DateTime.UtcNow;
            var requestId = context.TraceIdentifier;

            var originalBodyStream = context.Request.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                _logger.LogInformation
                    (
                    "Requisição {RequestMethod} {RequestoPath} iniciada às {RequestTime} - ID: {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    requestTime,
                    requestId
                    );

                //Aqui passamos para o próximo middleware
                await _next(context);

                var elapsedMs = (DateTime.UtcNow - requestTime).TotalMilliseconds;

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

                _logger.LogInformation
                    (
                    "Resposta {StatusCode} para {RequestPath} completada em {ElapsedMs}ms - Id: {RequestId}",
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs,
                    requestId
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError
                    (ex,
                    "Erro ao processar {RequestMethod} {RequestPath} - ID: {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    requestId
                    );

                context.Response.Body = originalBodyStream;
                throw;
            }
        }
    }
}
