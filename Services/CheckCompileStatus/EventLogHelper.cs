using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CheckCompileStatus
{
    public static class EventLogHelper
    {
        public static void WriteEventLog(string error)
        {
            WriteEventLog(error, EventLogEntryType.Error);
        }

        public static void WriteEventLog(string error, EventLogEntryType eventType)
        {
            if (!EventLog.SourceExists("CheckCompileStatus"))
                EventLog.CreateEventSource("CheckCompileStatus", "CheckCompileStatus");

            using (EventLog log = new EventLog("CheckCompileStatus"))
            {
                log.Source = "CheckCompileStatus";
                log.WriteEntry(error, eventType);
            }
        }
    }
}
