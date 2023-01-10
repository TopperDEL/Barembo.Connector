using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class BackgroundEntryServiceTest
    {
        BackgroundEntryService _backgroundEntryService;
        Moq.Mock<IEntryStoreService> _entryStoreServiceMock;
        Moq.Mock<IThumbnailGeneratorService> _thumbnailGeneratorService;
        Moq.Mock<IAttachmentStoreService> _attachmentStoreServiceMock;
        Moq.Mock<IAttachmentPreviewStoreService> _attachmentPreviewStoreServiceMock;
        Moq.Mock<IAttachmentPreviewGeneratorService> _attachmentPreviewGeneratorServiceMock;
        Moq.Mock<IFileAccessHelper> _fileAccessHelperMock;
        StoreBuffer _storeBuffer;

        [TestInitialize]
        public void Init()
        {
            _entryStoreServiceMock = new Moq.Mock<IEntryStoreService>();
            _attachmentStoreServiceMock = new Moq.Mock<IAttachmentStoreService>();
            _attachmentPreviewStoreServiceMock = new Moq.Mock<IAttachmentPreviewStoreService>();
            _attachmentPreviewGeneratorServiceMock = new Moq.Mock<IAttachmentPreviewGeneratorService>();
            _thumbnailGeneratorService = new Moq.Mock<IThumbnailGeneratorService>();
            _fileAccessHelperMock = new Moq.Mock<IFileAccessHelper>();
            _storeBuffer = new StoreBuffer();

            _backgroundEntryService = new BackgroundEntryService(_entryStoreServiceMock.Object, _attachmentStoreServiceMock.Object, _attachmentPreviewStoreServiceMock.Object, _attachmentPreviewGeneratorServiceMock.Object, _thumbnailGeneratorService.Object, _fileAccessHelperMock.Object, _storeBuffer);
        }

        [TestMethod]
        public async Task AddAttachment_Creates_BackgroundAction()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            Entry entry = _backgroundEntryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = null; //Should not get touched

            var result = await _backgroundEntryService.AddAttachmentAsync(entryReference, entry, attachment, stream, "filepath");

            Assert.IsTrue(result);
            var backgroundAction = await _storeBuffer.GetNextBackgroundAction();
            Assert.IsNotNull(backgroundAction);
            Assert.AreEqual(BackgroundActionTypes.AddAttachment, backgroundAction.ActionType);
            Assert.AreEqual(entryReference.EntryKey, ((EntryReference)backgroundAction.GetParameters()["EntryReference"]).EntryKey);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SetThumbnail_SetsThumbnailForImage_AndSavesEntry()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            Entry entry = _backgroundEntryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            attachment.Type = AttachmentType.Image;
            MemoryStream stream = null; //Should not get touched

            var result = await _backgroundEntryService.SetThumbnailAsync(entryReference, entry, attachment, stream, "folder/file.mp4");

            Assert.IsTrue(result);
            var backgroundAction = await _storeBuffer.GetNextBackgroundAction();
            Assert.IsNotNull(backgroundAction);
            Assert.AreEqual(BackgroundActionTypes.SetThumbnail, backgroundAction.ActionType);
            Assert.AreEqual(entryReference.EntryKey, ((EntryReference)backgroundAction.GetParameters()["EntryReference"]).EntryKey);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }
    }
}
