using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Serialization;
using TaskChecker.Tools;
using Task = TaskChecker.Models.Task;

namespace TaskChecker.ViewModels
{
    public class TaskListVm : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance should display a ratio bar.
        /// </summary>
        public bool HasRatio => Tasks.Count > 1;

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        public ObservableCollection<TaskVm> Tasks { get; set; } = new ObservableCollection<TaskVm>();

        /// <summary>
        /// Gets the workspace.
        /// </summary>
        public Workspace Workspace { get; }

        /// <summary>
        /// Gets the todo tasks ratio.
        /// </summary>
        public string TodoRatio
        {
            get
            {
                if (HasRatio)
                {
                    int count = Tasks.Count(t => t.Status == Task.Status.Todo);
                    if (count == 0) return "0*";

                    double ratio = Math.Floor((double)count / (Tasks.Count - 1) * 100);
                    return $"{(int)ratio}*";
                }

                return "0*";
            }
        }

        /// <summary>
        /// Gets the doing tasks ratio.
        /// </summary>
        public string DoingRatio
        {
            get
            {
                if (HasRatio)
                {
                    int count = Tasks.Count(t => t.Status == Task.Status.Doing);
                    if (count == 0) return "0*";

                    double ratio = Math.Floor((double)count / (Tasks.Count - 1) * 100);
                    return $"{(int)ratio}*";
                }

                return "0*";
            }
        }

        /// <summary>
        /// Gets the done tasks ratio.
        /// </summary>
        public string DoneRatio
        {
            get
            {
                if (HasRatio && Tasks.Count(t => t.Status == Task.Status.Done) > 0)
                {
                    // Get the done ratio from the two others to avoid precision errors.

                    int countTodo = Tasks.Count(t => t.Status == Task.Status.Todo);
                    double ratioTodo = countTodo == 0 ? 0 : Math.Floor((double)countTodo / (Tasks.Count - 1) * 100);

                    int countDoing = Tasks.Count(t => t.Status == Task.Status.Doing);
                    double ratioDoing = countDoing == 0 ? 0 : Math.Floor((double)countDoing / (Tasks.Count - 1) * 100);

                    return $"{100 - ((int)ratioTodo + (int)ratioDoing)}*";
                }

                return "0*";
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                UpdateTaskList();
            }
        }

        private string _title;

        public string ID { get; }

        /// <summary>
        /// Gets or sets the selected task.
        /// </summary>
        public TaskVm SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }

        private TaskVm _selectedTask;

        #endregion Properties

        #region Constructor

        public TaskListVm(Workspace workspace, string id, string title)
        {
            _title = title;
            Workspace = workspace;
            ID = id;
            TaskVm addTask = new TaskVm();
            addTask.AskSibling += OnAddTaskAskSibling;
            Tasks.Add(addTask);

            UpdateTaskList();
        }

        #endregion Constructor

        #region Public methods

        /// <summary>
        /// Adds a new task in the task list.
        /// </summary>
        /// <param name="name">The new task name.</param>
        /// <param name="parent">The parent task.</param>
        public void AddTask(string name, Task parent = null)
        {
            Task newTask = new Task(name);
            AddTask(newTask, parent);
        }

        /// <summary>
        /// Adds an already created task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="parent">The parent task.</param>
        public void AddTask(Task task, Task parent = null)
        {
            if (parent != null)
            {
                TaskVm parentVm = Tasks.FirstOrDefault(t => t.Model == parent);
                parentVm?.AddSubTask(task);
            }
            else
            {
                TaskVm newVm = new TaskVm(Workspace, task, ID);
                newVm.AskDelete += OnTaskVmAskDelete;
                newVm.PropertyChanged += OnTaskPropertyChanged;
                newVm.SubTaskTitleChanged += OnTaskTitleChanged;
                Tasks.Add(newVm);
            }

            UpdateTaskList();
        }

        #endregion Public methods

        #region Callbacks

        private void OnTaskVmAskDelete(TaskVm taskVm)
        {
            taskVm.AskDelete -= OnTaskVmAskDelete;
            Tasks.Remove(taskVm);

            Workspace.DeleteDescriptionFile(taskVm.ListID, taskVm.ID);

            UpdateTaskList();
        }

        private void OnAddTaskAskSibling()
        {
            TaskVm newTask = new TaskVm(Workspace, new Task(""), ID);
            newTask.AskDelete += OnTaskVmAskDelete;
            newTask.PropertyChanged += OnTaskPropertyChanged;
            Tasks.Insert(1, newTask);

            UpdateTaskList();
        }

        private void OnTaskPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Task.Status) || e.PropertyName == nameof(Task.Title))
            {
                UpdateTaskList();
            }
            else if (e.PropertyName == nameof(TaskVm.IsSelected))
            {
                TaskVm task = (TaskVm)sender;

                if (task.IsSelected) SelectedTask = task;
            }
        }

        private void OnTaskTitleChanged(TaskVm taskVm)
        {
            UpdateTaskList();
        }

        #endregion Callbacks

        #region Commands

        public ICommand CreateNewTaskCommand => _createNewTaskCommand ?? (_createNewTaskCommand = new RelayCommand(CreateNewTask));

        private RelayCommand _createNewTaskCommand;

        private void CreateNewTask(object sender)
        {
            AddTask("");
        }

        #endregion Commands

        #region Private methods

        private void UpdateTaskList()
        {
            Workspace.SaveListHierarchy(this);
            RefreshCompletionBar();
        }

        private void RefreshCompletionBar()
        {
            OnPropertyChanged(nameof(HasRatio));
            OnPropertyChanged(nameof(TodoRatio));
            OnPropertyChanged(nameof(DoingRatio));
            OnPropertyChanged(nameof(DoneRatio));
        }

        #endregion Private methods
    }
}