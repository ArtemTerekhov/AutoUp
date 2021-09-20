using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class ForumViewModel
    {

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public IEnumerable<ForumLink> ForumLink { get; set; }
        public FilterForumViewModel FilterForumViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
    }
}
