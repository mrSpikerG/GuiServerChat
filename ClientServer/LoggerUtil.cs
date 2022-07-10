using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    public static class LoggerUtil
    {

        public static void LogMessage(string msg, InfoLevel level)
        {
            Task.Factory.StartNew(() =>
            {
                switch (level)
                {
                    case InfoLevel.INFO:
                        File.AppendAllText("Logs.txt", $"[{DateTime.Now}] [Info]:  {msg}\n");
                        File.AppendAllText("Debug.txt", $"[{DateTime.Now}] [Info]:  {msg}\n");
                        break;
                    case InfoLevel.WARN:
                        File.AppendAllText("Debug.txt", $"[{DateTime.Now}] [Warn]:  {msg}\n");
                        break;
                    case InfoLevel.ERROR:
                        File.AppendAllText("Debug.txt", $"[{DateTime.Now}] [Error]:  {msg}\n");
                        break;
                    case InfoLevel.DEBUG:
                        File.AppendAllText("Debug.txt", $"[{DateTime.Now}] [Debug]:  {msg}\n");
                        break;
                }
            });
        }

    }
    public enum InfoLevel
    {
        INFO,
        DEBUG,
        WARN,
        ERROR
    }
}
