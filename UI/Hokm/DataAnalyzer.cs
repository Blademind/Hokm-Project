using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Hokm
{
    class DataAnalyzer
    {
        private string startData;
        private string clientID;
        private string rulerID;
        private string[] players;
        public DataAnalyzer(string clientID,string rulerID, string startData)
        {
            this.clientID = clientID;
            this.rulerID = rulerID;
            this.startData = startData;
            this.players = new string[3];
            SetRealOtherID();
        }

        public DataAnalyzer()
        {
            this.players = new string[3];
        }


        public void SetClientID(string clientID)
        {
            this.clientID = clientID;
        }
        public void SetRulerID(string rulerID)
        {
            this.rulerID = rulerID;
        }

        public void SetStartData(string startData)
        {
            this.startData = startData;
            SetRealOtherID();
        }



        public string ClearString(string data)
        {
            List<string> charsToRemove = new List<string>() { "[", "]", " "};
            foreach (var c in charsToRemove)
            {
                data = data.Replace(c, string.Empty);
            }
            return data.ToLower();
        }

        public string GetClientID()
        {
            return this.clientID;
        }

        public string GetRuler()
        {
            return this.rulerID;
        }

        public string GetStrong()
        {
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
            int pFrom = this.startData.IndexOf(",teams:") + ",teams:".Length;
            int pTo = this.startData.LastIndexOf(",strong");

            string teamsString = ClearString(this.startData.Substring(pFrom, pTo - pFrom));

            string[] teams = teamsString.Split('|');

            return teams;
        }
    
        public void SetRealOtherID()
        {
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
            Console.WriteLine(this.players);
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
            return this.players[realID];
        }

    }
}
