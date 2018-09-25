namespace UCFParkingBot.Library
{
    public class Garage : Building
    {
        public readonly int MaxSpots;
        public int SpotsAvailable;
        public decimal PercentAvailable { get; private set; }

        public Garage(string name, int maxSpots, double[] coordinates, int spotsAvailable = 0 )
            : base(name, coordinates)
        {
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
