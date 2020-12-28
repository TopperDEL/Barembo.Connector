using Barembo.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class ThumbnailGeneratorService : IThumbnailGeneratorService
    {
        public bool ThumbnailCallback()
        {
            return false;
        }

        public async Task<string> GenerateThumbnailBase64FromImageAsync(Stream imageStream)
        {
            return await Task.Factory.StartNew(() =>
            {
                Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                Bitmap bmp = new Bitmap(imageStream);

                int width = 600;
                int X = bmp.Width;
                int Y = bmp.Height;
                int height = (width * Y) / X;

                var thumbnail = bmp.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
                MemoryStream thumbStream = new MemoryStream();
                thumbnail.Save(thumbStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                thumbStream.Position = 0;
                return Convert.ToBase64String(thumbStream.ToArray());
            });
        }
    }
}
