using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StateMachine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await Task.FromResul(true);
            var stateMachine = new AsyncStateMachine
            {
                _builder = AsyncTaskMethodBuilder<bool>.Create(),
                _state = -1
            };

            stateMachine._builder.Start(ref stateMachine);
            var result = stateMachine._builder.Task.Result;
        }

        public class CustomAwaiter : INotifyCompletion
        {
            public bool IsCompleted { get; set; }

            public bool GetResult()
            {
                return true;
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
            public AsyncTaskMethodBuilder<bool> _builder;
            //public StateMachine _this;
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
