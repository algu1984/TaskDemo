using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TaskDemo
{
    public class StateMachine
    {
        [Fact]
        public void FactMethodName()
        {
            //await Task.FromResul(100)
            var t = new AsyncStateMachine
            {
                _this = this,
                _builder = AsyncTaskMethodBuilder<int>.Create(),
                _state = -1
            };

            t._builder.Start(ref t);
            t._builder.Task.Result.Should().Be(10);
        }

        public class CustomAwaiter : INotifyCompletion
        {
            public bool IsCompleted { get; set; }

            public int GetResult()
            {
                return 10;
            }
            public void OnCompleted(Action continuation)
            {
                //working hard ......
                Thread.Sleep(100);

                continuation();
                IsCompleted = true;
            }
        }



        private class AsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> _builder;
            public StateMachine _this;
            public int _state;
            private CustomAwaiter _taskAwaiter;

            public void MoveNext()
            {
                int num = _state;
                var taskAwaiter = new CustomAwaiter();

                if (num != 0)
                {
                    if (!taskAwaiter.IsCompleted)
                    {
                        _state = 0;
                        _taskAwaiter = taskAwaiter;
                        AsyncStateMachine state = this;
                        _builder.AwaitOnCompleted(ref taskAwaiter, ref state);
                        return;
                    }

                }
                var res = taskAwaiter.GetResult();
                taskAwaiter = null;

                _state = -2;
                _builder.SetResult(res);
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                //throw new System.NotImplementedException();
            }
        }
    }

    
}