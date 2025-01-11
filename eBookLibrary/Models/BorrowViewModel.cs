using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class BorrowViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; } // Foreign key to the Book
        public Book Book { get; set; }
        public int? RemainingTime { get; set; }
        public DateTime BorrowDate { get; set; } // Date when the book was borrowed
        public DateTime ReturnDate { get; set; } // Expected return date

        public int WaitingListCount { get; set; }
        public DateTime? NextAvailabilityDate { get; set; }
    }
}