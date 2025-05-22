using ExemploMiddleware.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseMiddleware<CultureMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

app.UseMiddleware<MyCustomMiddleware>();

app.Use(async (context, next) =>
{
    //Lógica antes do próximo middleware

    await next();

    //Lógica após o próximo middlerware
});


app.UseAuthorization();

app.MapControllers();

app.Run(async (context) => 
{
    await context.Response.WriteAsync("Este middleware encerra o pipeline");
});
