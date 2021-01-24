using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class StoreAccessServiceTest
    {
        StoreAccessService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new StoreAccessService();
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public void ShareBookAccess_Creates_UsableAccess()
        {
            //ToDo
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public void ShareBookShareAccess_Creates_UsableAccess()
        {
            //ToDo
        }
    }
}
