using System;
using System.Collections.Generic;

namespace TaskChecker.Models
{
    [Serializable]
    public class Task
    {
        public enum Status
        {
            None,
            Todo,
            Doing,
            Done
        };

        #region Properties

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the task description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        public Status CurrentStatus { get; set; }

        /// <summary>
        /// Gets or sets the sub tasks.
        /// </summary>
        public List<Task> SubTasks { get; set; } = new List<Task>();

        /// <summary>
        /// Gets the task completion.
        /// </summary>
        public int Completion
        {
            get
            {
                if (CurrentStatus == Status.Done) return 100;

                if (CurrentStatus == Status.Todo) return 0;

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

        public Task()
        {
            Title = "";
            CurrentStatus = Status.Todo;
        }

        public Task(string title)
        {
            Title = title;
            CurrentStatus = Status.Todo;
        }

        #endregion Constructor
    }
}