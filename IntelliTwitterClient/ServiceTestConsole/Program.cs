using System.Diagnostics;
using IntelliTwitterClient.Business;
using IntelliTwitterClient.Business.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog log = new EventLog();

            log.Source = "C:/Users/Daniel van Cann/Documents/GitHub/nl.fhict.intellicloud.backend/IntelliTwitterClient/IntelliTwitterClient/bin/Debug/log";

            TwitterManager manager = new TwitterManager(log);

            manager.StartStreaming();

            Console.ReadLine();
        }
    }
}
