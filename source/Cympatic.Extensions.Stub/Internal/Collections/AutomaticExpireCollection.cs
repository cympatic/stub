using Cympatic.Extensions.Stub.Internal.Interfaces;

namespace Cympatic.Extensions.Stub.Internal.Collections;

internal class AutomaticExpireCollection<TItem> : IAsyncDisposable, IDisposable
    where TItem : IAutomaticExpireItem
{
    private readonly object _lock = new();
    private readonly HashSet<TItem> _internalList = [];

    private readonly TimeSpan _timerInterval;

    private Timer? _timer;
    private TimeSpan _ttl;

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _internalList.Count;
            }
        }
    }

    protected AutomaticExpireCollection() : this(new TimeSpan(0, 1, 0))
    { }

    protected AutomaticExpireCollection(TimeSpan timerInterval)
    {
        _timerInterval = timerInterval;

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
        _timer?.Change(_timerInterval, _timerInterval);
    }

    public virtual IEnumerable<TItem> All()
    {
        return Find(_ => true);
    }

    public virtual void Add(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        item.Id = Guid.NewGuid();
        item.CreatedDateTime = DateTimeOffset.UtcNow;

        lock (_lock)
        {
            _internalList.Add(item);
        }
    }

    public virtual bool Remove(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_lock)
        {
            return _internalList.Remove(item);
        }
    }

    public virtual void Clear()
    {
        lock (_lock)
        {
            _internalList.Clear();
        }
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

    protected void AddOrUpdate(IEnumerable<TItem> items, Action<IEnumerable<TItem>, TItem> addOrUpdateAction)
    {
        ArgumentNullException.ThrowIfNull(items);

        List<TItem> list;
        lock (_lock)
        {
            list = [.. _internalList];
        }

        items
            .ToList()
            .ForEach(item => addOrUpdateAction(list, item));
    }

    protected IEnumerable<TItem> Find(Func<TItem, bool> predicate)
    {
        lock (_lock)
        {
            return _internalList.Where(predicate);
        }
    }

    private void CleanUpTimer(object? state)
    {
        lock (_lock)
        {
            _internalList.RemoveWhere(model => model.CreatedDateTime.Add(_ttl) < DateTimeOffset.UtcNow);
        }
    }
}
