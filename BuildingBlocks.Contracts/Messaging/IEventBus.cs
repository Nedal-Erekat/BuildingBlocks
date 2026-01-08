namespace BuildingBlocks.Contracts.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
    void Subscribe<T, TH>()
        where T : class
        where TH : IEventHandler<T>;
}

