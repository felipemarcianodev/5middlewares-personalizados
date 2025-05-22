using System.Globalization;

namespace ExemploMiddleware.API.Middlewares
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var cultureQuery = context.Request.Query["culture"];

            if (!string.IsNullOrEmpty(cultureQuery)) 
            {
                var culture = new CultureInfo(cultureQuery);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            else
            {
                var culture = context.Request.Headers["Accept-Language"]
                    .FirstOrDefault()
                    ?.Split(',')
                    .FirstOrDefault()
                    ?.Split(';')
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(culture))
                {
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            await _next(context);
        }
    }
}
