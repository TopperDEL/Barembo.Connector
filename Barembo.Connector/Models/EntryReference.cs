using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public struct EntryReference
    {
        public BookReference BookReference { get; set; }
        public string EntryKey { get; set; }
        public string EntryId { get; set; }
    }
}
