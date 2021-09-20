using System;

namespace AutoUp.Models
{
    public class Ticker
    {
        public string @base { get; set;}
        public string target { get; set; }
        public string price { get; set; }
        public string volume { get; set; }
        public string change { get; set; }

    }
}
