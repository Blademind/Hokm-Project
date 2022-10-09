using Hokm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Hokm
{
    public partial class GameClient : Form
    {
        private Dictionary<Card, PictureBox> dict = new Dictionary<Card, PictureBox>();
        private Card[] hand = new Card[13];
        int[] enemies_cards = { 13, 13, 13 };
        
        
        public string ClearString(string data)
        {
            List<string> charsToRemove = new List<string>() { "[", "]"};
            foreach (var c in charsToRemove)
            {
                data = data.Replace(c, string.Empty);
            }
            return data.ToLower();
        }

        public string GetStrong(string data)
        {
            List<string> list = new List<string>() { "♠", "♣", "♦", "♥️" };
            int from = data.IndexOf("strong:") + "strong:".Length;
            string strng = ClearString(data.Substring(from));

            switch (strng)
            {
                case "spades":
                    return list[0];
                case "clubs":
                    return list[1];
                case "diamonds":
                    return list[2];
                case "hearts":
                    return list[3];
            }
            return strng;
        }

        public string[] GetTeams(string data)
        {
            int pFrom = data.IndexOf(",teams:") + ",teams:".Length;
            int pTo = data.LastIndexOf(",strong");

            string teamsString = ClearString(data.Substring(pFrom, pTo - pFrom));

            string[] teams = teamsString.Split('|');

            return teams;
        }

        public void SetHand(string data)
        {
            int pFrom = 0;
            int pTo = data.LastIndexOf(",teams");

            string cardsString = ClearString(data.Substring(pFrom, pTo - pFrom));

            string[] cards = cardsString.Split('|');


            for (int i=0; i< cards.Length; i++)
            {
                string[] cardInfo = cards[i].Split('*');
                this.hand[i] = new Card(cardInfo[0], cardInfo[1]);
            }
        }

        public void MyCardToMiddle(string played)
        {
            if (played != "None")
            {
                string[] cardInfo = played.Split('*');
                Card p = new Card(cardInfo[0], cardInfo[1]);
                this.Controls[p.ToString()].Location = new System.Drawing.Point(530, 470);               

            }
        }

        public void EnemiesCardToMiddle(string played, int enemy)
        {
            if (played != "None")
            {
                //string[] cardInfo = played.Split('*');
                //Card p = new Card(cardInfo[0], cardInfo[1]);
                string n = enemy.ToString() + "EP";

                if (enemy == 0)
                    this.Controls[n].Location = new System.Drawing.Point(340, 380);
                else if (enemy == 1)
                    this.Controls[n].Location = new System.Drawing.Point(530, 230);
                else
                    this.Controls[n].Location = new System.Drawing.Point(660, 380);

            }
        }

        // My Cards
        public void UpdateMyCards(string played)
        {
            if (played != "None")
            {
                string[] cardInfo = played.Split('*');
                Card remove = new Card(cardInfo[0], cardInfo[1]);
                Card[] l = new Card[this.hand.Length];
                int j = 0;
                for (int i = 0; i<this.hand.Length; i++)
                {
                    Console.WriteLine(this.hand[i]);
                    Console.WriteLine(remove);

                    if (this.hand[i].ToString() != remove.ToString())
                    {
                        l[j] = this.hand[i];
                        Console.WriteLine(j);
                        j++;
                    }
                    else
                    {
                        Console.WriteLine("FOUND!!!!!!!!!!!!!!!!!");
                    }
                }
                this.hand = l.Where(c => c != null).ToArray(); ;
                Console.WriteLine(this.hand);
            }
            MyCardToMiddle(played);
            UpdateMyCardsVisuals();
        }

        public void UpdateMyCardsVisuals()
        {
            int[] sizes = {100, 160};
            int[] length = {780, 240, 677 };
            int jump = (length[0] - length[1]) / 13;

            string dir = @"D:\Doron\עבודות יב\ערן\HOKM\Hokm-Project\UI\Hokm\Cards";
            string[] files = Directory.GetFiles(dir, "*.png");

            Random rnd = new Random();

            foreach (Card c in this.hand)
            {
                    Console.WriteLine(c);
                    PictureBox cardBox = new PictureBox();
                    cardBox.Size = new System.Drawing.Size(sizes[0], sizes[1]);
                    cardBox.Location = new System.Drawing.Point(length[0], length[2]);

                    // Image
                    string one = c.value;
                    string two = c.shape;
                    Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(one + "_of_" + two);
                    cardBox.Image = bmp;

                    cardBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    cardBox.Name = c.ToString();
                    length[0] -= jump;
                    this.Controls.Add(cardBox);
            }
        }

        // Enemies
        public void UpdateEnemiesCards(int enemy)
        {
            this.enemies_cards[enemy]--;
            
        }

        public void UpdateEnemiesCardsVisuals(int enemy)
        {
            int[,] lengthAll = { 
                {55, 55, 200, 770},
                {780, 240, 37, 37},
                {950, 950, 200, 770},
            };
            int[] length = { lengthAll[enemy, 0], lengthAll[enemy, 1], lengthAll[enemy, 2], lengthAll[enemy, 3] };

            string dir = @"D:\Doron\עבודות יב\ערן\HOKM\Hokm-Project\UI\Hokm\Cards";
            string[] files = Directory.GetFiles(dir, "*.png");

            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject("back");
            Random rnd = new Random();

            if (enemy == 1)
            {
                int jump = (length[0] - length[1]) / 13;
                int[] sizes = { 100, 160 };

                foreach (Card c in this.hand)
                {
                    PictureBox cardBox = new PictureBox();
                    cardBox.Size = new System.Drawing.Size(sizes[0], sizes[1]);
                    cardBox.Location = new System.Drawing.Point(length[0], length[2]);

                    // Image 
                    cardBox.Image = bmp;

                    cardBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    cardBox.Name = enemy.ToString() + "EP";
                    length[0] -= jump;
                    this.Controls.Add(cardBox);
                }
            }
            else
            {
                if (enemy == 0)
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipY);

                int[] sizes = { 160, 100 };

                int jump = (length[2] - length[3]) / 18;

                foreach (Card c in this.hand)
                {
                    PictureBox cardBox = new PictureBox();
                    cardBox.Size = new System.Drawing.Size(sizes[0], sizes[1]);
                    cardBox.Location = new System.Drawing.Point(length[0], length[2]);
                    // Image 
                    cardBox.Image = bmp;

                    cardBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    cardBox.Name = enemy.ToString() + "EP";
                    length[2] -= jump;
                    this.Controls.Add(cardBox);
                }
            }
        }

        public GameClient()
        {
            InitializeComponent();

            string startData = "clubs*2|diamonds*2|spades*3|hearts*4|spades*ace|clubs*jack|hearts*7|spades*8|diamonds*9" +
                "|clubs*king|clubs*ace|spades*2|hearts*8,teams:[team1]|[team2],strong:hearts";


            // Set hokm
            info_text.Text = "Hokm: " + GetStrong(startData);

            // Set teams
            string[] teams = GetTeams(startData);
            score_text.Text = teams[0] + "\n" + teams[1];

            //Set Hand
            SetHand(startData);

            //Visual
            UpdateMyCards("None");


            //Thread.Sleep(4000);

            //
            UpdateEnemiesCardsVisuals(0);
            UpdateEnemiesCardsVisuals(1);
            UpdateEnemiesCardsVisuals(2);

            Console.WriteLine("========");
            UpdateMyCards("clubs*king");
            Console.WriteLine("========");
            UpdateMyCards("clubs*ace");

            EnemiesCardToMiddle("a", 0);
            EnemiesCardToMiddle("a", 1);
            EnemiesCardToMiddle("a", 2);

            



            /*

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
            }*/
        }
    }
}
