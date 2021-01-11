using Barembo.Interfaces;
using Barembo.Models;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Barembo.Helper;

namespace Barembo.Connector.Test.StoreServices
{
    [TestClass]
    public class ContributorServiceTest
    {
        ContributorStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new ContributorStoreService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task SaveContributor_Saves_Contributor()
        {
            Book book = new Book();
            Contributor contributorToSave = new Contributor();
            BookReference reference = new BookReference();
            reference.BookId = book.Id;
            reference.AccessGrant = "use this access";

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Contributor>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Contributor), contributorToSave))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveAsync(reference, contributorToSave);

            Assert.IsTrue(result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveContributor_ReturnsFalse_IfContributorCouldNotBeSaved()
        {
            Book book = new Book();
            Contributor contributorToSave = new Contributor();
            BookReference reference = new BookReference();
            reference.BookId = book.Id;
            reference.AccessGrant = "use this access";

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Contributor>(Moq.It.Is<StoreAccess>(s => s.AccessGrant == reference.AccessGrant), Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.Contributor), contributorToSave))
                             .Returns(Task.FromResult(false)).Verifiable();

            var result = await _service.SaveAsync(reference, contributorToSave);

            Assert.IsFalse(result);
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task List_Lists_AllContributors()
        {
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.AccessGrant = "use this access";
            var contrib1 = new Contributor() { Id = "contributor1" };
            var contrib2 = new Contributor() { Id = "contributor2" };
            var contributorsToLoad = new List<StoreObject>();
            var contributor1 = new StoreObject(book.Id + "/Contributors/contributor1.json", "contributor1", new StoreMetaData());
            contributorsToLoad.Add(contributor1);
            var contributor2 = new StoreObject(book.Id + "/Contributors/contributor2.json", "contributor2", new StoreMetaData());
            contributorsToLoad.Add(contributor2);

            _storeServiceMock.Setup(s => s.ListObjectsAsync(Moq.It.Is<StoreAccess>(a => a.AccessGrant == bookReference.AccessGrant), Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.Contributors)))
                             .Returns(Task.FromResult(contributorsToLoad as IEnumerable<StoreObject>));
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Contributor>(Moq.It.Is<StoreAccess>(a => a.AccessGrant == bookReference.AccessGrant), Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.Contributor && k.Properties[StoreKeyHelper.PROPERTY_CONTRIBUTOR_ID] == contributor1.Id)))
                             .Returns(Task.FromResult(contrib1));
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Contributor>(Moq.It.Is<StoreAccess>(a => a.AccessGrant == bookReference.AccessGrant), Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.Contributor && k.Properties[StoreKeyHelper.PROPERTY_CONTRIBUTOR_ID] == contributor2.Id)))
                             .Returns(Task.FromResult(contrib2));

            var entries = (await _service.ListAsync(bookReference)).ToList();

            Assert.AreEqual(2, entries.Count());
            Assert.AreEqual(contributor1.Id, entries[0].Id);
            Assert.AreEqual(contributor2.Id, entries[1].Id);
        }
    }
}
