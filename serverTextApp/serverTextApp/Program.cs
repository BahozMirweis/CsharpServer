using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using serverTextApp;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Start(IPAddress.Loopback, 1234); 
            Console.ReadLine();
        }
    }
}