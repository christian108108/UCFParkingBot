namespace UCFParkingBot.AzureFunction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Tweetinvi;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Models;
    using Microsoft.Azure.Services.AppAuthentication;


    public class Garage
    {
        public string Name { get; set; }
        public int MaxSpots { get; set; }
        public int SpotsAvailable { get; set; }
        public decimal PercentAvailable { get; set; }

        public Garage(string name, int maxSpots, int spotsAvailable = 0, decimal percentAvailable = 0)
        {
            Name = name;
            MaxSpots = maxSpots;
            SpotsAvailable = spotsAvailable;
            PercentAvailable = percentAvailable;
        }
    }

    public static class UCFParkingBot
    {
        public static SecretBundle CONSUMER_KEY { get; private set; }
        public static SecretBundle CONSUMER_SECRET { get; private set; }
        public static SecretBundle ACCESS_TOKEN { get; private set; }
        public static SecretBundle ACCESS_TOKEN_SECRET { get; private set; }
        public static string KEY_VAULT_NAME = Properties.Resources.keyVaultName;

        [FunctionName("TweetSpotsAvailable")]
        public static async System.Threading.Tasks.Task RunAsync([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var html = @"https://secure.parking.ucf.edu/GarageCount/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//strong");

            List<Garage> garages = new List<Garage>
            {
                new Garage( "Garage A", 1623 ),
                new Garage( "Garage B", 1259 ),
                new Garage( "Garage C", 1852 ),
                new Garage( "Garage D", 1241 ),
                new Garage( "Garage H", 1284 ),
                new Garage( "Garage I", 1231 ),
                new Garage( "Libra Garage", 1007 )
            };

            int i = 0;
            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    int.TryParse(node.InnerText, out int spotsAvailable);
                    garages[i].SpotsAvailable = spotsAvailable;
                    garages[i].PercentAvailable = (decimal)garages[i].SpotsAvailable / garages[i].MaxSpots * 100;
                }
                i++;
            }

            List<string> listOfStrings = new List<string>
            {
                "Spots available"
            };

            foreach (Garage garage in garages)
            {
                listOfStrings.Add($"{garage.Name}: {garage.SpotsAvailable}");
            }

            string output = string.Join("\n", listOfStrings);

            //log.Info($"\n{output}");



            //If there are only 10% or fewer spots left, tweet!
            if ( garages.Min( garage => garage.PercentAvailable ) < 10 )
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
