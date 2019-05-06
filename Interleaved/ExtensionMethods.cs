using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BAMCIS.Parallel.Interleaved
{
    /// <summary>
    /// Contains the Interleaved extension method.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Provides an IEnumerable of tasks that will be yielded as the tasks finish so each
        /// item can be processed in the order it finishes, not in the order that were added to the
        /// input IEnumerable or in the order they were started.
        /// 
        /// https://blogs.msdn.microsoft.com/pfxteam/2012/08/02/processing-tasks-as-they-complete/
        /// </summary>
        /// <typeparam name="T">The type of the return value from the async task</typeparam>
        /// <param name="tasks">The tasks that were launched and awaiting finish</param>
        /// <returns>The tasks that were input ordered by their finish time</returns>
        public static IEnumerable<Task<T>> Interleaved<T>(this IEnumerable<Task<T>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }

            List<Task<T>> inputTasks = tasks.ToList();

            TaskCompletionSource<T>[] buckets = new TaskCompletionSource<T>[inputTasks.Count];
            Task<T>[] results = new Task<T>[buckets.Length];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new TaskCompletionSource<T>();
                results[i] = buckets[i].Task;
            }

            int nextTaskIndex = -1;

            foreach (Task<T> inputTask in inputTasks)
            {
                inputTask.ContinueWith(completed =>
                {
                    TaskCompletionSource<T> bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];

                    if (completed.IsFaulted)
                    {
                        if (completed.Exception != null)
                        {
                            bucket.TrySetException(completed.Exception.InnerExceptions);
                        }
                    }
                    else if (completed.IsCanceled)
                    {
                        bucket.TrySetCanceled();
                    }
                    else
                    {
                        bucket.TrySetResult(completed.Result);
                    }

                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            return results;
        }
    }
}
