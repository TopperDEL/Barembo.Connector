using Barembo.Interfaces;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class ThumbnailGeneratorServiceTest
    {
        private IThumbnailGeneratorService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new ThumbnailGeneratorService();
        }

        [TestMethod]
        public async Task ThumbnailFromImgae_Gets_Generated()
        {
            try
            {
                File.Delete("TestImageThumbnail.jpg");
            }
            catch { }

            using (FileStream image = new FileStream("TestImage.jpg", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromImageAsync(image);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestImageThumbnail.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestImageThumbnail.jpg"));
            }
        }

        [TestMethod]
        public async Task ThumbnailFromImage_Gets_GeneratedEvenIfStreamIsNotAtPositionZero()
        {
            try
            {
                File.Delete("TestImageThumbnail2.jpg");
            }
            catch { }

            using (FileStream image = new FileStream("TestImage.jpg", FileMode.Open))
            {
                image.Position = 10;
                var result = await _service.GenerateThumbnailBase64FromImageAsync(image);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestImageThumbnail2.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestImageThumbnail2.jpg"));
            }
        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_Generated()
        {
            try
            {
                File.Delete("TestVideoThumbnail1.jpg");
            }
            catch { }

            using (FileStream video = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromVideoAsync(video, 0f);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestVideoThumbnail1.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestVideoThumbnail1.jpg"));
            }
        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter25Percent()
        {
            try
            {
                File.Delete("TestVideoThumbnail2.jpg");
            }
            catch { }

            using (FileStream video = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromVideoAsync(video, 0.25f);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestVideoThumbnail2.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestVideoThumbnail2.jpg"));
            }
        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter50Percent()
        {
            try
            {
                File.Delete("TestVideoThumbnail3.jpg");
            }
            catch { }

            using (FileStream video = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromVideoAsync(video, 0.25f);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestVideoThumbnail3.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestVideoThumbnail3.jpg"));
            }
        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter75Percent()
        {
            try
            {
                File.Delete("TestVideoThumbnail4.jpg");
            }
            catch { }

            using (FileStream video = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromVideoAsync(video, 0.75f);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestVideoThumbnail4.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestVideoThumbnail4.jpg"));
            }
        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter99Percent()
        {
            try
            {
                File.Delete("TestVideoThumbnail5.jpg");
            }
            catch { }

            using (FileStream video = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromVideoAsync(video, 0.99f);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync("TestVideoThumbnail5.jpg", resultBytes);

                Assert.IsTrue(File.Exists("TestVideoThumbnail5.jpg"));
            }
        }
    }
}
