using System.Collections.Generic;

namespace AutoUp.Models
{
    public class Forum
    {
        public int ForumId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public decimal UpPrice { get; set; }
        public string UpTime { get; set; }
        public ICollection<ForumLink> ForumLinks { get; set; }
        public ICollection<ForumTime> ForumTimes { get; set; }

        public Forum()
        {
            ForumLinks = new HashSet<ForumLink>();
            ForumTimes = new HashSet<ForumTime>();
        }
    }
}
