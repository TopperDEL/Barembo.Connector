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
        public async Task Thumbnail_Gets_Generated()
        {
            try
            {
                File.Delete("TestImageThumbnail.jpg");
            }
            catch { }

            FileStream image = new FileStream("TestImage.jpg", FileMode.Open);
            var result = await _service.GenerateThumbnailBase64FromImageAsync(image);
            var resultBytes = Convert.FromBase64String(result);
            await File.WriteAllBytesAsync("TestImageThumbnail.jpg", resultBytes);

            Assert.IsTrue(File.Exists("TestImageThumbnail.jpg"));
        }
    }
}
