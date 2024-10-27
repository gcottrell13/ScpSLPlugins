using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;
using System;
using System.Collections.Generic;

namespace CustomGameModes.API;


internal class EventHelper : IDisposable
{
    private List<Action> _unsubscribeFunctions = new();

    public void Dispose()
    {
        foreach (var unsub in _unsubscribeFunctions)
        {
            unsub();
        }
    }

    public void Register<T>(Event<T> @event, CustomEventHandler<T> handler)
        where T : class, IExiledEvent
    {
        _unsubscribeFunctions.Add(() => @event.Unsubscribe(handler));
        @event.Subscribe(handler);
    }

    public void Register<T>(Event<T> @event, CustomAsyncEventHandler<T> handler)
        where T : class, IExiledEvent
    {
        _unsubscribeFunctions.Add(() => @event.Unsubscribe(handler));
        @event.Subscribe(handler);
    }
}
