using System.ComponentModel.DataAnnotations;

namespace AutoUp.ViewModels
{
    public class PaymentsViewModel
    {
        [Display(Name = "Сумма пополнения счета в Евро")]
        [RegularExpression(@"^\d*(\.\d{1,2})?$", ErrorMessage = "Недопустимый формат")]
        public string Amount { get; set; }
    }
}
