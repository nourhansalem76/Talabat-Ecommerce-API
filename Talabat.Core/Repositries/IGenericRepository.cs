using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositries
{
    public interface IGenericRepository<T> where T: BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecification<T> spec);
        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);
        Task<T> GetEntityAsyncSpec(ISpecification<T> spec);

        Task AddAsync (T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
