using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
        
        public ObservableCollection<TaskVm> Tasks { get; set; }

        public TaskList Model { get; set; }

        public TaskListVm(TaskList model)
        {
            Model = model;
            Tasks = new ObservableCollection<TaskVm>(Model.Tasks.Select(t => new TaskVm(t)));
        }

        public void AddTask(string name)
        {
            Model.Task newTask = new Model.Task(name);

            Model.Tasks.Add(newTask);
        }

        public void RemoveTask()
        {

        }

        #region Commands

        public ICommand CreateNewTaskCommand => _createNewTaskCommand ?? (_createNewTaskCommand = new RelayCommand(CreateNewTask));

        private RelayCommand _createNewTaskCommand;

        private void CreateNewTask(object sender)
        {
            AddTask("");
        }
        
        #endregion
    }
}
