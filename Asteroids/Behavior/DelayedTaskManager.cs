using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Asteroids
{
    internal class DelayedTaskManager : Behavior, IDisposable
    {
        private SortedDictionary<long, Action> taskDictionary;
        private KeyValuePair<long, Action> nearest;
        private Stopwatch stopwatch;

        public DelayedTaskManager(GameObject gameObject) : base(gameObject)
        {
            taskDictionary = new SortedDictionary<long, Action>();
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        internal override void OnFrame()
        {
            if (nearest.Key != 0 && nearest.Key <= stopwatch.ElapsedMilliseconds)
            {
                nearest.Value?.Invoke();
                taskDictionary.Remove(nearest.Key);
                nearest = taskDictionary.FirstOrDefault();
            }
        }

        public void Invoke(long delay, Action task)
        {
            long invokationTime = stopwatch.ElapsedMilliseconds + delay;
            while (taskDictionary.ContainsKey(invokationTime))
                invokationTime++;
            taskDictionary.Add(invokationTime, task);
            if (nearest.Key == 0 || nearest.Key > invokationTime) nearest = taskDictionary.First();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            taskDictionary.Clear();
        }
    }
}
