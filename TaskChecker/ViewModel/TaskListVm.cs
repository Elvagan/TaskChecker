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
    internal class TaskListVm : ViewModelBase
    {
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string _title;

        public ObservableCollection<TaskVm> Tasks { get; set; } = new ObservableCollection<TaskVm>();

        public TaskListVm(string title)
        {
            _title = title;
            /*Tasks = new ObservableCollection<TaskVm>(Model.Tasks.Select(t => new TaskVm(t)));

            foreach (var taskVm in Tasks)
            {
                taskVm.AskDelete += OnTaskVmAskDelete;
            }*/
        }

        public void AddTask(string name, Model.Task parent = null)
        {
            Model.Task newTask = new Model.Task(name);
            AddTask(newTask, parent);
        }

        public void AddTask(Model.Task task, Model.Task parent = null)
        {
            if (parent != null)
            {
                TaskVm parentVm = Tasks.FirstOrDefault(t => t.Model == parent);

                parentVm?.AddSubTask(task);
            }
            else
            {
                TaskVm newVm = new TaskVm(task);
                newVm.AskDelete += OnTaskVmAskDelete;
                Tasks.Add(newVm);
            }
        }

        public void RemoveTask()
        {
        }

        #region Callback

        private void OnTaskVmAskDelete(TaskVm taskVm)
        {
            taskVm.AskDelete -= OnTaskVmAskDelete;
            Tasks.Remove(taskVm);
        }

        #endregion Callback

        #region Commands

        public ICommand CreateNewTaskCommand => _createNewTaskCommand ?? (_createNewTaskCommand = new RelayCommand(CreateNewTask));

        private RelayCommand _createNewTaskCommand;

        private void CreateNewTask(object sender)
        {
            AddTask("");
        }

        #endregion Commands
    }
}