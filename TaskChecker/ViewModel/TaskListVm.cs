using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskChecker.Model;
using TaskChecker.Tool;

namespace TaskChecker.ViewModel
{
    class TaskListVm : ViewModelBase
    {
        public string Title
        {
            get => Model.Title;
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        public List<Model.Task> Tasks => Model.Tasks;

        public TaskList Model { get; set; }

        public TaskListVm(TaskList model)
        {
            Model = model;
        }

        public void AddTask(string name)
        {
            Model.Task newTask = new Model.Task(name);

            Model.Tasks.Add(newTask);
        }

        public void RemoveTask()
        {

        }
    }
}
