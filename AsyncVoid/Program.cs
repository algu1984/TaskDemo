using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncVoid
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var context = new AsyncTestSyncContext(SynchronizationContext.Current);
            SynchronizationContext.SetSynchronizationContext(context);
            Test();
            await context.WaitForCompletionAsync();
        }

        private static async void Test()
        {
            await Task.Delay(1000);

            Console.ReadKey();
        }
    }
}
