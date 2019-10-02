using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using UCFParkingBot.Library;

namespace UCFParkingBot.Twitter
{
    public static class TweetSpotsAvailable
    {
        [FunctionName("TweetSpotsAvailable")]
        public static void Run([TimerTrigger("0 */15 * * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            ParkingDataFunctions.SetParkingData();
            string parkingData = ParkingDataFunctions.ParkingDataAsString();
            // Log the full output of the parking data
            log.LogInformation(parkingData);

            Garage leastAvailableGarage = ParkingDataFunctions.GetLeastAvailableGarageBySpots();
            //If there are fewer than 100 spots left, tweet!
            if (leastAvailableGarage.SpotsAvailable < 100)
            {
                //get Twitter API keys from Key Vault and authenticate with Twitter
                TwitterHelper.LoginToTwitter();

                bool isTweetPublished = TwitterHelper.PublishTweet(parkingData);
                if (isTweetPublished)
                {
                    log.LogInformation($"Tweet published at {DateTime.UtcNow} UTC!");
                    TwitterHelper.CleanUpTimeline(3);
                }
                else
                {
                    log.LogInformation($"Tweet could not be published at {DateTime.UtcNow} UTC!");
                }
            }
            else
            {
                log.LogInformation($"Tweet not published at {DateTime.UtcNow} UTC. Garage with least availability: {leastAvailableGarage.Name} with {leastAvailableGarage.SpotsAvailable} spots free.");
            }
        }
    }
}
