using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TaskChecker.Tools;
using Task = TaskChecker.Models.Task;

namespace TaskChecker.ViewModels
{
    internal class TaskVm : ViewModelBase
    {
        #region Events

        /// <summary>
        /// Occurs when the task asks for its deletion.
        /// </summary>
        public event Action<TaskVm> AskDelete;

        /// <summary>
        /// Occurs when the task asks for a sibling.
        /// </summary>
        public event Action AskSibling;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public Task.Status Status => Model.CurrentStatus;

        /// <summary>
        /// Gets or sets the completion.
        /// </summary>
        public string Completion => Model.Completion + " %";

        /// <summary>
        /// Gets or sets the status name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating if this task is fake (used to create new tasks)
        /// </summary>
        public bool IsFake { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public Task Model { get; set; }

        /// <summary>
        /// Gets or sets the sub tasks.
        /// </summary>
        public ObservableCollection<TaskVm> SubTasks { get; set; }

        /// <summary>
        /// Gets or sets the parent task.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the task title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if this instance is expanded.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating if this instance should get the edit focus.
        /// </summary>
        public bool ShouldGetFocus
        {
            get => _shouldGetFocus;
            set
            {
                _shouldGetFocus = value;
                OnPropertyChanged();
            }
        }

        private bool _shouldGetFocus;

        #endregion Properties

        #region Constructors

        public TaskVm()
        {
            Model = new Task("") { CurrentStatus = Task.Status.None };
            Parent = null;
            IsFake = true;
            SubTasks = new ObservableCollection<TaskVm>();
        }

        public TaskVm(Task model, TaskVm parent = null)
        {
            Model = model;
            Parent = parent;
            IsFake = false;
            SubTasks = new ObservableCollection<TaskVm> { new TaskVm() };
            SubTasks.ElementAt(0).AskSibling += OnSubTaskAskSibling;

            foreach (var modelSubTask in Model.SubTasks)
            {
                TaskVm subTask = new TaskVm(modelSubTask, this);
                subTask.PropertyChanged += OnSubTaskPropertyChanged;
                subTask.AskDelete += OnSubTaskAskDelete;
                SubTasks.Add(subTask);
            }

            ShouldGetFocus = true;
        }

        #endregion Constructors

        #region Callbacks

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

        private void OnSubTaskAskSibling()
        {
            AddSubTask(new Task(""));
            RefreshStatus();
        }

        #endregion Callbacks

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

        #region Add Sibling

        public ICommand AddSiblingTaskCommand => _addSiblingTaskCommand ?? (_addSiblingTaskCommand = new RelayCommand(AddSiblingTask));

        private RelayCommand _addSiblingTaskCommand;

        private void AddSiblingTask(object sender)
        {
            AskSibling?.Invoke();
        }

        #endregion Add Sibling

        #region Remove Task

        public ICommand RemoveTaskCommand => _removeTaskCommand ?? (_removeTaskCommand = new RelayCommand(RemoveTask));

        private RelayCommand _removeTaskCommand;

        private void RemoveTask(object sender)
        {
            RemoveSubTasks();
            AskDelete?.Invoke(this);
        }

        #endregion Remove Task

        #endregion Commands

        #region Public methods

        /// <summary>
        /// Adds a sub task.
        /// </summary>
        /// <param name="task"></param>
        public void AddSubTask(Task task)
        {
            Model.SubTasks.Insert(0, task);
            TaskVm newTaskVm = new TaskVm(task, this);
            SubTasks.Insert(1, newTaskVm);
            newTaskVm.PropertyChanged += OnSubTaskPropertyChanged;
            newTaskVm.AskDelete += OnSubTaskAskDelete;
            IsExpanded = true;

            RefreshStatus();
        }

        /// <summary>
        /// Adds a new sub task
        /// </summary>
        /// <param name="name"></param>
        public void AddSubTask(string name)
        {
            Task newTask = new Task(name);
            AddSubTask(newTask);
        }

        /// <summary>
        /// Removes all sub tasks (and disconnect events).
        /// </summary>
        private void RemoveSubTasks()
        {
            foreach (var subTask in SubTasks)
            {
                if (subTask.IsFake)
                {
                    subTask.AskSibling -= OnSubTaskAskSibling;
                }
                else
                {
                    subTask.PropertyChanged -= OnSubTaskPropertyChanged;
                    subTask.AskDelete -= OnSubTaskAskDelete;
                    subTask.AskSibling -= OnSubTaskAskSibling;
                    subTask.RemoveSubTasks();
                }
            }

            Model.SubTasks.Clear();
            SubTasks.Clear();

            RefreshStatus();
        }

        #endregion Public methods

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
            List<Task.Status> statuses = new List<Task.Status>();
            foreach (var subTask in SubTasks)
            {
                if (subTask.IsFake) continue;
                statuses.Add(subTask.ComputeNewStatus());
            }

            if (statuses.Count == 0)
            {
                statuses.Add(Model.CurrentStatus);
            }

            if (statuses.All(s => s == Task.Status.Done)) return Task.Status.Done;

            return statuses.All(s => s == Task.Status.Todo) ? Task.Status.Todo : Task.Status.Doing;
        }

        private void RefreshStatus()
        {
            Task.Status newStatus = ComputeNewStatus();
            if (Status != newStatus) Model.CurrentStatus = newStatus;

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(Completion));
        }

        #endregion Private methods
    }
}