using System;
using System.Collections.Generic;
using System.Text;

namespace Hokm
{
    public class GameHandler
    {
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
		public List<List<string>> GetDeck()
        {
			/* Deck looks like [[rank, suit], [rank, suit],....] */
			return new List<List<string>>() { new List<string>() };
        }
	}
}
