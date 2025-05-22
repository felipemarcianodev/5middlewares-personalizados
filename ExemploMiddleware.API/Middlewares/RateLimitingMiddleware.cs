
namespace ExemploMiddleware.API.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly int _requestLimit;
        private static readonly object _lock = new();
        private readonly static Dictionary<string, Queue<DateTime>> _requestStore = new();
        private readonly TimeSpan _timeWindow;
        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            int requestLimit = 100,
            int timeWindowsSeconds = 60
            )
        {
            _next = next;
            _logger = logger;
            _requestLimit = requestLimit;
            _timeWindow = TimeSpan.FromSeconds(timeWindowsSeconds);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = GetClientIpAddress(context);
            var now = DateTime.UtcNow;

            if(!IsRequestAllowed(ipAddress, now))
            {
                _logger.LogWarning("Taxa de requisições excedida para o IP: {IpAddress}", ipAddress);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.Add("Retry-After", "60");

                await context.Response.WriteAsync("Muitas requisições. Por favor, tente novamente mais tarde!");

                return;
            }

            await _next(context);
        }

        private bool IsRequestAllowed(string ipAddress, DateTime requestTime)
        {
            lock(_lock)
            {
                if(!_requestStore.TryGetValue(ipAddress, out var requestQueue))
                {
                    requestQueue = new Queue<DateTime>();
                    _requestStore[ipAddress] = requestQueue;
                }

                while(requestQueue.Count > 0 && requestTime - requestQueue.Peek() > _timeWindow)
                {
                    requestQueue.Dequeue();
                }

                if(requestQueue.Count >= _requestLimit)
                {
                    return false;
                }

                requestQueue.Enqueue(requestTime);
                return true;
            }
        }

        private string GetClientIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
