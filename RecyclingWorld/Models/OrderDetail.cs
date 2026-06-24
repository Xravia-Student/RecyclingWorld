using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclingWorld.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
//code for the OrderDetail model, which represents the details of an order. It has properties for Id, OrderId, ProductId, Quantity, and Price. The OrderId and ProductId are foreign keys that link to the Order and Product models, respectively. Found online at order models github examples