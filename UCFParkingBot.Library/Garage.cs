namespace UCFParkingBot.Library
{
    public class Garage
    {
        public readonly string Name;
        public readonly int MaxSpots;
        public int SpotsAvailable;
        public decimal PercentAvailable { get; private set; }

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
