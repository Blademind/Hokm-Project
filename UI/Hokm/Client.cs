using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.DirectoryServices.ActiveDirectory;

namespace Hokm
{
    public partial class Client
    {
        /// <summary>
        /// First Part of Client Object which has the functions that are
        /// Related to the sockets level and server communication
        /// </summary>
        /// 

        // Initating socket variables
        public Socket clientSock;
        public IPEndPoint ipPort;
        public IPEndPoint serverIpPort;
        public Byte[] buf;
        public int rec;
        public int msgSize;
        public string msg;
        public string msgFrag;
        public string[] strongSuits;

        // Initiating game variables
        public List<string> deck = new List<string>();
        public int ruler { get; set; }
        public int clientId { get; set; }

        public Client(IPAddress ip, int port)
        {
            /// <summary>
            /// Function initiates client socket in order to connect with the server and 
            /// get starting information about the game
            /// <param name="ip"> the ip to connect to </param> 
            /// <param name="port"> the port in which we are connecting </param>
            /// <return> None </return>
            /// </summary>
            /// 

            this.serverIpPort = new IPEndPoint(new IPAddress(new byte[4] { 127, 0, 0, 1 }), 55555);
            //this.serverIpPort = new IPEndPoint(new IPAddress(new byte[4] { 127, 0, 0, 1 }), 55555);
            for (int i = 0; i < 4; i++)
            {
                idCard[i] = new List<string>();
            }
            try
            {
                this.ipPort = new IPEndPoint(ip, port);
            }
            catch (Exception)
            {
                Console.WriteLine("was not able to bind said ip/port");
            }
            //client_sock.Bind(ip_port);
            InitSock();

        }
        public void InitSock()
        {
            /// <summary>
            /// Restarts client and connects to server
            /// <return> None </return>
            /// </summary>
            /// 

            this.clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.buf = new byte[8];
            Server_Connect();
        }
        public void Server_Connect()
        {

            /// <summary>
            /// Function initiates client socket in order to connect with the server and 
            /// get starting information about the game
            /// <return> None </return>
            /// </summary>
            /// 

            try
            {
                clientSock.Connect(this.serverIpPort);
                Console.WriteLine("connected to server");
                Thread th = new Thread(new ThreadStart(Listen));
                th.Start();
            }
            catch (SocketException)
            {
                Console.WriteLine("was not able to connect to server");
            }
        }
        public void Listen()
        {
            ///<summary>
            /// Function takes care of listening to the server and receiving important information about game state
            /// <return> None </return>
            ///</summary>
            ///

            //try
            //{
            bool runOnce = true;
            bool endRun = true;

            while (true)
            {
                this.rec = this.clientSock.Receive(this.buf);
                byte[] data = new byte[this.rec];
                Array.Copy(this.buf, data, this.rec);
                this.msg = "";
                this.msgSize = Int32.Parse(Encoding.ASCII.GetString(data)); // the message's size
                // Receive from the server
                while (this.msg.Length < this.msgSize)
                {
                    this.buf = new byte[this.msgSize - msg.Length];
                    try
                    {
                        this.rec = this.clientSock.Receive(this.buf);
                        data = new byte[this.rec];
                        Array.Copy(this.buf, data, this.rec);
                        this.msgFrag = Encoding.ASCII.GetString(data);
                    }
                    catch
                    {
                        Console.WriteLine("Error");
                        break;
                    }
                    this.msg += this.msgFrag;
                }

                // Raw message from server
                string new_msg = this.msg;
                Console.WriteLine("raw_message: " + new_msg);
                if (msg == "SERVER_DISCONNECTED")
                {
                    this.clientSock.Close();
                    Console.WriteLine("Server has crashed, press any key when the server is back online. . .");
                    Console.ReadLine();
                    break;
                }
                // Parsing message
                new_msg = GameMessageParser(new_msg);

                if (new_msg.Length != 0)
                    Console.WriteLine(new_msg);

                // Summary has been sent, it's our turn
                if (msg.Contains("played_suit:"))
                {
                    // If we are client id number 3 handling socket errors with the server
                    Console.WriteLine("Its our turn");
                    //if (clientId == 3 && runOnce && ruler == 3)
                    //{

                    //    // Cleaning buffer
                    //    this.buf = new byte[8];
                    //    this.rec = this.clientSock.Receive(this.buf);
                    //    data = new byte[this.rec];
                    //    Array.Copy(this.buf, data, this.rec);

                    //    // Requesting data again
                    //    this.msgSize = Int32.Parse(Encoding.ASCII.GetString(data)); // the message's size
                    //    this.buf = new byte[this.msgSize];
                    //    this.rec = this.clientSock.Receive(this.buf);
                    //    data = new byte[this.rec];
                    //    Array.Copy(this.buf, data, this.rec);
                    //    new_msg = Encoding.ASCII.GetString(data);

                    //    GameMessageParser(new_msg);
                    //    runOnce = false;
                    //}

                    // Playing turn
                    PlayTurn(msg);
                }

                // Game has ended
                if (msg == "GAME_OVER")
                {
                    // print all the cards played during the game
                    foreach (var card in idCard)
                    {
                        Console.Write("[" + card.Key + ": ");
                        card.Value.ForEach(card => Console.Write(card + ", "));
                        Console.Write("] ");
                    }
                    endRun = false;
                    break;
                }

                // Checks whether we are the rulers
                if (new_msg.Contains("The ruler is: "))
                {
                    if (clientId == ruler)
                    {
                        Console.WriteLine("We are the rulers");
                    }
                }
                this.buf = new byte[8];
            }

            // If it's out first turn, initiate socket connection
            if (endRun)
            {
                InitSock();
                endRun = false;
            }

            //}


            // Server has disconnected
            //catch (Exception ex)
            //{
            //    if (ex is System.FormatException || ex is SocketException)
            //    {
            //        //Environment.Exit(0);
            //        //MessageBox.Show("Server is not responding, please make sure the server is running or try again later.");
            //        return;
            //    }
            //}
        }
    }
}
