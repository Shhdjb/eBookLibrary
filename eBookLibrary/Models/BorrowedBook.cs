using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
public class BorrowedBook
{
    public int Id { get; set; }
    public int BookId { get; set; } // Foreign key to the Book
    public DateTime BorrowDate { get; set; } // Date when the book was borrowed
    public DateTime ReturnDate { get; set; } // Expected return date
    public int UserId { get; set; } // User who borrowed the book (int instead of string)
}

}