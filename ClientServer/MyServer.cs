using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServer
{
    internal class MyServer
    {
        static TcpListener tcpListener;
        List<MyClient> clients;
        readonly int PORT;

        public MyServer(int port = 8008)
        {
            clients = new List<MyClient>();
            this.PORT = port;


            if (!File.Exists("BannedUser.txt")) { File.Create("BannedUser.txt"); }
            if (!File.Exists("RegistredUser.txt")) { File.Create("RegistredUser.txt"); }


        }

        internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, PORT);
                tcpListener.Start();

                LoggerUtil.LogMessage("SERVER START", InfoLevel.INFO);

                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    MyClient myClient = new MyClient(client, this);
                    Thread clientThread = new Thread(new ThreadStart(myClient.Work));
                    clientThread.Start();


                    Task.Factory.StartNew(() =>
                    {
                        do
                        {
                            string cmd = Console.ReadLine();
                            if (cmd.StartsWith("ban "))
                            {
                                string[] command = cmd.Split(' ');
                                string ip = command[1];
                                File.AppendAllText("BannedUser.txt", $"{ip}\n");
                                LoggerUtil.LogMessage($"{ip} banned", InfoLevel.INFO);
                            }
                        } while (true);
                    });

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseServer();
            }
        }

        internal void BroadcastMsg(string msg, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].networkStream.Write(data, 0, data.Length);
                    LoggerUtil.LogMessage($"{id} send message: {msg}", InfoLevel.INFO);
                }
            }
        }

        internal void DeleteConnetion(string id)
        {
            MyClient client = clients.FirstOrDefault(x => x.Id.Equals(id));
            if (client != null)
            {
                clients.Remove(client);
                LoggerUtil.LogMessage($"remove connection {client.Name}", InfoLevel.DEBUG);
                return;
            }
        }

        internal void AddConnection(MyClient myClient)
        {
            clients.Add(myClient);
            LoggerUtil.LogMessage($"add connection {myClient.Name}", InfoLevel.DEBUG);
        }

        internal void CloseServer()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            LoggerUtil.LogMessage("SERVER STOP", InfoLevel.INFO);
        }


    }
}
