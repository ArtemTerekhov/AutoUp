namespace AutoUp.Models
{
    public class ForumTime
    {
        public int ForumTimeId { get; set; }
        public int ForumId { get; set; }
        public Forum Forum { get; set; }
        public int UserId { get; set; }
        public string DateTime { get; set; }
        public string Date { get; set; }
    }
}
