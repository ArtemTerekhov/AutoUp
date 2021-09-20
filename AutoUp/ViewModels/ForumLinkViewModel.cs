using System.ComponentModel.DataAnnotations;

namespace AutoUp.ViewModels
{
    public class ForumLinkViewModel
    {
        public int? ForumLinkId { get; set; }
        public int ForumId { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Логин форума")]
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Секретное слово")]
        public string SecretWord { get; set; }

        [Display(Name = "Url - ccылка темы")]
        [Required(ErrorMessage = "Не указан Url ссылки темы")]
        public string LinkUrl { get; set; }
        public string ForumDelay { get; set; }
        public string LinkState { get; set; }

        /*
        public string[] DummyForumTimes { get; set; }
        */

        [Display(Name = "Баланс")]
        public string Balance { get; set; }

    }
}
