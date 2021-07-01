namespace Insurance.Car
{
    public class Insured
    {
        public Insured(string identity, string zipCode, bool mainDriver)
        {
            Identity = identity;
            ZipCode = zipCode;
            MainDriver = mainDriver;
        }

        public string Identity { get; private set; }
        public string ZipCode { get; private set; }
        public bool MainDriver { get; private set; }
    }
}
