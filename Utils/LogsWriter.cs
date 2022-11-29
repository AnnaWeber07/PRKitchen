using System;
using System.Threading;

namespace AnnaWebKitchenFin.Utils
{
    public class LogsWriter
    {
        public static void Log(string log)
        {
            Console.WriteLine($"{GetThreadId()}: {log}");
        }

        private static string GetThreadId()
        {
            var idx = Thread.CurrentThread.ManagedThreadId;
            return $"{DateTime.Now:HH:mm:ss:ffff} (Thread {idx})";
        }
    }
}