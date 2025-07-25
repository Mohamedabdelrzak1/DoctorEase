using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IUnitOfWork
    {
        //public IGenericRepository<Product ,int> ProductRepository { get;  }
        IGenericRepository<TEntity, TKey>  GetRepository<TEntity, TKey>() where TEntity :BaseEntity<TKey>; //DI
        Task<int> SaveChangesAsync();
    }
}
