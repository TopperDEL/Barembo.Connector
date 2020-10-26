using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Helper
{
    abstract class VersionHelper
    {
        /// <summary>
        /// The current version of the app. Can be used to migrate in case of future adjustments.
        /// </summary>
        public readonly static string CURRENT_VERSION = "1";

        protected VersionHelper()
        {

        }
    }
}
