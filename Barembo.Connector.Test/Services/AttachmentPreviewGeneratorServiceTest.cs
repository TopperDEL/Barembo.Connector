using Barembo.Interfaces;
using Barembo.Models;
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
    public class AttachmentPreviewGeneratorServiceTest
    {
        IAttachmentPreviewGeneratorService _service;
        Moq.Mock<IThumbnailGeneratorService> _thumbnailGeneratorServiceMock;

        [TestInitialize]
        public void Init()
        {
            _thumbnailGeneratorServiceMock = new Moq.Mock<IThumbnailGeneratorService>();
            _service = new AttachmentPreviewGeneratorService(_thumbnailGeneratorServiceMock.Object);
        }

        [TestMethod]
        public async Task Generate_GeneratesAttachmentPreview_ForImage()
        {
            Attachment attachment = new Attachment();
            attachment.Type = AttachmentType.Image;

            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromImageAsync(Moq.It.IsAny<Stream>())).Returns(Task.FromResult("preview")).Verifiable();

            using (FileStream image = new FileStream("TestImage.jpg", FileMode.Open))
            {
                var result = await _service.GeneratePreviewAsync(attachment, image, "TestImage.jpg");

                Assert.AreEqual(AttachmentType.Image, result.Type);
                Assert.IsNotNull(result.PreviewPartsBase64);
                Assert.AreEqual(1, result.PreviewPartsBase64.Count);
                Assert.AreEqual("preview", result.PreviewPartsBase64[0]);
            }

            _thumbnailGeneratorServiceMock.Verify();
        }

        [TestMethod]
        public async Task Generate_GeneratesAttachmentPreviewWith6Parts_ForVideo()
        {
            Attachment attachment = new Attachment();
            attachment.Type = AttachmentType.Video;

            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(Moq.It.IsAny<Stream>(),0f,"TestVideo.mp4")).Returns(Task.FromResult("part1")).Verifiable();
            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(Moq.It.IsAny<Stream>(),0.2f,"TestVideo.mp4")).Returns(Task.FromResult("part2")).Verifiable();
            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(Moq.It.IsAny<Stream>(),0.4f,"TestVideo.mp4")).Returns(Task.FromResult("part3")).Verifiable();
            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(Moq.It.IsAny<Stream>(),0.6f,"TestVideo.mp4")).Returns(Task.FromResult("part4")).Verifiable();
            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(Moq.It.IsAny<Stream>(),0.8f,"TestVideo.mp4")).Returns(Task.FromResult("part5")).Verifiable();
            _thumbnailGeneratorServiceMock.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(Moq.It.IsAny<Stream>(),0.9f,"TestVideo.mp4")).Returns(Task.FromResult("part6")).Verifiable();


            using (FileStream image = new FileStream("TestVideo.mp4", FileMode.Open))
            {
                var result = await _service.GeneratePreviewAsync(attachment, image, "TestVideo.mp4");

                Assert.AreEqual(AttachmentType.Video, result.Type);
                Assert.IsNotNull(result.PreviewPartsBase64);
                Assert.AreEqual(6, result.PreviewPartsBase64.Count);
                Assert.AreEqual("part1", result.PreviewPartsBase64[0]);
                Assert.AreEqual("part2", result.PreviewPartsBase64[1]);
                Assert.AreEqual("part3", result.PreviewPartsBase64[2]);
                Assert.AreEqual("part4", result.PreviewPartsBase64[3]);
                Assert.AreEqual("part5", result.PreviewPartsBase64[4]);
                Assert.AreEqual("part6", result.PreviewPartsBase64[5]);
            }

            _thumbnailGeneratorServiceMock.Verify();
        }
    }
}
