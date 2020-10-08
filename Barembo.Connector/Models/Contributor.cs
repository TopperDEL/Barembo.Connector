using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A Contributor to a book
    /// </summary>
    public class Contributor
    {
        /// <summary>
        /// The Id of a Contributor
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The name of that Contributor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The AccessRights of that Contributor
        /// </summary>
        public AccessRights AccessRights { get; set; }

        /// <summary>
        /// The serialised Access to the Book for this Contributor
        /// </summary>
        public string AccessGrant { get; set; }
    }
}
