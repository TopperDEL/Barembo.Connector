using Barembo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    internal struct LoadingQueueEntry<T>
    {
        public StoreAccess StoreAccess{ get; set; }
        public StoreKey StoreKey { get; set; }
        public ElementLoadedDelegate<T> ElementLoaded { get; set; }
        public ElementLoadingDequeuedDelegate ElementLoadingDequeued { get; set; }
    }
}
