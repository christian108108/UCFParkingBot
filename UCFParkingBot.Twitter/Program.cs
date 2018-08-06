namespace UCFParkingBot.Twitter
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
        private static SecretBundle CONSUMER_KEY { get; set; }
        private static SecretBundle CONSUMER_SECRET { get; set; }
        private static SecretBundle ACCESS_TOKEN { get; set; }
        private static SecretBundle ACCESS_TOKEN_SECRET { get; set; }

        [FunctionName("TweetSpotsAvailable")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var parkingData = new ParkingDataFunctions();
            string output = parkingData.ToString();

            var garages = parkingData.Garages;
            
            // Log the full output of the parking data
            log.Info($"{output}");


            //If there are only 10% or fewer spots left, tweet!
            if ( parkingData.GetLeastAvailableGarageByPercentage().PercentAvailable < 10 )
            {
                //get Twitter API keys from Key Vault
                await SetTwitterKeysAsync();                

                //authenticate with Twitter
                Auth.SetUserCredentials(CONSUMER_KEY.Value, CONSUMER_SECRET.Value, ACCESS_TOKEN.Value, ACCESS_TOKEN_SECRET.Value);

                Tweet.PublishTweet(output);
                log.Info($"Tweet published at {DateTime.UtcNow} UTC!");
            }
            else
            {
                Garage least = parkingData.GetLeastAvailableGarageByPercentage();
                log.Info($"Tweet not published at {DateTime.UtcNow} UTC. Garage with least availability: {least.Name} with {least.PercentAvailable.ToString("F")}% ({least.SpotsAvailable} spots free).");
            }
        }

        /// <summary>
        /// Gets Twitter API keys from Azure Key Vault and sets them for Twitter to be able to use them
        /// </summary>
        /// <returns>void</returns>
        public static async System.Threading.Tasks.Task SetTwitterKeysAsync()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            // fetching keys from Azure Key Vault to use with Twitter's API
            CONSUMER_KEY = await keyVaultClient.GetSecretAsync($"https://ucfparkingbot.vault.azure.net/secrets/twitter-consumer-key");
            CONSUMER_SECRET = await keyVaultClient.GetSecretAsync($"https://ucfparkingbot.vault.azure.net/secrets/twitter-consumer-secret");
            ACCESS_TOKEN = await keyVaultClient.GetSecretAsync($"https://ucfparkingbot.vault.azure.net/secrets/twitter-access-token");
            ACCESS_TOKEN_SECRET = await keyVaultClient.GetSecretAsync($"https://ucfparkingbot.vault.azure.net/secrets/twitter-access-token-secret");
        }
    }
}
