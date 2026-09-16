using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;
using CP4.Application.Interfaces.Repositories;
using CP4.Application.Interfaces.Services;
using CP4.Application.Mappings;

namespace CP4.Application.Services;

public class TimeService : ITimeService
{
    private readonly ITimeRepository _timeRepository;

    public TimeService(ITimeRepository timeRepository)
    {
        _timeRepository = timeRepository;
    }

    public async Task<IEnumerable<TimeResumoDto>> GetAllAsync()
    {
        var times = await _timeRepository.GetAllAsync();

        return times.Select(TimeMapper.ToResumoDto);
    }

    public async Task<TimeResumoDto?> GetByIdAsync(int id)
    {
        var time = await _timeRepository.GetByIdAsync(id);

        return time == null
            ? null
            : TimeMapper.ToResumoDto(time);
    }

    public async Task<IEnumerable<TimeResumoDto>> GetByJogoAsync(string jogo)
    {
        var times = await _timeRepository.GetByJogoAsync(jogo);

        return times.Select(TimeMapper.ToResumoDto);
    }

    public async Task<TimeResumoDto> CreateAsync(TimeCreateDto dto)
    {
        var time = TimeMapper.ToEntity(dto);

        await _timeRepository.AddAsync(time);

        return TimeMapper.ToResumoDto(time);
    }

    public async Task<TimeResumoDto?> UpdateAsync(int id, TimeCreateDto dto)
    {
        var time = await _timeRepository.GetByIdAsync(id);

        if (time == null)
            return null;

        TimeMapper.UpdateEntity(time, dto);

        await _timeRepository.UpdateAsync(time);

        return TimeMapper.ToResumoDto(time);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var time = await _timeRepository.GetByIdAsync(id);

        if (time == null)
            return false;

        await _timeRepository.DeleteAsync(time);

        return true;
    }
}