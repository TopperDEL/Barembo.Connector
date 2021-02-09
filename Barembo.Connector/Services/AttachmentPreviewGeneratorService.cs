using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class AttachmentPreviewGeneratorService : IAttachmentPreviewGeneratorService
    {
        readonly IThumbnailGeneratorService _thumbnailGeneratorService;
        public AttachmentPreviewGeneratorService(IThumbnailGeneratorService thumbnailGeneratorService)
        {
            _thumbnailGeneratorService = thumbnailGeneratorService;
        }

        public async Task<AttachmentPreview> GeneratePreviewAsync(Attachment attachment, Stream attachmentBinary, string filePath)
        {
            List<string> parts = new List<string>();

            if(attachment.Type == AttachmentType.Image)
            {
                var thumbnail = await _thumbnailGeneratorService.GenerateThumbnailBase64FromImageAsync(attachmentBinary);
                parts.Add(thumbnail);
            }
            else
            {
                var part1 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(attachmentBinary, 0f, filePath);
                parts.Add(part1);

                var part2 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(attachmentBinary, 0.2f, filePath);
                parts.Add(part2);

                var part3 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(attachmentBinary, 0.4f, filePath);
                parts.Add(part3);

                var part4 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(attachmentBinary, 0.6f, filePath);
                parts.Add(part4);

                var part5 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(attachmentBinary, 0.8f, filePath);
                parts.Add(part5);

                var part6 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(attachmentBinary, 0.9f, filePath);
                parts.Add(part6);
            }

            return new AttachmentPreview(attachment.Type, parts);
        }
    }
}
