using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace UCFParkingBot.Library
{
    public class ParkingDataFunctions
    {
        public List<Garage> Garages { get; set; }

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

            this.Garages = garages;
        }

        public override string ToString()
        {
            if (this.Garages == default(List<Garage>) || this.Garages.Count == 0)
            {
                throw new InvalidOperationException("Parking data has not yet been set.");
            }

            List<string> listOfStrings = new List<string>
            {
                "Spots available"
            };

            foreach (Garage garage in this.Garages)
            {
                listOfStrings.Add($"{garage.Name}: {garage.SpotsAvailable}");
            }

            string output = string.Join("\n", listOfStrings);

            return output;
        }

        public Garage MostAvailableGarage()
        {
            if (this.Garages == default(List<Garage>) || this.Garages.Count == 0)
            {
                throw new InvalidOperationException("Garage data has not yet been set");
            }

            Garage mostAvailable = this.Garages[0];
            foreach (Garage g in this.Garages)
            {
                if (g.PercentAvailable > mostAvailable.PercentAvailable)
                {
                    mostAvailable = g;
                }
            }

            return mostAvailable;
        }

        public Garage LeastAvailableGarage()
        {
            if (this.Garages == default(List<Garage>) || this.Garages.Count == 0)
            {
                throw new InvalidOperationException("Garage data has not yet been set");
            }

            Garage leastAvailable = this.Garages[0];
            foreach (Garage g in this.Garages)
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
