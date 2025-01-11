using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class WaitingList
    {
        public int Id { get; set; }
        public int BookId { get; set; } // Foreign key to the Book
        public int UserId { get; set; } // User waiting for the book
        public DateTime DateAdded { get; set; } // When the user was added to the waiting list
    }

}