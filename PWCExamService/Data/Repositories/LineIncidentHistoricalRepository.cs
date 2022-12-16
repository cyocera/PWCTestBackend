using PWCExamService.Data.Context;

namespace PWCExamService.Data.Repositories
{
    public interface ILineIncidentHistorical : IRepository<LineIncidentHistorical>
    {
        
    }
    public class LineIncidentHistoricalRepository : Repository<LineIncidentHistorical>, ILineIncidentHistorical
    {
        public LineIncidentHistoricalRepository(AppDBContext context) : base(context)
        {

        }
    }
}
