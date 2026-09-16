using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;
using CP4.Application.Results;

namespace CP4.Application.Interfaces.Services;

public interface IPerfilCompetitivoService
{
    Task<IEnumerable<PerfilCompetitivoResponseDto>> GetAllAsync();
    Task<PerfilCompetitivoResponseDto?> GetByIdAsync(int id);
    Task<PerfilCompetitivoResponseDto?> GetByJogadorAsync(int jogadorId);
    Task<PerfilCompetitivoResult> CreateAsync(PerfilCompetitivoCreateDto dto);
    Task<PerfilCompetitivoResult> UpdateAsync(
        int id,
        PerfilCompetitivoCreateDto dto);
    Task<bool> DeleteAsync(int id);
}