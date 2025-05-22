using System.Threading.Tasks;

namespace ExemploMiddleware.API.Middlewares
{
    public class MyCustomMiddleware
    {
        private readonly RequestDelegate _next;

        public MyCustomMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            //Lógica antes do próximo Middleware

            await _next(context);

            //Lógica após o próximo middleware
        }
    }
}
