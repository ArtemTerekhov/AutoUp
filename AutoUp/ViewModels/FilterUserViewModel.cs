using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoUp.Models;


namespace AutoUp.ViewModels
{
    public class FilterUserViewModel
    {
        public FilterUserViewModel(List<User> users, int? user, string login)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            users.Insert(0, new User { Login = "Все", UserId = 0 });
            Users = new SelectList(users, "UserId", "Login", user);
            SelectedUser = user;
            SelectedLogin = login;
        }

        public SelectList Users { get; private set; }
        public int? SelectedUser { get; private set; }
        public string SelectedLogin { get; private set; }
    }
}
