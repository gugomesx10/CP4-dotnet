using CP4.Application.DTOs;
using CP4.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CP4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogadorController : ControllerBase
{
    private readonly IJogadorService _jogadorService;

    public JogadorController(IJogadorService jogadorService)
    {
        _jogadorService = jogadorService;
    }

    /// <summary>
    /// Retorna todos os jogadores cadastrados com seus times.
    /// </summary>
    [HttpGet]
    [SwaggerResponse(200, "Jogadores encontrados com sucesso")]
    [SwaggerResponse(204, "Nenhum jogador encontrado")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(
                "PageNumber e PageSize devem ser maiores que zero.");

        pageSize = Math.Min(pageSize, 100);

        var jogadores = await _jogadorService
            .GetAllAsync(pageNumber, pageSize);

        if (!jogadores.Items.Any())
            return NoContent();

        return Ok(jogadores);
    }

    /// <summary>
    /// Retorna um jogador pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Jogador encontrado com sucesso")]
    [SwaggerResponse(404, "Jogador não encontrado")]
    public async Task<IActionResult> GetById(int id)
    {
        var jogador = await _jogadorService.GetByIdAsync(id);

        if (jogador == null)
            return NotFound();

        return Ok(jogador);
    }

    /// <summary>
    /// Retorna jogadores filtrados pelo ID do time.
    /// </summary>
    [HttpGet("time/{timeId}")]
    [SwaggerResponse(200, "Jogadores encontrados com sucesso")]
    [SwaggerResponse(204, "Nenhum jogador encontrado para este time")]
    public async Task<IActionResult> GetByTime(
        int timeId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(
                "PageNumber e PageSize devem ser maiores que zero.");

        pageSize = Math.Min(pageSize, 100);

        var jogadores = await _jogadorService
            .GetByTimeAsync(timeId, pageNumber, pageSize);

        if (!jogadores.Items.Any())
            return NoContent();

        return Ok(jogadores);
    }

    /// <summary>
    /// Retorna jogadores filtrados pela função.
    /// </summary>
    [HttpGet("funcao/{funcao}")]
    [SwaggerResponse(200, "Jogadores encontrados com sucesso")]
    [SwaggerResponse(204, "Nenhum jogador encontrado para esta função")]
    public async Task<IActionResult> GetByFuncao(
        string funcao,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(
                "PageNumber e PageSize devem ser maiores que zero.");

        pageSize = Math.Min(pageSize, 100);

        var jogadores = await _jogadorService
            .GetByFuncaoAsync(funcao, pageNumber, pageSize);

        if (!jogadores.Items.Any())
            return NoContent();

        return Ok(jogadores);
    }

    /// <summary>
    /// Cadastra um novo jogador vinculado a um time.
    /// </summary>
    [HttpPost]
    [SwaggerResponse(201, "Jogador criado com sucesso")]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Time não encontrado")]
    public async Task<IActionResult> Create(JogadorCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jogador = await _jogadorService.CreateAsync(dto);

        if (jogador == null)
            return NotFound("Time não encontrado.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = jogador.Id },
            jogador
        );
    }

    /// <summary>
    /// Atualiza um jogador existente.
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerResponse(200, "Jogador atualizado com sucesso")]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Jogador ou time não encontrado")]
    public async Task<IActionResult> Update(int id, JogadorCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jogador = await _jogadorService.UpdateAsync(id, dto);

        if (jogador == null)
            return NotFound("Jogador ou time não encontrado.");

        return Ok(jogador);
    }

    /// <summary>
    /// Remove um jogador pelo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerResponse(204, "Jogador removido com sucesso")]
    [SwaggerResponse(404, "Jogador não encontrado")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _jogadorService.DeleteAsync(id);

        if (!removido)
            return NotFound();

        return NoContent();
    }
}