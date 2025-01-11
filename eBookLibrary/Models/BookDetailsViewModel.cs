using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public List<FeedbackViewModel> Feedbacks { get; set; }
    }
}