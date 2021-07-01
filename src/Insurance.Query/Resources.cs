using Microsoft.Azure.Documents.Client;
using System;

namespace Insurance.Query
{
    internal static class Resources
    {
        internal static class ReadDatabase
        {
            internal const string Name = "Insurance";
            internal const string Collection = "Quotations";
            internal const string ConnectionStringKey = "WriteDb";
            internal static Uri CollectionUri => UriFactory.CreateDocumentCollectionUri(Name, Collection);
        }
    }
}
