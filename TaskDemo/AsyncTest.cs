using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TaskDemo
{
    public class AsyncTest
    {
        [Fact]
        public void Test1()
        {
            Action act = TestWithAwait;
            try
            {
                act();
            }
            catch (Exception e)
            {
            }
            
        }

        [Fact]
        public void Test2()
        {
            Action act = TestWithoutAwait;
            try
            {
                act();
            }
            catch (Exception e)
            {
            }
        }

        private void TestWithoutAwait()
        {
            Test();
        }

        private async void TestWithAwait()
        {
            await Test();
        }

        private async Task Test()
        {
            throw new Exception("ex");
        }

    }
}