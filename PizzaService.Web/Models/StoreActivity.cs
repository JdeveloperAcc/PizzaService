using Microsoft.EntityFrameworkCore;

namespace PizzaService.Web.Models
{
    public abstract class StoreActivity : Manipulator<Order>
    {
        protected Adapter<DbContext> Adapter { get; }

        public StoreActivity(Adapter<DbContext> adapt)
        {
            this.Adapter = adapt;
        }
    }
}
