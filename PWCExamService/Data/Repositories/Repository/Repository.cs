using Microsoft.EntityFrameworkCore;
using PWCExamService.Data.Context;
using System.Linq.Expressions;

namespace PWCExamService.Data
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression);
        Task Insert(T entity);
        Task Update(T Entity);
        Task UpdateRange(IEnumerable<T> entities);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(T entity);
    }
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDBContext context;

        public Repository(AppDBContext context)
        {
            this.context = context;
        }

        public async Task Insert(T entity)
        {
            context.Set<T>().Add(entity);
        }
        public async Task InsertRange(IEnumerable<T> entities)
        {
            context.Set<T>().AddRange(entities);
        }
        public async Task Update(T Entity)
        {
            context.Entry(Entity).State = EntityState.Modified;
        }
        public async Task UpdateRange(IEnumerable<T> entities) 
        {
            context.Set<T>().UpdateRange(entities);
        }
        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>().Where(expression).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }
        public async Task<T> GetById(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }
        public async Task Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
    }
}
