namespace UCFParkingBot.Library
{
    using System;
    using System.Net.Http;
    using System.Collections.Generic;
    using Microsoft.Azure.KeyVault;

    public static class GoogleMapsFunctions
    {
        public static string GenerateURI(Building origin, Building destination, string APIKey)
        {
            // checking to see if anything is null...
            if(origin == null ||
               destination == null ||
               origin.Coordinates == null ||
               destination.Coordinates == null)
            {
                throw new ArgumentNullException("Please use a building with coordinates");
            }


            // comma joined coordinates for Google's spec
            string originCoords = string.Join(",", origin.Coordinates);
            string destinationCoords = string.Join(",", destination.Coordinates);


            // building the URI...
            var uriBuilder = new UriBuilder("https://maps.googleapis.com")
            {
                Path = "/maps/api/directions/json",
            };

            // collecting all the query parameters before I join them
            var queryParams = new List<string>()
            {
                $"origin={originCoords}",
                $"destination={destinationCoords}",
                "mode=walking",
                $"key={APIKey}"
            };

            // joining the parameters with &s
            // setting the query field for uri builder
            uriBuilder.Query = string.Join("&", queryParams);

            return uriBuilder.Uri.ToString();
        }
    
        ///
        /// Returns formatted JSON response from the Google Maps API
        ///
        public static string GetWalkingDirections(Building origin, Building destination, string APIKey)
        {
            string googleReq = GoogleMapsFunctions.GenerateURI(origin, destination, APIKey);

            HttpClient client = new HttpClient();
            
            var response = client.GetAsync(googleReq).Result.Content;

            var data = response.ReadAsStringAsync().Result;

            return data;
        }
        public static string GoogleMapsAPIKey
        {
            get
            {
                if (AzureFunctions.keyVaultClient == null)
                {
                    AzureFunctions.LogIntoKeyVault();
                }

                return AzureFunctions.keyVaultClient.GetSecretAsync("https://ucfparkingbot-keyvault.vault.azure.net/", "google-maps-key").Result.Value;
            }
        }
    }
}