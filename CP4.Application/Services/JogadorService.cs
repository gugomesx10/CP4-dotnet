using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;
using CP4.Application.Interfaces.Repositories;
using CP4.Application.Interfaces.Services;
using CP4.Application.Mappings;

namespace CP4.Application.Services;

public class JogadorService : IJogadorService
{
    private readonly IJogadorRepository _jogadorRepository;
    private readonly ITimeRepository _timeRepository;

    public JogadorService(
        IJogadorRepository jogadorRepository,
        ITimeRepository timeRepository)
    {
        _jogadorRepository = jogadorRepository;
        _timeRepository = timeRepository;
    }

    public async Task<IEnumerable<JogadorResumoDto>> GetAllAsync()
    {
        var jogadores = await _jogadorRepository.GetAllAsync();

        return jogadores.Select(JogadorMapper.ToResumoDto);
    }

    public async Task<JogadorResumoDto?> GetByIdAsync(int id)
    {
        var jogador = await _jogadorRepository.GetByIdAsync(id);

        return jogador == null
            ? null
            : JogadorMapper.ToResumoDto(jogador);
    }

    public async Task<IEnumerable<JogadorResumoDto>> GetByTimeAsync(int timeId)
    {
        var jogadores = await _jogadorRepository.GetByTimeAsync(timeId);

        return jogadores.Select(JogadorMapper.ToResumoDto);
    }

    public async Task<IEnumerable<JogadorResumoDto>> GetByFuncaoAsync(string funcao)
    {
        var jogadores = await _jogadorRepository.GetByFuncaoAsync(funcao);

        return jogadores.Select(JogadorMapper.ToResumoDto);
    }

    public async Task<JogadorResumoDto?> CreateAsync(JogadorCreateDto dto)
    {
        var time = await _timeRepository.GetByIdAsync(dto.TimeId);

        if (time == null)
            return null;

        var jogador = JogadorMapper.ToEntity(dto);

        await _jogadorRepository.AddAsync(jogador);

        return JogadorMapper.ToResumoDto(jogador, time);
    }

    public async Task<JogadorResumoDto?> UpdateAsync(
        int id,
        JogadorCreateDto dto)
    {
        var jogador = await _jogadorRepository.GetByIdForUpdateAsync(id);

        if (jogador == null)
            return null;

        var time = await _timeRepository.GetByIdAsync(dto.TimeId);

        if (time == null)
            return null;

        JogadorMapper.UpdateEntity(jogador, dto);

        await _jogadorRepository.UpdateAsync(jogador);

        return JogadorMapper.ToResumoDto(jogador, time);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var jogador = await _jogadorRepository.GetByIdForUpdateAsync(id);

        if (jogador == null)
            return false;

        await _jogadorRepository.DeleteAsync(jogador);

        return true;
    }
}