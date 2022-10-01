using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Hokm
{
    public partial class Game : Form
    {
        private Dictionary<Card, PictureBox> dict = new Dictionary<Card, PictureBox>();

        public Game()
        {
            InitializeComponent();
            List<char> list = new List<char>() { '♠', '♥', '♦', '♣' };
            Random rand = new Random();
            label1.Text = "Hokm: " + list[rand.Next(0, 5)];

            string dir = @"C:\Users\alonl\OneDrive\מסמכים\Hokm-Project\UI\Hokm\Cards";
            string[] files = Directory.GetFiles(dir, "*.png");
            string name = "";

            foreach (var file in files)
            {
                name = file.Substring(file.LastIndexOf('\\') + 1);
                name = name.Substring(0, name.Length - 4);
                string[] cardShapeValue = name.Split("_");
                Card card = new Card(cardShapeValue[2], cardShapeValue[0]);
                Console.WriteLine(card);
            }
        }

    }
    class Card
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
