using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerGUI
{
    internal class MyClient
    {
        public string Name { get; set; }

        private MyServer _server;

        public string Id { get; private set; }
        public string Ip { get; private set; }
        protected TcpClient tcpClient;
        internal NetworkStream networkStream { get; set; }

        public MyClient(TcpClient tcpClient, MyServer myServer)
        {
            Id = Guid.NewGuid().ToString();
            _server = myServer;
            this.tcpClient = tcpClient;
            _server.AddConnection(this);
        }

        public void Work()
        {
            try
            {
                networkStream = tcpClient.GetStream();
                this.Name = GetMsg();

                //
                //  SaveUserIp
                //
                this.Ip= GetMsg();
                bool alreadyRegistred = false;
                string[] registredUsers = File.ReadAllText("RegistredUser.txt").Split('\n');
                foreach (string ip in registredUsers)
                {
                    if (this.Ip == ip) { alreadyRegistred = true; }
                }
                if (!alreadyRegistred) { File.AppendAllText("RegistredUser.txt", $"{this.Ip}\n"); }


                if (MainServerForm.listBox2.Items.IndexOf(this.Id.ToString()) != -1)
                {
                    MainServerForm.listBox2.Items[MainServerForm.listBox2.Items.IndexOf(this.Id.ToString())] += $" | {this.Name}";
                }
                


                string msg = $"{this.Name} in chat!";
                string date;
                _server.BroadcastMsg(msg, this.Id);
                Console.WriteLine(msg);

                while (true)
                {
                    try
                    {
                        msg = GetMsg();
                        msg = $"[{GetMsg()}] {Name} : {msg}";
                        

                        

                        string[] bannedUsers = File.ReadAllText("BannedUser.txt").Split('\n');

                        


                        //
                        //  checkForBan
                        //
                        bool canSend = true;
                        foreach (string ip in bannedUsers)
                        {
                            if (ip == this.Ip) { canSend = false; }
                        }

                        if (canSend)
                        {
                            _server.BroadcastMsg(msg, this.Id);
                            Console.WriteLine($"{this.Ip}\t" + msg);
                            LoggerControl.LogMessage($"{this.Ip}\t" + msg, InfoLevel.INFO);
                        }
                        else
                        {
                           LoggerControl.LogMessage($"user {this.Name} try to send message but he can't",InfoLevel.INFO);
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = $"{this.Name} OUT OF chat!";

                        //  send to user info
                        _server.BroadcastMsg(msg, this.Id);
                        
                        //  log info
                        LoggerControl.LogMessage(msg,InfoLevel.INFO);
                        
                        //  Write in server console
                        Console.WriteLine(msg);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerControl.LogMessage(ex.Message, InfoLevel.ERROR);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _server.DeleteConnetion(this.Id);
                Close();
            }
        }

        public string GetMsg()
        {
            byte[] data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int byteCount = 0;
            do
            {
                byteCount = networkStream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, byteCount));
            } while (networkStream.DataAvailable);

            return builder.ToString();
        }

        public void Close()
        {
            tcpClient.Close();
            networkStream.Close();
        }
    }
}
