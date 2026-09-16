using CP4.Application.Interfaces.Repositories;
using CP4.Domain.Entities;
using CP4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CP4.Infrastructure.Repositories;

public class TimeRepository : ITimeRepository
{
    private readonly ApplicationContext _context;

    public TimeRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<Time>> GetAllAsync()
    {
        return await _context.Times
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Time?> GetByIdAsync(int id)
    {
        return await _context.Times
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Time>> GetByJogoAsync(string jogo)
    {
        return await _context.Times
            .AsNoTracking()
            .Where(t => t.Jogo.ToLower() == jogo.ToLower())
            .ToListAsync();
    }

    public async Task AddAsync(Time time)
    {
        await _context.Times.AddAsync(time);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Time time)
    {
        _context.Times.Update(time);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Time time)
    {
        _context.Times.Remove(time);
        await _context.SaveChangesAsync();
    }
}