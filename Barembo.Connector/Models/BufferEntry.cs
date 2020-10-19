using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    internal class BufferEntry
    {
        [PrimaryKey]
        public string Id { get; set; }

        public byte[] BufferedContent { get; set; }
    }
}
