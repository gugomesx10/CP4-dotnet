using CP4.Domain.Entities;

namespace CP4.Application.Interfaces.Repositories;

public interface IJogadorRepository
{
    Task<List<Jogador>> GetAllAsync();
    Task<Jogador?> GetByIdAsync(int id);
    Task<Jogador?> GetByIdForUpdateAsync(int id);
    Task<List<Jogador>> GetByTimeAsync(int timeId);
    Task<List<Jogador>> GetByFuncaoAsync(string funcao);
    Task AddAsync(Jogador jogador);
    Task UpdateAsync(Jogador jogador);
    Task DeleteAsync(Jogador jogador);
}