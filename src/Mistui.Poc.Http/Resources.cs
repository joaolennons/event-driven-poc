namespace Mitsui.Poc.Http
{
    internal static class Resources
    {
        internal static class EventSourcing
        {
            internal static class Store
            {
                internal const string Name = "mydatabase";
                internal const string Collection = "events";
                internal const string ConnectionStringKey = "WriteDb";
            }

            internal static class Broker
            {
                internal static class Topics
                {
                    internal const string QuotationUpdated = nameof(QuotationUpdated);
                    internal const string QuotationPlanCalculationRequested = nameof(QuotationPlanCalculationRequested);
                    internal const string QuotationEmissionHasBeeenRequested = nameof(QuotationEmissionHasBeeenRequested);
                }

                internal const string ConnectionStringKey = "AzureWebJobsStorage";
            }
        }
    }

    internal static class HttpMethod
    {
        internal const string get = nameof(get);
        internal const string post = nameof(post);
        internal const string put = nameof(put);
        internal const string patch = nameof(patch);
    }
}
