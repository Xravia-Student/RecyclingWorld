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
    }
}
