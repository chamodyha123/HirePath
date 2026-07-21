using HirePathAI.API.Data;
using HirePathAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HirePathAI.API.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Implement async methods
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask; // This is just to make it async
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask; // This is just to make it async
        }
    }
}