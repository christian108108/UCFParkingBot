// note, this has nothing to do with Azure serverless functions
namespace UCFParkingBot.Library
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

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

        public static CloudTable GetStorageTableReference(string reference)
        {
            // grabbing connection string from KeyVault
            string storageConnectionString = AzureFunctions.keyVaultClient.GetSecretAsync("https://ucfparkingbot-keyvault.vault.azure.net/", "StorageConnectionString").Result.Value;
            
            // using connection string to authenticate with Azure Storage
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);

            CloudTableClient serviceClient = account.CreateCloudTableClient();

            return serviceClient.GetTableReference(reference);
        }
    }
}