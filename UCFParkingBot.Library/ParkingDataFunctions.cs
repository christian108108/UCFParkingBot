namespace UCFParkingBot.Library
{
    using System;
    using HtmlAgilityPack;
    using System.Collections.Generic;

    public static class ParkingDataFunctions
    {
        public static List<Garage> Garages { get; private set; }

        /// <summary>
        /// Sets parking data as a list of Garage objects with the appropriate data
        /// </summary>
        public static void SetParkingData()
        {
            string html = @"https://secure.parking.ucf.edu/GarageCount/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//strong");

            // if Libra garage is still listed, remove it! It's residential, so no need to tweet about it.
            if (nodes.Count == 7)
            {
                nodes.RemoveAt(nodes.Count - 1);
            }

            List<Garage> garages = new List<Garage>
            {
                new Garage( "Garage A", 1623 ),
                new Garage( "Garage B", 1259 ),
                new Garage( "Garage C", 1852 ),
                new Garage( "Garage D", 1241 ),
                new Garage( "Garage H", 1284 ),
                new Garage( "Garage I", 1231 ),
            };

            int i = 0;
            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    int.TryParse(node.InnerText, out int spotsAvailable);
                    garages[i].SpotsAvailable = spotsAvailable;
                    garages[i].SetPercentAvailable();
                }
                i++;
            }

            Garages = garages;
        }

        /// <summary>
        /// Makes pretty string to either tweet, message, speak, etc. with the name and number of spots available per garage.
        /// </summary>
        /// <returns>If parking data is set, this method will return a string of all the parking spots available per garage.</returns>
        public static string ParkingData()
        {
            // if parking data hasn't been set, throw an error
            if (Garages.Count == 0 || Garages == null)
            {
                throw new InvalidOperationException("Use SetParkingData() before using this method.");
            }

            List<string> listOfStrings = new List<string>
            {
                "Spots available"
            };
            

            foreach (Garage garage in Garages)
            {
                listOfStrings.Add($"{garage.Name}: {garage.SpotsAvailable}");
            }

            string output = string.Join("\n", listOfStrings);

            return output;
        }

        /// <summary>
        /// Gets most available garage based off of percentage
        /// </summary>
        /// <example>Garage with 75% spots free, when all other garages have less than 75% spots free.</example>
        /// <returns>Garage object with the highest availability by percentage</returns>
        public static Garage GetMostAvailableGarageByPercentage()
        {
            // if parking data hasn't been set, throw an error
            if (Garages.Count == 0 || Garages == null)
            {
                throw new InvalidOperationException("Use SetParkingData() before using this method.");
            }

            Garage mostAvailable = Garages[0];
            foreach (Garage g in Garages)
            {
                if (g.PercentAvailable > mostAvailable.PercentAvailable)
                {
                    mostAvailable = g;
                }
            }

            return mostAvailable;
        }

        /// <summary>
        /// Gets most available garage based off of spots
        /// </summary>
        /// <example>Garage with 500 spots free, when all other garages have fewer than 500 spots free.</example>
        /// <returns>Garage object with the highest availability by spots</returns>
        public static Garage GetMostAvailableGarageBySpots()
        {
            // if parking data hasn't been set, throw an error
            if (Garages.Count == 0 || Garages == null)
            {
                throw new InvalidOperationException("Use SetParkingData() before using this method.");
            }

            Garage mostAvailable = Garages[0];
            foreach (Garage g in Garages)
            {
                if (g.SpotsAvailable > mostAvailable.SpotsAvailable)
                {
                    mostAvailable = g;
                }
            }

            return mostAvailable;
        }

        /// <summary>
        /// Gets least available garage based off of percentage
        /// </summary>
        /// <example>Garage with 8% spots free, when all other garages have more than 8% spots free.</example>
        /// <returns>Garage object with the lowest availability by percentage</returns>
        public static Garage GetLeastAvailableGarageByPercentage()
        {
            // if parking data hasn't been set, throw an error
            if (Garages.Count == 0 || Garages == null)
            {
                throw new InvalidOperationException("Use SetParkingData() before using this method.");
            }

            Garage leastAvailable = Garages[0];
            foreach (Garage g in Garages)
            {
                if (g.PercentAvailable < leastAvailable.PercentAvailable)
                {
                    leastAvailable = g;
                }
            }

            return leastAvailable;
        }

        /// <summary>
        /// Gets least available garage based off of spots
        /// </summary>
        /// <example>Garage with 10 spots free, when all other garages have more than 10 spots free.</example>
        /// <returns>Garage object with the lowest availability by spots</returns>
        public static Garage GetLeastAvailableGarageBySpots()
        {
            // if parking data hasn't been set, throw an error
            if (Garages.Count == 0 || Garages == null)
            {
                throw new InvalidOperationException("Use SetParkingData() before using this method.");
            }

            Garage leastAvailable = Garages[0];
            foreach (Garage g in Garages)
            {
                if (g.SpotsAvailable < leastAvailable.SpotsAvailable)
                {
                    leastAvailable = g;
                }
            }

            return leastAvailable;
        }
    }
}
