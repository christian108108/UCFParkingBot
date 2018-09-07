namespace UCFParkingBot.Twitter
{
    using System;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Tweetinvi;
    using UCFParkingBot.Library;

    public static class Program
    {
        [FunctionName("TweetSpotsAvailable")]
        public static void Run([TimerTrigger("0 */15 * * * 1-5")]TimerInfo myTimer, TraceWriter log)
        {
            ParkingDataFunctions.SetParkingData();
            string output = ParkingDataFunctions.ParkingData();
            
            // Log the full output of the parking data
            log.Info($"{output}");



            Garage least = ParkingDataFunctions.GetLeastAvailableGarageBySpots();
            //If there are fewer than 100 spots left, tweet!
            if ( least.PercentAvailable < 100 )
            {
                //get Twitter API keys from Key Vault
                TwitterFunctions.SetTwitterKeys();

                //authenticate with Twitter
                Auth.SetUserCredentials(TwitterFunctions.CONSUMER_KEY, TwitterFunctions.CONSUMER_SECRET, TwitterFunctions.ACCESS_TOKEN, TwitterFunctions.ACCESS_TOKEN_SECRET);

                Tweet.PublishTweet(output);
                log.Info($"Tweet published at {DateTime.UtcNow} UTC!");
            }
            else
            {
                log.Info($"Tweet not published at {DateTime.UtcNow} UTC. Garage with least availability: {least.Name} with {least.SpotsAvailable} spots free.");
            }
        }
    }
}
