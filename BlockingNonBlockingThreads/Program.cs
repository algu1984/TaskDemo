using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockingNonBlocking
{
    class Program
    {
        static async Task Main(string[] args)
        {
            List<Task> tasks = new List<Task>();

            var threads = new ConcurrentDictionary<int, object>();

            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run( () =>
                {
                    threads[Thread.CurrentThread.ManagedThreadId] = null;
                    Thread.Sleep(100);
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("threads count = {0}", threads.Count);
            Console.ReadKey();
        }
    }
}
