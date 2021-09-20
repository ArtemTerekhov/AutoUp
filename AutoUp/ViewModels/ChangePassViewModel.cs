using System.ComponentModel.DataAnnotations;

namespace AutoUp.ViewModels
{
    public class ChangePassViewModel
    {
        public int UserId { get; set; }

        [Compare("ActualPassword", ErrorMessage = "Пароль введен неверно")]
        [Display(Name = "Старый Пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Новый Пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть не менее 8 символов")]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }

        public string ActualPassword { get; set; }


    }
}
