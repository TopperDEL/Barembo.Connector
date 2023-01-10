using Barembo.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class FileAccessHelper : IFileAccessHelper
    {
        public async Task<Stream> OpenFileAsync(string filePath)
        {
            return File.OpenRead(filePath);
        }
    }
}
