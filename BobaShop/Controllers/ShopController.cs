using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BobaShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BobaShop.Controllers
{
    public class ShopController : Controller
    {
        // add db connection
        private readonly BobaShopContext _context;

        // constructor - method use to create an instance of this class
        public ShopController(BobaShopContext _context)
        {
            // accept in an instance of our db connection class and use this object to connect
            this._context = _context;
        }

        // GET: /shop
        public IActionResult Index()
        {
            var categories = _context.Category.OrderBy(c => c.Name).ToList();
            return View(categories);
        }

        public IActionResult Browse(string category)
        {
            // store the selected category name in the ViewBag so we can display in the view heading
            ViewBag.Category = category;

            // get the list of products for the selected category and pass the list to the view
            var products = _context.Product.Where(p => p.Category.Name == category).OrderBy(p => p.Name).ToList();

            return View(products);
        }

        public IActionResult ProductDetails(string product)
        {

            ViewBag.Product = product;

            var selectedProduct = _context.Product.SingleOrDefault(p => p.Name == product);

            return View(selectedProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int Quantity, int ProductId)
        {
            // get the product id and quantity
            var product = _context.Product.SingleOrDefault(p => p.ProductId == ProductId);
            var price = product.Price;

            // get the username
            var cartUsername = GetCartUsername();

            // create a fake cart
            var cart = new Cart
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = price,
                Username = cartUsername
            };

            _context.Cart.Add(cart);
            _context.SaveChanges();

            // show the cart page
            return RedirectToAction("Cart");
        }

        private String GetCartUsername()
        {
            // check if we already are storing the username in the user session
            if(HttpContext.Session.GetString("CartUsername") == null)
            {
                var cartUsername = "";

                // if user is already logged in
                if(User.Identity.IsAuthenticated)
                {
                    cartUsername = User.Identity.Name;
                }
                else
                {
                    // get a temporary and unique id to represent the user
                    cartUsername = Guid.NewGuid().ToString();
                }

                // store the cartUsername in a session variable
                HttpContext.Session.SetString("CartUsername", cartUsername);
            }

            // get the username that is stored in the session
            return HttpContext.Session.GetString("CartUsername");
        }

        public IActionResult Cart()
        {
            // get the username
            var cartUsername = GetCartUsername();

            // 2. query the db to get the user's cart items
            // join in db
            var cartItems = _context.Cart.Include(c => c.Product).Where(c => c.Username == cartUsername).ToList();

            // 3. load a view and pass the cart items to it for display
            return View(cartItems);
        }
    }
}