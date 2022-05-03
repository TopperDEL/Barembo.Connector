using Barembo.Interfaces;
using LibVLCSharp.Shared;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class ThumbnailGeneratorService : IThumbnailGeneratorService
    {
        static ThumbnailGeneratorService()
        {
            Core.Initialize();
        }

        public static Func<Stream, float, string, Task<string>> VideoThumbnailAsyncCallback { get; set; }

        public async Task<string> GenerateThumbnailBase64FromImageAsync(Stream imageStream)
        {
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
                SKImageInfo resizeInfo = new SKImageInfo(targetWidth, targetHeight);//, info.ColorType, info.AlphaType, info.ColorSpace);

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
            if (VideoThumbnailAsyncCallback != null)
            {
                return await VideoThumbnailAsyncCallback(videoStream, positionPercent, filePath).ConfigureAwait(false);
            }
            //Source: https://github.com/ZeBobo5/Vlc.DotNet/blob/develop/src/Samples/Samples.Core.Thumbnailer/Program.cs

            videoStream.Position = 0;

            var tempFile = Path.GetTempFileName();

            var options = new[]
            {
                "--intf", "dummy", /* no interface                   */
                "--vout", "dummy", /* we don't want video output     */
                "--no-audio", /* we don't want audio decoding   */
                "--no-video-title-show", /* nor the filename displayed     */
                "--no-stats", /* no stats */
                "--no-sub-autodetect-file", /* we don't want subtitles        */
                "--no-snapshot-preview", /* no blending in dummy vout      */
            };

            using (var libvlc = new LibVLC(options))
            {
                var mediaPlayer = new MediaPlayer(libvlc);
                Media video = new Media(libvlc, new StreamMediaInput(videoStream));
                mediaPlayer.Media = video;
                mediaPlayer.EnableHardwareDecoding = true;

                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

                mediaPlayer.TimeChanged += (sender, e) =>
                {
                    mediaPlayer?.TakeSnapshot(0, tempFile, 600, 0);
                    tcs?.TrySetResult(true);
                    mediaPlayer?.Stop();
                    mediaPlayer?.Dispose();
                };
                mediaPlayer.Play();
                mediaPlayer.Position = positionPercent;

                await tcs.Task.ConfigureAwait(false);
            }

            using (FileStream fstream = new FileStream(tempFile, FileMode.Open))
            {
                var bytes = new byte[fstream.Length];
                var readLength = await fstream.ReadAsync(bytes, 0, (int)fstream.Length).ConfigureAwait(false);
                if (readLength == fstream.Length)
                {
                    return Convert.ToBase64String(bytes);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
