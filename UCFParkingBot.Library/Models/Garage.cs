namespace UCFParkingBot.Library
{
    public class Garage : Building
    {
        public readonly int MaxSpots;
        public int SpotsAvailable;
        public decimal PercentAvailable { get => this.SpotsAvailable / this.MaxSpots * 100; }

        public Garage(string name, int maxSpots, double[] coordinates, int spotsAvailable = 0 )
            : base(name, coordinates)
        {
            this.PartitionKey = "garage";
            this.RowKey = name;

            this.MaxSpots = maxSpots;
            this.SpotsAvailable = spotsAvailable;
        }
    }
}
