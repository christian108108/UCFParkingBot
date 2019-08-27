using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Models;

namespace UCFParkingBot.Library
{
    public class TwitterHelper
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
            // if we haven't logged into KeyVault yet
            if (AzureHelper.KeyVaultClient == null)
                AzureHelper.LogIntoKeyVault();

            // fetching keys from Azure Key Vault to use with Twitter's API
            CONSUMER_KEY = AzureHelper.KeyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-consumer-key").Result.Value;
            CONSUMER_SECRET = AzureHelper.KeyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-consumer-secret").Result.Value;
            ACCESS_TOKEN = AzureHelper.KeyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-access-token").Result.Value;
            ACCESS_TOKEN_SECRET = AzureHelper.KeyVaultClient.GetSecretAsync($"https://ucfparkingbot-keyvault.vault.azure.net/", "twitter-access-token-secret").Result.Value;

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
        /// Publishes tweet with given content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool PublishTweet(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Tweet content cannot be null or empty.", nameof(content));

            var newTweet = Tweet.PublishTweet(content);

            return newTweet.IsTweetPublished;
        }

        /// <summary>
        /// Cleans up timeline by deleting all the old tweets that are no longer irrelevant
        /// </summary>
        /// <param name="recentTweetsToKeep">number of most recent tweets to keep</param>
        public static void CleanUpTimeline(int recentTweetsToKeep)
        {
            if (recentTweetsToKeep < 0)
                throw new ArgumentException("Please enter a positive number. Cannot keep negative number of tweets.");

            // keeps x most recent tweets
            var deletableTweets = GetDeletableTweets(recentTweetsToKeep);

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

        /// <summary>
        /// Gets list of tweets that should be deleted based off how recent the tweet is and activity.
        /// </summary>
        /// <param name="tweets"></param>
        /// <param name="recentTweetsToKeep">number of most recent tweets to keep</param>
        /// <returns>List of deletable tweets!</returns>
        private static IEnumerable<ITweet> GetDeletableTweets(int recentTweetsToKeep)
        {
            if (recentTweetsToKeep < 0)
                throw new ArgumentException("Please enter a positive number. Cannot keep negative number of tweets.");

            var timeline = User.GetAuthenticatedUser().GetUserTimeline(200);

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
                    if (i >= recentTweetsToKeep)
                    {
                        deletableTweets.Add(tweet);
                    }
                    i++;
                }
            }

            return deletableTweets;
        }
    }
}