using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Barembo.Helper
{
    internal static class StoreKeyHelper
    {
        public const string PROPERTY_BOOK_ID = "BookId";
        public const string PROPERTY_ENTRY_ID = "EntryId";
        public const string PROPERTY_CONTRIBUTOR_ID = "ContributorId";
        public const string PROPERTY_ATTACHMENT_ID = "AttachmentId";
        public const string PROPERTY_BOOK_SHARE_ID = "BookShareId";
        public static string Convert(StoreKey storeKey)
        {
            switch (storeKey.StoreKeyType)
            {
                case StoreKeyTypes.BookShelf:
                    return "BookShelf.json";
                case StoreKeyTypes.Book:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Book.json";
                case StoreKeyTypes.Entry:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Entries/" + storeKey.Properties[PROPERTY_CONTRIBUTOR_ID] + "/" + storeKey.Properties[PROPERTY_ENTRY_ID] + ".json";
                case StoreKeyTypes.Entries:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Entries/";
                case StoreKeyTypes.Attachment:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/" + storeKey.Properties[PROPERTY_ENTRY_ID] + "/" + storeKey.Properties[PROPERTY_ATTACHMENT_ID];
                case StoreKeyTypes.Contributor:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Contributors/" + storeKey.Properties[PROPERTY_CONTRIBUTOR_ID] + ".json";
                case StoreKeyTypes.Contributors:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Contributors/";
                case StoreKeyTypes.BookShare:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Shares/" + storeKey.Properties[PROPERTY_BOOK_SHARE_ID] + ".json";
                case StoreKeyTypes.BookShares:
                    return storeKey.Properties[PROPERTY_BOOK_ID] + "/Shares/";
                default:
                    throw new ArgumentException("Unknonwn StoreKeyType:"+ storeKey.StoreKeyType.ToString());
            }
        }

        public static string GetStoreObjectId(StoreKey storeKey)
        {
            var key = storeKey.ToString();

            return GetStoreObjectId(key);
        }

        public static string GetStoreObjectId(string key)
        {
            var localKey = key;
            if(key.Contains("Book.json"))
            {
                localKey = localKey.Split('/').First();
            }
            else
            {
                localKey = localKey.Replace(".json", "");
                localKey = localKey.Split('/').Last();
            }

            return localKey;
        }
    }
}
