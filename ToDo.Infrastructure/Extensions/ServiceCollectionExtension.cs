using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Infrastructure.Persistence;
using ToDo.Infrastructure.Repositories;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) // This method extends IServiceCollection and registers services required for the application infrastructure layer.
        {
            services.AddDbContext<ToDoDbContext>(options => options.UseNpgsql(
                configuration.GetConnectionString("ToDo")));
            services.AddScoped<IToDoRepository, ToDoRepository>();

        }
        public static void ApplyDatabaseMigrations(this IServiceProvider serviceProvider) //Add migration to running database on docker
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            dbContext.Database.Migrate(); 
        }
    }
}
