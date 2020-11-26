using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskChecker.Tool;
using Task = TaskChecker.Model.Task;

namespace TaskChecker.ViewModel
{
    class TaskVm : ViewModelBase
    {
        #region Events

        /// <summary>
        /// Occurs when the status has changed.
        /// </summary>
        public event Action StatusChanged;

        #endregion

        public string Title
        {
            get => Model.Title;
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        public Model.Task.State Status => Model.CurrentState;

        public string StatusString
        {
            get
            {
                switch (Model.CurrentState)
                {
                    case Task.State.Todo:
                        return "To Do";
                    case Task.State.Doing:
                        return "Doing";
                    case Task.State.Done:
                        return "Done";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int Completion => Model.Completion;

        public ObservableCollection<TaskVm> SubTasks { get; set; }

        public Model.Task Model { get; set; }

        #region Constructor

        public TaskVm(Model.Task model)
        {
            Model = model;
            SubTasks = new ObservableCollection<TaskVm>(Model.SubTasks.Select(t => new TaskVm(t)));

            foreach (var subTask in SubTasks)
            {
                subTask.PropertyChanged += OnSubTaskPropertyChanged;
            }
        }

        #endregion

        #region Callback

        private void OnSubTaskPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Status)) return;

            Task.State newStatus = ComputeNewStatus();

            if (Status != newStatus) Model.CurrentState = newStatus;

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(Completion));
        }

        #endregion

        #region Commands

        #region Change Status

        public ICommand ChangeStatusCommand => _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand(ChangeStatus));

        private RelayCommand _changeStatusCommand;

        private void ChangeStatus(object sender)
        {
            SwitchStatus();
        }

        #endregion

        #region Add Task

        public ICommand AddTaskCommand => _addTaskCommand ?? (_addTaskCommand = new RelayCommand(AddTask));

        private RelayCommand _addTaskCommand;

        private void AddTask(object sender)
        {
            AddSubTask("");
        }

        #endregion

        #endregion

        #region Private methods

        private void SwitchStatus(Task.State? status = null)
        {
            if (status == null)
            {
                if (Model.CurrentState == Task.State.Done) Model.CurrentState = Task.State.Todo;
                else Model.CurrentState += 1;
            }
            else
            {
                Model.CurrentState = status.Value;
            }

            if (Model.CurrentState == Task.State.Done)
            {
                foreach (var subTask in SubTasks)
                {
                    subTask.SwitchStatus(Task.State.Done);
                }
            }
            else if (Model.CurrentState == Task.State.Todo)
            {
                foreach (var subTask in SubTasks)
                {
                    subTask.SwitchStatus(Task.State.Todo);
                }
            }

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(Completion));
        }

        private Task.State ComputeNewStatus()
        {
            List<Task.State> Statuses = new List<Task.State>();
            foreach (var subTask in SubTasks)
            {
                Statuses.Add(subTask.ComputeNewStatus());
            }

            if (Statuses.Count == 0)
            {
                Statuses.Add(Model.CurrentState);
            }

            if (Statuses.All(s => s == Task.State.Done)) return Task.State.Done;

            return Statuses.All(s => s == Task.State.Todo) ? Task.State.Todo : Task.State.Doing;
        }

        private void AddSubTask(string name)
        {
            Model.Task newTask = new Model.Task(name);

            Model.SubTasks.Add(newTask);
            TaskVm newTaskVm = new TaskVm(newTask);
            SubTasks.Add(newTaskVm);
            newTaskVm.PropertyChanged += OnSubTaskPropertyChanged;
        }
        #endregion
    }
}
