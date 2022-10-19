using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine(@"
  __   ___   ___  ___  ______       ___  ______  __   _ 
 |. | /_  | |_  ||_  ||____  |     |_  ||____  | \ \ | |
  | |   | |   | |  |_|     | |       | |     | |  \ \| |
  | |___| | __| |     _____| |_      | |     | | __\ ` |
  |_______||____|    /________/      | |     |_||______|
                                     |_|                
              ____  ___  __   __._______                
             |__  ||_  | \ \ / /|.  __  |               
                | |  |_|  \ V /  | |  | |               
            ____| |     ___\  \  | | _| |               
           /____/\_\   |______|  |_||___|               ");
           Console.WriteLine(@"
           .------..------..------..------.
           |H.--. ||O.--. ||K.--. ||M.--. |
           | :/\: || :/\: || :/\: || (\/) |
           | (__) || :\/: || :\/: || :\/: |
           | '--'H|| '--'O|| '--'K|| '--'M|
           `------'`------'`------'`------'");
            Thread.Sleep(2000);
            //Client cli1 = new Client(new IPAddress(new byte[4] { 127, 0, 0, 1 }), 1234);
            Client cli1 = new Client(new IPAddress(new byte[4] { 192, 168, 0, 176 }), 1234);
            //Client cli2 = new Client(new IPAddress(new byte[4] { 192, 168, 0, 176 }), 1235);
            //Client cli3 = new Client(new IPAddress(new byte[4] { 192, 168, 0, 176 }), 1236);
            //Client cli4 = new Client(new IPAddress(new byte[4] { 192, 168, 0, 176 }), 1237);
        }
    }
}
