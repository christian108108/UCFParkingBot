using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace UCFParkingBot.Library
{
    public class ParkingDataFunctions
    {
        public List<Garage> garages { get; set; }

        /// <summary>
        /// Sets parking data as a list of Garage objects with the appropriate data
        /// </summary>
        public void SetParkingData()
        {
            string html = @"https://secure.parking.ucf.edu/GarageCount/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//strong");

            List<Garage> garages = new List<Garage>
            {
                new Garage( "Garage A", 1623 ),
                new Garage( "Garage B", 1259 ),
                new Garage( "Garage C", 1852 ),
                new Garage( "Garage D", 1241 ),
                new Garage( "Garage H", 1284 ),
                new Garage( "Garage I", 1231 ),
                new Garage( "Libra Garage", 1007 )
            };

            int i = 0;
            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    int.TryParse(node.InnerText, out int spotsAvailable);
                    garages[i].SpotsAvailable = spotsAvailable;
                    garages[i].PercentAvailable = (decimal)garages[i].SpotsAvailable / garages[i].MaxSpots * 100;
                }
                i++;
            }

            this.garages = garages;
        }

        public override string ToString()
        {
            if (this.garages == default(List<Garage>) || this.garages.Count == 0)
            {
                throw new InvalidOperationException("Parking data has not yet been set.");
            }

            List<string> listOfStrings = new List<string>
            {
                "Spots available"
            };

            foreach (Garage garage in garages)
            {
                listOfStrings.Add($"{garage.Name}: {garage.SpotsAvailable}");
            }

            string output = string.Join("\n", listOfStrings);

            return output;
        }

        public Garage GetMostAvailableGarage()
        {
            if (this.garages == default(List<Garage>) || garages.Count == 0)
            {
                throw new InvalidOperationException("Garage data has not yet been set");
            }

            Garage mostAvailable = garages[0];
            foreach (Garage g in garages)
            {
                if (g.PercentAvailable > mostAvailable.PercentAvailable)
                {
                    mostAvailable = g;
                }
            }

            return mostAvailable;
        }

        public Garage GetLeastAvailableGarage()
        {
            if (this.garages == default(List<Garage>) || garages.Count == 0)
            {
                throw new InvalidOperationException("Garage data has not yet been set");
            }

            Garage leastAvailable = garages[0];
            foreach (Garage g in garages)
            {
                if (g.PercentAvailable < leastAvailable.PercentAvailable)
                {
                    leastAvailable = g;
                }
            }

            return leastAvailable;
        }
    }
}
