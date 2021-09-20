using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class FilterRoleViewModel
    {
        public FilterRoleViewModel(List<Role> roles, int? role, string roleName)
        {
           
            Roles = new SelectList(roles, "RoleId", "Name", role);
            SelectedRole = role;
            SelectedName = roleName;
        }

        public SelectList Roles { get; private set; }
        public int? SelectedRole { get; private set; }
        public string SelectedName { get; private set; }
    }
}
