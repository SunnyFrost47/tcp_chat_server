using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace ChatServerTCP
{
    class ServerTCP
    {
        static TcpListener tcpListener;
        List<ClientTCP> clients = new List<ClientTCP>();

        protected internal void AddConnection(ClientTCP clientTCP)
        {
            clients.Add(clientTCP);
        }

        protected internal void RemoveConnection(string id)
        {
            ClientTCP client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
            }
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientTCP clientTCP = new ClientTCP(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientTCP.Process));
                    clientThread.Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            foreach (var item in clients)
            {
                if (item.Id != id)
                {
                    item.Stream.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            foreach (var item in clients)
            {
                item.Close();
            }
            Environment.Exit(0);
        }
    }
}
