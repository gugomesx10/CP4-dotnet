using CP4.Application.Common;
using CP4.Domain.Entities;

namespace CP4.Application.Interfaces.Repositories;

public interface IPerfilCompetitivoRepository
{
    Task<PagedResult<PerfilCompetitivo>> GetAllAsync(
        int pageNumber,
        int pageSize);

    Task<PerfilCompetitivo?> GetByIdAsync(int id);
    Task<PerfilCompetitivo?> GetByIdForUpdateAsync(int id);
    Task<PerfilCompetitivo?> GetByJogadorAsync(int jogadorId);

    Task<bool> ExistsByJogadorAsync(
        int jogadorId,
        int? perfilIdIgnorado = null);

    Task AddAsync(PerfilCompetitivo perfil);
    Task UpdateAsync(PerfilCompetitivo perfil);
    Task DeleteAsync(PerfilCompetitivo perfil);
}