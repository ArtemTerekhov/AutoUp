namespace AutoUp.Models
{
    public class CheckoutResponse
    {
        public int id { get; set; }
        public string status { get; set; }
        public bool do_not_convert { get; set; }
        public string price_currency { get; set; }
        public string price_amount { get; set; }
        public string pay_currency { get; set; }
        public string pay_amount { get; set; }
        public bool lightning_network { get; set; }
        public string receive_currency { get; set; }
        public string receive_amount { get; set; }
        public string created_at { get; set; }
        public string expire_at { get; set; }
        public string payment_address { get; set; }
        public string order_id { get; set; }
        public string payment_url { get; set; }
        public string underpaid_amount { get; set; }
        public string overpaid_amount { get; set; }
        public bool is_refundable { get; set; }
    }
}
