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
            this.Name = name;
            this.MaxSpots = maxSpots;
            this.SpotsAvailable = spotsAvailable;
            SetPercentAvailable();
        }

        public void SetPercentAvailable()
        {
            this.PercentAvailable = (decimal)SpotsAvailable / MaxSpots * 100;
        }
    }
}
