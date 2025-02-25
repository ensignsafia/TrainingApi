using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TrainingApi.Shared; 

var builder = WebApplication.CreateBuilder(args);

// Data and API dependencies
builder.Services
    .AddDbContext<TrainingDb>(options => options.UseInMemoryDatabase("training"));
builder.Services.AddScoped<TrainingService>();
// Authentication and authorization dependencies
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorizationBuilder().AddPolicy("trainer_access", policy =>
    policy.RequireRole("trainer").RequireClaim("permission", "admin"));
// OpenAPI dependencies
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Set up OpenAPI-related endpoints
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
    // Seed the database with mock data
    app.InitializeDatabase();
}

// Redirect for OpenAPI view
app.MapGet("/", () => Results.Redirect("/scalar/v1"));
app.MapTrainerApi();

app.Run();
