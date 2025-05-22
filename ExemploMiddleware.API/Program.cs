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
    //L�gica antes do pr�ximo middleware

    await next();

    //L�gica ap�s o pr�ximo middlerware
});


app.UseAuthorization();

app.MapControllers();

app.Run(async (context) => 
{
    await context.Response.WriteAsync("Este middleware encerra o pipeline");
});
