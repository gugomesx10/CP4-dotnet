using CP4.Application.Common;
using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;

namespace CP4.Application.Interfaces.Services;

public interface ITimeService
{
    Task<PagedResult<TimeResumoDto>> GetAllAsync(
        int pageNumber,
        int pageSize);

    Task<TimeResumoDto?> GetByIdAsync(int id);
    Task<PagedResult<TimeResumoDto>> GetByJogoAsync(
        string jogo,
        int pageNumber,
        int pageSize);
    Task<TimeResumoDto> CreateAsync(TimeCreateDto dto);
    Task<TimeResumoDto?> UpdateAsync(int id, TimeCreateDto dto);
    Task<bool> DeleteAsync(int id);
}