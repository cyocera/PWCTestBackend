using PWCExamService.Data.Context;
using PWCExamService.Data.Repositories;

namespace PWCExamService.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository users { get; }
        ILineIncidentHistorical lineIncidentHistorical { get; }
        int Save();
    }
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDBContext context;

        public UnitOfWork(AppDBContext context)
        {
            this.context = context;
            users = new UsersRepository(this.context);
            lineIncidentHistorical = new LineIncidentHistoricalRepository(this.context);
        }

        public IUserRepository users { get; set; }
        public ILineIncidentHistorical lineIncidentHistorical { get; set; }

        public void Dispose()
        {
            context.Dispose();
        }

        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
