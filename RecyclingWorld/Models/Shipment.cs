using System.ComponentModel.DataAnnotations.Schema;

namespace SD7501_RecyclingWorld_22502551.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedArrival { get; set; } // Nullable to allow for cases where shipment details are not yet available
        public string ShippingStatus { get; set; }
    }
}
//code for the Shipment model, which represents the shipment details of an order. It has properties for Id, OrderId, Carrier, TrackingNumber, ShippedDate, EstimatedArrival, and ShippingStatus. The OrderId is a foreign key that links to the Order model. The ShippedDate and EstimatedArrival are nullable DateTime fields to accommodate cases where shipment details may not be available at the time of order creation.
