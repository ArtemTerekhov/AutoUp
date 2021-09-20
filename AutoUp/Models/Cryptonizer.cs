using System;

namespace AutoUp.Models
{
    public class Cryptonizer
    {
        public Ticker ticker { get; set; }
        public int timestamp { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
    }
}
