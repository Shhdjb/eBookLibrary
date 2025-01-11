using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBookLibrary.Models
{
    public class WaitingListPageViewModel
    {
        public string BookTitle { get; set; }
        public List<WaitingListViewModel> WaitingList { get; set; }
    }
}