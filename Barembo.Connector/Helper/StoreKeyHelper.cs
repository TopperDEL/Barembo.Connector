using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Helper
{
    public static class StoreKeyHelper
    {
        public const string PROPERTY_BOOK_ID = "BookID";
        public static string Convert(StoreKey storeKey)
        {
            switch (storeKey.StoreKeyType)
            {
                case StoreKeyTypes.BookShelf:
                    return "BookShelf.json";
                case StoreKeyTypes.Book:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Book.json";
            }
            return "";
        }
    }
}
