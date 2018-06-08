using Elfo.Wardein.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Abstractions
{
    public interface IAmPersistenceService
    {
        DBItem GetItemById(string id);

        IList<DBItem> GetAll();

        void UpdateCachedItems(IList<DBItem> itemList);

        void UpdateCachedItem(DBItem item);

        void PersistOnDisk();

        void InvalidateCache();
    }
}
