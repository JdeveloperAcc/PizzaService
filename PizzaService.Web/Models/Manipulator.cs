using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaService.Web.Models
{
    public abstract class Manipulator<T>
    {
        public abstract int AddDataIncrement(T[] incremental);
        public abstract T GetDataItem(uint id);
        public abstract T GetDataNamed(string name);
        public abstract IEnumerable<T> GetAggregatedResults();
        public abstract void CommitResultOk();
        public abstract void CommitResultError();
        public abstract void ConfirmDeducted(IEnumerable<T> sent);

        // Async Tasks
        public virtual async Task<int> AddDataIncrementAsync(T[] incremental) => await Task<bool>.Run(() => AddDataIncrement(incremental));
        public virtual async Task<T> GetDataItemAsync(uint id) => await Task<T>.Run(() => GetDataItem(id));
        public virtual async Task<T> GetDataNamedAsync(string name) => await Task<T>.Run(() => GetDataNamed(name));
        public virtual async Task<IEnumerable<T>> GetAggregatedResultsAsync() => await Task<IEnumerable<T>>.Run(GetAggregatedResults);
        public virtual async Task CommitResultOkAsync() => await Task.Run(CommitResultOk);
        public virtual async Task CommitResultErrorAsync() => await Task.Run(CommitResultError);
        public virtual async Task ConfirmDeductedAsync(IEnumerable<T> sent) => await Task.Run(() => ConfirmDeducted(sent));
    }
}
