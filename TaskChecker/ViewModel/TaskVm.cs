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
    internal class TaskVm : ViewModelBase
    {
        #region Events

        /// <summary>
        /// Occurs when the status has changed.
        /// </summary>
        public event Action<TaskVm> AskDelete;

        #endregion Events

        public TaskVm Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                OnPropertyChanged();
            }
        }

        private TaskVm _parent;

        public string Title
        {
            get => Model.Title;
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        public Model.Task.Status Status => Model.CurrentStatus;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        private bool _isExpanded;

        public string StatusString
        {
            get
            {
                switch (Model.CurrentStatus)
                {
                    case Task.Status.Todo:
                        return "To Do";

                    case Task.Status.Doing:
                        return "Doing";

                    case Task.Status.Done:
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

        public TaskVm(Model.Task model, TaskVm parent = null)
        {
            Model = model;
            Parent = parent;
            SubTasks = new ObservableCollection<TaskVm>(Model.SubTasks.Select(t => new TaskVm(t, this)));

            foreach (var subTask in SubTasks)
            {
                subTask.PropertyChanged += OnSubTaskPropertyChanged;
                subTask.AskDelete += OnSubTaskAskDelete;
            }
        }

        #endregion Constructor

        #region Callback

        private void OnSubTaskPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Status)) return;

            RefreshStatus();
        }

        private void OnSubTaskAskDelete(TaskVm sender)
        {
            SubTasks.Remove(sender);
            Model.SubTasks.Remove(sender.Model);

            RefreshStatus();
        }

        #endregion Callback

        #region Commands

        #region Change Status

        public ICommand ChangeStatusCommand => _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand(ChangeStatus));

        private RelayCommand _changeStatusCommand;

        private void ChangeStatus(object sender)
        {
            SwitchStatus();
        }

        #endregion Change Status

        #region Add Task

        public ICommand AddTaskCommand => _addTaskCommand ?? (_addTaskCommand = new RelayCommand(AddTask));

        private RelayCommand _addTaskCommand;

        private void AddTask(object sender)
        {
            AddSubTask("");
        }

        #endregion Add Task

        #region Remove

        public ICommand RemoveTaskCommand => _removeTaskCommand ?? (_removeTaskCommand = new RelayCommand(RemoveTask));

        private RelayCommand _removeTaskCommand;

        private void RemoveTask(object sender)
        {
            RemoveSubTasks();
            AskDelete?.Invoke(this);
        }

        #endregion Remove

        #endregion Commands

        #region Private methods

        private void SwitchStatus(Task.Status? status = null)
        {
            if (status == null)
            {
                if (Model.CurrentStatus == Task.Status.Done) Model.CurrentStatus = Task.Status.Todo;
                else Model.CurrentStatus += 1;
            }
            else
            {
                Model.CurrentStatus = status.Value;
            }

            if (Model.CurrentStatus == Task.Status.Done)
            {
                foreach (var subTask in SubTasks)
                {
                    subTask.SwitchStatus(Task.Status.Done);
                }
            }
            else if (Model.CurrentStatus == Task.Status.Todo)
            {
                foreach (var subTask in SubTasks)
                {
                    subTask.SwitchStatus(Task.Status.Todo);
                }
            }

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(Completion));
        }

        private Task.Status ComputeNewStatus()
        {
            List<Task.Status> Statuses = new List<Task.Status>();
            foreach (var subTask in SubTasks)
            {
                Statuses.Add(subTask.ComputeNewStatus());
            }

            if (Statuses.Count == 0)
            {
                Statuses.Add(Model.CurrentStatus);
            }

            if (Statuses.All(s => s == Task.Status.Done)) return Task.Status.Done;

            return Statuses.All(s => s == Task.Status.Todo) ? Task.Status.Todo : Task.Status.Doing;
        }

        private void RefreshStatus()
        {
            Task.Status newStatus = ComputeNewStatus();
            if (Status != newStatus) Model.CurrentStatus = newStatus;

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(Completion));
        }

        public void AddSubTask(Task task)
        {
            Model.SubTasks.Add(task);
            TaskVm newTaskVm = new TaskVm(task, this);
            SubTasks.Add(newTaskVm);
            newTaskVm.PropertyChanged += OnSubTaskPropertyChanged;
            newTaskVm.AskDelete += OnSubTaskAskDelete;
            IsExpanded = true;

            RefreshStatus();
        }

        public void AddSubTask(string name)
        {
            Task newTask = new Task(name);
            AddSubTask(newTask);
        }

        private void RemoveSubTasks()
        {
            foreach (var subTask in SubTasks)
            {
                subTask.PropertyChanged -= OnSubTaskPropertyChanged;
                subTask.AskDelete -= OnSubTaskAskDelete;
                subTask.RemoveSubTasks();
            }

            Model.SubTasks.Clear();
            SubTasks.Clear();

            RefreshStatus();
        }

        #endregion Private methods
    }
}