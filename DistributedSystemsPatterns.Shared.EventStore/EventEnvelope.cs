namespace DistributedSystemsPatterns.Shared.EventStore;

public record EventEnvelope<T>(T Event, EventMetadata Metadata) where T : class, IEvent;
