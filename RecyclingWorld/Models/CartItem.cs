namespace RecyclingWorld.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public decimal PricePerKg { get; set; }
        public int Quantity { get; set; } //Killograms of the product being purchased

        public double LineTotal => (double)PricePerKg * Quantity; // Calculates the total price for this cart item by multiplying the price per kilogram by the quantity (in kilograms). The result is returned as a double, which represents the total cost for this item in the shopping cart. This property is read-only and is computed on-the-fly whenever it is accessed, ensuring that it always reflects the current price and quantity of the item.
    }
}
