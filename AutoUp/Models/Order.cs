using System;

namespace AutoUp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public float PriceAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
