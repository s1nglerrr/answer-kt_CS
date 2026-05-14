using EventStore.Client;
using System.Text;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class EventStoreService
    {
        private readonly EventStoreClient _client;

        public EventStoreService(EventStoreClient client)
        {
            _client = client;
        }

        public async Task AppendEventAsync<T>(string streamName, T @event)
        {
            EventData eventData = new EventData(
                Uuid.NewUuid(),
                typeof(T).Name,
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );

            await _client.AppendToStreamAsync(
                streamName,
                StreamState.Any,
                new[] { eventData }
            );
        }

        public async Task<List<T>> ReadEventsAsync<T>(string streamName)
        {
            List<T> events = new List<T>();
            EventStoreClient.ReadStreamResult result = _client.ReadStreamAsync(
                Direction.Forwards,
                streamName,
                StreamPosition.Start
            );

            await foreach (ResolvedEvent resolvedEvent in result)
            {
                string json = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
                T? @event = JsonSerializer.Deserialize<T>(json);
                events.Add(@event);
            }

            return events;
        }

        public async Task<bool> StreamExists(string streamName)
        {
            try
            {
                EventStoreClient.ReadStreamResult result = _client.ReadStreamAsync(
                    Direction.Backwards,
                    streamName,
                    StreamPosition.End,
                    1
                );

                await foreach (ResolvedEvent _ in result)
                {
                    return true;
                }

                return false;
            }
            catch (StreamNotFoundException)
            {
                return false;
            }
        }
    }
}
