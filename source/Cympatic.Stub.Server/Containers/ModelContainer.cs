using Cympatic.Stub.Abstractions.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Cympatic.Stub.Server.Containers
{
    public abstract class ModelContainer<TModel> : IDisposable
        where TModel : StubModel
    {
        private readonly TimeSpan _timerInterval;

        private bool disposedValue = false;
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

        public virtual void SetTimeToLive(TimeSpan timeToLive)
        {
            _ttl = timeToLive;
            _timer.Change(_timerInterval, _timerInterval);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (var manualResetEventSlim = new ManualResetEventSlim())
                    {
                        _timer.Dispose(manualResetEventSlim.WaitHandle);
                        manualResetEventSlim.Wait();
                    }
                    _timer.Dispose();
                    _timer = null;
                }

                disposedValue = true;
            }
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
