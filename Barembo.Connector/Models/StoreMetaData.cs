using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public struct StoreMetaData : IEquatable<StoreMetaData>
    {
        public static string STOREMETADATA_TIMESTAMP
        {
            get { return "BAREMBO:TIMESTAMP"; }
        }

        public string Key { get; set; }
        public string Value { get; set; }

        internal StoreMetaData(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public bool Equals(StoreMetaData other)
        {
            return Key == other.Key && Value == other.Value;
        }
    }
}
