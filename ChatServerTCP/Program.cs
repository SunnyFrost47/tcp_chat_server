using System;
using System.Threading;

namespace ChatServerTCP
{
    class Program
    {
        static ServerTCP server;
        static Thread listenThread;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerTCP();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
