using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Linq;

namespace Hokm
{
    public partial class Client
    {

        // Initating socket variables
        public Socket clientSock;
        public IPEndPoint serverIpPort;
        public Byte[] buf;
        public int rec;
        public int msgSize;
        public string msg;
        public string msgFrag;
        public string[] strongSuits;
        public static string startingData;
        public GameClient gameClient;


        // Initiating game variables
        public List<string> deck = new List<string>();
        public static int ruler { get; set; }
        public static int clientId { get; set; }

        public void initSock()
        {
            this.clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.buf = new byte[8];
            ServerConnect();
        }
        public Client(IPAddress ip, int port)
        {
            /// <summary>
            /// Function initiates client socket in order to connect with the server and 
            /// get starting information about the game
            /// <param name="ip"> the ip to connect to </param> 
            /// <param name="port"> the port in which we are connecting </param>
            /// <return> None </return>
            /// </summary>
            this.serverIpPort = new IPEndPoint(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 55555);
            for (int i = 0; i < 4; i++)
            {
                idCard[i] = new List<string>();
            }
            //client_sock.Bind(ip_port);
            initSock();
        }
        public void ServerConnect()
        {

            /// <summary>
            /// Function initiates client socket in order to connect with the server and 
            /// get starting information about the game
            /// <return> None </return>
            /// </summary>

            try
            {
                clientSock.Connect(this.serverIpPort);
                Console.WriteLine("connected to server");
                Listen();
                //Thread th = new Thread(new ThreadStart(Listen));
                //th.Start();
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

                // Parsing message
                new_msg = GameMessageParser(new_msg);

                if (new_msg.Length != 0)
                    Console.WriteLine(new_msg);

                // Summary has been sent, it's our turn
                if (msg.Contains("played_suit:"))
                {
                    Console.WriteLine("Its our turn");
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
