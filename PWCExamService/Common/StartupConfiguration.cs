using Microsoft.EntityFrameworkCore;
using PWCExamService.Data.Context;
using PWCExamService.Data.UnitOfWork;
using PWCExamService.Managers;

namespace PWCExamService.Common
{
    public static class StartupConfiguration
    {
        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDBContext>();
                context.Database.Migrate();
            }
        }

        public static void InitializeServices(this IServiceCollection services) 
        {
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<ISubtesManager, SubtesManager>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
