using Barembo.Helper;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class MagicLinkServiceTest
    {
        IMagicLinkGeneratorService _genService;
        IMagicLinkResolverService _resService;

        [TestInitialize]
        public void Init()
        {
            _genService = new MagicLinkService();
            _resService = new MagicLinkService();
        }

        [TestMethod]
        public void URL_HasScheme_Barembo()
        {
            BookShareReference bookShareReference = new BookShareReference();

            var generated = _genService.GetMagicLinkFor(bookShareReference);

            Assert.IsTrue(generated.StartsWith("barembo://"));
        }

        [TestMethod]
        public void URL_Describes_BookShareReference()
        {
            BookShareReference bookShareReference = new BookShareReference();

            var generated = _genService.GetMagicLinkFor(bookShareReference);

            Assert.IsTrue(generated.StartsWith("barembo://BSR/"));
        }

        [TestMethod]
        public void URL_Contains_Base64ConvertedBookShareReference()
        {
            BookShareReference bookShareReference = new BookShareReference();

            var generated = _genService.GetMagicLinkFor(bookShareReference);

            var base64 = Convert.ToBase64String(JSONHelper.SerializeToJSON(bookShareReference));

            Assert.AreEqual(base64, generated.Replace("barembo://BSR/",""));
        }

        [TestMethod]
        public void ResolveBookShareReference_Resolves_BookShareReference()
        {
            BookShareReference bookShareReference = new BookShareReference();
            bookShareReference.StoreAccess = new StoreAccess("myGrant");

            var generated = _genService.GetMagicLinkFor(bookShareReference);

            var resolved = _resService.GetBookShareReferenceFrom(generated);

            Assert.AreEqual(bookShareReference.StoreAccess.AccessGrant, resolved.StoreAccess.AccessGrant);
        }
    }
}
