using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Domain.Interfaces;

public interface IRepository<T>
{
    Task<User> CreateAsync(T obj, IClientSessionHandle session);
    Task<ICollection<T>> GetAllAsync();
    Task<T> GetByIdAsync(string id);
    Task<User> UpdateAsync(T obj);
    Task<bool> DeleteByIdAsync(string id);   
}
