using ApiMultipleUploadDb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultipleUploadDb.Repositories
{
    public class GenericRepo<T> : IGenericRepo<T>
    {
        private readonly ApplicationDbContext dbContext;

        public GenericRepo(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<T> Add(T entity)
        {
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
