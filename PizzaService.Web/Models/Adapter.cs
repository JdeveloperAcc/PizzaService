using Microsoft.EntityFrameworkCore;

namespace PizzaService.Web.Models
{
    public class Adapter<Context> where Context : DbContext
    {
        private readonly Context _storageIO;

        public Adapter(Context accessed)
        {
            this._storageIO = accessed;
        }

        public UsedContext GetStorageIO<UsedContext>() where UsedContext : DbContext
        {
            return _storageIO as UsedContext;
        }
    }
}
