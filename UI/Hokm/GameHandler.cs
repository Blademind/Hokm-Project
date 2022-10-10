using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Hokm
{
    public partial class Client
    {
        public string[] cardShapeSuit;
        public string strongSuit;
        public List<string> usedCards = new List<string>();
        public string[] ranks = { "rank_2", "rank_3", "rank_4", "rank_5", "rank_6", "rank_7", "rank_8", "rank_9", "rank_10", "rank_J", "rank_Q", "rank_K", "rank_A" };
        public Dictionary<int, List<string>> idCard = new Dictionary<int, List<string>>() {  };
        public string[] startingDeck;
        public dynamic GameMessageParser(string msg)
        {
            if (Char.IsUpper(msg[0])) // card deck
            {
                string[] cards = msg.Split("|");
                if (cards.Length == 5 && clientId == ruler) // starting deck for ruler
                {
                    startingDeck = cards;
                    SendStrongSuit();
                }
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
            else if (msg.Contains("round_cards:"))
            {
                string[] arr = msg.Split(",")[2].Split(":")[1].Split("|");
                for(int i=0; i< arr.Length; i++)
                {
                    idCard[i].Add(arr[i]);
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
        public int CardByRank(string rank)
        {
            /* Function returns first lowest card with specified rank of the deck */
            foreach (string card in deck)
            {
                if (card.Split("*")[1] == rank && card.Split("*")[0] != strongSuit)
                {
                    return deck.FindIndex(a => a.Contains(card));
                }
            }
            return -1;
        }
		public void PlayTurn(string played)
		{
            /* Algorithmic function to play the turns */
            int index = 0;
            string rank = ranks[index];
            string playedSuit = played.Split(",")[0].Split(":")[1];
            string[] playedCards = played.Split(",")[1].Split(":")[1].Split("|");
            // first turn
            if (playedSuit == "")
            {
                bool found = false;

                // parsing through deck checking for ace in non strong suit
                foreach(string card in deck.ToArray())
                {
                    if (card.Split("*")[1] == "rank_A" && card.Split("*")[0] != strongSuit)
                    {
                        SendCard(deck.FindIndex(a => a.Contains(card)));
                        found = true;
                    }
                }
                if (!found) // ace not found, sending lower card
                {
                    while (CardByRank(rank) == -1 && index<=12)
                    {
                        rank = ranks[index];
                        index += 1;

                    }
                    SendCard(CardByRank(rank));
                }
            }
            else // not first turn
            {
                bool found = false;
                string suit = playedSuit;
                foreach (string card in deck.ToArray())
                {
                    if (card.Split("*")[1] == "rank_A" && card.Split("*")[0] != strongSuit && card.Split("*")[0] == suit)
                    {
                        SendCard(deck.FindIndex(a => a.Contains(card)));
                        found = true;
                    }
                }
                if (!found) // ace not found, sending lower card
                {
                    List<string> availableSuits = new List<string>() { "DIAMONDS", "SPADES", "CLUBS", "HEARTS" };
                    if (playedCards.Count(s => s == "") == 1) // 4th player
                    {
                        suit = playedSuit;
                        index = 12;
                        while(CardByRank(rank) == -1 && index >= 0) // find highest rank possible
                        {
                            rank = ranks[index];
                            index--;
                        }
                        int size = Array.IndexOf(ranks, deck[CardByRank(rank)].Split("*")[1]);
                        bool flag = true;
                        foreach (string card in playedCards)
                        {
                            if (card != "")
                            {
                                if(FindSuit(rank, suit) != -1)
                                {
                                    Console.WriteLine(playedSuit + ", " + deck[FindSuit(rank, suit)].Split("*")[0]);
                                    if (Array.IndexOf(ranks, card.Split("*")[1]) > size && playedSuit == deck[FindSuit(rank, suit)].Split("*")[0])
                                    {
                                        flag = false; break;
                                    }
                                }
                                else {
                                    break;
                                }
                            }
                        }
                        if (FindSuit(suit, rank) == -1 || !flag) // card not found
                        {
                            bool temp_flag = true;
                            suit = playedSuit;
                            availableSuits.Remove(suit);
                            if (!flag || IsExists(playedSuit))
                            {
                                flag = true;
                                index = 0;
                                rank = ranks[index];
                                while (FindSuit(suit, rank) == -1 && index <= 12)
                                {
                                    rank = ranks[index];
                                    index += 1;
                                }
                                if(FindSuit(suit, rank) != -1)
                                {
                                    flag = false;
                                    temp_flag = false;
                                    Console.WriteLine("1: " + deck[FindSuit(suit, rank)]);
                                    SendCard(FindSuit(suit, rank));
                                }
                            }
                            if (flag || FindSuit(suit,rank) == -1 && temp_flag)
                            {
                                suit = strongSuit;
                                availableSuits.Remove(suit);
                                index = 0;
                                rank = ranks[index];
                                while (FindSuit(suit, rank) == -1 && index <= 12)
                                {
                                    rank = ranks[index];
                                    index += 1;
                                }
                                if (FindSuit(suit, rank) == -1) // card not found
                                {
                                    suit = availableSuits[0];
                                    availableSuits.Remove(availableSuits[0]);
                                    index = 0;
                                    rank = ranks[index];
                                    while (FindSuit(suit, rank) == -1 && index <= 12)
                                    {
                                        rank = ranks[index];
                                        index += 1;
                                    }
                                    if (FindSuit(suit, rank) == -1) // card not found
                                    {
                                        suit = availableSuits[0];
                                        availableSuits.Remove(availableSuits[0]);
                                        index = 0;
                                        rank = ranks[index];
                                        while (FindSuit(suit, rank) == -1 && index <= 12)
                                        {
                                            rank = ranks[index];
                                            index += 1;
                                        }
                                        Console.WriteLine("4: "+deck[FindSuit(suit, rank)]);
                                        SendCard(FindSuit(suit, rank));
                                    }
                                    else  // a card was found
                                    {
                                        Console.WriteLine(deck[FindSuit(suit, rank)]);
                                        SendCard(FindSuit(suit, rank));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("2: " + deck[FindSuit(suit, rank)]);
                                    SendCard(FindSuit(suit, rank));
                                }
                            }
                        }
                        else if (flag)
                        {
                            Console.WriteLine("3: " + deck[CardByRank(rank)]);
                            SendCard(CardByRank(rank));
                        }

                    }
                    else
                    {
                        availableSuits.Remove(suit);
                        index = 0;
                        rank = ranks[index];
                        while (FindSuit(suit, rank) == -1 && index <= 12)
                        {
                            rank = ranks[index];
                            index += 1;
                        }
                        if (FindSuit(suit, rank) == -1) // card not found
                        {
                            suit = strongSuit;
                            availableSuits.Remove(suit);
                            index = 0;
                            rank = ranks[index];
                            while (FindSuit(suit, rank) == -1 && index <= 12)
                            {
                                rank = ranks[index];
                                index += 1;
                            }
                            if (FindSuit(suit, rank) == -1) // card not found
                            {
                                suit = availableSuits[0];
                                availableSuits.Remove(availableSuits[0]);
                                index = 0;
                                rank = ranks[index];
                                while (FindSuit(suit, rank) == -1 && index <= 12)
                                {
                                    rank = ranks[index];
                                    index += 1;
                                }
                                if (FindSuit(suit, rank) == -1) // card not found
                                {
                                    suit = availableSuits[0];
                                    availableSuits.Remove(availableSuits[0]);
                                    index = 0;
                                    rank = ranks[index];
                                    while (FindSuit(suit, rank) == -1 && index <= 12)
                                    {
                                        rank = ranks[index];
                                        index += 1;
                                    }
                                    Console.WriteLine("22: " + deck[FindSuit(suit, rank)]);
                                    SendCard(FindSuit(suit, rank));
                                }
                                else  // a card was found
                                {
                                    Console.WriteLine("44: " + deck[FindSuit(suit, rank)]);
                                    SendCard(FindSuit(suit, rank));
                                
                                }
                            }
                            else
                            {
                                Console.WriteLine("33: " + deck[FindSuit(suit, rank)]);
                                SendCard(FindSuit(suit, rank));
                            }                            
                            }
                        else
                            {
                            Console.WriteLine("11: " + deck[FindSuit(suit, rank)]);
                            SendCard(FindSuit(suit, rank));
                        }
                    }
                }
            }

		}
        public bool IsExists(string suit)
        {
            foreach (string card in deck)
            {
                if (card.Split("*")[0] == suit)
                {
                    return true;
                }
            }
            return false;
        }
        public int FindSuit(string suit, string rank) // finds rank in suit
        {
            foreach (string card in deck)
            {
                if (card.Split("*")[0] == suit && rank == card.Split("*")[1])
                {
                    return deck.FindIndex(a => a.Contains(card));
                }
            }
            return -1;
        }
		public void SendCard(int index)
		{
            /* create a card and send to server */

            // Random r = new Random();
            string msg_to_send = "play_card:" + deck[index];
            deck.Remove(deck[index]);
            byte[] buffer = Encoding.ASCII.GetBytes(msg_to_send.Length.ToString("D8") + msg_to_send);
            clientSock.Send(buffer);

		}
        public void SendStrongSuit()
        {
            /* Function sends strong suit to server if ruler */

            strongSuits = new string[4] { "SPADES", "CLUBS", "DIAMONDS", "HEARTS" };
            List<string> startingDeckSuits = new List<string>();
            List<string> compareDeck = new List<string>();
            int sendingIndex = 0;

            // Adding suits
            foreach(string card in startingDeck)
            {
                startingDeckSuits.Add(card.Split("*")[0]);
            }
            Dictionary<string, int> s = startingDeckSuits.GroupBy(p => p).OrderByDescending(r => r.Count()).ToDictionary(q => q.Key, q => q.Count());

            // Spposed to the the biggest value
            int MaxValue = s.First().Value;
            string key = s.First().Key;
            bool found = false;
            foreach(var item in s)
            {
                // If there are suits that have the same number of cards in the deck
                if (item.Value == MaxValue)
                {
                    found = true;
                    foreach(string card in startingDeck)
                    {
                        // Adding the cards that have the same amound of suits in the deck
                        if (card.Split("*")[0] == key || card.Split("*")[0] == item.Key)
                        {
                            compareDeck.Add(card);
                            break;
                        }
                    }
                } 
            }

            // There is only one suit with more cards --> sending first index of startingDeck dictionary
            if (!found)
            {
                sendingIndex = Array.IndexOf(strongSuits, key);
            }

            // Now, checking or highest card and sending it's suit as highest index
            else 
            {
                List<int> indexes = new List<int>();
                foreach(string card in compareDeck)
                {
                    indexes.Add(Array.IndexOf(ranks, card.Split("*")[1]));
                }

                // The index of the maximum rank in indexes will be the same index as in compareDeck
                var maxIndex = indexes.IndexOf(indexes.Max());
                for (int i = 0; i < compareDeck.Count(); i++)
                {
                    if(i == maxIndex)
                    {
                        sendingIndex = Array.IndexOf(strongSuits, compareDeck[i].Split("*")[0]);
                    }
                }
            }

            // Sending strong suit
            Console.WriteLine("Sending strong suit...");
            Console.WriteLine("set_strong:" + strongSuits[sendingIndex]);
            string msg_to_send = "set_strong:" + strongSuits[sendingIndex];
            Console.WriteLine(msg_to_send.Length.ToString("D8") + msg_to_send);
            byte[] buffer = Encoding.ASCII.GetBytes(msg_to_send.Length.ToString("D8")+msg_to_send);
            clientSock.Send(buffer);

        }
		public List<List<string>> GetDeck()
        {
			/* Deck looks like [[rank, suit], [rank, suit],....] */
			return new List<List<string>>() { new List<string>() };
        }
	}
}
