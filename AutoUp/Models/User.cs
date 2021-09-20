using System.Collections.Generic;

namespace AutoUp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Telegram { get; set; }
        public string Jabber { get; set; }
        public decimal Balance { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
     
        public ICollection<ForumLink> ForumLinks { get; set; }

        public User()
        {
                      ForumLinks = new HashSet<ForumLink>();
        }
    }
}
