using Barembo.Helper;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Services
{
    public class MagicLinkService : IMagicLinkGeneratorService, IMagicLinkResolverService
    {
        public BookShareReference GetBookShareReferenceFrom(string magicLink)
        {
            return JSONHelper.DeserializeFromJSON<BookShareReference>(Convert.FromBase64String(magicLink.Substring(14)));
        }

        public string GetMagicLinkFor(BookShareReference bookShareReference)
        {
            var base64 = Convert.ToBase64String(JSONHelper.SerializeToJSON(bookShareReference));
            return "barembo://BSR/" + base64;
        }
    }
}
