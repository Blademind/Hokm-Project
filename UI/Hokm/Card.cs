using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Hokm.Properties;
using System.Security.Policy;

namespace Hokm
{
    public class Card
    {
        public string shape { get; set; }
        public string value { get; set; }

        public int x, y;

        public Bitmap bmp { get; set; }

        public Card(string shape, string value, bool back=false)
        {
            this.shape = shape;
            this.value = value;
            this.x = 100;
            this.y = 160;

            if (!back)
            {
                this.bmp = (Bitmap)Resources.ResourceManager.GetObject(this.value + "_of_" + this.shape);
            }
            else
            {
                this.bmp = (Bitmap)Resources.ResourceManager.GetObject("back");
            }

        }


        public void RotateBMP()
        {
            this.bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.x = 160;
            this.y = 100;
        }

        public Bitmap GetBMP()
        {
            return this.bmp;
        }

        public override string ToString()
        {
            return "shape: " + shape + ", value: " + value;
        }

    }
}
