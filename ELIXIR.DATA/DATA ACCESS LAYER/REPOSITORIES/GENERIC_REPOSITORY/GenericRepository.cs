using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.CORE.INTERFACES;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs;
using ELIXIR.DATA.SERVICES;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected StoreContext _context;
        protected DbSet<T> dbSet;
        //private StoreContext context;
        protected readonly ILogger _logger;

        public GenericRepository(
            StoreContext context,
            ILogger logger
        )

        {
            _context = context;
            _logger = logger;
            this.dbSet = context.Set<T>();
        }

        public GenericRepository(StoreContext context)
        {
            this._context = context;
        }

        public virtual async Task<IReadOnlyList<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        //public virtual async Task<bool> Add(T entity)
        //{
        //    await _context.AddAsync(entity);
        //    return true;
        //}

        public virtual Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        //public virtual Task<bool> Upsert(T entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<T> GetEntityWithSpec(ISpecification<T>spec)
        //{
        //    return await ApplySpecification(spec).FirstOrDefaultAsync();
        //}

        //public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        //{
        //    return await ApplySpecification(spec).ToListAsync();
        //}

        //private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        //{
        //    return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        //}

 
    }
}
