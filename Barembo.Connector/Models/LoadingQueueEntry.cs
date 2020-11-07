using Barembo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    internal struct LoadingQueueEntry<T> : IEquatable<LoadingQueueEntry<T>>
    {
        public LoadElementAsyncDelegate<T> LoadElementAsync { get; set; }
        public ElementLoadedDelegate<T> ElementLoaded { get; set; }
        public ElementLoadingDequeuedDelegate ElementLoadingDequeued { get; set; }

        public bool Equals(LoadingQueueEntry<T> other)
        {
            throw new NotImplementedException();
        }
    }
}
