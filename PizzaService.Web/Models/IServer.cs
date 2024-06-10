using System.Collections.Generic;

namespace PizzaService.Web.Models
{
    internal interface IServer<T>
    {
        bool Commit(IEnumerable<T> listed);

        bool CommitStatus(IEnumerable<T> listed);
    }
}
