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
builder.Services.AddScoped<PlanetService>();
builder.Services.AddEndpointsApiExplorer();

//entry
var app = builder.Build();
app.MapControllers();
app.Run();