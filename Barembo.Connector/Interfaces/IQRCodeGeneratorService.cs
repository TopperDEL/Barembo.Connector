using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Interfaces
{
    public interface IQRCodeGeneratorService
    {
        byte[] GetQRCodePNGFor(BookShareReference bookShareReference);
    }
}
