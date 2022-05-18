// Build a config object, using env vars and JSON providers.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Event Grid Client
Azure.Messaging.EventGrid.EventGridPublisherClient client = new Azure.Messaging.EventGrid.EventGridPublisherClient(
    new Uri(Environment.GetEnvironmentVariable("EventGridTopicUri")),
    new Azure.AzureKeyCredential(Environment.GetEnvironmentVariable("EventGridAccessKey")));

EventGridEventPublisher.Models.Entity e = new EventGridEventPublisher.Models.Entity();

e.EntityId = Guid.NewGuid();
e.DisplayValue = "New Entity";

// Add EventGridEvents to a list to publish to the domain
// Don't forget to specify the topic you want the event to be delivered to!
List<Azure.Messaging.EventGrid.EventGridEvent> eventsList = new List<Azure.Messaging.EventGrid.EventGridEvent>
{
    new Azure.Messaging.EventGrid.EventGridEvent(
        "entities",
        "Entity_Created",
        "1.0",
        e)
    {
        Topic = "EntitiesTopic"
    }
};

// Send the event
await client.SendEventAsync(eventsList[0]);
