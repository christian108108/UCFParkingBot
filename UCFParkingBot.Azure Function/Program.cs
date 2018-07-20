namespace UCFParkingBot.AzureFunction
{
    using System;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Tweetinvi;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Models;
    using Microsoft.Azure.Services.AppAuthentication;
    using UCFParkingBot.Library;

    public static class Program
    {
        public static SecretBundle CONSUMER_KEY { get; private set; }
        public static SecretBundle CONSUMER_SECRET { get; private set; }
        public static SecretBundle ACCESS_TOKEN { get; private set; }
        public static SecretBundle ACCESS_TOKEN_SECRET { get; private set; }
        public static string KEY_VAULT_NAME = Properties.Resources.keyVaultName;

        [FunctionName("TweetSpotsAvailable")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var parkingData = new ParkingDataFunctions();
            parkingData.SetParkingData();
            string output = parkingData.ToString();

            var garages = parkingData.Garages;
            //log.Info($"\n{output}");



            //If there are only 10% or fewer spots left, tweet!
            if ( parkingData.LeastAvailableGarage().PercentAvailable < 100 )
            {
                //get Twitter API keys from Key Vault
                await GetTwitterKeysAsync();                

                //authenticate with Twitter
                Auth.SetUserCredentials(CONSUMER_KEY.Value, CONSUMER_SECRET.Value, ACCESS_TOKEN.Value, ACCESS_TOKEN_SECRET.Value);

                Tweet.PublishTweet(output);
                log.Info($"Tweeted at {DateTime.UtcNow}!");
            }
        }

        /// <summary>
        /// Gets Twitter API keys from Azure Key Vault and sets them for Twitter to be able to use them
        /// </summary>
        /// <returns>void</returns>
        public static async System.Threading.Tasks.Task GetTwitterKeysAsync()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            try
            {
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                // fetching keys from Azure Key Vault to use with Twitter's API
                CONSUMER_KEY = await keyVaultClient.GetSecretAsync($"https://{KEY_VAULT_NAME}.vault.azure.net/secrets/twitter-consumer-key");
                CONSUMER_SECRET = await keyVaultClient.GetSecretAsync($"https://{KEY_VAULT_NAME}.vault.azure.net/secrets/twitter-consumer-secret");
                ACCESS_TOKEN = await keyVaultClient.GetSecretAsync($"https://{KEY_VAULT_NAME}.vault.azure.net/secrets/twitter-access-token");
                ACCESS_TOKEN_SECRET = await keyVaultClient.GetSecretAsync($"https://{KEY_VAULT_NAME}.vault.azure.net/secrets/twitter-access-token-secret");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
