using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;

namespace CP4.Application.Interfaces.Services;

public interface ITimeService
{
    Task<IEnumerable<TimeResumoDto>> GetAllAsync();
    Task<TimeResumoDto?> GetByIdAsync(int id);
    Task<IEnumerable<TimeResumoDto>> GetByJogoAsync(string jogo);
    Task<TimeResumoDto> CreateAsync(TimeCreateDto dto);
    Task<TimeResumoDto?> UpdateAsync(int id, TimeCreateDto dto);
    Task<bool> DeleteAsync(int id);
}