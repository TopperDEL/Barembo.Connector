using Barembo.Interfaces;
using Barembo.Models;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.StoreServices
{
    [TestClass]
    public class AttachmentPreviewStoreServiceTest
    {
        IAttachmentPreviewStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new AttachmentPreviewStoreService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task Load_ReturnsAttachmentPreview()
        {
            Book book = new Book();
            Entry entry = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            AttachmentPreview preview = new AttachmentPreview();

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.AttachmentPreview)))
                             .Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = true })).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<AttachmentPreview>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.AttachmentPreview)))
                             .Returns(Task.FromResult(preview)).Verifiable();

            var result = await _service.LoadAsync(reference, attachment);

            Assert.AreEqual(preview, result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task Save_SavesAttachmentPreview()
        {
            Book book = new Book();
            Entry entry = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            AttachmentPreview preview = new AttachmentPreview();

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant),
                                                                    Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.AttachmentPreview),
                                                                    preview))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveAsync(reference, attachment, preview);

            Assert.IsTrue(result);
            _storeServiceMock.Verify();
        }
    }
}
