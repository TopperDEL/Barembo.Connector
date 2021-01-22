using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Interfaces
{
    public interface IMagicLinkResolverService
    {
        BookShareReference GetBookShareReferenceFrom(string magicLink);
    }
}
