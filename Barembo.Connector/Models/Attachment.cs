using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public enum AttachmentType
    {
        Image,
        Video
    }
    public class Attachment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public AttachmentType Type { get; set; }
        public long Size { get; set; }
    }
}
