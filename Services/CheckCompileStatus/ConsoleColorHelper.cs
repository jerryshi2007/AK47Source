using System;

namespace CheckCompileStatus
{
    public enum MessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        High = 4,
    }

    public static class ConsoleColorHelper
    {
        public static void Set(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageType.High:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }
        }

        public static void Reset()
        {
            Console.ResetColor();
        }
    }
}
