using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class PersonalLibraryViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CoverImagePath { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime? BorrowEndDate { get; set; }
        public bool IsOwned { get; set; }
        public int UserId { get; set; }
        public string Format { get; set; } // Include the format field


    }

}