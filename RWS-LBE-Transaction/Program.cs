using Microsoft.EntityFrameworkCore;
using RWS_LBE_Transaction.Data;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services;
using RWS_LBE_Transaction.Services.Implementations;
using RWS_LBE_Transaction.Services.Interfaces; 

var builder = WebApplication.CreateBuilder(args);

// 1) Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Bind external API configurations
builder.Services.Configure<ExternalApiConfig>(
    builder.Configuration.GetSection("ExternalApiConfig")
);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRlpServiceTransaction, RlpServiceTransaction>();
builder.Services.AddScoped<IRlpServiceCampaign, RlpServiceCampaign>();
builder.Services.AddScoped<IRlpServiceVoucher, RlpServiceVoucher>();
builder.Services.AddScoped<IRlpServiceBooking, RlpServiceBooking>();
builder.Services.AddScoped<IVmsService, VmsService>();
builder.Services.AddScoped<ITransactionSequenceService, TransactionSequenceService>();
builder.Services.AddScoped<IErrorHandler, ErrorHandler>();

// Add http client helper implementation
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("WARNING: Building http client helper in development mode...");
    builder.Services.AddHttpClient<IApiHttpClient, ApiHttpClient>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return handler;
    });
}
else
{
    builder.Services.AddHttpClient<IApiHttpClient, ApiHttpClient>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}

var app = builder.Build();

// Log incoming requests
app.UseMiddleware<RequestLoggingMiddleware>();

//Conditionally run the JWT middleware for "/api/v1/" 
var apiPrefix = "/api/v1/";
var protectedPrefixes = new[]
{
    apiPrefix + "transaction",
    apiPrefix + "voucher" 
};

app.UseWhen(
    ctx =>
    {
        var path = ctx.Request.Path.Value ?? "";
        Console.WriteLine($"[UseWhen] Incoming request path: {path}");
        return protectedPrefixes.Any(p =>
            path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    },
    branch =>
    {
        Console.WriteLine("[UseWhen] JWT Middleware will run for this path.");
        branch.UseMiddleware<JwtInterceptorMiddleware>();
    });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var secretBase64 = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(secretBase64))
    throw new InvalidOperationException("Missing Jwt:Secret in configuration");

byte[] keyBytes;
try
{
    keyBytes = Convert.FromBase64String(secretBase64);
}
catch
{
    throw new InvalidOperationException("Jwt:Secret must be a Base64-encoded string");
}

// Auto-migrate DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // This applies any pending migrations
}


TokenInterceptor.SetJwtSecret(keyBytes);

app.UseHttpsRedirection(); 

app.UseMiddleware<AuditLogMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
