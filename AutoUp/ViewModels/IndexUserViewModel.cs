using System.Collections.Generic;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class IndexUserViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterUserViewModel FilterUserViewModel { get; set; }
        public SortUserViewModel SortUserViewModel { get; set; }
    }
}
