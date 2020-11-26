using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskChecker.Model
{

    class Task
    {
        public enum State
        {
            Todo,
            Doing,
            Done
        };

        #region Properties

        public string Title { get; set; }

        public State CurrentState { get; set; }

        public List<Task> SubTasks { get; set; } = new List<Task>();

        public int Completion
        {
            get
            {
                if (CurrentState == State.Done) return 100;
                else if (CurrentState == State.Todo) return 0;

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

        #endregion

        #region Constructor

        public Task(string title)
        {
            Title = title;
            CurrentState = State.Todo;
        }

        #endregion

    }
}
