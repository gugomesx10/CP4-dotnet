using CP4.Application.DTOs;
using CP4.Application.Interfaces.Services;
using CP4.Application.Results;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CP4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfilCompetitivoController : ControllerBase
{
    private readonly IPerfilCompetitivoService _perfilService;

    public PerfilCompetitivoController(
        IPerfilCompetitivoService perfilService)
    {
        _perfilService = perfilService;
    }

    [HttpGet]
    [SwaggerResponse(200, "Perfis competitivos encontrados com sucesso")]
    [SwaggerResponse(204, "Nenhum perfil competitivo encontrado")]
    [SwaggerResponse(400, "Parâmetros de paginação inválidos")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(
                "PageNumber e PageSize devem ser maiores que zero.");

        pageSize = Math.Min(pageSize, 100);

        var perfis = await _perfilService
            .GetAllAsync(pageNumber, pageSize);

        if (!perfis.Items.Any())
            return NoContent();

        return Ok(perfis);
    }

    /// <summary>
    /// Retorna um perfil competitivo pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Perfil competitivo encontrado com sucesso")]
    [SwaggerResponse(404, "Perfil competitivo não encontrado")]
    public async Task<IActionResult> GetById(int id)
    {
        var perfil = await _perfilService.GetByIdAsync(id);

        if (perfil == null)
            return NotFound();

        return Ok(perfil);
    }

    /// <summary>
    /// Retorna o perfil competitivo de um jogador.
    /// </summary>
    [HttpGet("jogador/{jogadorId}")]
    [SwaggerResponse(200, "Perfil competitivo encontrado com sucesso")]
    [SwaggerResponse(404, "Perfil competitivo não encontrado para este jogador")]
    public async Task<IActionResult> GetByJogador(int jogadorId)
    {
        var perfil = await _perfilService.GetByJogadorAsync(jogadorId);

        if (perfil == null)
            return NotFound();

        return Ok(perfil);
    }

    /// <summary>
    /// Cadastra um perfil competitivo para um jogador.
    /// </summary>
    [HttpPost]
    [SwaggerResponse(201, "Perfil competitivo criado com sucesso")]
    [SwaggerResponse(400, "Jogador já possui perfil competitivo")]
    [SwaggerResponse(404, "Jogador não encontrado")]
    public async Task<IActionResult> Create(
        PerfilCompetitivoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _perfilService.CreateAsync(dto);

        if (result.Status ==
            PerfilCompetitivoResultStatus.JogadorNaoEncontrado)
        {
            return NotFound("Jogador não encontrado.");
        }

        if (result.Status ==
            PerfilCompetitivoResultStatus.PerfilJaExiste)
        {
            return BadRequest(
                "Este jogador já possui perfil competitivo.");
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Perfil!.Id },
            result.Perfil);
    }

    /// <summary>
    /// Atualiza um perfil competitivo existente.
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerResponse(200, "Perfil competitivo atualizado com sucesso")]
    [SwaggerResponse(400, "Jogador já possui outro perfil competitivo")]
    [SwaggerResponse(404, "Perfil competitivo ou jogador não encontrado")]
    public async Task<IActionResult> Update(
        int id,
        PerfilCompetitivoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _perfilService.UpdateAsync(id, dto);

        if (result.Status ==
            PerfilCompetitivoResultStatus.PerfilNaoEncontrado)
        {
            return NotFound("Perfil competitivo não encontrado.");
        }

        if (result.Status ==
            PerfilCompetitivoResultStatus.JogadorNaoEncontrado)
        {
            return NotFound("Jogador não encontrado.");
        }

        if (result.Status ==
            PerfilCompetitivoResultStatus.PerfilJaExiste)
        {
            return BadRequest(
                "Este jogador já possui outro perfil competitivo.");
        }

        return Ok(result.Perfil);
    }

    /// <summary>
    /// Remove um perfil competitivo pelo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerResponse(204, "Perfil competitivo removido com sucesso")]
    [SwaggerResponse(404, "Perfil competitivo não encontrado")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _perfilService.DeleteAsync(id);

        if (!removido)
            return NotFound();

        return NoContent();
    }
}