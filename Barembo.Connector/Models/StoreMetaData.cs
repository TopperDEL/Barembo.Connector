using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public struct StoreMetaData
    {
        public const string STOREMETADATA_TIMESTAMP = "BAREMBO:TIMESTAMP";

        public string Key { get; set; }
        public string Value { get; set; }

        internal StoreMetaData(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
