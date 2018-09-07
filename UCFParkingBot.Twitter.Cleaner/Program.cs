using System;
using UCFParkingBot.Library;
using Tweetinvi;

namespace UCFParkingBot.Twitter.Cleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitterFunctions.SetTwitterKeys();

            Auth.SetUserCredentials(TwitterFunctions.CONSUMER_KEY, TwitterFunctions.CONSUMER_SECRET, TwitterFunctions.ACCESS_TOKEN, TwitterFunctions.ACCESS_TOKEN_SECRET);

            

        }
    }
}
