using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public delegate Task<T> LoadElementAsyncDelegate<T>();
    public delegate void ElementLoadedDelegate<in T>(T loadedElement);
    public delegate void ElementLoadingDequeuedDelegate();
    public interface IQueuedPriorityLoaderService<T>
    {
        void LoadWithHighPriority(LoadElementAsyncDelegate<T> loadElementAsync, ElementLoadedDelegate<T> elementLoaded, ElementLoadingDequeuedDelegate loadingDequeued);
        void StopAllLoading(bool disposeAfterwards);
        bool HasEntriesToLoad { get; }
    }
}
