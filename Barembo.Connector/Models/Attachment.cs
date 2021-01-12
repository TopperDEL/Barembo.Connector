using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public class Attachment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public AttachmentType Type { get; set; }
        public long Size { get; set; }
    }
}
