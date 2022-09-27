using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Http.Headers;

namespace serverTextApp
{
    internal class Server
    {
        private static TcpListener listener;
        const int maxSize = 10;
        static Dictionary<int, Client> connectedClients = new Dictionary<int, Client>();
        const int byteSize = 1024 * 1024;

        internal static void Start(IPAddress address, int port)
        {
            IPEndPoint ep = new IPEndPoint(address, port);
            listener = new TcpListener(ep);
            listener.Start();

            listener.BeginAcceptTcpClient(new AsyncCallback(TCPconnection), null);

            Console.WriteLine(@"  
            ===================================================  
                   Started listening requests at: {0}:{1}  
            ===================================================",
            ep.Address, ep.Port);
        }

        private static void TCPconnection(IAsyncResult re)
        {
            TcpClient client = listener.EndAcceptTcpClient(re);
            
            if (connectedClients.Count <= maxSize)
            {
                byte[] nameBuffer = new byte[byteSize];
                client.GetStream().Read(nameBuffer, 0, byteSize);
                Client clientObj = new Client(connectedClients.Count, Encoding.Unicode.GetString(nameBuffer).TrimEnd('\0'));
                connectedClients.Add(connectedClients.Count, clientObj);
                clientObj.TCPClient.connect(client);
            }

            listener.BeginAcceptTcpClient(new AsyncCallback(TCPconnection), null);
        }

        public static void removeClient(int id)
        {
            connectedClients.Remove(id);
        }

        public static void sendToAllClients(string sender, byte[] message)
        {
            string messageString = sender + ": " + Encoding.Unicode.GetString(message).TrimEnd('\0') + "\n";
            byte[] messageBytes = Encoding.Unicode.GetBytes(messageString);

            Console.WriteLine(messageString);

            foreach (Client client in connectedClients.Values)
            {
                NetworkStream stream = client.TCPClient.Stream;
                stream.Write(messageBytes, 0, messageBytes.Length);
            }
        }
    }
}
