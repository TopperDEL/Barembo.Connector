using Barembo.Exceptions;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Barembo.Connector.Test
{
    [TestClass]
    public class BookShelfServiceTest
    {
        BookShelfService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new BookShelfService();
        }

        [TestMethod]
        public async Task Throws_NoBookShelfExists()
        {
            try
            {
                await _service.LoadAsync();
                Assert.IsTrue(false, "No exception thrown");
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(NoBookShelfExistsException));
            }
        }
    }
}
