using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A StoreObject holds the key and the id of an object in the store.
    /// </summary>
    public struct StoreObject
    {
        /// <summary>
        /// The Key ist the "full-path" to an object in the store.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Id is the "filename" of an object
        /// </summary>
        public string Id { get; set; }

        internal StoreObject(string key, string id)
        {
            Key = key;
            Id = id;
        }
    }
}
