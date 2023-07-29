using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositries;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericReposirtory<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context;

        public GenericReposirtory(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
                return (IReadOnlyList<T>) await _context.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();
            else
               return await _context.Set<T>().ToListAsync();

        }
        public async Task<T> GetByIdAsync(int id)
          => await _context.Set<T>().FindAsync(id);


        public async Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecification<T> spec)
        {
           return await ApplySpecification(spec).ToListAsync();
            
        }

        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task<T> GetEntityAsyncSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.QueryBuilder(_context.Set<T>(), spec);

        }

        public async Task AddAsync(T entity)
        => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity)
       => _context.Set<T>().Update(entity);

        public void Delete(T entity)
         => _context.Set<T>().Remove(entity);
    }
}
