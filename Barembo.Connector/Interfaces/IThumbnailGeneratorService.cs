using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IThumbnailGeneratorService
    {
        Task<string> GenerateThumbnailBase64FromImageAsync(Stream imageStream);
        Task<string> GenerateThumbnailBase64FromVideoAsync(Stream videoStream, float positionPercent);
    }
}
