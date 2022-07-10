using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ServerGUI
{
    public partial class MainServerForm : Form
    {

        private MyServer server = new MyServer();
        public MainServerForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.FormClosing += CloseServer;

            this.textBox1.KeyUp += SendMsg;
            this.button1.Click += banUser;
            this.button2.Click += sendPrivateMsg;

            try
            {
                Thread thread = new Thread(new ThreadStart(server.Listen));
                thread.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                server.CloseServer();
            }
        }



        private void SendMsg(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                if (textBox1.Text != String.Empty)
                {
                    LoggerControl.LogMessage("send message from console", InfoLevel.INFO);
                    server.BroadcastMsg($"[{DateTime.Now}] {textBox1.Text}", "");
                    textBox1.Text = String.Empty;
                }
            }
        }

        public void sendPrivateMsg(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null && this.textBox1.Text != String.Empty)
            {
                listBox2.Items.Add(listBox2.SelectedItem.ToString().Substring(0, 36));

                server.PrivateMsg(this.textBox1.Text, listBox2.SelectedItem.ToString().Substring(0, 36));
                this.textBox1.Text = String.Empty;
            }
        }

        public void banUser(object sender, EventArgs e)
        {

            if (listBox2.SelectedItem != null)
            {

                foreach (var item in server.clients)
                {
                    if (item.Id.Equals(listBox2.SelectedItem.ToString().Substring(0,36)))
                    {
                        string ip = item.Ip;
                        File.AppendAllText("BannedUser.txt", $"{ip}\n");
                        LoggerControl.LogMessage($"{ip} banned", InfoLevel.INFO);
                        break;
                    }
                }
            }
        }


        private void CloseServer(object sender, FormClosingEventArgs e)
        {
            server.CloseServer();
        }
    }
}
