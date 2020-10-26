using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// Contains information about a StoreObject
    /// </summary>
    public struct StoreObjectInfo : IEquatable<StoreObjectInfo>
    {
        /// <summary>
        /// True, if the object exists on the store (and is accessable). False if not.
        /// </summary>
        public bool ObjectExists { get; set; }

        /// <summary>
        /// The size of the object if it exists (and is accessable).
        /// </summary>
        public long Size { get; set; }

        public bool Equals(StoreObjectInfo other)
        {
            throw new NotImplementedException();
        }
    }
}
