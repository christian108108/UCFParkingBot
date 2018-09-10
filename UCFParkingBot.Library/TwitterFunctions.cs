namespace UCFParkingBot.Library
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tweetinvi;
    using Tweetinvi.Models;

    public class TwitterFunctions
    {
        private static string CONSUMER_KEY { get; set; }
        private static string CONSUMER_SECRET { get; set; }
        private static string ACCESS_TOKEN { get; set; }
        private static string ACCESS_TOKEN_SECRET { get; set; }

        /// <summary>
        /// Gets Twitter API keys from Azure Key Vault and sets them for Twitter to be able to use them
        /// </summary>
        /// <returns>void</returns>
        public static void LoginToTwitter()
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

            if (string.IsNullOrWhiteSpace(CONSUMER_KEY) ||
                string.IsNullOrWhiteSpace(CONSUMER_SECRET) ||
                string.IsNullOrWhiteSpace(ACCESS_TOKEN) ||
                string.IsNullOrWhiteSpace(ACCESS_TOKEN_SECRET)){
                throw new Exception("Could not retrieve keys from KeyVault.");
            }

            var creds = Auth.CreateCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
            var authenticatedUser = User.GetAuthenticatedUser(creds);

            if (authenticatedUser == null)
            {
                throw new Exception("Could not authenticate with Twitter.");
            }

            Auth.SetCredentials(creds);

        }

        /// <summary>
        /// Gets keys from KeyVault and returns a dictionary with all the Twitter keys
        /// </summary>
        public static Dictionary<string, string> GetTwitterKeys()
        {
            // authenticating with Azure Managed Service Identity
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            Dictionary<string, string> twitterKeys = new Dictionary<string, string>();

            // fetching keys from Azure Key Vault to use with Twitter's API
            CONSUMER_KEY = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-consumer-key").Result.Value;
            CONSUMER_SECRET = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-consumer-secret").Result.Value;
            ACCESS_TOKEN = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-access-token").Result.Value;
            ACCESS_TOKEN_SECRET = keyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-access-token-secret").Result.Value;

            if (string.IsNullOrWhiteSpace(CONSUMER_KEY) ||
                string.IsNullOrWhiteSpace(CONSUMER_SECRET) ||
                string.IsNullOrWhiteSpace(ACCESS_TOKEN) ||
                string.IsNullOrWhiteSpace(ACCESS_TOKEN_SECRET))
            {
                throw new Exception("Could not retrieve keys from KeyVault.");
            }

            twitterKeys.Add("CONSUMER_KEY", CONSUMER_KEY);
            twitterKeys.Add("CONSUMER_SECRET", CONSUMER_SECRET);
            twitterKeys.Add("ACCESS_TOKEN", ACCESS_TOKEN);
            twitterKeys.Add("ACCESS_TOKEN_SECRET", ACCESS_TOKEN_SECRET);

            return twitterKeys;
        }

        /// <summary>
        /// Gets list of tweets that should be deleted based off how recent the tweet is and activity.
        /// </summary>
        /// <param name="tweets"></param>
        /// <param name="recentTweetsToKeep">number of most recent tweets to keep</param>
        /// <returns>List of deletable tweets!</returns>
        public static List<ITweet> GetDeletableTweets(int recentTweetsToKeep)
        {
            if (recentTweetsToKeep < 0)
            {
                throw new ArgumentException("Please enter a positive number. Cannot keep negative number of tweets.");
            }

            var timeline = User.GetAuthenticatedUser().GetUserTimeline(200).ToList();

            List<ITweet> deletableTweets = new List<ITweet>();

            int i = 0;
            foreach (var tweet in timeline)
            {
                if (tweet.Text.StartsWith("Spots available") &&
                    !tweet.Favorited &&
                    !tweet.Retweeted &&
                    (tweet.QuoteCount == 0 || tweet.QuoteCount == null) &&
                    (tweet.ReplyCount == 0 || tweet.ReplyCount == null))
                {
                    if(i >= recentTweetsToKeep)
                    {
                        deletableTweets.Add(tweet);
                    }
                    i++;
                }
            }

            return deletableTweets;
        }

        /// <summary>
        /// Cleans up timeline by deleting all the old tweets that are no longer irrelevant
        /// </summary>
        /// <param name="recentTweetsToKeep">number of most recent tweets to keep</param>
        public static void CleanUpTimeline(int recentTweetsToKeep)
        {
            if (recentTweetsToKeep < 0)
            {
                throw new ArgumentException("Please enter a positive number. Cannot keep negative number of tweets.");
            }

            // keeps x most recent tweets
            var deletableTweets = TwitterFunctions.GetDeletableTweets(recentTweetsToKeep);

            foreach (var tweet in deletableTweets)
            {
                Console.WriteLine($"Tweet #{tweet.IdStr} from {tweet.CreatedAt} is ready to delete.");
                bool isDestroyed = tweet.Destroy();
                if (isDestroyed)
                {
                    Console.WriteLine("Tweet successfully destroyed!");
                }
                else
                {
                    Console.WriteLine("Tweet not deleted");
                }
            }
        }
    }
}