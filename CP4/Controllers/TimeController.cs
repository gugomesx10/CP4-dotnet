using CP4.Application.DTOs;
using CP4.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CP4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeController : ControllerBase
{
    private readonly ITimeService _timeService;

    public TimeController(ITimeService timeService)
    {
        _timeService = timeService;
    }

    /// <summary>
    /// Retorna todos os times cadastrados.
    /// </summary>
    [HttpGet]
    [SwaggerResponse(200, "Times encontrados com sucesso")]
    [SwaggerResponse(204, "Nenhum time encontrado")]
    public async Task<IActionResult> GetAll()
    {
        var times = await _timeService.GetAllAsync();

        if (!times.Any())
            return NoContent();

        return Ok(times);
    }

    /// <summary>
    /// Retorna um time pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Time encontrado com sucesso")]
    [SwaggerResponse(404, "Time não encontrado")]
    public async Task<IActionResult> GetById(int id)
    {
        var time = await _timeService.GetByIdAsync(id);

        if (time == null)
            return NotFound();

        return Ok(time);
    }

    /// <summary>
    /// Retorna times filtrados pelo jogo.
    /// </summary>
    [HttpGet("jogo/{jogo}")]
    [SwaggerResponse(200, "Times encontrados com sucesso")]
    [SwaggerResponse(204, "Nenhum time encontrado para este jogo")]
    public async Task<IActionResult> GetByJogo(string jogo)
    {
        var times = await _timeService.GetByJogoAsync(jogo);

        if (!times.Any())
            return NoContent();

        return Ok(times);
    }

    /// <summary>
    /// Cadastra um novo time.
    /// </summary>
    [HttpPost]
    [SwaggerResponse(201, "Time criado com sucesso")]
    [SwaggerResponse(400, "Dados inválidos")]
    public async Task<IActionResult> Create(TimeCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var time = await _timeService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = time.Id },
            time
        );
    }

    /// <summary>
    /// Atualiza um time existente.
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerResponse(200, "Time atualizado com sucesso")]
    [SwaggerResponse(400, "Dados inválidos")]
    [SwaggerResponse(404, "Time não encontrado")]
    public async Task<IActionResult> Update(int id, TimeCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var time = await _timeService.UpdateAsync(id, dto);

        if (time == null)
            return NotFound();

        return Ok(time);
    }

    /// <summary>
    /// Remove um time pelo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerResponse(204, "Time removido com sucesso")]
    [SwaggerResponse(404, "Time não encontrado")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _timeService.DeleteAsync(id);

        if (!removido)
            return NotFound();

        return NoContent();
    }
}