using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading.Channels;

namespace ChatServerTCP
{
    class ClientTCP
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerTCP server;

        public ClientTCP(TcpClient tcpClient, ServerTCP serverTCP)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverTCP;
            serverTCP.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                userName = message;

                message = userName + " вошёл в чат";
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        Stream = client.GetStream();
                        message = GetMessage();
                        //message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                    catch
                    {
                        message = String.Format("{0} покидает чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                if (bytes == 0) throw new Exception();
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null) Stream.Close();
            if (client != null) client.Close();
        }
    }
}
