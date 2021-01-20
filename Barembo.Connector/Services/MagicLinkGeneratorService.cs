using Barembo.Helper;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Services
{
    public class MagicLinkGeneratorService : IMagicLinkGeneratorService
    {
        public string GetMagicLinkFor(BookShareReference bookShareReference)
        {
            var base64 = Convert.ToBase64String(JSONHelper.SerializeToJSON(bookShareReference));
            return "barembo://BSR/" + base64;
        }
    }
}
