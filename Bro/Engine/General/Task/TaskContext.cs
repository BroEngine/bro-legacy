using System;
using System.Collections.Generic;

namespace Bro
{
    public class TaskContext : ITaskContext, IDisposable
    {
        private List<Task> _tasks = new List<Task>();

        public void Dispose()
        {
            if (_tasks.Count > 0)
            {
                var taskToDispose = _tasks;
                _tasks = new List<Task>();
                foreach (var task in taskToDispose)
                {
                    task.Terminate();
                }
            }
        }

        public void Add(Task task)
        {
            _tasks.Add(task);
        }

        public void Remove(Task task)
        {
            _tasks.FastRemove(task);
        }
    }
}