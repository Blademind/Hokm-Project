using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hokm;

namespace Hokm
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Client cli1 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1234);
            //Client cli2 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1235);
            //Client cli3 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1236);
            //Client cli4 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1237);



            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // reciving data
            string startData = "clubs*rank_2|diamonds*rank_2|spades*rank_3|hearts*rank_4|" +
            "spades*rank_A|clubs*rank_J|hearts*rank_7|spades*rank_8|diamonds*rank_9" +
            "|clubs*rank_K|clubs*rank_A|spades*rank_2|hearts*rank_8,teams:[1+3]|[2+4],strong:hearts";
            string clientID = "4";
            string ruler = "1";
            // got my data
            GameClient g = new GameClient();
            new Thread(
                () =>
                {
                    Application.Run(g);
                }
            ).Start();

            g.PlayOtherCard("clubs*rank_2", 2);

            while (true)
                Console.WriteLine("!!!!");



        }
    }
}
