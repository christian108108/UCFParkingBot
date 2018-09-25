namespace UCFParkingBot.Library
{
    using System;
    using HtmlAgilityPack;
    using System.Collections.Generic;

    public static class GoogleMapsFunctions
    {
        public static string GenerateURI(Building origin, Building destination)
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
            var uriBuilder = new UriBuilder("https://www.google.com/")
            {
                Path = "/maps/dir/",
            };

            // collecting all the query parameters before I join them
            var queryParams = new List<string>()
            {
                "?api=1",
                $"origin={originCoords}",
                $"destination={destinationCoords}",
                "travelmode=walking"
            };

            // joining the parameters with &s
            // setting the query field for uri builder
            uriBuilder.Query = string.Join("&", queryParams);

            return uriBuilder.Uri.ToString();
        }
    }
}