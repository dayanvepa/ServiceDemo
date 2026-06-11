using FluentValidation;
using FluentValidation.AspNetCore;
using ServiceDemo.API.Middleware;
using ServiceDemo.Application.Mappings;
using ServiceDemo.Application.Services;
using ServiceDemo.Application.Services.Interfaces;
using ServiceDemo.Application.Validators.Product;
using ServiceDemo.Infrastructure;
using Serilog;
using System.Reflection;
using System.Text;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger(); //

builder.Host.UseSerilog();

Log.Information("Starting ServiceDemo.API...");

builder.Services.AddControllers();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Application Services
builder.Services.AddScoped<IProductService, ProductService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Pipeline
app.MapOpenApi();
app.MapScalarApiReference();



app.UseCors("AllowAll");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// 
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider
            .GetRequiredService<ServiceDemo.Infrastructure.Data.ServiceDemoDbContext>();
        context.Database.EnsureCreated();
        Log.Information("Database connection successful.");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Could not connect to database on startup. The app will continue.");
        
    }
}

app.Run();