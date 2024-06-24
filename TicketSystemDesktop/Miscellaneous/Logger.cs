using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystemDesktop.Miscellaneous
{
    public class Logger
    {
        public static void Log(LogStatus status, string caller, string message)
        {
            DateTime now = DateTime.Now;
            File.AppendAllText("./Log/", $"{now} {status}\nAt {caller}\n\nMessage:\n{message}");
        }
    }

    public enum LogStatus
    {
        Info,
        Warning, 
        Error
    }
}
