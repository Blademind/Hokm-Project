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
            Client cli1 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1234);
            Client cli2 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1235);
            Client cli3 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1236);
            Client cli4 = new Client(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 1237);



            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Game());
        }
    }
}
