using Auth10Api.Domain.Entities;
using MongoDB.Driver;

namespace Auth10Api.Domain.Interfaces;

public interface IRepository<T>
{
    Task<User> PostAsync(T obj, IClientSessionHandle session);
    Task<ICollection<T>> GetAsync();
    Task<T> GetByIdAsync(string id);
    Task<User> PutAsync(T obj);
    Task<bool> DeleteAsync(string id);   
}
