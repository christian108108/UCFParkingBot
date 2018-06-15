namespace UCFParkingBot
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var html = @"https://secure.parking.ucf.edu/GarageCount/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//strong");

            List<string> garages = new List<string>
            {
                { "Garage A" },
                { "Garage B" },
                { "Garage C" },
                { "Garage D" },
                { "Garage H" },
                { "Garage I" },
                { "Libra Garage" }
            };

            List<int> counts = new List<int>();
            Dictionary<string, int> garagesAndCounts = new Dictionary<string, int>();

            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    int.TryParse(node.InnerText, out int currentCount);
                    counts.Add(currentCount);
                }
            }

            garagesAndCounts = garages.Zip(counts, (g, c) => new { g, c }).ToDictionary(item => item.g, item => item.c);

            List<string> listOfStrings = new List<string>();

            listOfStrings.Add("Spots available:");
            foreach (KeyValuePair<string, int> kvp in garagesAndCounts)
            {
                listOfStrings.Add($"{kvp.Key}: {kvp.Value}");
            }

            string output = string.Join("\n", listOfStrings);

            Console.WriteLine(output);
            Console.ReadKey();
        }
    }
}