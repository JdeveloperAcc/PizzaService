using Microsoft.EntityFrameworkCore;

namespace PizzaService.Web.Models
{
    public class Agregator
    {
        public readonly DbContext Context;

        private static  Manipulator<Order> Instance;

        protected Agregator() { }

        public Agregator(DbContext context)
        {
            this.Context = context;
        }

        public Manipulator<Order> SingletonManipulator
        {
            // Return a static reference

            get 
            { 
                if (Instance == null) 
                    Instance = new MemoryStore(new Adapter<DbContext>(Context)); 
                return Instance;
            } 
        }
    }
}
