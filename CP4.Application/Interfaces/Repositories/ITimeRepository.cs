using CP4.Domain.Entities;

namespace CP4.Application.Interfaces.Repositories;

public interface ITimeRepository
{
    Task<List<Time>> GetAllAsync();
    Task<Time?> GetByIdAsync(int id);
    Task<List<Time>> GetByJogoAsync(string jogo);
    Task AddAsync(Time time);
    Task UpdateAsync(Time time);
    Task DeleteAsync(Time time);
}