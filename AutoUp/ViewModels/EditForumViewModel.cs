using System.ComponentModel.DataAnnotations;

namespace AutoUp.ViewModels
{
    public class EditForumViewModel
    {
        public int ForumId { get; set; }
        
        [Display(Name = "Название форума")]
        [Required(ErrorMessage = "Не указано название форума")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Длина названия форума должно быть от 1 до 100 символов")]
        public string Name { get; set; }
       
        [Display(Name = "Url форума")]
        [Required(ErrorMessage = "Не указан Url форума")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Длина Url форума не должна быть от 5 до 200 символов")]
        public string Url { get; set; }
       
        [Display(Name = "Цена Upа")]
        [RegularExpression(@"^\d*(\.\d{1,2})?$", ErrorMessage = "Недопустимый формат")]
        public string UpPrice { get; set; }

        [Display(Name = "Время Upа")]
        public string UpTime { get; set; }
      
        public string[] DummyForumTimes { get; set; }
    }
}
