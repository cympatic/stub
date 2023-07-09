using Cympatic.Stub.Connectivity.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Server.Containers;

public abstract class ModelContainer<TModel> : IAsyncDisposable, IDisposable
    where TModel : StubModel
{
    private readonly object _lock = new();
    private readonly ConcurrentDictionary<string, HashSet<TModel>> _internalContainer = new();

    private readonly TimeSpan _timerInterval;
    private readonly ILogger<ModelContainer<TModel>> _logger;

    private Timer _timer;
    private TimeSpan _ttl;

    protected ModelContainer(ILogger<ModelContainer<TModel>> logger) : this(new TimeSpan(0, 1, 0), logger)
    { }

    protected ModelContainer(TimeSpan timerInterval, ILogger<ModelContainer<TModel>> logger)
    {
        _timerInterval = timerInterval;
        _logger = logger;

        _timer = new Timer(new TimerCallback(CleanUpTimer));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    public virtual void SetTimeToLive(TimeSpan timeToLive)
    {
        _ttl = timeToLive;
        _timer.Change(_timerInterval, _timerInterval);
    }

    public virtual IEnumerable<TModel> Get(string identifierValue)
    {
        _logger.LogDebug("{type}.Get with identifier: '{identifier}'", GetType().Name, identifierValue);

        if (_internalContainer.TryGetValue(identifierValue, out var items))
        {
            return items.ToArray();
        }

        return Array.Empty<TModel>();
    }

    public virtual void Remove(string identifierValue)
    {
        _logger.LogDebug("{type}.Clear for identifier: '{identifier}'", GetType().Name, identifierValue);

        _internalContainer.TryRemove(identifierValue, out var _);
    }

    public virtual void AddModel(string identifierValue, TModel model)
    {
        _logger.LogDebug("{type}.AddModel for identifier: '{identifier}' with\r\n{@model}", GetType().Name, identifierValue, model);

        _internalContainer.AddOrUpdate(identifierValue,
            new HashSet<TModel> { model },
            (key, oldValue) =>
            {
                lock (_lock)
                {
                    oldValue.Add(model);
                }
                return oldValue;
            });
    }

    protected virtual void AddOrUpdate(string identifierValue, IEnumerable<TModel> models, Action<HashSet<TModel>, TModel> addOrUpdateAction)
    {
        _logger.LogDebug("{type}.AddOrUpdate for identifier: '{identifier}' with Models:\r\n{@models}", GetType().Name, identifierValue, models);

        _internalContainer.AddOrUpdate(identifierValue,
            new HashSet<TModel>(models),
            (key, oldValue) =>
            {
                lock (_lock)
                {
                    models
                        .ToList()
                        .ForEach(model => addOrUpdateAction(oldValue, model));
                }
                return oldValue;
            });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Dispose();
        }
        _timer = null;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_timer is not null)
        {
            await _timer.DisposeAsync();
        }

        _timer = null;
    }

    private void CleanUpTimer(object state)
    {
        foreach (var key in _internalContainer.Keys)
        {
            _internalContainer.AddOrUpdate(key,
                new HashSet<TModel>(),
                (key, oldValue) =>
                {
                    lock (_lock)
                    {
                        oldValue.RemoveWhere(model => model.GetCreatedDateTime().Add(_ttl) < DateTimeOffset.UtcNow);
                    }
                    return oldValue;
                });
        }
    }
}
