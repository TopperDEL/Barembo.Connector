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
        public async Task<string> GenerateThumbnailBase64FromImageAsync(Stream imageStream)
        {
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
    }
}
