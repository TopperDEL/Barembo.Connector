using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Helper
{
    public static class StoreKeyHelper
    {
        public static string Convert(StoreKey storeKey)
        {
            switch (storeKey.StoreKeyType)
            {
                case StoreKeyTypes.BookShelf:
                    return "BookShelf.json";
            }
            return "";
        }
    }
}
