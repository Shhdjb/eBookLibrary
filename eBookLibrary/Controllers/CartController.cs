using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eBookLibrary.Models;

namespace eBookLibrary.Controllers
{
    public class CartController : Controller
    {
        // Display the shopping cart
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }


        // Add a book to the cart
        [HttpPost]
        public ActionResult AddToCart(int bookId, string title, decimal? buyPrice, decimal? discountPrice)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Validate the prices
            if (!buyPrice.HasValue || buyPrice <= 0)
            {
                // Log or handle the invalid price
                System.Diagnostics.Debug.WriteLine($"Error: Invalid BuyPrice for bookId {bookId}. BuyPrice: {buyPrice}");
                TempData["Error"] = $"The book '{title}' has an invalid price.";
                return RedirectToAction("Index");
            }

            // Check if the book is already in the cart
            var existingItem = cart.FirstOrDefault(item => item.BookId == bookId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                // Add the new item to the cart
                cart.Add(new CartItem
                {
                    BookId = bookId,
                    Title = title,
                    BuyPrice = buyPrice.Value,
                    DiscountPrice = discountPrice,
                    Quantity = 1
                });
            }

            // Save the updated cart in the session
            Session["Cart"] = cart;

            // Log the added book
            System.Diagnostics.Debug.WriteLine($"Book '{title}' added to cart. Final price: {(discountPrice.HasValue && discountPrice > 0 ? discountPrice.Value : buyPrice.Value)}");
            return RedirectToAction("Index");
        }




        // Remove an item from the cart
        [HttpPost]
        public ActionResult RemoveFromCart(int bookId)
        {
            // Retrieve the cart from the session or create a new one if it doesn't exist
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Find the book to remove in the cart
            var itemToRemove = cart.FirstOrDefault(item => item.BookId == bookId);

            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity > 1)
                {
                    // Decrease the quantity by 1 if it is greater than 1
                    itemToRemove.Quantity--;
                }
                else
                {
                    // Remove the book entirely if the quantity is 1
                    cart.Remove(itemToRemove);
                }
            }

            // Update the cart in the session
            Session["Cart"] = cart;

            // Redirect back to the cart page
            return RedirectToAction("Index");
        }


    }
}
