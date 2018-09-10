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
            string output = ParkingDataFunctions.ParkingDataAsString();
            
            // Log the full output of the parking data
            log.Info($"{output}");



            Garage leastAvailableGarage = ParkingDataFunctions.GetLeastAvailableGarageBySpots();
            //If there are fewer than 100 spots left, tweet!
            if ( leastAvailableGarage.SpotsAvailable < 100 )
            {
                //get Twitter API keys from Key Vault and authenticate with Twitter
                TwitterFunctions.LoginToTwitter();

                var newTweet = Tweet.PublishTweet(output);
                if (newTweet.IsTweetPublished)
                {
                    log.Info($"Tweet published at {DateTime.UtcNow} UTC!");
                    TwitterFunctions.CleanUpTimeline(3);
                }
                else
                {
                    log.Error($"Tweet could not be published at {DateTime.UtcNow} UTC!");
                }

            }
            else
            {
                log.Info($"Tweet not published at {DateTime.UtcNow} UTC. Garage with least availability: {leastAvailableGarage.Name} with {leastAvailableGarage.SpotsAvailable} spots free.");
            }
        }
    }
}
