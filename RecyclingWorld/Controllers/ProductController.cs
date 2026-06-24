using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecyclingWorld.Data;

namespace RecyclingWorld.Controllers
{
    public class ProductController : Controller // The ProductController class is responsible for handling HTTP requests related to products in the RecyclingWorld application. It interacts with the ApplicationDbContext to retrieve product data from the database and returns views to display that data to the user.
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index() // The Index action method is an asynchronous method that retrieves a list of products from the database, including their associated categories, and returns a view to display that list. It uses Entity Framework Core's Include method to perform eager loading of the related Category data, ensuring that the category information is available when the products are displayed in the view.
        {
            var products = await _db.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
    }
}
// The ProductController class is an ASP.NET Core MVC controller that manages the product-related actions in the RecyclingWorld application. It has a constructor that takes an ApplicationDbContext instance, which is used to interact with the database. The Index action method retrieves a list of products from the database, including their associated categories using the Include method for eager loading, and then returns the view with the list of products. This allows the application to display a list of products along with their category information on the Index view.