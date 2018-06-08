using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.Persistence
{
    public class JSONPersistence : IAmPersistenceService
    {
        private readonly string filePath;
        private IList<DBItem> cachedItems;
        private IOHelper ioHelper;

        public JSONPersistence(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException("File path is empty");

            this.filePath = filePath;
            this.ioHelper = new IOHelper(this.filePath);
            this.cachedItems = GetAll();
        }

        public IList<DBItem> GetAll()
        {
            if (!this.ioHelper.CheckIfFileExist())
                cachedItems = new List<DBItem>();
            else
                cachedItems = JsonConvert.DeserializeObject<IList<DBItem>>(this.ioHelper.GetFileContentFromPath());
            return cachedItems;
        }

        public DBItem GetItemById(string id)
        {
            if (cachedItems == null)
                cachedItems = GetAll();

            return cachedItems.FirstOrDefault(x => x.Id == id);
        }

        public void InvalidateCache()
        {
            this.cachedItems = null;
        }

        public void PersistOnDisk()
        {
            if (cachedItems == null)
                cachedItems = GetAll();

            this.ioHelper.PersistFileOnDisk(JsonConvert.SerializeObject(cachedItems));
        }

        public void UpdateCachedItem(DBItem item)
        {
            if (cachedItems == null)
                cachedItems = GetAll();

            var itemToUpdate = cachedItems.FirstOrDefault(x => x.Id == item.Id);
            if (itemToUpdate == null)
                cachedItems.Add(item);
            else
                itemToUpdate.RetryCount = item.RetryCount;
        }

        public void UpdateCachedItems(IList<DBItem> itemList)
        {
            this.cachedItems = itemList;
        }
    }
}
