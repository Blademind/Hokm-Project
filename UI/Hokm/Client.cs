using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Hokm
{
	public partial class Client
	{
        public Socket client_sock;
        public IPEndPoint ip_port;
        public IPEndPoint server_ip_port;
        public Byte[] buf;
        public int rec;
        public int msg_size;
        public string msg;
        public string msg_frag;
        public string[] strong_suits;


        public List<List<string>> deck { get; set; }
		public int ruler { get; set; }
		public int clientId { get; set; }

		public Client(IPAddress ip, int port)
		{
			this.client_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.buf = new byte[8];
			this.server_ip_port = new IPEndPoint(new IPAddress(new byte[4] { 192, 168, 1, 196 }), 55555);
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
				Console.WriteLine("raw_msg: "+ new_msg);
				new_msg = GameMessageParser(new_msg);
				if(new_msg.Length != 0)
					Console.WriteLine(new_msg);
				if (new_msg.Contains("The ruler is: ")){

					if (clientId == ruler) {
                        Console.WriteLine("We are the rulers");
						SendStrongSuit();
					}
                }
				this.buf = new byte[8];
			}
		}

	}
}
