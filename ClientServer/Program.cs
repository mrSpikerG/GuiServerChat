using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyServer server = new MyServer();
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

            Console.ReadKey();
        }
    }
}
