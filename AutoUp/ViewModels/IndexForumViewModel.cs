using System.Collections.Generic;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class IndexForumViewModel
    {
        public IEnumerable<Forum> Forums{ get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterForumViewModel FilterForumViewModel { get; set; }
        public SortForumViewModel SortForumViewModel { get; set; }
    }
}
