using Hokm.GUI;
using Hokm.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hokm
{
    public partial class GameClient : Form
    {
        private bool animations = true;

        private Card[] deck = new Card[13];
        private Card[] pDeck0 = new Card[13];
        private Card[] pDeck1 = new Card[13];
        private Card[] pDeck2 = new Card[13];
        private Card[][] playerDecks = new Card[3][];
        private PictureBox[] activeCards = new PictureBox[5];
        private DataAnalyzer dA = new DataAnalyzer();
        private AnimationHandler aH;
        private int roundN = 0;
        private bool firstTime = true;
        private Dictionary<string, int> scores = new Dictionary<string, int>();
        private string[] teams;

        // 

        private PictureBox CardInitializer(Card c, int x, int y, bool rot = false)
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

        private void ShowPanels(Control cc, bool show = true)
        {
            if (show)
            {
                cc.Visible = show;
                foreach (Control c in cc.Controls)
                {
                    c.Visible = show;
                }
            }
            else
            {
                foreach (Control c in cc.Controls)
                {
                    c.Visible = show;
                }
                cc.Visible = show;

            }
        }

        public void PopUpMessage(string title, string info, int mil)
        {
            round_title.Text = title;
            winner_label.Text = info;

            if (title.Length > 17)
                round_title.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            else
                round_title.Font = new Font("Segoe UI", 25F, FontStyle.Regular, GraphicsUnit.Point);

            if (info.Length > 22)
                winner_label.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            else
                winner_label.Font = new Font("Segoe UI", 21F, FontStyle.Regular, GraphicsUnit.Point);


            this.Invoke(new Action<int>((int _) => { ShowPanels(this.winning_panel); }), 0);

            Task.Delay(2300).ContinueWith(t => this.Invoke(new Action<int>((int _) => {
                ShowPanels(this.winning_panel, false);
            }), 0));
        }


        // Game setup
        private void FirstFiveCardsVisuals(Card[] cards)
        {
            int[] loc = { 310, 500 };
            foreach (Card c in cards)
            {
                Console.WriteLine(c);
                CardInitializer(c, loc[0], loc[1]);
                loc[0] += 115;
            }
        }

        public void FirstFiveCards(string cardsString)
        {
            string[] cards = cardsString.Split('|');
            Array.Sort(cards, StringComparer.InvariantCulture);
            Card[] fiveDeck = new Card[cards.Length];

            ShowPanels(this.ending_panel, false);
            ShowPanels(this.winning_panel, false);

            for (int i = 0; i < cards.Length; i++)
            {
                string[] cardInfo = cards[i].Split('*');

                fiveDeck[i] = new Card(cardInfo[0], cardInfo[1]);
            }
            FirstFiveCardsVisuals(fiveDeck);
        }

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

        private void StartInitializer(string startData, string clientID, string rulerID)
        {
            this.aH = new AnimationHandler(this);

            this.playerDecks[0] = this.pDeck0;
            this.playerDecks[1] = this.pDeck1;
            this.playerDecks[2] = this.pDeck2;

            dA.SetClientID(clientID);
            dA.SetRulerID(rulerID);
            dA.SetStartData(startData);

            // Set hokm+hakem
            info_text.ForeColor = SystemColors.ControlText;
            info_text.Text = "Hokm: " + dA.GetStrong() + "\n" + "Hakem: " + dA.GetRuler();

            // Set IDs on screen
            this.p_id_0.Text = dA.GetClientID() + " - You";
            this.p_id_1.Text = dA.GetFakeID(1);
            this.p_id_2.Text = dA.GetFakeID(0);
            this.p_id_3.Text = dA.GetFakeID(2);

            // Set teams
            this.teams = dA.GetTeams();
            score_text.ForeColor = SystemColors.ControlText;
            score_text.Text = teams[0] + ": 0" + "\n" + teams[1] + ": 0";
            scores.Add(teams[0], 0);
            scores.Add(teams[1], 0);

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

            this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer,
                    true);
            this.UpdateStyles();
        }

        public void PublicStartInitializer(string startData, string clientID, string rulerID)
        {
            this.Invoke(new Action<int>((int _) => { RemoveAllCards(); }), 0);
            this.Invoke(new Action<int>((int _) => { StartInitializer(startData, clientID, rulerID); }), 0);

        }

        ///////////////////////////////////////////////////////////////////////////////

        // Play Actions
        // Me

        private void UpdateMyDeck(string played)
        {
            string[] cardInfo = played.Split('*');
            Card remove = new Card(cardInfo[0], cardInfo[1]);
            Card[] l = new Card[this.deck.Length - 1];
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


            //this.Controls[p.ToString()].Location = new Point(530, 470);
            int[] cords = { 530, 470 };

            if (this.animations)
                this.Invoke(new Action<int>((int _) => { aH.AnimateCard(cords, (PictureBox)this.Controls[p.ToString()]); }), 0);
            else
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
            Card[] l = new Card[this.playerDecks[player].Length - 1];
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

            if (this.animations)
            {
                int[] cords = new int[2];

                if (player == 0)
                {
                    cords[0] = 403;
                    cords[1] = 350;
                }
                else if (player == 1)
                {
                    cords[0] = 530;
                    cords[1] = 230;
                }
                else
                {
                    cords[0] = 660;
                    cords[1] = 350;
                }

                this.Invoke(new Action<int>((int _) => { aH.AnimateCard(cords, p); }), 0);
            }
            else
            {
                if (player == 0)
                    p.Location = new System.Drawing.Point(403, 350);
                else if (player == 1)
                    p.Location = new System.Drawing.Point(530, 230);
                else
                    p.Location = new System.Drawing.Point(660, 350);
            }
            UpdateCardTexture(p, played);
            this.activeCards[player + 1] = (PictureBox)this.Controls[n];


        }

        public void PlayOtherCard(string played, int player)
        {
            player = dA.GetRealPlayerID(player.ToString());
            int k = UpdateOthersDeck(played, player);
            this.Invoke(new Action<int>((int _) => { OthersCardToMiddle(played, player, k); }), 0);
        }


        ///////////////////////////////////////////////////////////////////////////////

        // Round Over

        public void RemoveMiddleCards()
        {
            void Re()
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
            this.Invoke(new Action<int>((int _) => { Re(); }), 0);

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

        private void ScorePanelText(string winnerTeam)
        {
            this.scores[winnerTeam] += 1;

            this.score_text.Text = teams[0] + ": " + this.scores[teams[0]]
                + "\n" + teams[1] + ": " + this.scores[teams[1]];

            this.Invoke(new Action<int>((int _) => {
                PopUpMessage("End of Round " + this.roundN.ToString(), "Winner: " + winnerTeam, 2300);
            }), 0);
        }

        public void RoundEnding(string team)
        {
            this.Invoke(new Action<int>((int _) => {
                this.roundN++;
                this.winner_label.Text = "Winner: " + team;
                this.round_title.Text = "End of Round: " + this.roundN.ToString();
            }), 0);

            this.Invoke(new Action<int>((int _) => { ScorePanelText(team); }), 0);

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
            foreach (Control c in this.ending_panel.Controls)
            {
                c.Visible = true;
                if (c.Name == "ending_winner")
                {
                    c.Text = "Winner: " + winner;
                }
            }
        }

        private void exit_but_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void GameOver(string gWinner = null)
        {
            if (gWinner == null)
            {
                string[] t = this.score_text.Text.Split("\n");
                if (t[0].Contains("7"))
                {
                    gWinner = t[0].Remove(t[0].Length - 3);
                }
                else if (t[1].Contains("7"))
                {
                    gWinner = t[1].Remove(t[1].Length - 3);
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
                for (int i = 0; i < this.deck.Length; i++)
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
            }
            MyCardToMiddle(played);
            UpdateMyCardsVisuals();
        }

        public void UpdateMyCardsVisuals()
        {
            int[] sizes = { 100, 160 };
            int[] length = { 780, 240, 677 };
            int jump = (length[0] - length[1]) / 13;

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

        public GameClient(string startData = null, string clientID = null, string ruler = null)
        {
            InitializeComponent();
            StartInitializer(clientID, ruler, startData);
        }

        public GameClient(string fCards)
        {
            InitializeComponent();
            FirstFiveCards(fCards);
        }


        // Tests
        private void button1_Click(object sender, EventArgs e)
        {
            RoundEnding("1+2");
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

        private void round_title_Click(object sender, EventArgs e)
        {

        }

        private void info_text_Click(object sender, EventArgs e)
        {

        }
    }
}
