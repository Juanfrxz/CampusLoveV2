using System;
using CampusLove.Domain.Entities;

namespace CampusLove.Domain.Ports
{
    public interface IGenericRepository<T> where T : class
    {
        // Operaciones básicas CRUD
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<bool> InsertAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
    }
}