using System;

namespace PizzaService.Web.Models
{
    public class Submitted
    {
        public int? Qty { get; set; }
        public DateTime? Time { get; set; }
        public string Hash { get; set; }
    }
}
