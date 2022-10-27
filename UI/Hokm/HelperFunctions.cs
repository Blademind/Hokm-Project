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

namespace Hokm
{
    public static class HelperFunctions
    {
        /// <summary>
        /// Static class contains helper functions for client object
        /// </summary>
        /// 

        public static Dictionary<string, int> MakeCounter(List<string> deck)
        {
            ///<summary>
            /// Function makes a counter dictionary of the current deck (SPADES:5 , DIAMONDS: 3....)
            /// <param name="deck"> the current deck</param>
            /// <return> a counter dictioary of the current deck (as Dictionary<string, int>)</return>
            ///</summary>
            ///

            List<string> d = new List<string>();
            foreach (string card in deck)
            {
                d.Add(card.Split("*")[0]);
            }
            Dictionary<string, int> s = d.GroupBy(p => p).OrderBy(r => r.Count()).ToDictionary(q => q.Key, q => q.Count());
            return s;
        }
    }
}
