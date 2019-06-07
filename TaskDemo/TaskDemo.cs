using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace TaskDemo
{
    public class TaskDemo
    {
        private readonly ITestOutputHelper _output;

        public TaskDemo(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BlockingTasks()
        {
            //assert
            var blockingTasks = Enumerable.Range(0, 100).Select(x => Task.Run(() => Thread.Sleep(100))).ToArray();

            //act
            Task.WaitAll(blockingTasks);
        }

        [Fact]
        public void NonBlockingTasks()
        {
            //assert
           Stopwatch t = new Stopwatch();
            t.Start();
            //var blockingTasks = CreateTasks(100, async () => await Task.Delay(1000));
            var blockingTasks = Enumerable.Range(0, 1000).Select(x => Task.Run(async () => await Task.Delay(100))).ToArray();

            //act
            Task.WaitAll(blockingTasks);
            _output.WriteLine("t.Elapsed = {0}", t.Elapsed);
        }

        [Fact]
        public void BlockingTasksThreads()
        {
            //assert
            var threads = new ConcurrentDictionary<int, object>();
            var blockingTasks = Enumerable.Range(0, 1000).Select(x => Task.Run(() =>
            {
                threads[Thread.CurrentThread.ManagedThreadId] = null;
                Thread.Sleep(100);
            })).ToArray();

            //act
            Task.WaitAll(blockingTasks);

            _output.WriteLine("threads count = {0}", threads.Count);
        }

        [Fact]
        public void NonBlockingTasksThreads()
        {
            //assert
            var threads = new ConcurrentDictionary<int, object>();
            var blockingTasks = Enumerable.Range(0, 1000).Select(x => Task.Run( async () =>
            {
                threads[Thread.CurrentThread.ManagedThreadId] = null;
                await Task.Delay(100);
            })).ToArray();

            //act
            Task.WaitAll(blockingTasks);

            _output.WriteLine("threads count = {0}", threads.Count);
        }
    }
}