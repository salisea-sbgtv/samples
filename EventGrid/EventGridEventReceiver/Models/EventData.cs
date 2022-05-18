using EventGridEventPublisher.Models;

namespace EventGridEventReceiver.Models
{
    public class EventData
    {
        public string id { get; set; }
        public string topic { get; set; }
        public string subject { get; set; }
        public string eventType { get; set; }
        public string eventTime { get; set; }
        public Entity data { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
    }
}
