using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AutoUp.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Не указан логин")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Длина логина должна быть от 4 до 50 символов")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть не менее 8 символов")]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Некорректный адрес Email")]
        public string Email { get; set; }

        [Display(Name = "Telegram")]
        [StringLength(100, ErrorMessage = "Длина Telegram адреса должна быть меньше 100 символов")]
        public string Telegram { get; set; }

        [Display(Name = "Jabber")]
        [EmailAddress(ErrorMessage = "Некорректный адрес Jabber")]
        [Remote(action: "CheckEmail", controller: "Account", ErrorMessage = "Адрес Jabber уже используется")]
        public string Jabber { get; set; }

        [Display(Name = "Баланс")]
        [RegularExpression(@"^\d*(\.\d{1,2})?$", ErrorMessage = "Недопустимый формат для десятичного числа")]
        public string Balance { get; set; }

        public FilterRoleViewModel FilterRoleViewModel { get; set; }
    }
}
