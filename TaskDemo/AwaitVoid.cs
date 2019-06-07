using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TaskDemo
{
    [TestClass]
    public class AwaitVoid
    {
        [TestMethod]
        public async void AsyncVoidXunit()
        {
            await Task.Delay(1000);

            //throw new Exception();
        }
    }
}
