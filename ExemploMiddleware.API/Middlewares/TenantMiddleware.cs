namespace ExemploMiddleware.API.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(
            RequestDelegate next,
            ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            var host = context.Request.Host.Value;
            var tenant = host.Split('.').FirstOrDefault();

            if (string.IsNullOrEmpty(tenant))
            {
                context.Request.Headers.TryGetValue("X-Tenant", out var tentantHeader);

                tenant = tentantHeader.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(tenant))
            {
                await tenantService.SetCurrentTenantAsync(tenant);

                _logger.LogInformation("Tenant identificado: {Tenant}", tenant);

                context.Items["CurrentTenant"] = tenant;
            }
            else
            {
                _logger.LogWarning("Nenhum tenant identificado para requisição");
            }

            await _next(context);
        }
    }

    public interface ITenantService
    {
        Task SetCurrentTenantAsync(string tenant);
    }
}
