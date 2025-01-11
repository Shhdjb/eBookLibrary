using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Make sure this is int if you're passing an integer
        public int BookId { get; set; }
        public int Rating { get; set; }
        public DateTime DateAdded { get; set; }
        // Navigation property
        public virtual Book Book { get; set; }
    }
}


