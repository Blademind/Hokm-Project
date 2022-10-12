using System;
using System.CodeDom.Compiler;
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
using static Hokm.HelperFunctions;

namespace Hokm
{
    public partial class Client
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] cardShapeSuit;
        public string strongSuit;
        public List<string> usedCards = new List<string>();
        public string[] ranks = { "rank_2", "rank_3", "rank_4", "rank_5", "rank_6", "rank_7", "rank_8", "rank_9", "rank_10", "rank_J", "rank_Q", "rank_K", "rank_A" };
        public Dictionary<int, List<string>> idCard = new Dictionary<int, List<string>>() { };
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
                    //Dictionary<string, int> s = HelperFunctions.MakeCounter(deck);
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
                for (int i = 0; i < arr.Length; i++)
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
        public int CardByRank(string rank)
        {
            ///<summary>
            /// Functions find the index in the deck of a card that has the specified rank
            /// <param name="rank"> a specified suit </param>
            /// <return> the index in the deck of a card that has the specified rank (as int) </return>
            ///</summary>
            ///

            bool haveStrong = true;
            foreach (var card in deck)
            {
                if (card.Split("*")[0] != strongSuit)
                    haveStrong = false;
            }
            if (haveStrong)
            {
                deck = deck.OrderBy(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
                return 0;
            }
            else
            {
                foreach (string card in deck)
                {
                    if (card.Split("*")[1] == rank && card.Split("*")[0] != strongSuit)
                    {
                        return deck.FindIndex(a => a.Contains(card));
                    }
                }
            }
            return -1;
        }
        public void PlayTurn(string played)
        {
            ///<summary>
            /// Functions chooses what to card as going through a complex algorithm
            /// <param name="played"> information about cards played before out turn </param>
            /// <return> None </return>
            ///</summary>
            
            // If we have only one card in the deck, send it
            if (deck.Count == 1)
            {
                deck.ForEach(x => Console.Write(x + ", "));
                Console.WriteLine();
                SendCard(0);
                return;
            }

            int index = 0;
            string rank = ranks[index];
            string playedSuit = played.Split(",")[0].Split(":")[1];
            string[] playedCards = played.Split(",")[1].Split(":")[1].Split("|");

            // first turn
            if (playedSuit == "")
            {
                bool found = false;

                // parsing through deck checking for ace in non strong suit
                index = 12;
                rank = ranks[index];
                int highestPossible = CardByRank(rank);
                while (highestPossible == -1 && index >= 0)
                {
                    rank = ranks[index];
                    index--;
                    highestPossible = CardByRank(rank);
                }
                if (highestPossible != -1)
                {
                    deck.ForEach(x => Console.Write(x + ", "));
                    Console.WriteLine();
                    Console.WriteLine("010: " + deck[highestPossible]);
                    SendCard(highestPossible);
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
                        deck.ForEach(x => Console.Write(x + ", "));
                        Console.WriteLine();
                        Console.WriteLine("011: "+deck[deck.FindIndex(a => a.Contains(card))]);
                        SendCard(deck.FindIndex(a => a.Contains(card)));
                        found = true;
                    }
                }
                if (!found) // ace not found, sending lower card
                {
                    List<string> availableSuits = new List<string>() { "DIAMONDS", "SPADES", "CLUBS", "HEARTS" };
                    int cardToSend;
                    int friendId = Math.Abs(clientId - 2);
                    //if (playedCards.Count(s => s == "") == 1) // 4th player
                    //{
                    bool flag = false;
                    suit = playedSuit;
                    index = 12;
                    int size;
                    List<string> candidates = new List<string>();
                    cardToSend = FindSuit(suit, rank);
                    while (index >= 0) // find highest ranks candidates
                    {
                        if (cardToSend != -1)
                        {
                            candidates.Add(deck[cardToSend]);
                        }
                        rank = ranks[index];
                        index--;
                        cardToSend = FindSuit(suit, rank);
                    }
                    candidates.Sort();

                    // is any card in played cards bigger than my biggest card in the played suit?
                    if (candidates.Count != 0)
                    {
                        flag = true;
                        bool strongExists = false;
                        if (playedCards.Count(s => s == "") == 1)  // 4th player in line
                        {
                            foreach (string candidate in candidates)
                            {
                                size = Array.IndexOf(ranks, candidate.Split("*")[1]);
                                foreach (string card in playedCards)
                                {
                                    if (card != "")
                                    {
                                        if (Array.IndexOf(ranks, card.Split("*")[1]) > size && playedSuit == candidate.Split("*")[0])
                                        {
                                            flag = false; // flag false if any card is bigger than my biggest
                                            break;
                                        }
                                        else if (card.Split("*")[0] == strongSuit && playedSuit != strongSuit)
                                        {
                                            if (!IsExists(strongSuit))
                                            {
                                                strongExists = true;
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (flag)
                                {
                                    cardToSend = deck.IndexOf(candidate);
                                    break;
                                }
                                else if (strongExists)
                                {
                                    break;
                                }
                            }
                        }
                        else  // 2nd or 3rd in line
                        {
                            foreach (string candidate in candidates)
                            {
                                size = Array.IndexOf(ranks, candidate.Split("*")[1]);
                                foreach (string card in playedCards)
                                {
                                    if (card != "")
                                    {
                                        if (Array.IndexOf(ranks, card.Split("*")[1]) + 1 > size && playedSuit == candidate.Split("*")[0])  // 2 levels above highest playing card
                                        {
                                            flag = false; // flag false if any card is bigger than my biggest
                                            break;
                                        }
                                        else if (card.Split("*")[0] == strongSuit && playedSuit != strongSuit)
                                        {
                                            if (!IsExists(strongSuit))
                                            {
                                                strongExists = true;
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (flag)
                                {
                                    cardToSend = deck.IndexOf(candidate);
                                    break;
                                }
                                else if (strongExists)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (cardToSend == -1 || !flag) // card not found
                    {
                        bool temp_flag = true;
                        suit = playedSuit;
                        availableSuits.Remove(suit);
                        if (!flag || IsExists(playedSuit))
                        {
                            flag = true;
                            index = 0;
                            rank = ranks[index];
                            cardToSend = FindSuit(suit, rank);
                            while (cardToSend == -1 && index <= 12)
                            {
                                rank = ranks[index];
                                index += 1;
                                cardToSend = FindSuit(suit, rank);
                            }
                            if (cardToSend != -1)
                            {
                                flag = false;
                                temp_flag = false;
                                deck.ForEach(x => Console.Write(x + ", "));
                                Console.WriteLine();
                                Console.WriteLine("1: " + deck[cardToSend]);
                                SendCard(cardToSend);
                            }
                        }
                        if (flag || cardToSend == -1 && temp_flag)
                        {
                            suit = strongSuit;
                            availableSuits.Remove(suit);
                            index = 0;
                            rank = ranks[index];
                            cardToSend = FindSuit(suit, rank);
                            while (cardToSend == -1 && index <= 12)
                            {
                                rank = ranks[index];
                                index += 1;
                                cardToSend = FindSuit(suit, rank);

                            }
                            if (cardToSend == -1) // card not found
                            {
                                suit = availableSuits[0];
                                availableSuits.Remove(availableSuits[0]);
                                index = 0;
                                rank = ranks[index];
                                cardToSend = FindSuit(suit, rank);
                                while (cardToSend == -1 && index <= 12)
                                {
                                    rank = ranks[index];
                                    index += 1;
                                    cardToSend = FindSuit(suit, rank);
                                }
                                if (cardToSend == -1) // card not found
                                {
                                    suit = availableSuits[0];
                                    availableSuits.Remove(availableSuits[0]);
                                    index = 0;
                                    rank = ranks[index];
                                    cardToSend = FindSuit(suit, rank);
                                    while (cardToSend == -1 && index <= 12)
                                    {
                                        rank = ranks[index];
                                        index += 1;
                                        cardToSend = FindSuit(suit, rank);
                                    }
                                    deck.ForEach(x => Console.Write(x + ", "));
                                    Console.WriteLine();
                                    Console.WriteLine("4: " + deck[cardToSend]);
                                    SendCard(cardToSend);
                                }
                                else  // a card was found
                                {
                                    deck.ForEach(x => Console.Write(x + ", "));
                                    Console.WriteLine();
                                    Console.WriteLine("5: " + deck[cardToSend]);
                                    SendCard(cardToSend);
                                }
                            }
                            else
                            {
                                deck.ForEach(x => Console.Write(x + ", "));
                                Console.WriteLine();
                                Console.WriteLine("2: " + deck[cardToSend]);
                                SendCard(cardToSend);
                            }
                        }
                    }
                    // highest which wins all others
                    else if (flag)
                    {
                        deck.ForEach(x => Console.Write(x + ", "));
                        Console.WriteLine();
                        Console.WriteLine("3: " + deck[cardToSend]);
                        SendCard(cardToSend);
                    }

                    //}
                    //else  // 2nd or 3rd in line
                    //{
                    //    availableSuits.Remove(suit);
                    //    index = 0;
                    //    rank = ranks[index];
                    //    cardToSend = FindSuit(suit, rank);
                    //    while (cardToSend == -1 && index <= 12)
                    //    {
                    //        rank = ranks[index];
                    //        index += 1;
                    //        cardToSend = FindSuit(suit, rank);

                    //    }
                    //    if (cardToSend == -1) // card not found
                    //    {
                    //        suit = strongSuit;
                    //        availableSuits.Remove(suit);
                    //        index = 0;
                    //        rank = ranks[index];
                    //        cardToSend = FindSuit(suit, rank);

                    //        while (cardToSend == -1 && index <= 12)
                    //        {
                    //            rank = ranks[index];
                    //            index += 1;
                    //            cardToSend = FindSuit(suit, rank);
                    //        }
                    //        if (cardToSend == -1) // card not found
                    //        {
                    //            suit = availableSuits[0];
                    //            availableSuits.Remove(availableSuits[0]);
                    //            index = 0;
                    //            rank = ranks[index];
                    //            cardToSend = FindSuit(suit, rank);

                    //            while (cardToSend == -1 && index <= 12)
                    //            {
                    //                rank = ranks[index];
                    //                index += 1;
                    //                cardToSend = FindSuit(suit, rank);

                    //            }
                    //            if (cardToSend == -1) // card not found
                    //            {
                    //                suit = availableSuits[0];
                    //                availableSuits.Remove(availableSuits[0]);
                    //                index = 0;
                    //                rank = ranks[index];
                    //                cardToSend = FindSuit(suit, rank);

                    //                while (cardToSend == -1 && index <= 12)
                    //                {
                    //                    rank = ranks[index];
                    //                    index += 1;
                    //                    cardToSend = FindSuit(suit, rank);

                    //                }
                    //                Console.WriteLine("22: " + deck[cardToSend]);
                    //                SendCard(cardToSend);
                    //            }
                    //            else  // a card was found
                    //            {
                    //                Console.WriteLine("44: " + deck[FindSuit(suit, rank)]);
                    //                SendCard(cardToSend);

                    //            }
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine("33: " + deck[FindSuit(suit, rank)]);
                    //            SendCard(cardToSend);
                    //        }                            
                    //        }
                    //    else
                    //        {
                    //        Console.WriteLine("11: " + deck[FindSuit(suit, rank)]);
                    //        SendCard(cardToSend);
                    //    }
                    //}
                }
            }

        }
        // does the given id win?
        public bool Wins(int id)
        {
            return false;
        }
        public bool IsExists(string suit)
        {
            ///<summary>
            /// Functions checks if a card of a specified suit exists
            /// <param name="suit"> a specified suit </param>
            /// <return> true if card with suit exists (as bool) </return>
            ///</summary>
            
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
            ///<summary>
            /// Fucntion finds a card in the current deck with the specified suit and rank
            /// <param name="suit"> a specified suit </param>
            /// <param name="rank"> a specified rank </param>
            ///</summary>
            ///
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
            ///<summary>
            /// Functions sends card to server within the deck list
            /// <param name="index"> the index from the deck list </param>
            /// <return> None </return>
            ///</summary>

            // Random r = new Random();
            string msg_to_send = "play_card:" + deck[index];
            deck.Remove(deck[index]);
            byte[] buffer = Encoding.ASCII.GetBytes(msg_to_send.Length.ToString("D8") + msg_to_send);
            clientSock.Send(buffer);
            return;

        }
        public void SendStrongSuit()
        {
            ///<summary>
            /// Function sends strong suit to server if chosen as ruler
            /// <return> None </return>
            ///</summary> 

            strongSuits = new string[4] { "SPADES", "CLUBS", "DIAMONDS", "HEARTS" };
            List<string> startingDeckSuits = new List<string>();
            List<string> compareDeck = new List<string>();
            int sendingIndex = 0;

            // Adding suits
            foreach (string card in startingDeck)
            {
                startingDeckSuits.Add(card.Split("*")[0]);
            }
            Dictionary<string, int> s = startingDeckSuits.GroupBy(p => p).OrderByDescending(r => r.Count()).ToDictionary(q => q.Key, q => q.Count());

            // Spposed to the the biggest value
            int MaxValue = s.First().Value;
            string key = s.First().Key;
            bool found = false;
            foreach (var item in s)
            {
                // If there are suits that have the same number of cards in the deck
                if (item.Value == MaxValue)
                {
                    found = true;
                    foreach (string card in startingDeck)
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
                foreach (string card in compareDeck)
                {
                    indexes.Add(Array.IndexOf(ranks, card.Split("*")[1]));
                }

                // The index of the maximum rank in indexes will be the same index as in compareDeck
                var maxIndex = indexes.IndexOf(indexes.Max());
                for (int i = 0; i < compareDeck.Count(); i++)
                {
                    if (i == maxIndex)
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
            byte[] buffer = Encoding.ASCII.GetBytes(msg_to_send.Length.ToString("D8") + msg_to_send);
            clientSock.Send(buffer);

        }
    }
}
