using RecyclingWorld.Models;

namespace RecyclingWorld.ViewModels
{
    public class OrderSummaryViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public Order Order { get; set; }
        public decimal OrderTotal {  get; set; }
    }
}
