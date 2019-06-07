using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaitTypes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await "123";

            Console.ReadKey();
        }
    }
}
