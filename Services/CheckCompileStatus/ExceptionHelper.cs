using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckCompileStatus
{
    public static class ExceptionHelper
    {
        public static void SendException(string ex)
        {
            ConsoleColorHelper.Set(MessageType.Error);
            Console.WriteLine();
            Console.WriteLine(ex);
            ConsoleColorHelper.Reset();

            EventLogHelper.WriteEventLog(ex);
        }
    }
}
