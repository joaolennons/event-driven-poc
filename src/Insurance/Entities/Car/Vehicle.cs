namespace Insurance.Car
{
    public class Vehicle
    {
        public Vehicle(string chassis, string licensePlate)
        {
            Chassis = chassis;
            LicensePlate = licensePlate;
        }

        public string Chassis { get; private set; }
        public string LicensePlate { get; private set; }
    }
}
