using System;
using System.Collections.Generic;

namespace eBookLibrary.Models
{
    public class Book
    {
        // Basic Details
        public int Id { get; set; } // Unique identifier for the book
        public string Title { get; set; } // Title of the book
        public string Author { get; set; } // Author of the book
        public string Publisher { get; set; } // Publisher of the book

        // Pricing and Availability
        public decimal BuyPrice { get; set; } // Price for buying the book
        public decimal BorrowPrice { get; set; } // Price for borrowing the book
        public bool IsAvailableForBorrow { get; set; } // Indicates if the book can be borrowed
        public int CopiesAvailable { get; set; } // Add this field

        // Additional Details
        public string Genre { get; set; } // Genre of the book (e.g., Fiction, Non-fiction)
        public int YearOfPublishing { get; set; } // Year the book was published
        public int AgeLimit { get; set; } // Minimum age required to access this book (0 for no restriction)

        // Popularity Indicator
        public int Popularity { get; set; } // Popularity score (e.g., based on borrow/purchase counts)

        // Cover and Formats
        public string CoverImageUrl { get; set; } // URL for the book cover image (for web use)
        public string CoverImagePath { get; set; } // Physical or relative path to the cover image (for local storage)
        public string Formats { get; set; } // List of available formats (e.g., epub, mobi, PDF, etc.)

        // Discount Information
        public decimal? DiscountPrice { get; set; } // Discounted price (null if no discount is active)
        public DateTime? DiscountStartDate { get; set; } // Start date for the discount (null if no discount)
        public DateTime? DiscountEndDate { get; set; } // End date for the discount (null if no discount)

        // Inventory and Sales
        public int InStock { get; set; } // Total stock count for the book
        public bool IsBuyOnly { get; set; } // Indicates if the book is only available for purchase

        // Derived Property for Discount Logic
        public bool IsDiscountActive
        {
            get
            {
                // Check if a discount is currently active
                return DiscountPrice.HasValue &&
                       DiscountStartDate <= DateTime.Now &&
                       DiscountEndDate >= DateTime.Now;
            }
        }

        // Price Calculation Logic
        public decimal CurrentPrice
        {
            get
            {
                // Return the discounted price if the discount is active, otherwise return the buy price
                return IsDiscountActive ? DiscountPrice.Value : BuyPrice;
            }
        }

        // Constructor to Initialize Default Values
        public Book()
        {
            Formats = string.Empty; // Initialize as a string
            IsAvailableForBorrow = true;
            CopiesAvailable = 0;
            Popularity = 0;
            InStock = 0;
            IsBuyOnly = false;
        }
    }
}
