using System;
using System.Collections.Generic;

namespace Elfo.Wardein.Abstractions.Services
{
    public interface IAmPersistenceService<T> : IDisposable where T : new() 
    {
        T GetEntityById(string id, bool createEntityIfNotExist = true);

        IList<T> GetAll();

        void CreateOrUpdateCachedEntity(IList<T> entityList);

        void CreateOrUpdateCachedEntity(T entity);

        void PersistOnDisk();

        void InvalidateCache();
    }
}
