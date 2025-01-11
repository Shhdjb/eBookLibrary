using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class UserBook
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Change from string to int
        public int BookId { get; set; } // Foreign key for Book
        public bool IsBorrowed { get; set; }
        public DateTime? BorrowEndDate { get; set; }
        public bool IsOwned { get; set; }

        // Navigation property for the related Book
        public virtual Book Book { get; set; }
    }



}