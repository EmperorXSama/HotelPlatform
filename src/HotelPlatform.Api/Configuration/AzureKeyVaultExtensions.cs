// Api/Configuration/AzureKeyVaultExtensions.cs
using Azure.Identity;

namespace HotelPlatform.Api.Configuration;

public static class AzureKeyVaultExtensions
{
    public static IConfigurationBuilder AddAzureKeyVaultConfiguration(
        this IConfigurationManager configuration)
    {
        // First, build current configuration to get KeyVault URL
        var builtConfig = configuration.Build();
        var keyVaultUrl = builtConfig["KeyVault:Url"];

        if (string.IsNullOrEmpty(keyVaultUrl))
        {
            // Log warning but don't fail - allows local dev without Key Vault
            Console.WriteLine("⚠️ KeyVault:Url not configured. Skipping Azure Key Vault integration.");
            return configuration;
        }

        try
        {
            Console.WriteLine($"🔐 Loading secrets from Azure Key Vault: {keyVaultUrl}");

            configuration.AddAzureKeyVault(
                new Uri(keyVaultUrl),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    // Exclude options that might cause issues in local dev
                    ExcludeEnvironmentCredential = false,
                    ExcludeWorkloadIdentityCredential = true,
                    ExcludeManagedIdentityCredential = false,
                    ExcludeSharedTokenCacheCredential = true,
                    ExcludeVisualStudioCredential = false,
                    ExcludeVisualStudioCodeCredential = false,
                    ExcludeAzureCliCredential = false,
                    ExcludeAzurePowerShellCredential = false,
                    ExcludeInteractiveBrowserCredential = true
                }));

            Console.WriteLine("✅ Azure Key Vault configured successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to configure Azure Key Vault: {ex.Message}");
            // Don't throw - allows fallback to appsettings.json
        }

        return configuration;
    }
}