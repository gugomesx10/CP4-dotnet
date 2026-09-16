using CP4.Application.DTOs.Responses;

namespace CP4.Application.Results;

public enum PerfilCompetitivoResultStatus
{
    Sucesso,
    PerfilNaoEncontrado,
    JogadorNaoEncontrado,
    PerfilJaExiste
}

public record PerfilCompetitivoResult(
    PerfilCompetitivoResultStatus Status,
    PerfilCompetitivoResponseDto? Perfil = null);