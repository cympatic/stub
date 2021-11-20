using Cympatic.Stub.Connectivity.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cympatic.Stub.Server.Containers
{
    public abstract class ModelContainer<TModel> : IAsyncDisposable, IDisposable
        where TModel : StubModel
    {
        private readonly TimeSpan _timerInterval;

        private Timer _timer;
        private TimeSpan _ttl;

        protected readonly ConcurrentDictionary<string, HashSet<TModel>> _internalContainer;

        protected ModelContainer() : this(new TimeSpan(0, 1, 0))
        { }

        protected ModelContainer(TimeSpan timerInterval)
        {
            _timerInterval = timerInterval;

            _internalContainer = new ConcurrentDictionary<string, HashSet<TModel>>();
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
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        public virtual void SetTimeToLive(TimeSpan timeToLive)
        {
            _ttl = timeToLive;
            _timer.Change(_timerInterval, _timerInterval);
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
                        oldValue.RemoveWhere(model => model.GetCreatedDateTime().Add(_ttl) < DateTimeOffset.UtcNow);
                        return oldValue;
                    });
            }
        }
    }
}
