using CP4.Application.Interfaces.Repositories;
using CP4.Domain.Entities;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP4.Infrastructure.Repositories;

public class PerfilCompetitivoRepository : IPerfilCompetitivoRepository
{
    private readonly ApplicationContext _context;

    public PerfilCompetitivoRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<PerfilCompetitivo>> GetAllAsync()
    {
        return await _context.PerfisCompetitivos
            .AsNoTracking()
            .Include(p => p.Jogador)
            .ThenInclude(j => j!.Time)
            .ToListAsync();
    }

    public async Task<PerfilCompetitivo?> GetByIdAsync(int id)
    {
        return await _context.PerfisCompetitivos
            .AsNoTracking()
            .Include(p => p.Jogador)
            .ThenInclude(j => j!.Time)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PerfilCompetitivo?> GetByIdForUpdateAsync(int id)
    {
        return await _context.PerfisCompetitivos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PerfilCompetitivo?> GetByJogadorAsync(int jogadorId)
    {
        return await _context.PerfisCompetitivos
            .AsNoTracking()
            .Include(p => p.Jogador)
            .ThenInclude(j => j!.Time)
            .FirstOrDefaultAsync(p => p.JogadorId == jogadorId);
    }

    public async Task<bool> ExistsByJogadorAsync(
        int jogadorId,
        int? perfilIdIgnorado = null)
    {
        if (perfilIdIgnorado.HasValue)
        {
            var quantidade = await _context.PerfisCompetitivos
                .CountAsync(p =>
                    p.JogadorId == jogadorId &&
                    p.Id != perfilIdIgnorado.Value);

            return quantidade > 0;
        }

        var total = await _context.PerfisCompetitivos
            .CountAsync(p => p.JogadorId == jogadorId);

        return total > 0;
    }

    public async Task AddAsync(PerfilCompetitivo perfil)
    {
        await _context.PerfisCompetitivos.AddAsync(perfil);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PerfilCompetitivo perfil)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PerfilCompetitivo perfil)
    {
        _context.PerfisCompetitivos.Remove(perfil);
        await _context.SaveChangesAsync();
    }
}