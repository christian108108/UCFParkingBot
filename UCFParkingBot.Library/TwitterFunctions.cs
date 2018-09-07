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

            Auth.SetUserCredentials(TwitterFunctions.CONSUMER_KEY, TwitterFunctions.CONSUMER_SECRET, TwitterFunctions.ACCESS_TOKEN, TwitterFunctions.ACCESS_TOKEN_SECRET);
        }

        /// <summary>
        /// Given a list of tweets, this will return only the tweets that should be deleted based off how recent the tweet is and activity.
        /// </summary>
        /// <param name="tweets"></param>
        /// <returns>List of deletable tweets!</returns>
        public static List<ITweet> GetDeletableTweets()
        {
            var timeline = User.GetAuthenticatedUser().GetUserTimeline(200).ToList();

            List<ITweet> deletableTweets = new List<ITweet>();

            int i = 0;
            foreach (var tweet in timeline)
            {
                if (i > 3 &&
                    tweet.IsTweetPublished &&
                    !tweet.IsTweetDestroyed &&
                    !tweet.Favorited &&
                    !tweet.Retweeted &&
                    (tweet.QuoteCount == 0 || tweet.QuoteCount == null) &&
                    (tweet.ReplyCount == 0 || tweet.ReplyCount == null) &&
                    tweet.Text.StartsWith("Spots available"))
                {
                    deletableTweets.Add(tweet);
                }
                i++;
            }

            return deletableTweets;
        }

        /// <summary>
        /// Cleans up timeline by deleting all the old tweets that are no longer irrelevant
        /// </summary>
        public static void CleanUpTimeline()
        {
            var deletableTweets = TwitterFunctions.GetDeletableTweets();

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