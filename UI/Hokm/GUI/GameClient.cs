using Hokm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Hokm
{
    public partial class GameClient : Form
    {
        private Card[] deck = new Card[13];
        private Card[] pDeck0 = new Card[13];
        private Card[] pDeck1 = new Card[13];
        private Card[] pDeck2 = new Card[13];
        private Card[][] playerDecks = new Card[3][];
        
        // 

        public PictureBox CardInitializer(Card c, int x, int y, bool rotate=false)
        {
            Random rnd = new Random();
            PictureBox cardBox = new PictureBox();
            if (rotate)
                c.RotateBMP();
            cardBox.Size = new Size(c.x, c.y);
            cardBox.Location = new Point(x + rnd.Next(-3, 3), y + rnd.Next(-3, 3));

            // Image
            cardBox.Image = c.GetBMP();
            cardBox.SizeMode = PictureBoxSizeMode.StretchImage;


            cardBox.Name = c.ToString();
            this.Controls.Add(cardBox);

            return cardBox;
        }

        // Game setup
        public void SetStartingDeck(string data)
        {
            int pFrom = 0;
            int pTo = data.LastIndexOf(",teams");

            string cardsString = data.Substring(pFrom, pTo - pFrom);

            string[] cards = cardsString.Split('|');


            for (int i = 0; i < cards.Length; i++)
            {
                string[] cardInfo = cards[i].Split('*');
                this.deck[i] = new Card(cardInfo[0], cardInfo[1]);
            }
        }

        public void SetOthersStartingDeck()
        {
            for (int i = 0; i < pDeck0.Length; i++)
            {
                string[] cardInfo = "back*back".Split('*');
                this.pDeck0[i] = new Card(cardInfo[0], cardInfo[1], true);
                this.pDeck1[i] = new Card(cardInfo[0], cardInfo[1], true);
                this.pDeck2[i] = new Card(cardInfo[0], cardInfo[1], true);
            }
        }

        public void StartingDeckVisuals()
        {
            int[] length = { 780, 240, 677 };
            int jump = (length[0] - length[1]) / this.deck.Length;


            foreach (Card c in this.deck)
            {
                CardInitializer(c, length[0], length[2]);
                length[0] -= jump;
            }
        }

        public void OthersStartingDeckVisuals(int player)
        {
            int[,] lengthAll = {
                {55, 55, 200, 770},
                {780, 240, 37, 37},
                {950, 950, 200, 770},
            };
            int[] length = { lengthAll[player, 0], lengthAll[player, 1], lengthAll[player, 2], lengthAll[player, 3] };
            Card[] d = this.playerDecks[player];
            int i = 0;
            if (player == 1)
            {
                int jump = (length[0] - length[1]) / 13;

                foreach (Card c in d)
                {
                    PictureBox cardBox = CardInitializer(c, length[0], length[2], false);
                    length[0] -= jump;

                    cardBox.Name = player.ToString() + "EP" + i.ToString();
                    i++;
                    this.Controls.Add(cardBox);
                }
            }
            else
            {
                int jump = (length[2] - length[3]) / 18;

                foreach (Card c in d)
                {
                    PictureBox cardBox = CardInitializer(c, length[0], length[2], true);
                    length[2] -= jump;

                    cardBox.Name = player.ToString() + "EP" + i.ToString();
                    i++;
                    this.Controls.Add(cardBox);
                }
            }
        }

        public void StartInitializer(string clientID, string rulerID, string startData)
        {
            DataAnalyzer dA = new DataAnalyzer(clientID, rulerID, startData);

            this.playerDecks[0] = this.pDeck0;
            this.playerDecks[1] = this.pDeck1;
            this.playerDecks[2] = this.pDeck2;


            // Set hokm
            info_text.Text = "Hokm: " + dA.GetStrong();

            // Set teams
            string[] teams = dA.GetTeams();
            score_text.Text = teams[0] + "\n" + teams[1];

            //Set decks
            SetStartingDeck(dA.ClearString(startData));
            SetOthersStartingDeck();

            //Visuals
            StartingDeckVisuals();

            //Others
            OthersStartingDeckVisuals(1);
            OthersStartingDeckVisuals(2);
            OthersStartingDeckVisuals(0);

        }


        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        // Actions
        // Me

        public void UpdateMyDeck(string played)
        {
            string[] cardInfo = played.Split('*');
            Card remove = new Card(cardInfo[0], cardInfo[1]);
            Card[] l = new Card[this.deck.Length-1];
            int j = 0;
            for (int i = 0; i < this.deck.Length; i++)
            {

                if (this.deck[i].ToString() != remove.ToString())
                {
                    l[j] = this.deck[i];
                    Console.WriteLine(j);
                    j++;
                }
            }
            Console.WriteLine(this.deck);
        }

        public void MyCardToMiddle(string played)
        {
            string[] cardInfo = played.Split('*');
            Card p = new Card(cardInfo[0], cardInfo[1]);
            

            this.Controls[p.ToString()].Location = new Point(530, 470);
        }

        public void PlayCard(string played)
        {
            UpdateMyDeck(played);
            MyCardToMiddle(played);
        }

        // Others
        public int UpdateOthersDeck(string played, int player)
        {
            Card[] l = new Card[this.playerDecks[player].Length-1];
            int j = 0;
            Random rnd = new Random();
            int k = rnd.Next(0, l.Length);
            for (int i = 0; i < this.playerDecks[player].Length; i++)
            {

                if (i != k)
                {
                    l[j] = this.playerDecks[player][i];
                    Console.WriteLine(j);
                    j++;
                }
            }
            Console.WriteLine(this.playerDecks[player]);
            return k;
        }

        public void UpdateCardTexture(PictureBox p, string played)
        {
            string[] cardInfo = played.Split('*');
            p.Image = (Bitmap)Resources.ResourceManager.GetObject(cardInfo[1] + "_of_" + cardInfo[0]);
            p.Size = new Size(100, 160);
        }

        public void OthersCardToMiddle(string played, int player, int k)
        {
            string n = player.ToString() + "EP" + k.ToString();
            PictureBox p = (PictureBox)this.Controls[n];
            if (player == 0)
                p.Location = new System.Drawing.Point(400, 350);
            else if (player == 1)
                p.Location = new System.Drawing.Point(530, 230);
            else
                p.Location = new System.Drawing.Point(660, 350);

            UpdateCardTexture(p, played);

        }


        public void PlayOtherCard(string played, int player)
        {
            int k = UpdateOthersDeck(played, player);
            OthersCardToMiddle(played, player, k);
        }


        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        #region TRASH

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
                Card[] l = new Card[this.deck.Length];
                int j = 0;
                for (int i = 0; i<this.deck.Length; i++)
                {
                    Console.WriteLine(this.deck[i]);
                    Console.WriteLine(remove);

                    if (this.deck[i].ToString() != remove.ToString())
                    {
                        l[j] = this.deck[i];
                        Console.WriteLine(j);
                        j++;
                    }
                    else
                    {
                        Console.WriteLine("FOUND!!!!!!!!!!!!!!!!!");
                    }
                }
                this.deck = l.Where(c => c != null).ToArray(); ;
                Console.WriteLine(this.deck);
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

            foreach (Card c in this.deck)
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

                foreach (Card c in this.deck)
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

                foreach (Card c in this.deck)
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
#endregion


        public GameClient()
        {
            InitializeComponent();

            string startData = "clubs*2|diamonds*2|spades*3|hearts*4|spades*ace|clubs*jack|hearts*7|spades*8|diamonds*9" +
                "|clubs*king|clubs*ace|spades*2|hearts*8,teams:[1+3]|[2+4],strong:hearts";


            StartInitializer("1", "2", startData);

            // GAME
            PlayCard("clubs*king");

            PlayOtherCard("clubs*2", 0);
            PlayOtherCard("spades*3", 1);
            PlayOtherCard("hearts*4", 2);

            /*
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

            

            */

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
