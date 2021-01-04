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
            await GenerateAndTestVideo("TestVideoThumbnail_0prz.jpg", 0f);

        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter25Percent()
        {
            await GenerateAndTestVideo("TestVideoThumbnail_25prz.jpg", 0.25f);

        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter50Percent()
        {
            await GenerateAndTestVideo("TestVideoThumbnail_50prz.jpg", 0.50f);

        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter75Percent()
        {
            await GenerateAndTestVideo("TestVideoThumbnail_75prz.jpg", 0.75f);

        }

        [TestMethod]
        public async Task ThumbnailFromVideo_Gets_GeneratedForAfter99Percent()
        {
            await GenerateAndTestVideo("TestVideoThumbnail_99prz.jpg", 0.99f);
        }

        private async Task GenerateAndTestVideo(string filename, float position)
        {
            try
            {
                File.Delete(filename);
            }
            catch { }

            using (FileStream video = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GenerateThumbnailBase64FromVideoAsync(video, position);
                var resultBytes = Convert.FromBase64String(result);
                await File.WriteAllBytesAsync(filename, resultBytes);

                Assert.IsTrue(File.Exists(filename));
            }
        }
    }
}
