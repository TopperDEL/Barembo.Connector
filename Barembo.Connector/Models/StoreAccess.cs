using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public struct StoreAccess
    {
        public string AccessGrant { get; private set; }

        public StoreAccess(string accessGrant)
        {
            AccessGrant = accessGrant;
        }
    }
}
