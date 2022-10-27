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
            ///<summary>
            /// Initializing the class
            /// <param name="shape">The shape of the card</param>
            /// <param name="value">The value of the card</param>
            /// <param name="back">If the card's value is unkown, it would be considers as 'back' as it's texture</param>
            /// <return> None </return>
            ///</summary>

            // Saftey net
            this.value = value.ToLower();
            this.shape = shape.ToLower();

            // Make it easier to reach to the texture
            this.value = this.value.Replace("rank_j", "jack");
            this.value = this.value.Replace("rank_q", "queen");
            this.value = this.value.Replace("rank_k", "king");
            this.value = this.value.Replace("rank_a", "ace");
            this.value = this.value.Replace("rank_", "");

            this.x = 100;
            this.y = 160;

            if (!back)
            {
                Console.WriteLine(this.value + "_of_" + this.shape);
                this.bmp = (Bitmap)Resources.ResourceManager.GetObject(this.value + "_of_" + this.shape);
            }
            else
            {
                this.bmp = (Bitmap)Resources.ResourceManager.GetObject("back");
            }

        }


        public void RotateBMP()
        {
            ///<summary>
            /// Rotates the card's texture
            /// <return> None </return>
            ///</summary>
            
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
