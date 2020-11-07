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

        [TestInitialize]
        public void Init()
        {
            _service = new QueuedPriorityLoaderService<Entry>();
        }

        [TestMethod]
        public async Task Load_Loads_OneElement()
        {
            StoreAccess access1 = new StoreAccess("access1");
            StoreKey key1 = StoreKey.Entry("book1", "entry1", "contributor1");
            Entry entry1 = new Entry();

            bool elementLoaded = false;

            _service.LoadWithHighPriority(async () => await Task.FromResult(entry1),
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

            bool firstElementLoaded = false;
            bool secondElementLoaded = false;

            _service.LoadWithHighPriority(async () => await Task.FromResult(entry1),
                (entry) => { firstElementLoaded = true; },
                () => { Assert.IsTrue(false, "First Element could not be loaded"); });
            _service.LoadWithHighPriority(async () => await Task.FromResult(entry2),
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

            bool firstElementNotLoaded = false;
            bool secondElementLoaded = false;

            _service.LoadWithHighPriority(() => throw new Exception(),
                (entry) => { Assert.IsTrue(false, "First element should fail"); },
                () => { firstElementNotLoaded = true; });
            _service.LoadWithHighPriority(async () => await Task.FromResult(entry2),
                (entry) => { secondElementLoaded = true; },
                () => { Assert.IsTrue(false, "Second Element could not be loaded"); });


            while (_service.HasEntriesToLoad)
                await Task.Delay(10);

            Assert.IsTrue(firstElementNotLoaded);
            Assert.IsTrue(secondElementLoaded);
        }
    }
}
