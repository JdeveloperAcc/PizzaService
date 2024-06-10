using Microsoft.EntityFrameworkCore;

namespace PizzaService.Web.Models
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options)
            : base(options) { }

        public DbSet<Order> Todos => Set<Order>();
    }
}
