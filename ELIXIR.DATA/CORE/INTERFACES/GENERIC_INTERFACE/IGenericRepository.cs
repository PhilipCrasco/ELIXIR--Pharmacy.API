using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES
{
      public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T> GetById(int id);
       // Task<bool> Add(T entity);
       // Task<bool> Upsert(T entity);
        //Task<T> GetEntityWithSpec(ISpecification<T> spec);
        //Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    }
}
