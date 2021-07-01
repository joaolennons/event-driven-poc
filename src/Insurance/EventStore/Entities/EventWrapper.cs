using Mitsui.Poc.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace EventStore
{
    public class EventWrapper
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("stream")]
        public StreamInfo StreamInfo { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventData")]
        public JObject EventData { get; set; }
        internal IEvent GetEvent()
        {
            Type eventType = Type.GetType($"Mitsui.Poc.Events.{EventType}, Mitsui.Poc.Events");

            return (IEvent)EventData.ToObject(eventType);
        }
    }
}
