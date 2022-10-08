using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Hokm
{
	public class Client
	{
		private Socket client_sock;
		private string[] ip;
		private IPEndPoint ip_port;
		private IPEndPoint server_ip_port;
		private Byte[] buf;
		private int rec;
		private int msg_size;
		private string msg;
		private string msg_frag;
		private int port; // temp to delete

		public List<List<string>> deck { get; set; }
		public string ruler { get; set; }
		public string clientId { get; set; }

		public Client(IPAddress ip, int port)
		{
			this.port = port; // to delete
			this.client_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.buf = new byte[8];
			this.server_ip_port = new IPEndPoint(new IPAddress(new byte[4] { 192, 168, 1, 6 }), 55555);
			try
			{
				this.ip_port = new IPEndPoint(ip, port);
			}
			catch (Exception)
			{
				Console.WriteLine("was not able to bind said ip/port");
			}
			client_sock.Bind(ip_port);
			Server_Connect();
		}
		public void Server_Connect()
		{
			try
			{
				client_sock.Connect(this.server_ip_port);
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
			while (true)
			{
				this.rec = this.client_sock.Receive(this.buf);
				byte[] data = new byte[this.rec];
				Array.Copy(this.buf, data, this.rec);
				this.msg = "";
				this.msg_size = Int32.Parse(Encoding.ASCII.GetString(data));
				while (this.msg.Length < this.msg_size)
				{
					this.buf = new byte[this.msg_size - msg.Length];
					try
					{
						this.rec = this.client_sock.Receive(this.buf);
						data = new byte[this.rec];
						Array.Copy(this.buf, data, this.rec);
						this.msg_frag = Encoding.ASCII.GetString(data);
					}
					catch
					{
						Console.WriteLine("Error");
						break;
					}
					this.msg += this.msg_frag;
				}
				string new_msg = this.msg;
				MessageParser(new_msg);
				this.buf = new byte[8];
			}
		}

		public void MessageParser(string msg)
        {
            if (Char.IsUpper(msg[0])) // card deck
            {
                deck = new List<List<string>>();
                string[] cards = msg.Split("|");

                foreach (string card in cards)
                {
					string[] cardShapeSuit = card.Split("*");
					List<string> shapeSuit = new List<string>();
					foreach (string c in cardShapeSuit)
                    {
						shapeSuit.Add(c);
                    }
					deck.Add(shapeSuit);
                }
            }
            else // ruler or client_id
            {
				string[] splitMessage = msg.Split(":");
				if (splitMessage[0] == "ruler")
					ruler = splitMessage[1];

				else if (splitMessage[0] == "client_id")
					clientId = splitMessage[1];
            }
        }
	}
}
