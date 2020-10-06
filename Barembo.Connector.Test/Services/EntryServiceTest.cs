using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class EntryServiceTest
    {
        EntryService _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new EntryService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task SaveEntry_Saves_Entry()
        {
            Book book = new Book();
            Entry entryToSave = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entryToSave.Id;

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Entry>(Moq.It.Is<StoreAccess>(s=>s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s=>s.StoreKeyType == StoreKeyTypes.Entry), entryToSave))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveAsync(reference, entryToSave);

            Assert.IsTrue(result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveEntry_ReturnsFalse_IfEntryCouldNotBeSaved()
        {
            Book book = new Book();
            Entry entryToSave = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entryToSave.Id;

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Entry>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Entry), entryToSave))
                             .Returns(Task.FromResult(false)).Verifiable();

            var result = await _service.SaveAsync(reference, entryToSave);

            Assert.IsFalse(result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntry_Loads_ExistingEntry()
        {
            Book book = new Book();
            Entry entryToLoad = new Entry();
            EntryReference reference = new EntryReference();
            reference.BookReference = new BookReference();
            reference.BookReference.BookId = book.Id;
            reference.BookReference.AccessGrant = "use this access";
            reference.EntryId = entryToLoad.Id;

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Entry)))
                             .Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = true })).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.BookReference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Entry)))
                             .Returns(Task.FromResult(entryToLoad)).Verifiable();

            var result = await _service.LoadAsync(reference);

            Assert.AreEqual(entryToLoad, result);
            _storeServiceMock.Verify();
        }
    }
}
