using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A LocalStoreAdapter retrieves a value for a key from the local
    /// secure device store.
    /// </summary>
    public interface ILocalStoreAdapter
    {
        /// <summary>
        /// Get the value of a key if it exists
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <returns>The value if it exists, otherwise String.Empty</returns>
        string GetValue(string key);
    }
}
