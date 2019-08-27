using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

// note, this has nothing to do with Azure serverless functions
namespace UCFParkingBot.Library
{
    public static class AzureHelper
    {
        public static KeyVaultClient KeyVaultClient { get; private set; }

        // Logs into Azure KeyVault and makes keyVaultClient active
        public static void LogIntoKeyVault()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            KeyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }
    }
}