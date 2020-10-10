using Barembo.Interfaces;
using Barembo.Models;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class EntryServiceTest
    {
        EntryStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new EntryStoreService(_storeServiceMock.Object);
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

        [TestMethod]
        public async Task List_Lists_AllEntries()
        {
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.AccessGrant = "use this access";
            var entriesToLoad = new List<StoreObject>();
            var entry1 = new StoreObject(book.Id + "/Entries/entry1.json", "entry1");
            entriesToLoad.Add(entry1);
            var entry2 = new StoreObject(book.Id + "/Entries/entry2.json", "entry2");
            entriesToLoad.Add(entry2);
            var entry3 = new StoreObject(book.Id + "/Entries/entry3.json", "entry3");
            entriesToLoad.Add(entry3);

            _storeServiceMock.Setup(s => s.ListObjectsAsync(Moq.It.Is<StoreAccess>(a => a.AccessGrant == bookReference.AccessGrant), Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.Entries)))
            .Returns(Task.FromResult(entriesToLoad as IEnumerable<StoreObject>));

            var entries = (await _service.ListAsync(bookReference)).ToList();

            Assert.AreEqual(3, entries.Count());
            Assert.AreEqual(entry1.Id, entries[0].EntryId);
            Assert.AreEqual(entry1.Key, entries[0].EntryKey);
            Assert.AreEqual(entry2.Id, entries[1].EntryId);
            Assert.AreEqual(entry2.Key, entries[1].EntryKey);
            Assert.AreEqual(entry3.Id, entries[2].EntryId);
            Assert.AreEqual(entry3.Key, entries[2].EntryKey);
        }
    }
}
