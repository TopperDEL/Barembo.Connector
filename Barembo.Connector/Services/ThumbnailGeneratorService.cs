using Barembo.Interfaces;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class ThumbnailGeneratorService : IThumbnailGeneratorService
    {
        public static Func<Stream, long, Task<string>> VideoThumbnailCallback { get; set; }

        public async Task<string> GenerateThumbnailBase64FromImageAsync(Stream imageStream)
        {
            imageStream.Position = 0;
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imageStream))
            {
                image.Mutate(x => x.Resize(600, 0, KnownResamplers.Lanczos3));
                using (MemoryStream mstream = new MemoryStream())
                {
                    await image.SaveAsync(mstream, new JpegEncoder());
                    return Convert.ToBase64String(mstream.ToArray());
                }
            }
        }

        public async Task<string> GenerateThumbnailBase64FromVideoAsync(Stream videoStream, long position)
        {
            videoStream.Position = 0;

            if (VideoThumbnailCallback != null)
                return await VideoThumbnailCallback(videoStream, position);

            return null;
        }
    }
}
