using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultipleUploadDb.Repositories
{
    public interface IGenericRepo<T>
    {
        Task<T> Add(T entity);
    }
}
