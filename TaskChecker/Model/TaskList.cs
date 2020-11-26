using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskChecker.Model
{
    class TaskList
    {
        public string Title { get; set; }

        public List<Task> Tasks { get; set; }

        public TaskList(string title)
        {
            Title = title;
            Tasks = new List<Task>();
        }

        public void AddTask(Model.Task task, Model.Task parent = null)
        {
            if (parent != null && Tasks.Contains(parent))
            {
                var parentTask = Tasks.First(t => t == parent);
                parentTask.SubTasks.Add(task);
            }
            else
            {
                Tasks.Add(task);
            }
        }
        public void AddTask(string title, Model.Task parent = null)
        {
            Model.Task newTask = new Model.Task(title);

            AddTask(newTask, parent);
        }
    }
}
