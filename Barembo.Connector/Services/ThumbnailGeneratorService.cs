using Barembo.Interfaces;
using LibVLCSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class ThumbnailGeneratorService : IThumbnailGeneratorService
    {
        private static LibVLC _libVlc;

        /// <summary>
        /// Only initialize if LibVlc 4.* native binaries are present
        /// </summary>
        public static void InitializeLibVlc()
        {
            Core.Initialize();
            _libVlc = new LibVLC();
        }

        public static Func<Stream, int, int, Task<string>> ImageThumbnailAsyncCallback { get; set; }
        public static Func<Stream, float, string, Task<string>> VideoThumbnailAsyncCallback { get; set; }

        public async Task<string> GenerateThumbnailBase64FromImageAsync(Stream imageStream)
        {
            if (ImageThumbnailAsyncCallback != null)
            {
                return await ImageThumbnailAsyncCallback(imageStream, 600, 450).ConfigureAwait(false);
            }

            imageStream.Position = 0;
            using (var thumbnailStream = GetThumbnail(imageStream, 600, 450))
            {
                return Convert.ToBase64String(thumbnailStream.ToArray());
            }
        }

        private MemoryStream GetThumbnail(Stream baseImage, int targetWidth, int targetHeight)
        {
            using (SKBitmap sourceBitmap = SKBitmap.Decode(baseImage))
            {
                SKImageInfo resizeInfo = new SKImageInfo(targetWidth, targetHeight);

                // Test whether there is more room in width or height
                if (Math.Abs(sourceBitmap.Width - targetWidth) <= Math.Abs(sourceBitmap.Height - targetHeight))
                {
                    // More room in width, so leave image width set to canvas width
                    // and increase/decrease height by same ratio
                    double widthRatio = (double)targetWidth / (double)sourceBitmap.Width;
                    int newHeight = (int)Math.Floor(sourceBitmap.Height * widthRatio);

                    resizeInfo.Height = newHeight;
                }
                else
                {
                    // More room in height, so leave image height set to canvas height
                    // and increase/decrease width by same ratio                 
                    double heightRatio = (double)targetHeight / (double)sourceBitmap.Height;
                    int newWidth = (int)Math.Floor(sourceBitmap.Width * heightRatio);

                    resizeInfo.Width = newWidth;
                }

                using (SKBitmap scaledBitmap = sourceBitmap.Resize(resizeInfo, SKFilterQuality.High))
                {
                    using (SKImage scaledImage = SKImage.FromBitmap(scaledBitmap))
                    {
                        using (SKData data = scaledImage.Encode())
                        {
                            return new MemoryStream(data.ToArray());
                        }
                    }
                }
            }
        }

        public async Task<string> GenerateThumbnailBase64FromVideoAsync(Stream videoStream, float positionPercent, string filePath)
        {
            using (Media media = new Media(new StreamMediaInput(videoStream)))
            {
                var thumbnail = await media.GenerateThumbnailAsync(_libVlc, positionPercent, ThumbnailerSeekSpeed.Fast, 600, 450, true, PictureType.Png);
                var buffer = thumbnail.Buffer;
                var size = (uint)buffer.size;
                byte[] managedArray = new byte[size];
                Marshal.Copy(buffer.buffer, managedArray, 0, (int)size);
                return Convert.ToBase64String(managedArray);
            }
        }
    }
}
