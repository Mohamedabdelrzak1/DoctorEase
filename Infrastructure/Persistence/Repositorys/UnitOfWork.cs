using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositorys
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DoctorEaseDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories = new();

        public UnitOfWork(DoctorEaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            // Get Type Name
            var typeName = typeof(TEntity).Name;

            // Dic<string , Object> ==> string Key [Name Of Type] -- Object Object From Generic Repository
            // if (_repositories.ContainsKey(typeName))
            //     return (IGenericRepository<TEntity, TKey>)_repositories[typeName];

            if (_repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity, TKey>)value;

            else
            {
                // Create Object
                var Repo = new GenericRepository<TEntity, TKey>(_dbContext);

                // Store Object in Dic
                _repositories[typeName] = Repo;

                // Return Object
                return Repo;
            }
        }


        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
      
    }
}
