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
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Timer = System.Windows.Forms.Timer;

namespace Hokm
{
    public partial class GameClient : Form
    {
        private Card[] deck = new Card[13];
        private Card[] pDeck0 = new Card[13];
        private Card[] pDeck1 = new Card[13];
        private Card[] pDeck2 = new Card[13];
        private Card[][] playerDecks = new Card[3][];
        private PictureBox[] activeCards = new PictureBox[5];
        private DataAnalyzer dA = new DataAnalyzer();
        private int roundN = 0;
        private bool firstTime = true;
        // 

        private PictureBox CardInitializer(Card c, int x, int y, bool rot=false)
        {
            Random rnd = new Random();
            PictureBox cardBox = new PictureBox();
            if (this.firstTime && rot)
            {
                c.RotateBMP();
            }
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
        private void SetStartingDeck(string data)
        {
            int pFrom = 0;
            int pTo = data.LastIndexOf(",teams");

            string cardsString = data.Substring(pFrom, pTo - pFrom);

            string[] cards = cardsString.Split('|');
            Array.Sort(cards, StringComparer.InvariantCulture);

            for (int i = 0; i < cards.Length; i++)
            {
                string[] cardInfo = cards[i].Split('*');

                this.deck[i] = new Card(cardInfo[0], cardInfo[1]);
            }
        }

        private void SetOthersStartingDeck()
        {
            for (int i = 0; i < pDeck0.Length; i++)
            {
                string[] cardInfo = "back*back".Split('*');
                this.pDeck0[i] = new Card(cardInfo[0], cardInfo[1], true);
                this.pDeck1[i] = new Card(cardInfo[0], cardInfo[1], true);
                this.pDeck2[i] = new Card(cardInfo[0], cardInfo[1], true);
            }
        }

        private void StartingDeckVisuals()
        {
            int[] length = { 780, 240, 677 };
            int jump = (length[0] - length[1]) / this.deck.Length;


            foreach (Card c in this.deck)
            {
                CardInitializer(c, length[0], length[2]);
                length[0] -= jump;
            }
        }

        private void OthersStartingDeckVisuals(int player)
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

        private void ShowPanels(Control cc, bool show=true)
        {
            cc.Visible = show;
            foreach (Control c in cc.Controls)
            {
                c.Visible = show;
            }
        }

        public void StartInitializer(string clientID, string rulerID, string startData)
        {

            this.playerDecks[0] = this.pDeck0;
            this.playerDecks[1] = this.pDeck1;
            this.playerDecks[2] = this.pDeck2;

            dA.SetClientID(clientID);
            dA.SetRulerID(rulerID);
            dA.SetStartData(startData);

            // Set hokm+hakem
            info_text.Text = info_text.Text.Replace("hokm_card", dA.GetStrong());
            info_text.Text = info_text.Text.Replace("ruler_id", dA.GetRuler());

            // Set IDs on screen
            this.p_id_0.Text = dA.GetClientID();
            this.p_id_1.Text = dA.GetFakeID(1);
            this.p_id_2.Text = dA.GetFakeID(0);
            this.p_id_3.Text = dA.GetFakeID(2);

            // Set teams
            string[] teams = dA.GetTeams();
            score_text.Text = teams[0] + ": 0" + "\n" + teams[1] + ": 0";

            //Set decks
            SetStartingDeck(dA.ClearString(startData));
            SetOthersStartingDeck();

            //Visuals
            StartingDeckVisuals();

            //Others
            OthersStartingDeckVisuals(1);
            OthersStartingDeckVisuals(2);
            OthersStartingDeckVisuals(0);
            this.firstTime = false;

            ShowPanels(this.ending_panel, false);
            ShowPanels(this.winning_panel, false);

        }


        ///////////////////////////////////////////////////////////////////////////////

        // Play Actions
        // Me

        private void UpdateMyDeck(string played)
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
                    j++;
                }
            }
            this.deck = l;
        }

        private void MyCardToMiddle(string played)
        {
            string[] cardInfo = played.Split('*');
            Card p = new Card(cardInfo[0], cardInfo[1]);
            

            this.Controls[p.ToString()].Location = new Point(530, 470);
            this.activeCards[0] = (PictureBox)this.Controls[p.ToString()];
        }

        public void PlayCard(string played)
        {
            this.Invoke(new Action<int>((int _) => { UpdateMyDeck(played); }), 0);
            this.Invoke(new Action<int>((int _) => { MyCardToMiddle(played); }), 0);
            this.Invoke(new Action<int>((int _) => { this.ResumeLayout(false); }), 0);
        }

        // Others
        private int UpdateOthersDeck(string played, int player)
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
                    j++;
                }
            }
            this.playerDecks[player] = l;
            Console.WriteLine(this.playerDecks[player]);
            return k;
        }

        private void UpdateCardTexture(PictureBox p, string played)
        {
            string[] cardInfo = played.Split('*');
            Card c = new Card(cardInfo[0], cardInfo[1]);
            p.Image = (Bitmap)Resources.ResourceManager.GetObject(c.value + "_of_" + c.shape);
            p.Size = new Size(100, 160);
        }

        private void OthersCardToMiddle(string played, int player, int k)
        {
            PictureBox p = null;
            string n = "";
            int d = 0;
            while (p == null)
            {

                n = player.ToString() + "EP" + k.ToString();
                p = (PictureBox)this.Controls[n];
                d++;
                k = d;
            }
            if (player == 0)
                p.Location = new System.Drawing.Point(403, 350);
            else if (player == 1)
                p.Location = new System.Drawing.Point(530, 230);
            else
                p.Location = new System.Drawing.Point(660, 350);
            UpdateCardTexture(p, played);
            this.activeCards[player + 1] = (PictureBox)this.Controls[n];


        }

        public void PlayOtherCard(string played, int player)
        {
            player = dA.GetRealPlayerID(player.ToString());
            int k = UpdateOthersDeck(played, player);
            this.Invoke(new Action<int>((int _) => { OthersCardToMiddle(played, player, k);}), 0);         
        }


        ///////////////////////////////////////////////////////////////////////////////

        // Round Over

        public void RemoveMiddleCards()
        {
            void Lol()
            {
                foreach (PictureBox p in this.Controls.OfType<PictureBox>().ToList())
                {
                    if (p.Location == new Point(403, 350) ||
                        p.Location == new Point(530, 230) ||
                        p.Location == new Point(660, 350) ||
                        p.Location == new Point(530, 470))
                    {
                        this.Controls.Remove(p);
                    }

                }
            }
            this.Invoke(new Action<int>((int _) => { Lol(); }), 0);


            //for (int i = 0; i < this.activeCards.Length; i++)
            //{
            //    Console.WriteLine(">>>>>>>>>>>" + activeCards[i]);
            //    this.Controls.Remove(activeCards[i]);
            //}            
        }

        private void RefreshCards()
        {
            foreach (PictureBox p in this.Controls.OfType<PictureBox>().ToList())
            {
                this.Controls.Remove(p);
                p.Dispose();
            }
            //Visuals
            StartingDeckVisuals();

            //Others
            OthersStartingDeckVisuals(1);
            OthersStartingDeckVisuals(2);
            OthersStartingDeckVisuals(0);
        }

        private void EditScorePanel(string winner)
        {
            string[] t = this.score_text.Text.Split("\n");
            if (t[0].Contains(winner))
            {
                string s1 = t[0].Split(": ")[0];
                string s2 = t[0].Split(": ")[1];
                this.score_text.Text = this.score_text.Text.Replace(t[0], s1+": " + (int.Parse(s2) + 1).ToString());
            }
            else
            {
                string s1 = t[1].Split(": ")[0];
                string s2 = t[1].Split(": ")[1];
                this.score_text.Text = this.score_text.Text.Replace(t[1], s1+ ": " + (int.Parse(s2) + 1).ToString());
            }
        }

        public void RoundEnding(string winner)
        {
            var t = new Timer();
            //t.Interval = 10;
            Console.WriteLine("DELETING!");

            //t.Tick += (s, e) => { this.Invoke(new Action<int>((int _) => { RemoveMiddleCards(); }), 0); };
            //t.Start();
            
            this.roundN++;
            this.winner_label.Text = "Winner: " + winner;
            this.round_title.Text = "End of Round: " + this.roundN.ToString();
            this.Invoke(new Action<int>((int _) => { EditScorePanel(winner); }), 0);
            //this.Invoke(new Action<int>((int _) => { ShowPanels(this.winning_panel); }), 0);
            //var t = new Timer();
            //t.Interval = 3; // will tick in 2.4 seconds
            //t.Tick += (s, e) =>
            //{
            //    ShowPanels(this.winning_panel, false);
            //};
            //t.Start();
        }

        // Game Over

        private void RemoveAllCards()
        {
            foreach (PictureBox p in this.Controls.OfType<PictureBox>().ToList())
            {
                this.Controls.Remove(p);
                p.Dispose();
            }
        }

        private void EndingScreen(string winner)
        {
            this.ending_panel.Visible = true;
            foreach(Control c in this.ending_panel.Controls)
            {
                c.Visible = true;
                if (c.Name == "ending_winner")
                {
                    c.Text = "Winner: " + winner;
                }
            }
        }

        public void GameOver(string gWinner=null)
        {
            if (gWinner == null)
            {
                string[] t = this.score_text.Text.Split("\n");
                if (t[0].Contains("7"))
                {
                    gWinner = t[0].Remove(t[0].Length - 1);
                }
                else if (t[1].Contains("7"))
                {
                    gWinner = t[1].Remove(t[1].Length - 1);
                }
                else
                {
                    gWinner = "erorrr";
                }
            }

            this.Invoke(new Action<int>((int _) => { RemoveAllCards(); }), 0);
            this.Invoke(new Action<int>((int _) => { EndingScreen(gWinner); }), 0);
            // exit button
        }
        
        #region TRASH

        //                                                                                      .     
        ///////***************************///////////                                ||           \
        ///////////////////////////////////////////////////////////////////////////////          /
        ///// *********           **********           **********      //////////////##            |///////>
        ///////////////////////////////////////////////////////////////////////////////           \
        /////////////////////////////////------------///                                        /
        ////   \\(     //////////
        ////   \\(     ///////                                                                       
        ////   \(      /////
        ////   \((     ////
        ////     \\\(  ///
        ////          //
        ///////////////
         ////////////

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
                        j++;
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

        public GameClient(string startData=null, string clientID=null, string ruler=null)
        {
            InitializeComponent();

            if (startData == null)
                 startData = "clubs*rank_2|diamonds*rank_2|spades*rank_3|hearts*rank_4|" +
                    "spades*rank_A|clubs*rank_J|hearts*rank_7|spades*rank_8|diamonds*rank_9" +
                    "|clubs*rank_K|clubs*rank_A|spades*rank_2|hearts*rank_8,teams:[1+3]|[2+4],strong:hearts";
            if (clientID == null)
                clientID = "4";
            if (ruler == null)
                ruler = "1";
            StartInitializer(clientID, ruler, startData);

        }

        // Tests
        private void button1_Click(object sender, EventArgs e)
        {
            RoundEnding("1");
        }

        private void GameClient_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlayCard("spades*rank_8");
            PlayOtherCard("clubs*rank_2", 3);
            PlayOtherCard("spades*rank_2", 2);
            PlayOtherCard("hearts*rank_2", 1);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Enter VALuE: ");
            string x = Console.ReadLine();
            PlayCard(x);
            PlayOtherCard("clubs*rank_3", 3);
            PlayOtherCard("spades*rank_3", 2);
            PlayOtherCard("hearts*rank_3", 1);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            GameOver("2+4");
        }
    }
}
