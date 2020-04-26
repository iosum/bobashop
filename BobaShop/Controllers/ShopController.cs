﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BobaShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

            // check if this user has this product already in cart.  If so, update quantity
            var cartItem = _context.Cart.FirstOrDefault(c => c.ProductId == ProductId
                && c.Username == cartUsername);

            // if the product is not in the cart
            if(cartItem == null)
            {
                // add the product to the cart
                var cart = new Cart
                {
                    ProductId = ProductId,
                    Quantity = Quantity,
                    Price = price,
                    Username = cartUsername
                };   

                _context.Cart.Add(cart);
            }
            else
            {
                cartItem.Quantity += Quantity; // add the new quantity to the existing quantity
                _context.Update(cartItem);
            }



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

        public IActionResult RemoveFromCart(int id)
        {
            var cartItem = _context.Cart.SingleOrDefault(c => c.CartId == id);

            // delete the item
            _context.Cart.Remove(cartItem);
            _context.SaveChanges();

            // redirect to the cart page
            return RedirectToAction("Cart");

        }

        // if not logging in, redirect to the login view
        [Authorize]
        public IActionResult Checkout()
        {
            // check if the user is logged in
            MigrateCart();
            // if yes, go to the Checkout.cshtml to let the user fill out the form 
            return View();
        }

        // after the user filling out the form, it will hit this method
        // [Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order : get the inputs from the form
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order)
        {
            // auto-fill the date, user, and total properties rather than let the user enter these values
            order.OrderDate = DateTime.Now;
            order.UserId = User.Identity.Name;
            var cartItems = _context.Cart.Where(c => c.Username == User.Identity.Name);
            decimal cartTotal = (from c in cartItems
                                 select c.Quantity * c.Price).Sum();
            order.Total = cartTotal;

            // store the order in object with the external extension
            HttpContext.Session.SetObject("Order", order);

            return RedirectToAction("Payment");
        }


        private void MigrateCart()
        {
            // if the user shopping anonomyously
            if(HttpContext.Session.GetString("CartUsername") != User.Identity.Name)
            {
                // get the guid from the session
                var cartUsername = HttpContext.Session.GetString("CartUsername");
               

                // get the anon's cart item
                var cartItems = _context.Cart.Where(c => c.Username == cartUsername);

                // get all items and update the username
                foreach(var item in cartItems)
                {
                    // item.Username : guid from the cart db
                    // User.Identity.Name : the name after login in
                    // update the username from the guid to the user's email
                    item.Username = User.Identity.Name;
                    _context.Update(item);
                }

                // save
                _context.SaveChanges();

                // update the session variable from a GUID to the user's email
                HttpContext.Session.SetString("CartUsername", User.Identity.Name);
            }
        }

        public IActionResult Payment()
        {
            return View();
        }
        

    }
}