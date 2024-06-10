using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PizzaService.Web.Models
{
    internal class S2SFeedConsole : IServer<Order>
    {
        public bool Commit(IEnumerable<Order> listed)
        {
            string jsonString = JsonSerializer.Serialize(listed, new JsonSerializerOptions
                                                                        {
                                                                            WriteIndented = true
                                                                        });
            System.Console.Out.WriteLine(jsonString);

            return listed.Count() > 0;
        }

        public bool CommitStatus(IEnumerable<Order> listed)
        {
            return true;
        }
    }
}
