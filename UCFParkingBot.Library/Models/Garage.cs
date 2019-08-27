namespace UCFParkingBot.Library
{
    public class Garage
    {
        public readonly string Name;
        public readonly int MaxSpots;
        public int SpotsAvailable;

        public Garage(string name, int maxSpots)
        {
            this.Name = name;            
            this.MaxSpots = maxSpots;
        }
    }
}
