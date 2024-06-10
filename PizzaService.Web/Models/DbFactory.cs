using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PizzaService.Web.Models
{
    public class DbFactory : WebFactory
    {
        public override void Method<DbContext>(IHostApplicationBuilder builder, string schemaName)
        {
            Cfg config = Cfg.Read(builder.Configuration);
            switch(config.Store)
            {
                case StorageType.Memory:
                    builder.Services.AddDbContextPool<DbContext>(opt => opt.UseInMemoryDatabase(schemaName));
                    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
                    break;
                default:
                    throw new System.NotImplementedException();
            };
        }
    }
}
