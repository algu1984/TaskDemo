using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncVoid
{
    /// <summary>
    /// This implementation of <see cref="SynchronizationContext"/> allows the developer to track the count
    /// of outstanding "async void" operations, and wait for them all to complete.
    /// </summary>
    public class AsyncTestSyncContext : SynchronizationContext
    {
        readonly AsyncManualResetEvent @event = new AsyncManualResetEvent(true);
        Exception exception;
        readonly SynchronizationContext innerContext;
        int operationCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTestSyncContext"/> class.
        /// </summary>
        /// <param name="innerContext">The existing synchronization context (may be <c>null</c>).</param>
        public AsyncTestSyncContext(SynchronizationContext innerContext)
        {
            this.innerContext = innerContext;
        }

        /// <inheritdoc/>
        public override void OperationCompleted()
        {
            var result = Interlocked.Decrement(ref operationCount);
            if (result == 0)
                @event.Set();
        }

        /// <inheritdoc/>
        public override void OperationStarted()
        {
            Interlocked.Increment(ref operationCount);
            @event.Reset();
        }

        /// <inheritdoc/>
        public override void Post(SendOrPostCallback d, object state)
        {
            // The call to Post() may be the state machine signaling that an exception is
            // about to be thrown, so we make sure the operation count gets incremented
            // before the Task.Run, and then decrement the count when the operation is done.
            OperationStarted();

            try
            {
                if (innerContext == null)
                {
                    XunitWorkerThread.QueueUserWorkItem(() =>
                    {
                        try
                        {
                            d(state);
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            OperationCompleted();
                        }
                    });
                }
                else
                    innerContext.Post(_ =>
                    {
                        try
                        {
                            d(state);
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            OperationCompleted();
                        }
                    }, null);
            }
            catch { }
        }

        /// <inheritdoc/>
        public override void Send(SendOrPostCallback d, object state)
        {
            try
            {
                if (innerContext != null)
                    innerContext.Send(d, state);
                else
                    d(state);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        /// <summary>
        /// Returns a task which is signaled when all outstanding operations are complete.
        /// </summary>
        public async Task<Exception> WaitForCompletionAsync()
        {
            await @event.WaitAsync();

            return exception;
        }
    }

    class AsyncManualResetEvent
    {
        volatile TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        public AsyncManualResetEvent(bool signaled = false)
        {
            if (signaled)
                taskCompletionSource.TrySetResult(true);
        }

        public bool IsSet
        {
            get { return taskCompletionSource.Task.IsCompleted; }
        }

        public Task WaitAsync()
        {
            return taskCompletionSource.Task;
        }

        public void Set()
        {
            taskCompletionSource.TrySetResult(true);
        }

        public void Reset()
        {
            if (IsSet)
                taskCompletionSource = new TaskCompletionSource<bool>();
        }
    }

    class XunitWorkerThread : IDisposable
    {
        readonly Thread thread;

        public XunitWorkerThread(Action threadProc)
        {
            thread = new Thread(s => ((Action)s)()) { IsBackground = true };
            thread.Start(threadProc);
        }

        public void Dispose() { }

        public void Join()
        {
            if (thread != Thread.CurrentThread)
                thread.Join();
        }

        public static void QueueUserWorkItem(Action backgroundTask, EventWaitHandle finished = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var state = (State)_;

                try
                {
                    state.BackgroundTask();
                }
                finally
                {
                    if (state.Finished != null)
                        state.Finished.Set();
                }
            },
                                         new State { BackgroundTask = backgroundTask, Finished = finished });
        }

        class State
        {
            public Action BackgroundTask;
            public EventWaitHandle Finished;
        }
    }
}