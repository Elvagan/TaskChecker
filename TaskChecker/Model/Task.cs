using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskChecker.Model
{
    internal class Task
    {
        public enum Status
        {
            Todo,
            Doing,
            Done
        };

        #region Properties

        public string Title { get; set; }

        public Status CurrentStatus { get; set; }

        public List<Task> SubTasks { get; set; } = new List<Task>();

        public int Completion
        {
            get
            {
                if (CurrentStatus == Status.Done) return 100;
                else if (CurrentStatus == Status.Todo) return 0;

                double completion = 0.0;
                if (SubTasks.Count > 0)
                {
                    foreach (var subTask in SubTasks)
                    {
                        completion += subTask.Completion;
                    }

                    completion /= SubTasks.Count;
                }

                return (int)Math.Round(completion);
            }
        }

        #endregion Properties

        #region Constructor

        public Task(string title)
        {
            Title = title;
            CurrentStatus = Status.Todo;
        }

        #endregion Constructor
    }
}