using PWCExamService.Data.Context;

namespace PWCExamService.Data.Repositories
{
    public interface IUserRepository : IRepository<Users>
    {

    }
    public class UsersRepository : Repository<Users>, IUserRepository
    {
        public UsersRepository(AppDBContext context) : base(context)
        {

        }
    }
}
