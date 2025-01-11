using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; }


        // Calculated property for the price (takes discount into account if applicable)
        public decimal FinalPrice => DiscountPrice.HasValue ? DiscountPrice.Value : BuyPrice;
    }
}
