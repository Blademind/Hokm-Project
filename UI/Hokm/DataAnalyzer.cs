using System;
using System.Collections.Generic;
using System.Text;

namespace Hokm
{
    class DataAnalyzer
    {
        /// <summary>
        /// DataAnalyzer class parses the data that it receives from Client class for the UI
        /// </summary>
        /// 

        // Initializing variabels
        private string startData;
        private string clientID;
        private string rulerID;
        private string[] players;

        public DataAnalyzer(string clientID,string rulerID, string startData)
        {
            ///<summary>
            /// Initializing the class
            /// The class manages the starting string from the server and saves it inside the object
            /// <param name="clientID">The user's ID</param>
            /// <param name="rulerID">The hakm's ID</param>
            /// <param name="startData">The raw message from the server containing the deck, hokm, team and etc</param>
            /// <return> None </return>
            ///</summary>
            
            this.clientID = clientID.ToLower();
            this.rulerID = rulerID.ToLower();
            this.startData = startData.ToLower();
            this.players = new string[3];
            SetRealOtherID();
        }

        public DataAnalyzer()
        {
            ///<summary>
            /// Initializing the class in case of the data is not available yet
            /// <return> None </return>
            ///</summary>
            ///

            this.players = new string[3];
        }


        public void SetClientID(string clientID)
        {
            ///<summary>
            /// Setter for clientID
            /// <param name="clientID">The client ID/param>
            /// <return> None </return>
            ///</summary>
            ///

            this.clientID = clientID;
        }
        public void SetRulerID(string rulerID)
        {
            ///<summary>
            /// Setter for ruler id
            /// <param name="rulerID">The ruler ID/param>
            /// <return> None </return>
            ///</summary>
            ///

            this.rulerID = rulerID;
        }

        public void SetStartData(string startData)
        {
            ///<summary>
            /// The function sets the starting data for the game
            /// <param name="startData">The starting data for the game/param>
            /// <return> None </return>
            ///</summary>
            ///

            this.startData = startData;
            SetRealOtherID();
        }

        public string ClearString(string data)
        {
            ///<summary>
            /// The function is cleaning the data from any unnecessary characters
            /// <param name="data">The data/param>
            /// <return> Cleared String </return>
            ///</summary>
            ///

            List<string> charsToRemove = new List<string>() { "[", "]", " "};
            foreach (var c in charsToRemove)
            {
                data = data.Replace(c, string.Empty);
            }
            return data.ToLower();
        }

        public string GetClientID()
        {
            ///<summary>
            /// Getter for client id
            /// <return>int of the client id </return>
            ///</summary>
            ///

            return this.clientID;
        }

        public string GetRuler()
        {
            ///<summary>
            /// Getter for ruler id
            /// <return>int of the ruler id </return>
            ///</summary>
            ///

            return this.rulerID;
        }

        public string GetStrong()
        {
            ///<summary>
            /// The function returns the strong character
            /// <return> Hokm sign </return>
            ///</summary>
            ///

            List<string> list = new List<string>() { "♠", "♣", "♦", "♥️" };
            int from = this.startData.IndexOf("strong:") + "strong:".Length;
            string strng = ClearString(this.startData.Substring(from));

            switch (strng)
            {
                case "spades":
                    return list[0];
                case "clubs":
                    return list[1];
                case "diamonds":
                    return list[2];
                case "hearts":
                    return list[3];
            }
            return strng;
        }

        public string[] GetTeams()
        {
            ///<summary>
            /// The function returns the teams
            /// <return> Array of the two teams </return>
            ///</summary>
            ///

            int pFrom = this.startData.IndexOf(",teams:") + ",teams:".Length;
            int pTo = this.startData.LastIndexOf(",strong");

            string teamsString = ClearString(this.startData.Substring(pFrom, pTo - pFrom));

            string[] teams = teamsString.Split('|');

            return teams;
        }
        
        /*
            Each player has its 'Real' ID and 'Fake ID'.
            The 'Real' ID means the id of the player on screen.
            The 'Fake' ID means the id that the server recognizes the player as
        */

        public void SetRealOtherID()
        {
            ///<summary>
            /// The function sets the 'real' ID of the players
            /// <return> None </return>
            ///</summary>
            ///

            string[] teams = GetTeams();
            if (teams[0].Contains(GetClientID()))
            {

                this.players[1] = teams[0].Replace(GetClientID(), "").Replace("+", "");

                this.players[2] = teams[1].Split("+")[0];
                this.players[0] = teams[1].Split("+")[1];
            }
            else
            {
                this.players[1] = teams[1].Replace(GetClientID(), "").Replace("+", "");

                this.players[2] = teams[0].Split("+")[0];
                this.players[0] = teams[0].Split("+")[1];
            }

        }

        public int GetRealPlayerID(string fakeID)
        {
            ///<summary>
            /// The function returns the 'real' ID of a player by a 'fake' ID
            /// <param name="fakeID">The 'fake' ID of the player</param>
            /// <return>int of the ID </return>
            ///</summary>
            ///

            // Console.WriteLine(this.players);
            for (int i = 0; i < this.players.Length; i++)
            {
                if (this.players[i] == fakeID)
                {
                    return i;
                }
            }
            return 0;
        }

        public string GetFakeID(int realID)
        {
            ///<summary>
            /// Getter for fake id
            /// <param name="realID">The 'real' ID of the player</param>
            /// <return>int of the ID </return>
            ///</summary>
            ///

            return this.players[realID];
        }

    }
}
