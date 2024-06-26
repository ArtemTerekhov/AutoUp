﻿using System.Collections.Generic;

namespace AutoUp.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
