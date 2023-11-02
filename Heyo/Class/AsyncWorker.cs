using System;
using System.Threading;
using System.Windows.Threading;

namespace Heyo.Class
{
    public class AsyncWorker
    {
        public AsyncWorker(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public AsyncWorker()
        {
        }

        public Dispatcher Dispatcher { get; set; }

        public void InvokeAsync(Action method, Action callBack, Dispatcher dispatcher)
        {
            var thread = new Thread(
                    () =>
                    {
                        method();

                        if (callBack != null) dispatcher?.Invoke(callBack);
                    }
                )
                {IsBackground = true};
            thread.Start();
        }

        public void InvokeAsync(Action method, Action callBack)
        {
            InvokeAsync(method, callBack, Dispatcher);
        }

        public void InvokeAsync<T>(Func<T> method, Action<T> callBack, Dispatcher dispatcher)
        {
            var thread = new Thread(
                    () =>
                    {
                        var result = method();

                        if (callBack != null) dispatcher?.Invoke(callBack, result);
                    }
                )
                {IsBackground = true};
            thread.Start();
        }

        /// <summary>
        ///     异步调用method，结束后回到主线程中调用callback
        /// </summary>
        /// <typeparam name="T">method返回值类型</typeparam>
        /// <param name="method">具有一个返回值，没有参数的方法</param>
        /// <param name="callBack">具有一个参数的回调方法</param>
        public void InvokeAsync<T>(Func<T> method, Action<T> callBack)
        {
            if (Dispatcher == null) throw new Exception("Dispatcher can't be null");

            InvokeAsync(method, callBack, Dispatcher);
        }
    }
}