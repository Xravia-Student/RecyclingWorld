using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecyclingWorld.Data;
using Stripe;
using Stripe.Checkout;

namespace RecyclingWorld.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public PaymentApiController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        //Post api/payment/create-checkout-session

        [HttpPost("create-session/{orderId}")]
        public async Task<IActionResult> CreateSession(int orderId)

        {
            var order = await _db.Orders.Include(o => o.OrderDetails).ThenInclude(d => d.Product).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"]; // set the Stripe API key from configuration
            var domain = $"{Request.Scheme}://{Request.Host}";

            var lineItems = order.OrderDetails.Select(d => new SessionLineItemOptions // create a line item for each order detail
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "nzd",
                    UnitAmountDecimal = d.Price * 100,   // Stripe wants cents
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = d.Product.Title // use the product title for the line item name
                    }
                },
                Quantity = (long)d.Quantity
            }).ToList(); // convert quantity to long for Stripe

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                LineItems = lineItems,
                SuccessUrl = $"{domain}/Order/PaymentSuccess?orderId={orderId}",
                CancelUrl = $"{domain}/Order/Summary" //set the cancel URL to the order summary page
            };

            var service = new SessionService();
            Session session = service.Create(options);

            // store the Stripe session id on the order
            order.SessionId = session.Id;
            await _db.SaveChangesAsync();

            return Ok(new { url = session.Url }); // return the session URL to the client so they can be redirected to Stripe Checkout
        }
    }
}
