using CP4.Application.Interfaces.Repositories;
using CP4.Domain.Entities;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP4.Infrastructure.Repositories;

public class JogadorRepository : IJogadorRepository
{
    private readonly ApplicationContext _context;

    public JogadorRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<Jogador>> GetAllAsync()
    {
        return await _context.Jogadores
            .AsNoTracking()
            .Include(j => j.Time)
            .ToListAsync();
    }

    public async Task<Jogador?> GetByIdAsync(int id)
    {
        return await _context.Jogadores
            .AsNoTracking()
            .Include(j => j.Time)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<Jogador?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Jogadores
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<List<Jogador>> GetByTimeAsync(int timeId)
    {
        return await _context.Jogadores
            .AsNoTracking()
            .Include(j => j.Time)
            .Where(j => j.TimeId == timeId)
            .ToListAsync();
    }

    public async Task<List<Jogador>> GetByFuncaoAsync(string funcao)
    {
        return await _context.Jogadores
            .AsNoTracking()
            .Include(j => j.Time)
            .Where(j => j.Funcao.ToLower() == funcao.ToLower())
            .ToListAsync();
    }

    public async Task AddAsync(Jogador jogador)
    {
        await _context.Jogadores.AddAsync(jogador);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Jogador jogador)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Jogador jogador)
    {
        _context.Jogadores.Remove(jogador);
        await _context.SaveChangesAsync();
    }
}