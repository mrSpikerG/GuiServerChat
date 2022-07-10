using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerGUI
{
    public static class LoggerControl
    {

        public static void LogMessage(string msg, InfoLevel level)
        {
            Task.Factory.StartNew(() =>
            {

                string message = $"[{ DateTime.Now}] ";
                switch (level)
                {
                    case InfoLevel.INFO:

                        message += $"[Info]:  {msg}\n";
                        File.AppendAllText("Logs.txt", message);
                        File.AppendAllText("Debug.txt", message);
                        break;
                    case InfoLevel.WARN:
                        message += $"[Warn]:  {msg}\n";
                        File.AppendAllText("Debug.txt", message);
                        break;
                    case InfoLevel.ERROR:
                        message += $"[Error]:  {msg}\n";
                        File.AppendAllText("Debug.txt", message);
                        break;
                    case InfoLevel.DEBUG:
                        message += $"[Debug]:  {msg}\n";
                        File.AppendAllText("Debug.txt", message);
                        break;
                }
                MainServerForm.listBox1.Items.Add(message);
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
