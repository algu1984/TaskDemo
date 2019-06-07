using System;
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Task> tasks = new List<Task>();

            for (int i=0; i<1000; i++)
            {
                tasks.Add(Task.Run(async () => await Task.Delay(100)));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("t.Elapsed = {0}", sw.Elapsed);
            Console.ReadKey();
        }
    }
}
