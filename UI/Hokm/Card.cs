using System;
using System.Collections.Generic;
using System.Text;

namespace Hokm
{
    internal class Card
    {
        public string shape { get; set; }
        public string value { get; set; }
        public Card(string shape, string value)
        {
            this.shape = shape;
            this.value = value;
        }
        public override string ToString()
        {
            return "shape: " + shape + ", value: " + value;
        }
    }
}
