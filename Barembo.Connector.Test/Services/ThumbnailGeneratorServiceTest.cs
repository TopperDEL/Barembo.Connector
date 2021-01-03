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
            bool gotCalled = false;
            ThumbnailGeneratorService.VideoThumbnailCallback = (stream, position) =>
            {
                gotCalled = true;
                return Task.FromResult("");
            };

            var result = await _service.GenerateThumbnailBase64FromVideoAsync(new MemoryStream(), 0);

            Assert.IsTrue(gotCalled);
        }
    }
}
