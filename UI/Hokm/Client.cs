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
        public Socket clientSock;
        public IPEndPoint ipPort;
        public IPEndPoint serverIpPort;
        public Byte[] buf;
        public int rec;
        public int msgSize;
        public string msg;
        public string msgFrag;
        public string[] strongSuits;


        public List<string> deck = new List<string>();
		public int ruler { get; set; }
		public int clientId { get; set; }

		public Client(IPAddress ip, int port)
		{
			this.clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.buf = new byte[8];
			this.serverIpPort = new IPEndPoint(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 55555);
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
			Server_Connect();
		}
		public void Server_Connect()
		{
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
			while (true)
			{
				this.rec = this.clientSock.Receive(this.buf);
				byte[] data = new byte[this.rec];
				Array.Copy(this.buf, data, this.rec);
				this.msg = "";
				this.msgSize = Int32.Parse(Encoding.ASCII.GetString(data));  // the message's size
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
				string new_msg = this.msg;
				Console.WriteLine("raw_msg: "+ new_msg);
				new_msg = GameMessageParser(new_msg);

				if(new_msg.Length != 0)
					Console.WriteLine(new_msg);

				// summary has been sent, it's our turn
                if (msg.Contains("played_suit:"))
                {
                    Console.WriteLine("Its our turn");
                    PlayTurn(msg);
                }

				// game has ended
				if(msg == "GAME_OVER")
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
				// checks whether we are the rulers
                if (new_msg.Contains("The ruler is: ")){
					if (clientId == ruler) {
                        Console.WriteLine("We are the rulers");
					}
                }
				this.buf = new byte[8];
			}
		}

	}
}
