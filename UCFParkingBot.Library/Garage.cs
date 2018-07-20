namespace UCFParkingBot.Library
{
    public class Garage
    {
        public string Name { get; set; }
        public int MaxSpots { get; set; }
        public int SpotsAvailable { get; set; }
        public decimal PercentAvailable { get; set; }

        public Garage(string name, int maxSpots, int spotsAvailable = 0)
        {
            Name = name;
            MaxSpots = maxSpots;
            SpotsAvailable = spotsAvailable;
            PercentAvailable = (decimal)SpotsAvailable / MaxSpots * 100;
        }
    }
}
