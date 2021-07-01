using EventStore;
using Microsoft.Azure.Cosmos;
using Mitsui.Poc.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Repositories
{
    public class EventStore : IEventStore
    {
        private readonly CosmosClient _client;
        private readonly string _databaseId = "mydatabase";
        private readonly string _containerId = "events";

        public EventStore()
        {
            _client = new CosmosClient("https://localhost:8081/", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
        }

        public async Task<bool> AppendToStreamAsync(string streamId, int expectedVersion, IEnumerable<IEvent> events)
        {
            Container container = _client.GetContainer(_databaseId, _containerId);

            PartitionKey partitionKey = new PartitionKey(streamId);

            dynamic[] parameters = new dynamic[]
            {
                streamId,
                expectedVersion,
                SerializeEvents(streamId, expectedVersion, events)
            };

            return await container.Scripts.ExecuteStoredProcedureAsync<bool>("spAppendToStream", partitionKey, parameters);
        }

        private static string SerializeEvents(string streamId, int expectedVersion, IEnumerable<IEvent> events)
        {
            var items = events.Select(e => new EventWrapper
            {
                Id = $"{streamId}:{++expectedVersion}",
                StreamInfo = new StreamInfo
                {
                    Id = streamId,
                    Version = expectedVersion
                },
                EventType = e.GetType().Name,
                EventData = JObject.FromObject(e)
            });

            return JsonConvert.SerializeObject(items);
        }

        public async Task<EventStream> LoadStreamAsync(Guid streamId)
        {
            Container container = _client.GetContainer(_databaseId, _containerId);

            var sqlQueryText = "SELECT * FROM events e"
                + " WHERE e.stream.id = @streamId"
                + " ORDER BY e.stream.version";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText)
                .WithParameter("@streamId", streamId);

            int version = 0;
            var events = new List<IEvent>();

            var feedIterator = container.GetItemQueryIterator<EventWrapper>(queryDefinition);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var eventWrapper in response)
                {
                    version = eventWrapper.StreamInfo.Version;

                    events.Add(eventWrapper.GetEvent());
                }
            }

            return new EventStream(streamId.ToString(), version, events);
        }
    }
}
