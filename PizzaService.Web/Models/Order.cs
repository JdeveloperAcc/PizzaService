namespace PizzaService.Web.Models
{
    public class Order
    {
        public uint Id { get; set; }
        public string ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}
