using Elfo.Wardein.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Abstractions
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
