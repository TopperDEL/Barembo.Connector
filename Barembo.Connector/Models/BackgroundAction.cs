using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;

namespace Barembo.Models
{
    public enum BackgroundActionTypes
    {
        AddAttachment,
        SetThumbnail
    }

    public class BackgroundAction
    {
        /// <summary>
        /// The ID of a BackgroundAction.
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The creation date of the BackgroundAction.
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The type of the BackgroundAction.
        /// </summary>
        public BackgroundActionTypes ActionType { get; set; }

        /// <summary>
        /// The parameters for this BackgroundAction.
        /// </summary>
        public byte[] ParameterDictionaryAsJson { get; set; }

        public BackgroundAction()
        {
        }

        public BackgroundAction(BackgroundActionTypes actionType, Dictionary<string, object> parameters)
        {
            ActionType = actionType;

            Dictionary<string, byte[]> parameterJson = new Dictionary<string, byte[]>();
            foreach (var param in parameters)
            {
                if (param.Value is string)
                {
                    parameterJson.Add(param.Key, Encoding.UTF8.GetBytes(param.Value as string));
                }
                else
                {
                    parameterJson.Add(param.Key, Serialize(param.Value));
                }
            }

            ParameterDictionaryAsJson = Serialize(parameterJson);
        }

        public Dictionary<string, object> GetParameters()
        {
            Dictionary<string, byte[]> parameterJson;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameterJson = Deserialize<Dictionary<string, byte[]>>(ParameterDictionaryAsJson);

            foreach (var param in parameterJson)
            {
                if (param.Key == "FilePath")
                {
                    parameters.Add(param.Key, Encoding.UTF8.GetString(param.Value));
                }
                else if (param.Key == "EntryReference")
                {
                    parameters.Add(param.Key, Deserialize<EntryReference>(param.Value));
                }
                else if (param.Key == "Attachment")
                {
                    parameters.Add(param.Key, Deserialize<Attachment>(param.Value));
                }
                else
                {
                    throw new NotImplementedException("unknown parameter " + param.Key);
                }
            }
            return parameters;
        }

        private T Deserialize<T>(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BsonDataReader reader = new BsonDataReader(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                }
            }
        }

        private byte[] Serialize(object objectToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonDataWriter writer = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, objectToSerialize);
                }
                return ms.GetBuffer();
            }
        }
    }
}
