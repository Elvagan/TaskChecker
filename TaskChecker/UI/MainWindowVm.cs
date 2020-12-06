using TaskChecker.Tools;
using TaskChecker.ViewModels;

namespace TaskChecker.UI
{
    internal class MainWindowVm : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current task list View Model.
        /// </summary>
        public TaskListVm CurrentTaskListVm { get; set; }

        #endregion Properties

        #region Constructor

        public MainWindowVm()
        {
            CurrentTaskListVm = new TaskListVm("New Task List");
        }

        #endregion Constructor
    }
}