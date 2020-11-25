using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
