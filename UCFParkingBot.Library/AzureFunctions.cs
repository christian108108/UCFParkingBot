// note, this has nothing to do with Azure serverless functions
namespace UCFParkingBot.Library
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;

    public static class AzureFunctions
    {
        public static KeyVaultClient keyVaultClient { get; private set; }

        public static void LoginToKeyVault()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            AzureFunctions.keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

        }
    }
}