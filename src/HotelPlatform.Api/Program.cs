using Azure.Identity;
using HotelPlatform.Api;
using HotelPlatform.Api.Configuration;
using HotelPlatform.Api.Middleware;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Infrastructure.Data.Migrations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();
builder.Configuration.AddAzureKeyVaultConfiguration();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiServices(builder.Configuration);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}
var localStoragePath = Path.Combine(app.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(localStoragePath))
{
    Directory.CreateDirectory(localStoragePath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(localStoragePath),
    RequestPath = "/files"
});

app.UseHttpsRedirection();
app.UseAuthenticationHeaderValidation();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/debug/settings", (IOptions<StorageSettings> storageSettings, IConfiguration config) =>
{
    var settings = storageSettings.Value;
    return new
    {
        StorageSettings = new
        {
            PrimaryProvider = settings.PrimaryProvider,
            EnableFallback = settings.EnableFallback,
            ConnectionString = settings.AzureBlob.ConnectionString,
            AzureBlob = new
            {
                ConnectionStringLength = settings.AzureBlob.ConnectionString?.Length ?? 0,
                connectionStringsAzureBlobStorage = config.GetConnectionString("AzureBlobStorage"),
                ConnectionStringFirst20 = settings.AzureBlob.ConnectionString?.Length > 20 
                    ? settings.AzureBlob.ConnectionString[..20] + "..." 
                    : settings.AzureBlob.ConnectionString,
                settings.AzureBlob.ContainerName,
                settings.AzureBlob.BaseUrl,
                settings.AzureBlob.CreateContainerIfNotExists
            }
        },
        // Check raw configuration sources
        RawConfigCheck = new
        {
            StorageAzureBlobConnectionString = config["Storage:AzureBlob:ConnectionString"]?.Length > 0 
                ? $"HAS VALUE ({config["Storage:AzureBlob:ConnectionString"]?.Length} chars)" 
                : "EMPTY/NULL",
            ConnectionStringsAzureBlobStorage = config.GetConnectionString("AzureBlobStorage")?.Length > 0 
                ? $"HAS VALUE ({config.GetConnectionString("AzureBlobStorage")?.Length} chars)" 
                : "EMPTY/NULL",
            DatabaseConnectionString = config["Database:ConnectionString"]?.Length > 0 
                ? "HAS VALUE" 
                : "EMPTY/NULL",
            ConnectionStringsHotelDb = config.GetConnectionString("hoteldb")?.Length > 0 
                ? "HAS VALUE" 
                : "EMPTY/NULL",
            KeyVaultUrl = config["KeyVault:Url"]
        },
        // List all configuration providers in order
        ConfigProviders = ((IConfigurationRoot)config).Providers
            .Select(p => p.GetType().Name)
            .ToList()
    };
}).AllowAnonymous();
app.Run();
