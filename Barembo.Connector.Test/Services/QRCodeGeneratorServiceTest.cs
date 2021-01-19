using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class QRCodeGeneratorServiceTest
    {
        QRCodeGeneratorService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new QRCodeGeneratorService();
        }

        [TestMethod]
        public async Task GenerateForBookShare_Generates_QRCode()
        {
            try
            {
                File.Delete("BookShareReferenceQRCode.png");
            }
            catch { }

            BookShareReference bookShareReference = new BookShareReference();
            bookShareReference.StoreAccess = new StoreAccess("veeeeeeeeeeeeeeeeeeeeeryLooooooooooooooooooooooongStoooooooooooooooooooooreAaaaaaaaaaaaaaaaaaaccess");
            bookShareReference.StoreKey = StoreKey.BookShare(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var qrPng = _service.GetQRCodePNGFor(bookShareReference);

            await File.WriteAllBytesAsync("BookShareReferenceQRCode.png", qrPng);

            Assert.IsTrue(File.Exists("BookShareReferenceQRCode.png"));
        }
    }
}
