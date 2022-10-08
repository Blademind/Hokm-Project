using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Hokm
{
    public partial class Client
    {
        public string[] cardShapeSuit;
        public string strongSuit;

        public dynamic GameMessageParser(string msg)
        {
            if (Char.IsUpper(msg[0])) // card deck
            {
                string[] cards = msg.Split("|");
                if (cards.Length == 14)
                {
                    cards = msg.Split(",");
                    strongSuit = cards[2].Split(":")[1];

                    cards = cards[0].Split("|");
                    foreach (string card in cards[5..13])
                    {
                        deck.Add(card);
                    }
                    //int count = 0;
                    //foreach (string c in deck)
                    //{
                    //    Console.WriteLine(count);
                    //    count++;
                    //    Console.WriteLine(c);
                    //}
                }
                else
                {
                    foreach (string card in cards)
                    {
                        deck.Add(card);
                    }
                }
            }

            if (msg.Split(":").Length > 0)
            {
                string[] splitMessage = msg.Split(":");
                if (splitMessage[0] == "ruler")
                {
                    ruler = Int32.Parse(splitMessage[1]);

                    return "The ruler is: " + ruler;
                }

                else if (splitMessage[0] == "client_id")
                {
                    clientId = Int32.Parse(splitMessage[1]);

                    return "Your client ID is: " + clientId;
                }
            }
            return "";

        }
        public void GameLoop()
		{
			/* Runs the game loop */
		}
		public void PlayTurn(string played)
		{
            Console.WriteLine(strongSuit);
            /* Algorithmic function to play the turns */
            string playedSuit = played.Split(",")[0];
            string playedCards = played.Split(",")[1];

            // first turn
            if (playedSuit.Split(":")[1] == "")
            {
                // parsing through deck checking for ace in non strong suit
                foreach(string card in deck)
                {
                    if (card.Split("*")[1] == "rank_A" && card.Split("*")[0] != strongSuit)
                    {

                    }
                }
            }

		}
		public void SendCard()
		{
            /* create a card and send to server */
            Random r = new Random();
            string msg_to_send = "play_card:" + deck[r.Next(0, deck.Count)];
            byte[] buffer = Encoding.ASCII.GetBytes(msg_to_send.Length.ToString("D8") + msg_to_send);
            client_sock.Send(buffer);

		}
        public void SendStrongSuit()
        {
            strong_suits = new string[4] { "SPADES", "CLUBS", "DIAMONDS", "HEARTS" };
            Random rand = new Random();
            Console.WriteLine("Sending strong suit...");
            Console.WriteLine("set_strong:" + strong_suits[rand.Next(0, 4)]);
            string msg_to_send = "set_strong:" + strong_suits[rand.Next(0, 4)];
            Console.WriteLine(msg_to_send.Length.ToString("D8") + msg_to_send);
            byte[] buffer = Encoding.ASCII.GetBytes(msg_to_send.Length.ToString("D8")+msg_to_send);
            client_sock.Send(buffer);
        }
		public List<List<string>> GetDeck()
        {
			/* Deck looks like [[rank, suit], [rank, suit],....] */
			return new List<List<string>>() { new List<string>() };
        }
	}
}
