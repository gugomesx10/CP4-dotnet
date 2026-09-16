using CP4.Application.DTOs;
using CP4.Application.DTOs.Responses;
using CP4.Application.Interfaces.Repositories;
using CP4.Application.Interfaces.Services;
using CP4.Application.Mappings;
using CP4.Application.Results;

namespace CP4.Application.Services;

public class PerfilCompetitivoService : IPerfilCompetitivoService
{
    private readonly IPerfilCompetitivoRepository _perfilRepository;
    private readonly IJogadorRepository _jogadorRepository;

    public PerfilCompetitivoService(
        IPerfilCompetitivoRepository perfilRepository,
        IJogadorRepository jogadorRepository)
    {
        _perfilRepository = perfilRepository;
        _jogadorRepository = jogadorRepository;
    }

    public async Task<IEnumerable<PerfilCompetitivoResponseDto>> GetAllAsync()
    {
        var perfis = await _perfilRepository.GetAllAsync();

        return perfis.Select(PerfilCompetitivoMapper.ToResponseDto);
    }

    public async Task<PerfilCompetitivoResponseDto?> GetByIdAsync(int id)
    {
        var perfil = await _perfilRepository.GetByIdAsync(id);

        return perfil == null
            ? null
            : PerfilCompetitivoMapper.ToResponseDto(perfil);
    }

    public async Task<PerfilCompetitivoResponseDto?> GetByJogadorAsync(
        int jogadorId)
    {
        var perfil = await _perfilRepository.GetByJogadorAsync(jogadorId);

        return perfil == null
            ? null
            : PerfilCompetitivoMapper.ToResponseDto(perfil);
    }

    public async Task<PerfilCompetitivoResult> CreateAsync(
        PerfilCompetitivoCreateDto dto)
    {
        var jogador = await _jogadorRepository.GetByIdAsync(dto.JogadorId);

        if (jogador == null)
        {
            return new PerfilCompetitivoResult(
                PerfilCompetitivoResultStatus.JogadorNaoEncontrado);
        }

        var perfilJaExiste =
            await _perfilRepository.ExistsByJogadorAsync(dto.JogadorId);

        if (perfilJaExiste)
        {
            return new PerfilCompetitivoResult(
                PerfilCompetitivoResultStatus.PerfilJaExiste);
        }

        var perfil = PerfilCompetitivoMapper.ToEntity(dto);

        await _perfilRepository.AddAsync(perfil);

        return new PerfilCompetitivoResult(
            PerfilCompetitivoResultStatus.Sucesso,
            PerfilCompetitivoMapper.ToResponseDto(perfil, jogador));
    }

    public async Task<PerfilCompetitivoResult> UpdateAsync(
        int id,
        PerfilCompetitivoCreateDto dto)
    {
        var perfil = await _perfilRepository.GetByIdForUpdateAsync(id);

        if (perfil == null)
        {
            return new PerfilCompetitivoResult(
                PerfilCompetitivoResultStatus.PerfilNaoEncontrado);
        }

        var jogador = await _jogadorRepository.GetByIdAsync(dto.JogadorId);

        if (jogador == null)
        {
            return new PerfilCompetitivoResult(
                PerfilCompetitivoResultStatus.JogadorNaoEncontrado);
        }

        var perfilJaExiste =
            await _perfilRepository.ExistsByJogadorAsync(
                dto.JogadorId,
                perfil.Id);

        if (perfilJaExiste)
        {
            return new PerfilCompetitivoResult(
                PerfilCompetitivoResultStatus.PerfilJaExiste);
        }

        PerfilCompetitivoMapper.UpdateEntity(perfil, dto);

        await _perfilRepository.UpdateAsync(perfil);

        return new PerfilCompetitivoResult(
            PerfilCompetitivoResultStatus.Sucesso,
            PerfilCompetitivoMapper.ToResponseDto(perfil, jogador));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var perfil = await _perfilRepository.GetByIdForUpdateAsync(id);

        if (perfil == null)
            return false;

        await _perfilRepository.DeleteAsync(perfil);

        return true;
    }
}