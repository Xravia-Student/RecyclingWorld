using Microsoft.AspNetCore.Mvc;
using RecyclingWorld.Data;
using RecyclingWorld.Models;
using RecyclingWorld.Utilities;


namespace RecyclingWorld.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartKey = "Cart";

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, double quantity)
        {
            var product = _db.Products.Find(productId);
            if (product == null) return NotFound();
            if (quantity < 1) quantity = 1;

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.PricePerKg = product.GetPriceForQuantity(existing.Quantity);
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    PricePerKg = product.GetPriceForQuantity(quantity),
                    Quantity = quantity
                });
            }
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            cart.RemoveAll(c => c.ProductId == productId);
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }
    }
}
