using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class QueuedPriorityLoaderService<T> : IQueuedPriorityLoaderService<T>, IDisposable
    {
        ConcurrentStack<LoadingQueueEntry<T>> _stack;
        Task _loadingTask;
        CancellationTokenSource _source;

        public bool HasEntriesToLoad
        {
            get
            {
                return _stack.Count > 0;
            }
        }

        public QueuedPriorityLoaderService()
        {
            _stack = new ConcurrentStack<LoadingQueueEntry<T>>();

            _source = new CancellationTokenSource();
            _loadingTask = Task.Factory.StartNew(async () => await LoadFromStackAsync(_source.Token).ConfigureAwait(false), _source.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void LoadWithHighPriority(LoadElementAsyncDelegate<T> loadElementAsync, ElementLoadedDelegate<T> elementLoaded, ElementLoadingDequeuedDelegate loadingDequeued)
        {
            LoadingQueueEntry<T> entry = new LoadingQueueEntry<T> { LoadElementAsync = loadElementAsync, ElementLoaded = elementLoaded, ElementLoadingDequeued = loadingDequeued };
            _stack.Push(entry);
        }

        private async Task LoadFromStackAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_stack.Count > 0)
                {
                    if (_stack.TryPop(out LoadingQueueEntry<T> toDo))
                    {
                        try
                        {
                            try
                            {
                                var loadedObject = await toDo.LoadElementAsync();
                                toDo.ElementLoaded(loadedObject);
                            }
                            catch (Exception)
                            {
                                toDo.ElementLoadingDequeued();
                            }
                        }
                        catch
                        {
                            //The element got poped and it failed to ignore the using element - ignore it here
                        }
                    }
                    else
                    {
                        await Task.Delay(100).ConfigureAwait(false);
                    }
                }
                else
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_source != null)
                _source.Cancel();
            _loadingTask = null;
            _stack = null;
            _source = null;
        }

        public void StopAllLoading(bool disposeAfterwards)
        {
            _stack.Clear();
            if (disposeAfterwards)
                Dispose(true);
        }
    }
}
