using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace serverTextApp
{
    internal class Client
    {
        public int ClientId { get; set; }
        public TCP TCPClient { get; set; }

        public string Username { get; set; }

        public Client(int clientId, string username)
        {
            ClientId = clientId;
            Username = username;
            TCPClient = new TCP(clientId, username);
        }
    }

    internal class TCP
    {
        private readonly int id;
        private readonly string username;
        private int bufferSize;
        private byte[] buffer;
        public NetworkStream Stream { get; set; }

        public TCP(int id, string username)
        {
            this.id = id;
            this.username = username;
        }

        public void connect(TcpClient client)
        {
            Stream = client.GetStream();

            bufferSize = client.ReceiveBufferSize;
            buffer = new byte[bufferSize];

            Console.WriteLine(@"{0} has connected.", username);

            Stream.BeginRead(buffer, 0, bufferSize, new AsyncCallback(recieveCallBack), null);
        }
        
        private void recieveCallBack(IAsyncResult re)
        {
            try
            {
                Stream.EndRead(re);
                Server.sendToAllClients(username, buffer);
                buffer = new byte[bufferSize];
                Stream.BeginRead(buffer, 0, bufferSize, new AsyncCallback(recieveCallBack), null);
            } catch (IOException ex)
            {
                Server.removeClient(id);
            }
        }
    }
}
