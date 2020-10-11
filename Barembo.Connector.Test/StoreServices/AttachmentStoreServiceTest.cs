using Barembo.Interfaces;
using Barembo.Models;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class AttachmentStoreServiceTest
    {
        AttachmentStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new AttachmentStoreService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task LoadAsStream_Returns_StreamIfObjectExists()
        {
            Book book = new Book();
            Entry entry = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream returnStream = new MemoryStream();

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Attachment)))
                             .Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = true })).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectAsStreamAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Attachment)))
                             .Returns(Task.FromResult(returnStream as Stream)).Verifiable();

            var result = await _service.LoadAsStreamAsync(reference, attachment);

            Assert.AreEqual(returnStream, result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveAsStream_Saves_Stream()
        {
            Book book = new Book();
            Entry entry = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream saveStream = new MemoryStream();

            _storeServiceMock.Setup(s => s.PutObjectFromStreamAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant),
                                                                    Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Attachment),
                                                                    saveStream))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveFromStreamAsync(reference, attachment, saveStream);

            Assert.IsTrue(result);
            _storeServiceMock.Verify();
        }
    }
}
