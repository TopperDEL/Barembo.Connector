using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// LoginData is used to connect to the Store
    /// </summary>
    public class LoginData
    {
        /// <summary>
        /// The address of a satellite
        /// </summary>
        public string SatelliteAddress { get; set; }

        /// <summary>
        /// The API-key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The encryption passphrase/secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The access grant holding all of the access info (satellite, apikey, secret)
        /// </summary>
        public string AccessGrant { get; set; }
    }
}
