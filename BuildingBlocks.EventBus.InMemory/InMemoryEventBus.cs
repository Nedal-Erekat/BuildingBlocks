using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EventBus.InMemory;

public class InMemoryEventBus : Contracts.Messaging.IEventBus
{
    private readonly System.IServiceProvider _serviceProvider;
    private static readonly Dictionary<Type, List<Type>> _handlers = new();

    public InMemoryEventBus(System.IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        var eventType = typeof(T);
        if (!_handlers.ContainsKey(eventType)) return;

        using var scope = _serviceProvider.CreateScope();
        foreach (var handlerType in _handlers[eventType])
        {
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
            await ((IEventHandler<T>)handler).HandleAsync(@event);
        }
    }

    public void Subscribe<T, TH>()
        where T : class
        where TH : IEventHandler<T>
    {
        var eventType = typeof(T);
        var handlerType = typeof(TH);

        if (!_handlers.ContainsKey(eventType))
            _handlers[eventType] = new List<Type>();

        _handlers[eventType].Add(handlerType);
    }
}
