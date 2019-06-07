using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AwaitTypes
{
    public static class StringExtensions
    {
        public static MyAwaiter GetAwaiter(this string s)
        {
            return new MyAwaiter();
        }
    }

    public class MyAwaiter : INotifyCompletion
    {
        public bool IsCompleted
        {
            get { return false; }
        }

        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        public void GetResult()
        {

        }
    }
}
