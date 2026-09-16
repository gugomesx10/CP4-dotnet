using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;

namespace CP4.Application.Interfaces.Services;

public interface IJogadorService
{
    Task<IEnumerable<JogadorResumoDto>> GetAllAsync();
    Task<JogadorResumoDto?> GetByIdAsync(int id);
    Task<IEnumerable<JogadorResumoDto>> GetByTimeAsync(int timeId);
    Task<IEnumerable<JogadorResumoDto>> GetByFuncaoAsync(string funcao);
    Task<JogadorResumoDto?> CreateAsync(JogadorCreateDto dto);
    Task<JogadorResumoDto?> UpdateAsync(int id, JogadorCreateDto dto);
    Task<bool> DeleteAsync(int id);
}