namespace UCFParkingBot.Library
{
    public class Building
    {
        public readonly string Name;

        public readonly double latitude;

        public readonly double longitude;

        public readonly string abbreviation;

        public Building(string name, double latitude, double longitude, string abbreviation = null)
        {
            this.Name = name;
            this.latitude = latitude;
            this.longitude = longitude;
            if(!string.IsNullOrWhiteSpace(abbreviation))
            {
                this.abbreviation = abbreviation;
            }
        }
    }
}
