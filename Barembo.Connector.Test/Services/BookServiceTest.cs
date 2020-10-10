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
    public class BookServiceTest
    {
        BookStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new BookStoreService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task SaveBook_Saves_Book()
        {
            AccessRights accessRights = new AccessRights();
            Book bookToSave = new Book();
            BookReference reference = new BookReference();
            reference.BookId = bookToSave.Id;
            reference.AccessRights = accessRights;
            reference.AccessGrant = "use this access";
            reference.OwnerName = "Schiller";

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Book>(Moq.It.Is<StoreAccess>(s=>s.AccessGrant == reference.AccessGrant), Moq.It.Is<StoreKey>(s=>s.StoreKeyType == StoreKeyTypes.Book), bookToSave))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveAsync(reference, bookToSave);

            Assert.IsTrue(result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveBook_ReturnsFalse_IfBookCouldNotBeSaved()
        {
            AccessRights accessRights = new AccessRights();
            Book bookToSave = new Book();
            BookReference reference = new BookReference();
            reference.BookId = bookToSave.Id;
            reference.AccessRights = accessRights;
            reference.AccessGrant = "use this access";
            reference.OwnerName = "Schiller";

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Book>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Book), bookToSave))
                             .Returns(Task.FromResult(false)).Verifiable();

            var result = await _service.SaveAsync(reference, bookToSave);

            Assert.IsFalse(result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadBook_Loads_ExistingBook()
        {
            AccessRights accessRights = new AccessRights();
            Book bookToLoad = new Book();
            BookReference reference = new BookReference();
            reference.BookId = bookToLoad.Id;
            reference.AccessRights = accessRights;
            reference.AccessGrant = "use this access";
            reference.OwnerName = "Schiller";

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Book)))
                             .Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = true })).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Book>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Book)))
                             .Returns(Task.FromResult(bookToLoad)).Verifiable();

            var result = await _service.LoadAsync(reference);

            Assert.AreEqual(bookToLoad, result);
            _storeServiceMock.Verify();
        }
    }
}
