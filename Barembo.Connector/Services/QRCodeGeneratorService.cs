using Barembo.Helper;
using Barembo.Interfaces;
using Barembo.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Services
{
    public class QRCodeGeneratorService : IQRCodeGeneratorService
    {
        public byte[] GetQRCodePNGFor(BookShareReference bookShareReference)
        {
            var payload = Convert.ToBase64String(JSONHelper.SerializeToJSON(bookShareReference));

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
