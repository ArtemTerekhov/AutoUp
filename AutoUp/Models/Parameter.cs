namespace AutoUp.Models
{
    public class Parameter
    {
        public int Id { get; set; }
        public string PriceCurrency { get; set; } 
        public string ReceiveCurrency { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; } 
        public string CallbackUrl { get; set; } 
        public string SuccessUrl { get; set; } 
        public string CancelUrl { get; set; } 
        public string Token { get; set; } 
        public string AuthToken { get; set; }

    }
}
