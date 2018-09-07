namespace UCFParkingBot.Library
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;

    public class TwitterFunctions
    {
        public static string CONSUMER_KEY { get; private set; }
        public static string CONSUMER_SECRET { get; private set; }
        public static string ACCESS_TOKEN { get; private set; }
        public static string ACCESS_TOKEN_SECRET { get; private set; }

        /// <summary>
        /// Gets Twitter API keys from Azure Key Vault and sets them for Twitter to be able to use them
        /// </summary>
        /// <returns>void</returns>
        public static void SetTwitterKeys()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            // fetching keys from Azure Key Vault to use with Twitter's API
            CONSUMER_KEY = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-consumer-key").Result.Value;
            CONSUMER_SECRET = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-consumer-secret").Result.Value;
            ACCESS_TOKEN = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-access-token").Result.Value;
            ACCESS_TOKEN_SECRET = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-access-token-secret").Result.Value;
        }
    }
}
