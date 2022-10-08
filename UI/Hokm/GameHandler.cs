using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;

namespace Hokm
{
    public partial class Client
    {

        public dynamic GameMessageParser(string msg)
        {
            if (Char.IsUpper(msg[0])) // card deck
            {
                deck = new List<List<string>>();
                string[] cards = msg.Split("|");

                foreach (string card in cards)
                {
                    string[] cardShapeSuit = card.Split("*");
                    List<string> shapeSuit = new List<string>();
                    foreach (string c in cardShapeSuit)
                    {
                        shapeSuit.Add(c);
                    }
                    deck.Add(shapeSuit);
                }
            }
            else // ruler or client_id
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
		public void PlayTurn()
		{
			/* main algorythmics function */
		}
		public void SendCard()
		{
			/* create a card and send
			 * includes SetSuit */
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
