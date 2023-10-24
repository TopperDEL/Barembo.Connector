using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    public struct AttachmentPreview : IEquatable<AttachmentPreview>
    {
        public AttachmentType Type { get; set; }
        public List<string> PreviewPartsBase64 { get; set; }

        public AttachmentPreview(AttachmentType type, List<string> previewPartsBase64)
        {
            Type = type;
            PreviewPartsBase64 = previewPartsBase64;
        }

        public bool Equals(AttachmentPreview other)
        {
            return other.Type == Type && other.PreviewPartsBase64 == PreviewPartsBase64;
        }
    }
}