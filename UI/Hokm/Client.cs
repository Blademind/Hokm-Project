using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

		public Client(IPAddress ip, int port)
		{
			this.client_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.buf = new byte[1024];
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
				Console.WriteLine(Encoding.ASCII.GetString(data));
			}
		}
	}
}
