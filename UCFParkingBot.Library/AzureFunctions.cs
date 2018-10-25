// note, this has nothing to do with Azure serverless functions
namespace UCFParkingBot.Library
{
    using System.IO;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class AzureFunctions
    {
        public static KeyVaultClient keyVaultClient { get; private set; }

        // Logs into Azure KeyVault and makes keyVaultClient active
        public static void LogIntoKeyVault()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            AzureFunctions.keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

        }

        // Updates Azure Table Storage with new building data from a JSON file
        public static void UpdateTableWithJSON(string path)
        {
            // Read JSON directly from given path
            JArray jArray;
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jArray = (JArray)JToken.ReadFrom(reader);
            }

            // Log into keyvault
            AzureFunctions.LogIntoKeyVault();
            
            // Get Azure StorageConnectionString
            string StorageConnectionString = AzureFunctions.keyVaultClient.GetSecretAsync("https://ucfparkingbot-keyvault.vault.azure.net", "StorageConnectionString").Result.Value;

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("buildings");
            
            // Cycle through each building in the JSON file
            foreach(var jToken in jArray)
            {
                // Deserialize into building objects
                Building currentBuilding = Building.FromJson(jToken.ToString());
                
                // Insert or merge the building object into Azure Table storage
                TableOperation operation = TableOperation.InsertOrMerge(currentBuilding);
                table.ExecuteAsync(operation);
            }
        }
    }
}