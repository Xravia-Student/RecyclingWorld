using Microsoft.AspNetCore.Mvc; // For Controller
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecyclingWorld.Data; // For ApplicationDbContext
using RecyclingWorld.Models;
using RecyclingWorld.ViewModels;
using RecyclingWorld.Utilities;
using System.Security.Claims;
using Stripe;

namespace RecyclingWorld.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private const string CartKey = "Cart";

        public OrderController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _db = db;
            _userManager = userManager;
            _config = config; // Inject IConfiguration to access Stripe API keys from appsettings.json now works anywhere in the application, not just in the PaymentApiController.
        }

        public async Task<IActionResult> Summary()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();
            if (!cart.Any())
                return RedirectToAction("Index", "Product");

            var user = await _userManager.GetUserAsync(User);

            var vm = new OrderSummaryViewModel
            {
                CartItems = cart,
                OrderTotal = cart.Sum(i => i.PricePerKg * (decimal)i.Quantity),
                Order = new Order
                {
                    Name = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    PostalCode = user.PostalCode
                }
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(OrderSummaryViewModel vm)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            if (!cart.Any())
                return RedirectToAction("Index", "Product");

            var userId = _userManager.GetUserId(User);
            // Create a new order based on the view model and cart items

            var order = new Order
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                Name = vm.Order.Name,
                StreetAddress = vm.Order.StreetAddress,
                City = vm.Order.City,
                PostalCode = vm.Order.PostalCode,
                OrderTotal = cart.Sum(i => i.PricePerKg * (decimal)i.Quantity),
                OrderDetails = new List<OrderDetail>()
            };

            // Add order details for each cart item
            foreach (var item in cart)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product == null) continue;
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.PricePerKg // Assuming price is per kg and quantity is in kg
                });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(); // Save the order to the database

            var shipment = new Shipment
            {
                OrderId = order.Id,
                ShippingStatus = "Preparing",
                TrackingNumber = "RW" + order.Id + "-" + new Random().Next(1000, 9999),
                Carrier = "NZ Post Freight"
            };
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            // 4. clear the cart
            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        // Clear the cart after saving the order

            public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _db.Orders.Include(o => o.OrderDetails).ThenInclude(d => d.Product).Include(o => o.Shipment).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order); // Pass the order to the view to display order details and shipment information

        }
        //stripe redirects to this action after payment is successful

        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Shipment)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound(); // check if the order exists

            // verify with Stripe that the session was actually paid
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
            var service = new Stripe.Checkout.SessionService();
            var session = service.Get(order.SessionId);

            if (session.PaymentStatus == "paid") // check if the payment was successful
            {
                order.PaymentStatus = "Paid";
                order.OrderStatus = "Approved";// update order status to Approved after successful payment
                order.PaymentIntentId = session.PaymentIntentId;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Confirmation", new { id = orderId }); // redirect to the confirmation page to show order details and shipment information
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _db.Orders
                .Where(o => o.ApplicationUserId == userId)   // only this user's orders
                .Include(o => o.Shipment)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage() // This action allows admins to view and manage all orders in the system, including their shipment status and details.

        {
            var orders = await _db.Orders
                .Include(o => o.Shipment)
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShipment(int shipmentId, string status) // This action allows admins to update the shipment status of an order
        {
            var shipment = await _db.Shipments.FindAsync(shipmentId);
            if (shipment == null) return NotFound(); // check if the shipment exists

            shipment.ShippingStatus = status;
            if (status == "InTransit") shipment.ShippedDate = DateTime.Now;
            var order = await _db.Orders.FindAsync(shipment.OrderId); // find the order associated with the shipment
            if (order != null)
            {
                if (status == "InTransit") order.OrderStatus = "Shipped";
                if (status == "Delivered") order.OrderStatus = "Completed";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Manage"); // redirect back to the Manage view to see the updated shipment status


        }
    }
}
