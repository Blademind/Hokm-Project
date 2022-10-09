using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Hokm
{
    public partial class GameClient : Form
    {
        private Dictionary<Card, PictureBox> dict = new Dictionary<Card, PictureBox>();

        
        public GameClient()
        {
            InitializeComponent();
            List<char> list = new List<char>() { '♠', '♥', '♦', '♣' };
            Random rand = new Random();

            String txt = info_text.Text;
            int pFrom = txt.IndexOf("Hokm: ");
            info_text.Text = txt.Replace(txt.Substring(pFrom), "Hokm: " + list[rand.Next(0, 5)]);

            string dir = @"D:\Doron\עבודות יב\ערן\HOKM\Hokm-Project\UI\Hokm\Cards";
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
}
