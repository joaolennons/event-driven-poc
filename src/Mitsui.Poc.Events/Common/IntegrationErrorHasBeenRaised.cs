namespace Mitsui.Poc.Events
{
    public class IntegrationErrorHasBeenRaised : IEvent
    {
        public string Reason { get; set; }
        public string Source { get; set; }
    }
}
