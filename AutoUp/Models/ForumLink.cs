namespace AutoUp.Models
{
    public class ForumLink
    {
        public int ForumLinkId { get; set; }
        public int ForumId { get; set; }
        public Forum Forum { get; set; }
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SecretWord { get; set; }
        public string LinkUrl { get; set; }
        public int ForumDelay { get; set; }
        public bool LinkState { get; set; }
        //    public ICollection<ForumTime> ForumTimes { get; set; }

        /*
        public ForumLink()
        {
            ForumTimes = new HashSet<ForumTime>();
        }
        */

    }
}
