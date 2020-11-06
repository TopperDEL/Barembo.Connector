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
    public class QueuedPriorityLoaderServiceTest
    {
        QueuedPriorityLoaderService<Entry> _service;
        Moq.Mock<IStoreService> _storeServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new QueuedPriorityLoaderService<Entry>(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task Load_Loads_OneElement()
        {
            StoreAccess access1 = new StoreAccess("access1");
            StoreKey key1 = StoreKey.Entry("book1", "entry1", "contributor1");
            Entry entry1 = new Entry();

            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(access1, key1)).Returns(Task.FromResult(entry1)).Verifiable();
            bool elementLoaded = false;

            _service.LoadWithHighPriority(access1, key1,
                (entry) => { elementLoaded = true; },
                () => { Assert.IsTrue(false, "One Element could not be loaded"); });

            while (_service.HasEntriesToLoad)
                await Task.Delay(10);

            Assert.IsTrue(elementLoaded);
        }

        [TestMethod]
        public async Task Load_Loads_TwoElements()
        {
            StoreAccess access1 = new StoreAccess("access1");
            StoreKey key1 = StoreKey.Entry("book1", "entry1", "contributor1");
            Entry entry1 = new Entry();
            StoreAccess access2 = new StoreAccess("access2");
            StoreKey key2 = StoreKey.Entry("book2", "entry2", "contributor2");
            Entry entry2 = new Entry();

            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(access1, key1)).Returns(Task.FromResult(entry1)).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(access2, key2)).Returns(Task.FromResult(entry2)).Verifiable();
           
            bool firstElementLoaded = false;
            bool secondElementLoaded = false;

            _service.LoadWithHighPriority(access2, key2,
                (entry) => { firstElementLoaded = true; },
                () => { Assert.IsTrue(false, "First Element could not be loaded"); });
            _service.LoadWithHighPriority(access2, key2,
                (entry) => { secondElementLoaded = true; },
                () => { Assert.IsTrue(false, "Second Element could not be loaded"); });


            while (_service.HasEntriesToLoad)
                await Task.Delay(10);

            Assert.IsTrue(firstElementLoaded);
            Assert.IsTrue(secondElementLoaded);
        }

        [TestMethod]
        public async Task Load_Loads_TwoElementsWhereFirstElementFails()
        {
            StoreAccess access1 = new StoreAccess("access1");
            StoreKey key1 = StoreKey.Entry("book1", "entry1", "contributor1");
            Entry entry1 = new Entry();
            StoreAccess access2 = new StoreAccess("access2");
            StoreKey key2 = StoreKey.Entry("book2", "entry2", "contributor2");
            Entry entry2 = new Entry();

            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(access1, key1)).Throws(new Exception()).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(access2, key2)).Returns(Task.FromResult(entry2)).Verifiable();

            bool firstElementNotLoaded = false;
            bool secondElementLoaded = false;

            _service.LoadWithHighPriority(access2, key2,
                (entry) => { Assert.IsTrue(false, "First element should fail"); },
                () => { firstElementNotLoaded = true; });
            _service.LoadWithHighPriority(access2, key2,
                (entry) => { secondElementLoaded = true; },
                () => { Assert.IsTrue(false, "Second Element could not be loaded"); });


            while (_service.HasEntriesToLoad)
                await Task.Delay(10);

            Assert.IsTrue(firstElementNotLoaded);
            Assert.IsTrue(secondElementLoaded);
        }
    }
}
