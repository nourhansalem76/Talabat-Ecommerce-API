using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositries;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbcontext;

        private Hashtable _repository;

        public UnitOfWork(StoreContext dbcontext) 
        {
            _dbcontext = dbcontext;
        }
        public async Task<int> Complete()
        {
           return await _dbcontext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbcontext.DisposeAsync();
        }

        public IGenericRepository<Tentity>? Repository<Tentity>() where Tentity : BaseEntity
        {
            if(_repository is null)
                _repository = new Hashtable();
            var type = typeof(Tentity).Name;
            if(!_repository.ContainsKey(type))
            {
                var repository = new GenericReposirtory<Tentity>(_dbcontext);
                _repository.Add(type,repository);

            }
            return _repository[type] as IGenericRepository<Tentity>;


        }
    }
}
