﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static Hokm.HelperFunctions;
using static Hokm.Client;

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
        public GameClient gameClient;
        public string[] playedCards;
        public dynamic GameMessageParser(string msg)
        {
            if (Char.IsUpper(msg[0])) // card deck
            {
                string[] cards = msg.Split("|");

                if (cards.Length == 5)
                {
                    gameClient = new GameClient(msg);
                    new Thread(
                    () =>
                    {
                        Application.Run(gameClient);
                    }
                    ).Start();
                }

                if (cards.Length == 5 && clientId == ruler) // starting deck for ruler
                {
                    
                    startingDeck = cards;
                    SendStrongSuit();
                }
                if (cards.Length == 14)
                {
                    Console.WriteLine(msg);
                    gameClient.PublicStartInitializer(msg, clientId.ToString(), ruler.ToString());
                    //Console.WriteLine(msg);
                    //gameClient = new GameClient(msg, clientId.ToString(), ruler.ToString());
                    //new Thread(
                    //() =>
                    //{
                    //    Application.Run(gameClient);
                    //}
                    //).Start();
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
                gameClient.RemoveMiddleCards();
                string[] round_cards = msg.Split(",")[2].Split(":")[1].Split("|");

                for (int i = 0; i < round_cards.Length; i++)
                {
                    if (round_cards[i] != "" && i + 1 != clientId)
                        gameClient.PlayOtherCard(round_cards[i], i + 1);
                    else
                        gameClient.PlayCard(round_cards[i]);
                }
                gameClient.RoundEnding(msg.Split(":")[1].Split("+")[0], msg.Split(":")[1].Substring(0,3));

                string[] arr = msg.Split(",")[2].Split(":")[1].Split("|");
                for (int i = 0; i < arr.Length; i++)
                {
                    if (idCard[i].IndexOf(arr[i]) == -1)
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

            // Getting starting information from server about the current state of the round
            int index = 0;
            string rank = ranks[index];
            string playedSuit = played.Split(",")[0].Split(":")[1];
            playedCards = played.Split(",")[1].Split(":")[1].Split("|");

            // First turn
            if (playedSuit == "")
            {
                bool found = false;

                // Parsing through deck checking for highest rank possible
                index = 12;
                rank = ranks[index];
                int highestPossible = CardByRank(rank);

                while (highestPossible == -1 && index >= 0)
                {
                    rank = ranks[index];
                    index--;
                    highestPossible = CardByRank(rank);
                }

                // Sending highest possible card
                if (highestPossible != -1)
                {
                    deck.ForEach(x => Console.Write(x + ", "));
                    Console.WriteLine();
                    Console.WriteLine("010: " + deck[highestPossible]);
                    SendCard(highestPossible);
                }

            }
            // Not first turn
            else
            {
                // Trying to find ace
                bool found = false;
                string suit = playedSuit;
                foreach (string card in deck.ToArray())
                {
                    if (card.Split("*")[1] == "rank_A" && card.Split("*")[0] != strongSuit && card.Split("*")[0] == suit)
                    {
                        deck.ForEach(x => Console.Write(x + ", "));
                        Console.WriteLine();
                        Console.WriteLine("011: " + deck[deck.FindIndex(a => a.Contains(card))]);
                        SendCard(deck.FindIndex(a => a.Contains(card)));
                        found = true;
                    }
                }

                // Ace not found, sending lower card
                if (!found)
                {
                    List<string> availableSuits = new List<string>() { "DIAMONDS", "SPADES", "CLUBS", "HEARTS" };
                    int cardToSend; int friendId = 0;

                    // Finding our teammate id
                    switch (clientId)
                    {
                        case 1:
                            friendId = 2;
                            break;
                        case 2:
                            friendId = 1;
                            break;
                        case 3:
                            friendId = 4;
                            break;
                        case 4:
                            friendId = 3;
                            break;
                    }

                    // Intiating scanning variable
                    bool flag = false;
                    suit = playedSuit;
                    index = 12;
                    int size;
                    List<string> candidates = new List<string>();
                    cardToSend = FindSuit(suit, rank);

                    // Find highest ranks candidates for highest possible card
                    while (index >= 0)
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

                    // Scanning through candidates
                    if (candidates.Count != 0)
                    {
                        List<string> putCards = FindWinningCards(suit);
                        string last;
                        flag = true;
                        bool friendWins = false;
                        bool strongExists = false;
                        bool winningCardExists = false;
                        bool aceFound = false;
                        // 4th player in line
                        if (playedCards.Count(s => s == "") == 1)
                        {
                            foreach (string candidate in candidates)
                            {

                                // Checking for the smallest possible candidate winning card in the list
                                // Does not include strong suit card
                                size = Array.IndexOf(ranks, candidate.Split("*")[1]);
                                foreach (string card in playedCards)
                                {
                                    if (card != "")
                                    {
                                        // If my teammate sent the highest card, sending my lowest card in the played suit
                                        if (Wins(Array.IndexOf(playedCards, card), playedCards) && Array.IndexOf(playedCards, card) == friendId)
                                        {
                                            friendWins = true;
                                            flag = false;
                                            break;
                                        }

                                        // TODO
                                        else if (Array.IndexOf(ranks, card.Split("*")[1]) > size && playedSuit == candidate.Split("*")[0])
                                        {
                                            flag = false;
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

                                // TODO
                                if (flag)
                                {
                                    cardToSend = deck.IndexOf(candidate);
                                    break;
                                }
                                else if (friendWins)
                                {
                                    break;
                                }
                                else if (strongExists)
                                {
                                    break;
                                }
                            }
                        }

                        // 2nd or 3rd in line
                        else
                        {
                            // Scanning through candidates
                            foreach (string card in playedCards)
                            {
                                if (card != "")
                                {
                                    if (Array.IndexOf(putCards.ToArray(), card) == -1 && card.Split("*")[0] == suit)
                                        putCards.Add(card);
                                }
                            }
                            putCards = putCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
                            foreach (string candidate in candidates)
                            {

                                // Checking for the lowest possible card that can win what has been sent 2 levels above
                                size = Array.IndexOf(ranks, candidate.Split("*")[1]);
                                foreach (string card in playedCards)
                                {

                                    if (card != "")
                                    {
                                        if (card.Split("*")[1] != "rank_A")
                                        {
                                            // If my teammate has put an ace, sending the lowest card in the played suit
                                            if (Wins(Array.IndexOf(playedCards, card), playedCards) && Array.IndexOf(playedCards, card) == friendId && card.Split("*")[1] == "rank_A")
                                            {
                                                friendWins = true;
                                                flag = false;
                                                break;
                                            }

                                            // TODO
                                            if (putCards.Count > 1)
                                            {

                                                last = putCards[0];
                                                Console.WriteLine(putCards[0].Split("*")[1]);
                                                if (putCards[0].Split("*")[1] == "rank_A")
                                                {
                                                    foreach (string item in putCards.ToArray())
                                                    {
                                                        if (Array.IndexOf(ranks, last.Split("*")[1]) - Array.IndexOf(ranks, item.Split("*")[1]) > 1)
                                                        {
                                                            Console.WriteLine(last);
                                                            Console.WriteLine("deck1: " + deck.IndexOf(playedSuit + "*" + ranks[Array.IndexOf(ranks, last.Split("*")[1]) - 1]));
                                                            if (deck.IndexOf(playedSuit + "*" + ranks[Array.IndexOf(ranks, last.Split("*")[1]) - 1]) != -1)
                                                            {
                                                                cardToSend = deck.IndexOf(playedSuit + "*" + ranks[Array.IndexOf(ranks, last.Split("*")[1]) - 1]);
                                                                winningCardExists = true;
                                                            }
                                                            break;
                                                        }
                                                        last = item;

                                                    }
                                                }
                                            }

                                            if (winningCardExists)
                                            {
                                                List<string> plainCards = new List<string>();
                                                foreach(string c in playedCards)
                                                { 
                                                    if (c != "" && c.Split("*")[0] == suit)
                                                    {
                                                        plainCards.Add(c);
                                                    }
                                                }
                                                //List<string> descendingRanks = playedCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
                                                List<string> descendingRanks = plainCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
                                                descendingRanks.ForEach(a => Console.Write(a + ", "));
                                                Console.WriteLine();
                                                if (Array.IndexOf(ranks, descendingRanks[0].Split("*")[1]) < Array.IndexOf(ranks, deck[cardToSend].Split("*")[1]))
                                                    break;
                                                else
                                                    winningCardExists = false;
                                            }

                                            // 2 levels above highest playing card
                                            if (Array.IndexOf(ranks, card.Split("*")[1]) + 2 > size && playedSuit == candidate.Split("*")[0])
                                            {
                                                // Flag false if any card is bigger than my biggest one
                                                flag = false;
                                                break;
                                            }

                                            // If strong has been played, sending the lowest possible strong card which wins
                                            if (card.Split("*")[0] == strongSuit && playedSuit != strongSuit)
                                            {
                                                if (!IsExists(strongSuit))
                                                {
                                                    strongExists = true;
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            aceFound = true;
                                            flag = false;
                                            break;
                                        }
                                    }


                                }

                                // TODO

                                if (winningCardExists)
                                {
                                    Console.WriteLine("FOUND WINNING CARD");
                                    break;
                                }
                                if (flag)
                                {
                                    cardToSend = deck.IndexOf(candidate);
                                    break;
                                }
                                if (strongExists || friendWins || aceFound)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    // If a card wasn't found
                    if (cardToSend == -1 || !flag)
                    {

                        bool temp_flag = true;
                        suit = playedSuit;
                        availableSuits.Remove(suit);

                        // If winning card was not found and playedSuit exists in deck, send lowest possible card
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

                            // TODO
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

                        // If lowest possible card wasn't found
                        if (flag || cardToSend == -1 && temp_flag)
                        {
                            flag = false;
                            suit = strongSuit;
                            availableSuits.Remove(suit);
                            index = 12;
                            rank = ranks[index];
                            candidates = new List<string>();
                            cardToSend = FindSuit(suit, rank);

                            // Find highest ranks candidates
                            while (index >= 0)
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

                            // Scanning through candidates
                            if (candidates.Count != 0)
                            {
                                List<string> putCards = FindWinningCards(suit);
                                string last;
                                flag = true;
                                bool friendWins = false;
                                bool winningCardExists = false;
                                bool aceFound = false;
                                // 4th player in line
                                if (playedCards.Count(s => s == "") == 1)
                                {
                                    foreach (string candidate in candidates)
                                    {
                                        size = Array.IndexOf(ranks, candidate.Split("*")[1]);
                                        foreach (string card in playedCards)
                                        {
                                            // Checking for the smallest possible candidate winning card in the list
                                            // Does include strong suit card 
                                            if (card != "")
                                            {
                                                // If my teammate has put the winning card, sending the lowest card in the strong suit
                                                if (Wins(Array.IndexOf(playedCards, card), playedCards) && Array.IndexOf(playedCards, card) == friendId)
                                                {
                                                    friendWins = true;
                                                    flag = false;
                                                    break;
                                                }

                                                // Checking if one of the played cards is higher than any our candidates 
                                                else if (Array.IndexOf(ranks, card.Split("*")[1]) > size && playedSuit == candidate.Split("*")[0])
                                                {
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                        }

                                        // TODO
                                        if (flag)
                                        {
                                            cardToSend = deck.IndexOf(candidate);
                                            break;
                                        }

                                        else if (friendWins)
                                        {
                                            break;
                                        }
                                    }
                                }

                                // 2nd or 3rd in line
                                else
                                {
                                    // Scanning through candidates
                                    foreach (string card in playedCards)
                                    {
                                        if (card != "")
                                        {
                                            if (Array.IndexOf(putCards.ToArray(), card) == -1 && card.Split("*")[0] == suit)
                                                putCards.Add(card);
                                        }
                                    }
                                    putCards = putCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();

                                    // Scanning through candidates
                                    foreach (string candidate in candidates)
                                    {
                                        // Checking for the lowest possible card that can win what has been sent 2 levels above
                                        size = Array.IndexOf(ranks, candidate.Split("*")[1]);
                                        foreach (string card in playedCards)
                                        {
                                            if (card != "")
                                            {
                                                if (card.Split("*")[1] != "rank_A")
                                                {
                                                    // If my teammate sent an ace in strong suit, sending my lowest card in the played suit
                                                    if (Wins(Array.IndexOf(playedCards, card), playedCards) && Array.IndexOf(playedCards, card) == friendId && card.Split("*")[1] == "rank_A")
                                                    {
                                                        friendWins = true;
                                                        flag = false;
                                                        break;
                                                    }
                                                    // TODO
                                                    if (putCards.Count > 1)
                                                    {
                                                        last = putCards[0];
                                                        if (putCards[0].Split("*")[1] == "rank_A")
                                                        {
                                                            foreach (string item in putCards.ToArray())
                                                            {
                                                                if (Array.IndexOf(ranks, last.Split("*")[1]) - Array.IndexOf(ranks, item.Split("*")[1]) > 1)
                                                                {
                                                                    Console.WriteLine(last);
                                                                    Console.WriteLine("deck1: " + deck.IndexOf(playedSuit + "*" + ranks[Array.IndexOf(ranks, last.Split("*")[1]) - 1]));
                                                                    if (deck.IndexOf(playedSuit + "*" + ranks[Array.IndexOf(ranks, last.Split("*")[1]) - 1]) != -1)
                                                                    {
                                                                        cardToSend = deck.IndexOf(playedSuit + "*" + ranks[Array.IndexOf(ranks, last.Split("*")[1]) - 1]);
                                                                        winningCardExists = true;
                                                                    }
                                                                    break;
                                                                }
                                                                last = item;

                                                            }
                                                        }
                                                    }
                                                    if (winningCardExists)
                                                    {
                                                        List<string> plainCards = new List<string>();
                                                        foreach (string c in playedCards)
                                                        {
                                                            if (c != "" && c.Split("*")[0] == suit)
                                                            {
                                                                plainCards.Add(c);
                                                            }
                                                        }
                                                        //List<string> descendingRanks = playedCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
                                                        List<string> descendingRanks = plainCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
                                                        descendingRanks.ForEach(a => Console.Write(a + ", "));
                                                        Console.WriteLine();
                                                        if (Array.IndexOf(ranks, descendingRanks[0].Split("*")[1]) < Array.IndexOf(ranks, deck[cardToSend].Split("*")[1]))
                                                            break;
                                                        else
                                                            winningCardExists = false;
                                                    }
                                                    // 3 levels above highest playing card
                                                    else if (Array.IndexOf(ranks, card.Split("*")[1]) + 2 > size && playedSuit == candidate.Split("*")[0])  // 2 levels above highest playing card
                                                    {
                                                        flag = false;
                                                        break;
                                                    }
                                                }

                                                else
                                                {
                                                    aceFound = true;
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                        }

                                        // TODO
                                        if (winningCardExists)
                                        {
                                            Console.WriteLine("FOUND WINNING STRONG CARD");  
                                            break;
                                        }
                                        if (friendWins || aceFound)
                                            break;
                                        if (flag)
                                        {
                                            cardToSend = deck.IndexOf(candidate);
                                            break;
                                        }
                                    }
                                }

                                // If card not found, send the lowest strong suit card
                                if (cardToSend == -1)
                                {
                                    index = 0;
                                    rank = ranks[index];
                                    cardToSend = FindSuit(suit, rank);
                                    while (cardToSend == -1 && index <= 12)
                                    {
                                        rank = ranks[index];
                                        index += 1;
                                        cardToSend = FindSuit(suit, rank);
                                    }
                                }
                            }

                            // If card not found, send a non playing card and non strong card
                            if (cardToSend == -1)
                            {
                                //if (deck[0].Split("*")[0] == availableSuits[0])
                                //{
                                //    suit = availableSuits[0];
                                //    availableSuits.Remove(suit);

                                //}
                                //else
                                //{
                                //    suit = availableSuits[1];
                                //    availableSuits.Remove(suit);
                                //}

                                index = 0;
                                rank = ranks[index];
                                cardToSend = CardByRank(rank);
                                while (cardToSend == -1 && index <= 12)
                                {
                                    rank = ranks[index];
                                    index += 1;
                                    cardToSend = CardByRank(rank);
                                }
                                deck.ForEach(x => Console.Write(x + ", "));
                                Console.WriteLine();
                                Console.WriteLine("4: " + deck[cardToSend]);
                                SendCard(cardToSend);
                                //// If card not found, send the lowest non playing and non strong card
                                //if (cardToSend == -1)
                                //{
                                //    suit = availableSuits[0];
                                //    Console.WriteLine("Last available suit: " + suit);
                                //    index = 0;
                                //    rank = ranks[index];
                                //    cardToSend = FindSuit(suit, rank);

                                //    while (cardToSend == -1 && index <= 12)
                                //    {
                                //        rank = ranks[index];
                                //        index += 1;
                                //        cardToSend = FindSuit(suit, rank);
                                //    }

                                //    deck.ForEach(a => Console.Write(a + ", "));
                                //    Console.WriteLine();

                                //    Console.WriteLine("4: " + deck[cardToSend]);
                                //    SendCard(cardToSend);

                                //}

                                //// TODO
                                //else
                                //{
                                //    deck.ForEach(x => Console.Write(x + ", "));
                                //    Console.WriteLine();
                                //    Console.WriteLine("5: " + deck[cardToSend]);
                                //    SendCard(cardToSend);
                                //}
                            }

                            // TODO
                            else
                            {
                                deck.ForEach(x => Console.Write(x + ", "));
                                Console.WriteLine();
                                Console.WriteLine("2: " + deck[cardToSend]);
                                SendCard(cardToSend);
                            }
                        }
                    }

                    // If currently winning card was found, send it
                    else if (flag)
                    {
                        deck.ForEach(x => Console.Write(x + ", "));
                        Console.WriteLine();
                        Console.WriteLine("3: " + deck[cardToSend]);
                        SendCard(cardToSend);
                    }
                }
            }

        }
        public bool Wins(int id, string[] playedCards)
        {
            ///<summary>
            /// Checks if the given id wins
            /// <param name="id"> player's id</param>
            /// <param name="playedCards"> recently played cards</param>
            /// <return> true if given id has the highest cards</return>
            ///</summary>
            bool max = true;
            foreach (string card in playedCards)
            {
                if (card != "")
                {
                    // TODO
                    if (Array.IndexOf(ranks, card.Split("*")[1]) > Array.IndexOf(ranks, playedCards[id].Split("*")[1]))
                    {
                        max = false;
                        break;
                    }

                    // TODO
                    else if (card.Split("*")[0] == strongSuit && playedCards[id].Split("*")[0] == strongSuit)
                    {
                        if (Array.IndexOf(ranks, card.Split("*")[1]) > Array.IndexOf(ranks, playedCards[id].Split("*")[1]))
                        {
                            max = false;
                            break;
                        }
                    }
                }
            }
            return max;
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
        public int FindSuit(string suit, string rank)
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
        public List<string> FindWinningCards(string suit)
        {
            ///<summary>
            /// Function finds highest winning card in a given suit according to last placed cards
            /// <param name="suit"> a specified suit </param>
            /// <return> returns all recently put cards in the correct suit by a descending order (as List<string>)</return>
            ///</summary> 
            List<string> putCards = new List<string>();
            
            // Adds all recently put cards to a list
            foreach (var id in idCard)
            {
                foreach (string card in id.Value)
                {
                    if (card.Split("*")[0] == suit)
                        putCards.Add(card);
                }
            }
            // Order from high to low according to ranks
            putCards = putCards.OrderByDescending(a => Array.IndexOf(ranks, a.Split("*")[1])).ToList();
            return putCards;
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
