using System.Collections.Generic;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class StatisticsViewModel
    {
        public string Balance;
        public List<Forum> Forums { get; set; }
        public Dictionary<int, List<ForumLink>> ForumLinks { get; set; }
        public Dictionary<int, List<ForumTime>> ForumTimes { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortForumViewModel SortForumViewModel { get; set; }
    }
}
