using Cympatic.Extensions.Stub.Internal.Interfaces;

namespace Cympatic.Extensions.Stub.Internal.Collections;

internal abstract class AutomaticExpireCollection<TItem> : IDisposable
    where TItem : IAutomaticExpireItem
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly HashSet<TItem> _internalList = [];

    private readonly Timer _timer;
    private readonly TimeSpan _timerInterval;
    private TimeSpan _ttl;

    public int Count
    {
        get
        {
            _lock.EnterWriteLock();
            try
            {
                return _internalList.Count;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    protected AutomaticExpireCollection() : this(new TimeSpan(0, 0, 1), new TimeSpan(0, 1, 0))
    { }

    protected AutomaticExpireCollection(TimeSpan timerInterval) : this(timerInterval, new TimeSpan(0, 1, 0))
    { }

    protected AutomaticExpireCollection(TimeSpan timerInterval, TimeSpan timeToLive)
    {
        _timerInterval = timerInterval;
        _ttl = timeToLive;

        _timer = new Timer(new TimerCallback(CleanUpTimer), null, _timerInterval, _timerInterval);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void SetTimeToLive(TimeSpan timeToLive)
    {
        _ttl = timeToLive;
        _timer?.Change(TimeSpan.Zero, _timerInterval);
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

        _lock.EnterWriteLock();
        try
        {
            _internalList.Add(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public virtual bool Remove(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _lock.EnterWriteLock();
        try
        {
            return _internalList.Remove(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public virtual void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _internalList.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        using (var manualEvent = new ManualResetEventSlim())
        {
            _timer.Dispose(manualEvent.WaitHandle);
            manualEvent.WaitHandle.WaitOne();
        }

        _lock.Dispose();
    }

    protected void AddOrUpdate(IEnumerable<TItem> items, Action<IEnumerable<TItem>, TItem> addOrUpdateAction)
    {
        ArgumentNullException.ThrowIfNull(items);

        _lock.EnterUpgradeableReadLock();
        try
        {
            items.ToList().ForEach(item => addOrUpdateAction([.. _internalList], item));
        }
        finally
        {
            _lock.ExitUpgradeableReadLock();
        }
    }

    protected IEnumerable<TItem> Find(Func<TItem, bool> predicate)
    {
        _lock.EnterReadLock();
        try
        {
            return _internalList.Where(predicate);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private void CleanUpTimer(object? state)
    {
        _lock.EnterWriteLock();
        try
        {
            _internalList.RemoveWhere(model => model.CreatedDateTime.Add(_ttl) < DateTimeOffset.UtcNow);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}
