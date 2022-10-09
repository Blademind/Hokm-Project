using System;
using System.Collections.Generic;
using System.IO;
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

            string dir = @"D:\Doron\עבודות יב\ערן\HOKM\Hokm-Project\UI\Hokm\Cards";
            string[] files = Directory.GetFiles(dir, "*.png");
            string name = "";

            foreach (var file in files)
            {
                name = file.Substring(file.LastIndexOf('\\') + 1);
                name = name.Substring(0, name.Length - 4);
                string[] cardShapeValue = name.Split("_");
                Cardssss card = new Cardssss(cardShapeValue[2], cardShapeValue[0]);
                Console.WriteLine(card);
            }
        }

    }
    class Cardssss
    {
        public string shape { get; set; }
        public string value { get; set; }
        public Cardssss(string shape, string value)
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
