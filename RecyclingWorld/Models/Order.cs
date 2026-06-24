using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclingWorld.Models
{
    public class Order
    {
        public int Id { get; set; }

        //applicatttionUser link
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; } 
        public string OrderStatus { get; set; }
        public int PaymentStatus { get; set; }

        //stripe inmplementation fields 
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }
        //delivery details

        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        //relationship with order details
        public Shipment Shipment { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
