using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Abstractions.Services.Models;
using Elfo.Wardein.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.Persistence
{
    [Obsolete]
    public class WindowsServiceStatsPersistenceInJSON : IAmPersistenceService<WindowsServiceStats>
    {
        private readonly string filePath;
        private IList<WindowsServiceStats> cachedEntities;
        private IOHelper ioHelper;

        public WindowsServiceStatsPersistenceInJSON(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException("File path is empty");

            this.filePath = filePath;
            this.ioHelper = new IOHelper(this.filePath);
            this.cachedEntities = GetAll();
        }

        public IList<WindowsServiceStats> GetAll()
        {
            if (!this.ioHelper.CheckIfFileExist())
                cachedEntities = new List<WindowsServiceStats>();
            else
                cachedEntities = JsonConvert.DeserializeObject<IList<WindowsServiceStats>>(this.ioHelper.GetFileContent());
            return cachedEntities;
        }

        public WindowsServiceStats GetEntityById(string id, bool createEntityIfNotExist = true)
        {
            if (cachedEntities == null)
                cachedEntities = GetAll();

            var entity = cachedEntities.FirstOrDefault(x => x.Id == id);
            if(createEntityIfNotExist && entity == null)
                entity = new WindowsServiceStats { Id = id, RetryCount = 0 };

            return entity;
        }

        public void InvalidateCache()
        {
            this.cachedEntities = null;
        }

        public void PersistOnDisk()
        {
            if (cachedEntities == null)
                cachedEntities = GetAll();

            this.ioHelper.PersistFileOnDisk(JsonConvert.SerializeObject(cachedEntities));
        }

        public void CreateOrUpdateCachedEntity(IList<WindowsServiceStats> entityList)
        {
            this.cachedEntities = entityList;
        }

        public void CreateOrUpdateCachedEntity(WindowsServiceStats entity)
        {
            if (cachedEntities == null)
                cachedEntities = GetAll();

            var itemToUpdate = cachedEntities.FirstOrDefault(x => x.Id == entity.Id);
            if (itemToUpdate == null)
                cachedEntities.Add(entity);
            else
                itemToUpdate.RetryCount = entity.RetryCount;
        }

        public void Dispose()
        {
            PersistOnDisk();
        }
    }
}
