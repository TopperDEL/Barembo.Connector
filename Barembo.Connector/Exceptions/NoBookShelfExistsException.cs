using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Exceptions
{
    public class NoBookShelfExistsException : Exception
    {
        public string AccessGrant { get; set; }
        public string StoreKey { get; set; }
        public string AdditionalError { get; set; }
        public NoBookShelfExistsException(string accessGrant, string storeKey, string additionalError)
        {
            AccessGrant = accessGrant;
            StoreKey = storeKey;
            AdditionalError = additionalError;
        }
    }
}
