using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace PizzaService.Web.Models
{
    public class WebFactory
    {
        public virtual void Method<Context>
            (IHostApplicationBuilder builder, string schemaName) where Context : DbContext
        { }
    }
}
