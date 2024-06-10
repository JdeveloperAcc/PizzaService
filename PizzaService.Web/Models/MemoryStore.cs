using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaService.Web.Models
{
    public class MemoryStore : StoreActivity
    {
        public MemoryStore(Adapter<DbContext> adapt) : base(adapt) { }

        public override int AddDataIncrement(Order[] incremental)
        {
            throw new System.NotImplementedException();
        }

        public override Order GetDataItem(uint id)
        {
            throw new System.NotImplementedException();
        }

        public override Order GetDataNamed(string name)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<Order> GetAggregatedResults()
        {
            throw new System.NotImplementedException();
        }

        public override void CommitResultOk()
        { }

        public override void CommitResultError()
        { }

        public override void ConfirmDeducted(IEnumerable<Order> sent)
        {
            throw new System.NotImplementedException(); 
        }

        public override async Task<int> AddDataIncrementAsync(Order[] incremental)
        {
            List<Order> validated = new List<Order>();
            foreach (Order od in incremental)
            {
                // Check mandatory fields
                if (!string.IsNullOrWhiteSpace(od.ProductId) &&
                    od.Quantity > 0)
                {
                    validated.Add(od);
                }
            }

            if (validated.Count > 0)
            {
                var g = validated.GroupBy(x => x.ProductId);
                foreach (var group in g)
                {
                    Order aggregated = new Order()
                    {
                        ProductId = group.Key,
                        // Calc positive orders only
                        Quantity = (uint)group.Sum(y => y.Quantity)
                    };

                    Order update = await Adapter.GetStorageIO<TodoDb>().
                        Todos.
                        Where(p => p.ProductId == aggregated.ProductId).FirstOrDefaultAsync<Order>();
                    if (update != null)
                    {
                        update.Quantity += aggregated.Quantity;
                    }
                    else
                    {
                        Adapter.GetStorageIO<TodoDb>().Todos.Add(aggregated);
                    }
                }

                return await Adapter.GetStorageIO<TodoDb>().SaveChangesAsync();
            }
            else
                return 0;
        }

        public override async Task<Order> GetDataItemAsync(uint id)
        {
            return await Adapter.GetStorageIO<TodoDb>().Todos.FindAsync(id)
                    is Order od ? od : null;                   
        }

        public override async Task<Order> GetDataNamedAsync(string name)
        {
            return await Adapter.GetStorageIO<TodoDb>().
                Todos.
                Where(p => p.ProductId == name).FirstOrDefaultAsync<Order>()
                    is Order od ? od : null;
        }

        public override async Task<IEnumerable<Order>> GetAggregatedResultsAsync()
        {
            return await Adapter.GetStorageIO<TodoDb>().Todos.ToListAsync();
        }

        public override async Task ConfirmDeductedAsync(IEnumerable<Order> sent)
        {
            foreach(Order od in sent)
            {
                Order update = await Adapter.GetStorageIO<TodoDb>().
                        Todos.
                        Where(p => p.ProductId == od.ProductId).FirstOrDefaultAsync<Order>();
                if (update != null)
                {
                    update.Quantity -= update.Quantity >= od.Quantity ? od.Quantity : 0;

                    if (update.Quantity == 0)
                    {
                        var delete = update;
                        Adapter.GetStorageIO<TodoDb>().Todos.Remove(delete);
                    }
                }
            }

            await Adapter.GetStorageIO<TodoDb>().SaveChangesAsync();
        }
    }
}
