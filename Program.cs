using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Dependency injection
//controller
builder.Services.AddControllers()
//change field in response to snake case lower
//because .net default using camel case to name their field
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = 
            System.Text.Json.JsonNamingPolicy.SnakeCaseLower;  
    });
//db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")
    ).UseSnakeCaseNamingConvention()
);
//service/application

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<PlanetService>();


//entry
var app = builder.Build();
app.Use(async (context, next) =>
{
    Console.WriteLine($"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode}");
    await next();
});
app.MapControllers();
app.Run();