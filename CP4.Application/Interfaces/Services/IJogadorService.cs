using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;
using CP4.Application.Common;

namespace CP4.Application.Interfaces.Services;

public interface IJogadorService
{
    Task<PagedResult<JogadorResumoDto>> GetAllAsync(
        int pageNumber,
        int pageSize);
    Task<JogadorResumoDto?> GetByIdAsync(int id);
    Task<PagedResult<JogadorResumoDto>> GetByTimeAsync(
        int timeId,
        int pageNumber,
        int pageSize);
    Task<PagedResult<JogadorResumoDto>> GetByFuncaoAsync(
        string funcao,
        int pageNumber,
        int pageSize);
    Task<JogadorResumoDto?> CreateAsync(JogadorCreateDto dto);
    Task<JogadorResumoDto?> UpdateAsync(int id, JogadorCreateDto dto);
    Task<bool> DeleteAsync(int id);
}