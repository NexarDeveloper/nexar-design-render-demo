using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nexar.Renderer.Utilities
{
    public class ThreadHelper
    {
        private enum E_ThreadConfiguration : int
        {
            single = 0,
            multiple
        }

        public string ThreadName
        {
            get
            {
                return threadName;
            }
        }

        private const int THREAD_SLEEP_INTERVAL_MS = 10;

        private readonly int threadPeriodMS;

        private E_ThreadConfiguration threadConfiguration;
        private bool threadRunning = false;
        private string threadName;
        private int threadCount;
        private Thread? thread;
        private Action? threadCallback;
        private Func<Task>? threadCallbackAsync;
        private Action<object>? multiThreadCallback;
        private Action<Exception>? exceptionCallback;
        private Action? threadStartCallback;
        private Action<object>? multiThreadStartCallback;
        private Action? threadStopCallback;
        private Action<object>? multiThreadStopCallback;
        private List<Thread> multiThreads = new List<Thread>();
        private List<Thread> startCallbacksExecuted = new List<Thread>();
        private bool isBackground;

        private int ThreadSleepCountThreshold
        {
            get
            {
                return threadPeriodMS / THREAD_SLEEP_INTERVAL_MS;
            }
        }

        public ThreadHelper(
            string threadName,
            int threadPeriodMS,
            Func<Task> threadCallbackAsync,
            Action<Exception>? exceptionCallback = null,
            Action? threadStartCallback = null,
            Action? threadStopCallback = null,
            bool isBackground = false)
        {
            threadConfiguration = E_ThreadConfiguration.single;
            this.threadName = threadName;
            this.threadCallbackAsync = threadCallbackAsync;
            this.threadPeriodMS = threadPeriodMS;
            this.exceptionCallback = exceptionCallback;
            this.threadStartCallback = threadStartCallback;
            this.threadStopCallback = threadStopCallback;
            this.isBackground = isBackground;
        }


        public ThreadHelper(
            string threadName,
            int threadPeriodMS,
            Action threadCallback,
            Action<Exception>? exceptionCallback = null,
            Action? threadStartCallback = null,
            Action? threadStopCallback = null,
            bool isBackground = false)
        {
            threadConfiguration = E_ThreadConfiguration.single;
            this.threadName = threadName;
            this.threadCallback = threadCallback;
            this.threadPeriodMS = threadPeriodMS;
            this.exceptionCallback = exceptionCallback;
            this.threadStartCallback = threadStartCallback;
            this.threadStopCallback = threadStopCallback;
            this.isBackground = isBackground;
        }

        public ThreadHelper(
            string threadName,
            int threadCount,
            int threadPeriodMS,
            Action<object> multiThreadCallback,
            Action<Exception>? exceptionCallback = null,
            Action<object>? multiThreadStartCallback = null,
            Action<object>? multiThreadStopCallback = null,
            bool isBackground = false)
        {
            threadConfiguration = E_ThreadConfiguration.multiple;
            this.threadName = threadName;
            this.threadCount = threadCount;
            this.multiThreadCallback = multiThreadCallback;
            this.threadPeriodMS = threadPeriodMS;
            this.exceptionCallback = exceptionCallback;
            this.multiThreadStartCallback = multiThreadStartCallback;
            this.multiThreadStopCallback = multiThreadStopCallback;
            this.isBackground = isBackground;
        }

        public void StartThreads()
        {
            if (!threadRunning)
            {
                threadRunning = true;

                switch (threadConfiguration)
                {
                    case E_ThreadConfiguration.single:
                    {
                        if (thread == null)
                        {
                            thread = new Thread(ThreadHandler);
                            thread.Name = threadName;
                            thread.Priority = ThreadPriority.Normal;
                            thread.IsBackground = isBackground;

                            thread.Start();

                            threadStartCallback?.Invoke();
                        }

                        break;
                    }

                    case E_ThreadConfiguration.multiple:
                    {
                        if (multiThreads.Count == 0)
                        {
                            for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
                            {
                                Thread thread = new Thread(new ParameterizedThreadStart(ParameterizedThreadHandler));
                                multiThreads.Add(thread);
                                thread.Name = threadName + threadIndex.ToString();
                                thread.IsBackground = isBackground;
                                thread.Priority = ThreadPriority.Normal;
                            }

                            startCallbacksExecuted.Clear();

                            foreach (Thread thread in multiThreads)
                            {
                                if (multiThreadStartCallback != null)
                                {
                                    multiThreadStartCallback(multiThreads.IndexOf(thread));
                                    startCallbacksExecuted.Add(thread);
                                }

                                thread.Start(multiThreads.IndexOf(thread));
                            }
                        }

                        break;
                    }
                }
            }
        }

        public void RequestStopThreads()
        {
            threadRunning = false;
        }

        public void StopThreads()
        {
            RequestStopThreads();

            switch (threadConfiguration)
            {
                case E_ThreadConfiguration.single:
                    {
                        if (thread != null)
                        {
                            if (thread.IsAlive)
                            {
                                thread.Join();
                            }
                            thread = null;
                            threadStopCallback?.Invoke();
                        }

                        break;
                    }

                case E_ThreadConfiguration.multiple:
                    {
                        if (multiThreads.Count > 0)
                        {
                            foreach (Thread thread in multiThreads)
                            {
                                if (thread.IsAlive)
                                {
                                    thread.Join();
                                }
                            }
                        }

                        foreach (Thread thread in startCallbacksExecuted)
                        {
                            multiThreadStopCallback?.Invoke(multiThreads.IndexOf(thread));
                        }

                        startCallbacksExecuted.Clear();
                        multiThreads.Clear();

                        break;
                    }
            }
        }

        private void ThreadHandler()
        {
            ParameterizedThreadHandler(null);
        }

        private void ParameterizedThreadHandler(object? threadParameter)
        {
            Thread.Sleep(1000);
            int sleepCounter = 0;

            InvokeThreadHandler(threadParameter);

            while (threadRunning)
            {
                if (sleepCounter >= ThreadSleepCountThreshold)
                {
                    InvokeThreadHandler(threadParameter);
                    sleepCounter = 0;
                }
                else
                {
                    sleepCounter++;
                }

                Thread.Sleep(THREAD_SLEEP_INTERVAL_MS);
            }
        }

        private void InvokeThreadHandler(object? threadParameter)
        {
            try
            {
                if (threadParameter == null)
                {
                    if (threadCallback != null)
                    {
                        threadCallback.Invoke();
                    }
                    else if (threadCallbackAsync != null)
                    {
                        threadCallbackAsync().Wait();
                    }
                }
                else
                {
                    if (multiThreadCallback != null)
                    {
                        var asyncResult = threadCallback?.BeginInvoke(null, threadParameter);
                        threadCallback?.EndInvoke(asyncResult);
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionCallback?.Invoke(ex);
            }
        }
    }
}
