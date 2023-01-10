using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IFileAccessHelper
    {
        Task<Stream> OpenFileAsync(string filePath);
    }
}
