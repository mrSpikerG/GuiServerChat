using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerGUI
{
    internal class MyServer
    {
        internal static TcpListener tcpListener;
        internal List<MyClient> clients;
        internal readonly int PORT;

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

                LoggerControl.LogMessage("SERVER START", InfoLevel.INFO);

                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    MyClient myClient = new MyClient(client, this);
                    Thread clientThread = new Thread(new ThreadStart(myClient.Work));
                    clientThread.Start();




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
                    LoggerControl.LogMessage($"{id} send message: {msg}", InfoLevel.INFO);
                }
            }
        }

        internal void PrivateMsg(string msg, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id)
                {
                    clients[i].networkStream.Write(data, 0, data.Length);
                    LoggerControl.LogMessage($"{id} send message: {msg}", InfoLevel.INFO);
                }
            }
        }

        internal void DeleteConnetion(string id)
        {
            MyClient client = clients.FirstOrDefault(x => x.Id.Equals(id));
            if (client != null)
            {
                clients.Remove(client);

                foreach (string item in MainServerForm.listBox2.Items)
                {
                    if (client.Id.Equals(item.Substring(0, 36))){
                        MainServerForm.listBox2.Items.Remove(item);
                        break;
                    }
                }
                LoggerControl.LogMessage($"remove connection {client.Name}", InfoLevel.DEBUG);
                return;
            }
        }

        internal void AddConnection(MyClient myClient)
        {
            clients.Add(myClient);
            MainServerForm.listBox2.Items.Add($"{myClient.Id}");
            LoggerControl.LogMessage($"add connection {myClient.Name}", InfoLevel.DEBUG);
        }

        internal void CloseServer()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            LoggerControl.LogMessage("SERVER STOP", InfoLevel.INFO);
        }


    }
}
