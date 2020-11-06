using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Interfaces
{
    public delegate void ElementLoadedDelegate<T>(T loadedElement);
    public delegate void ElementLoadingDequeuedDelegate();
    public interface IQueuedPriorityLoaderService<T>
    {
        void LoadWithHighPriority(StoreAccess access, StoreKey storeKey, ElementLoadedDelegate<T> elementLoaded, ElementLoadingDequeuedDelegate loadingDequeued);
    }
}
