using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Helper
{
    static class JSONHelper
    {
        internal static byte[] SerializeToJSON(object objectToSerialize)
        {
            var JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(objectToSerialize);
            return Encoding.UTF8.GetBytes(JSONString);
        }

        internal static T DeserializeFromJSON<T>(byte[] JSONBytes)
        {
            var JSONString = Encoding.UTF8.GetString(JSONBytes);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JSONString);
        }
    }
}
