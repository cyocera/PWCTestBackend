using Microsoft.EntityFrameworkCore;
using PWCExamService.Data;

namespace PWCExamService.Data.Context
{
    public partial class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<LineIncidentHistorical> LineIncidentHistorical{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
