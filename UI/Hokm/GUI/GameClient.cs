using Hokm.GUI;
using Hokm.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hokm
{
    public partial class GameClient : Form
    {
        #region Variables
        /// <summary>
        /// Initiating variables 
        /// </summary>
        private bool animations = true;

        private Card[] deck = new Card[13];
        private Card[] pDeck0 = new Card[13];
        private Card[] pDeck1 = new Card[13];
        private Card[] pDeck2 = new Card[13];
        private Card[][] playerDecks = new Card[3][];
        private PictureBox[] activeCards = new PictureBox[5];

        private DataAnalyzer dA = new DataAnalyzer();
        private AnimationHandler aH;
        private Dictionary<string, int> scores = new Dictionary<string, int>();

        private int roundN = 0;
        private bool firstTime = true;
        private string[] teams;
        #endregion

        #region General
        private PictureBox CardInitializer(Card c, int x, int y, bool rot = false)
        {
            ///<summary>
            /// Initializes a card onscreen using the appropriate values 
            /// <param name="c">The Card to show on screen</param>
            /// <param name="x">the x value on screen</param>
            /// <param name="y">the y value on screen</param>
            /// <param name="rot">enable rotation</param>
            /// <return> The PictureBox of the card </return>
            ///</summary>

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
            cardBox.SizeMode = PictureBoxSizeMode.StretchImage;


            cardBox.Name = c.ToString();
            this.Controls.Add(cardBox);

            return cardBox;
        }

        private void ShowPanels(Control cc, bool show = true)
        {
            ///<summary>
            /// A simple function that shows or hides the control and its childrens
            /// <param name="cc">The control we want to hide</param>
            /// <param name="show">Either show or hide</param>
            /// <return> None </return>
            ///</summary>

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

        public void PopUpMessage(string title, string info, int mil=2300)
        {
            ///<summary>
            /// Shows a popup message to the user on top of the screen
            /// <param name="title">The title of the popup</param>
            /// <param name="info">The subtext of the popup</param>
            /// <param name="mil">The time duration of the popup to show</param>
            /// <return> None </return>
            ///</summary>
            
            round_title.Text = title;
            winner_label.Text = info;

            // In order to avoid overflow
            if (title.Length > 17)
                round_title.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            else
                round_title.Font = new Font("Segoe UI", 25F, FontStyle.Regular, GraphicsUnit.Point);

            if (info.Length > 22)
                winner_label.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            else
                winner_label.Font = new Font("Segoe UI", 21F, FontStyle.Regular, GraphicsUnit.Point);


            this.Invoke(new Action<int>((int _) => { ShowPanels(this.winning_panel); }), 0);

            Task.Delay(mil).ContinueWith(t => this.Invoke(new Action<int>((int _) => {
                ShowPanels(this.winning_panel, false);
            }), 0));
        }
        #endregion

        #region Game setup
        private void FirstFiveCardsVisuals(Card[] cards)
        {
            ///<summary>
            /// Shows the first 5 cards on screen
            /// <param name="cards">The array of the first five cards</param>
            /// <return> None </return>
            ///</summary>
            
            int[] loc = { 310, 500 };
            foreach (Card c in cards)
            {
                // Console.WriteLine(c);
                CardInitializer(c, loc[0], loc[1]);
                loc[0] += 115;
            }
        }

        public void FirstFiveCards(string cardsString)
        {
            ///<summary>
            /// Manages the first five cards at the start of the game
            /// <param name="cardsString">The message from the server that includes the 
            ///     first 5 cards</param>
            /// <return> None </return>
            ///</summary>

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
            ///<summary>
            /// Manages the starting deck of the player using a Card array
            /// <param name="data">The message from the server that includes the 
            ///     player's deck, hokm and hakm</param>
            /// <return> None </return>
            ///</summary>
            
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
            ///<summary>
            /// Manages the starting deck of the other players
            /// <return> None </return>
            ///</summary>
            
            for (int i = 0; i < pDeck0.Length; i++)
            {
                string[] cardInfo = "back*back".Split('*');
                this.pDeck0[i] = new Card(cardInfo[0], cardInfo[1], true);
                this.pDeck1[i] = new Card(cardInfo[0], cardInfo[1], true);
                this.pDeck2[i] = new Card(cardInfo[0], cardInfo[1], true);
            }
        }
        #endregion

        #region Deck visuals
        private void StartingDeckVisuals()
        {
            ///<summary>
            /// generates the player's deck screen
            /// <return> None </return>
            ///</summary>
            
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
            ///<summary>
            /// Generates the others player's deck screen by ID
            /// <param name="player">The ID of the player</param>
            /// <return> None </return>
            ///</summary>

            int[,] lengthAll = {
                {55, 55, 200, 770},
                {780, 240, 37, 37},
                {950, 950, 200, 770},
            };
            int[] length = { lengthAll[player, 0], lengthAll[player, 1], lengthAll[player, 2], lengthAll[player, 3] };
            Card[] d = this.playerDecks[player];
            int i = 0;
            // Every player ID, requires a different attributes for the card to be displayed correctly
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
            ///<summary>
            /// Setup for all the different and important arguments for the UI to work correctly
            /// <param name="startData">The raw message from the server</param>
            /// <param name="clientID">The user's ID</param>
            /// <param name="rulerID">The hakm's ID</param>
            /// <return> None </return>
            ///</summary>

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

            //Set ruler color
            if (dA.GetRuler() == dA.GetClientID())
                this.p_id_0.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            else if (dA.GetRuler() == dA.GetFakeID(1))
                this.p_id_1.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            else if (dA.GetRuler() == dA.GetFakeID(0))
                this.p_id_2.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            else
                this.p_id_3.ForeColor = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));

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
            
            // Avoid screen stuttering
            this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer,
                    true);
            this.UpdateStyles();
        }

        public void PublicStartInitializer(string startData, string clientID, string rulerID)
        {
            // Avoid threading issues
            this.Invoke(new Action<int>((int _) => { RemoveAllCards(); }), 0);
            this.Invoke(new Action<int>((int _) => { StartInitializer(startData, clientID, rulerID); }), 0);
        }

        #endregion

        #region Play Actions
        // Player

        private void UpdateMyDeck(string played)
        {
            ///<summary>
            /// When a player plays a card, the visuals and his deck needs to be update
            /// this function manages the operation
            /// <param name="played">The played card string</param>
            /// <return> None </return>
            ///</summary>


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
            ///<summary>
            /// Moves the desired card to the middle
            /// <param name="played">The played card</param>
            /// <return> None </return>
            ///</summary>

            string[] cardInfo = played.Split('*');
            Card p = new Card(cardInfo[0], cardInfo[1]);


            int[] cords = { 530, 470 };

            // move to known location
            if (this.animations)
                this.Invoke(new Action<int>((int _) => { aH.AnimateCard(cords, (PictureBox)this.Controls[p.ToString()]); }), 0);
            else
                this.Controls[p.ToString()].Location = new Point(530, 470);

            this.activeCards[0] = (PictureBox)this.Controls[p.ToString()];
        }

        public void PlayCard(string played)
        {
            ///<summary>
            /// Manages the logics and visuals when playing a card
            /// <param name="played">The played card</param>
            /// <return> None </return>
            ///</summary>

            this.Invoke(new Action<int>((int _) => { UpdateMyDeck(played); }), 0);
            this.Invoke(new Action<int>((int _) => { MyCardToMiddle(played); }), 0);
            this.Invoke(new Action<int>((int _) => { this.ResumeLayout(false); }), 0);
        }

        // Other players
        private int UpdateOthersDeck(string played, int player)
        {
            // When a player plays a card, the visuals and his deck needs to be update
            // and change the texture of the card 
            // this function manages the operation
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
            // Console.WriteLine(this.playerDecks[player]);
            return k;
        }

        private void UpdateCardTexture(PictureBox p, string played)
        {
            // Updates the card texture from the back to the front
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
            // Manages the logics and visuals when others play a card
            player = dA.GetRealPlayerID(player.ToString());
            int k = UpdateOthersDeck(played, player);
            this.Invoke(new Action<int>((int _) => { OthersCardToMiddle(played, player, k); }), 0);
        }
        #endregion

        #region Round Over

        public void RemoveMiddleCards()
        {
            ///<summary>
            /// Clears all the middle cards at the end of the round
            /// <return> None </return>
            ///</summary>
            void Re()
            {
                foreach (PictureBox p in this.Controls.OfType<PictureBox>().ToList())
                {
                    // Clears every card location in the middle in case of a bug
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

        private void ScorePanelText(string winnerTeam)
        {
            ///<summary>
            /// Manages the values and viewing of the scores panel
            /// <param name="winnerTeam">The winner team string</param>
            /// <return> None </return>
            ///</summary>
            
            this.scores[winnerTeam] += 1;

            this.score_text.Text = teams[0] + ": " + this.scores[teams[0]]
                + "\n" + teams[1] + ": " + this.scores[teams[1]];

            this.Invoke(new Action<int>((int _) => {
                PopUpMessage("End of Round " + this.roundN.ToString(), "Winner: " + winnerTeam, 2300);
            }), 0);
        }

        public void RoundEnding(string team)
        {
            ///<summary>
            /// Manages the logics and visuals when at the end of each round
            /// <param name="team">The winner team</param>
            /// <return> None </return>
            ///</summary>

            this.Invoke(new Action<int>((int _) => {
                this.roundN++;
                this.winner_label.Text = "Winner: " + team;
                this.round_title.Text = "End of Round: " + this.roundN.ToString();
            }), 0);

            this.Invoke(new Action<int>((int _) => { ScorePanelText(team); }), 0);

        }
        #endregion

        #region Game Over

        private void RemoveAllCards()
        {
            ///<summary>
            /// Removes every card onscreen
            /// <return> None </return>
            ///</summary>
            
            foreach (PictureBox p in this.Controls.OfType<PictureBox>().ToList())
            {
                this.Controls.Remove(p);
                p.Dispose();
            }
        }

        private void EndingScreen(string winner)
        {
            ///<summary>
            /// Enables the ending screen at the end of the game
            /// <param name="winner">The game winner team</param>
            /// <return> None </return>
            ///</summary>
            
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
            ///<summary>
            /// Manages the logics and visuals when at the end of the game
            /// <param name="gWinner">The game winner</param>
            /// <return> None </return>
            ///</summary>
            
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
                    gWinner = "ERORR?";
                }
            }

            //this.Invoke(new Action<int>((int _) => { RemoveAllCards(); }), 0);
            //this.Invoke(new Action<int>((int _) => { EndingScreen(gWinner); }), 0);
            //// exit button
            Task.Delay(2300).ContinueWith(t => this.Invoke(new Action<int>((int_ ) => { RemoveAllCards(); }), 0));
            Task.Delay(2300).ContinueWith(t => this.Invoke(new Action<int>((int_ ) => { EndingScreen(gWinner); }), 0));
        }

        #endregion

        public GameClient(string startData = null, string clientID = null, string ruler = null)
        {
            // Start when the 5 first cards are not necessary
            InitializeComponent();
            StartInitializer(clientID, ruler, startData);
        }

        public GameClient(string fCards)
        {
            // Start when the 5 first cards are necessary
            InitializeComponent();
            FirstFiveCards(fCards);
        }
    }
}
