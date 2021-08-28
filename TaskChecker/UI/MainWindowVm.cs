using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;
using TaskChecker.Models;
using TaskChecker.Tools;
using TaskChecker.Tools.PackageManger;
using TaskChecker.ViewModels;

namespace TaskChecker.UI
{
    public class MainWindowVm : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current task list View Model.
        /// </summary>
        public TaskListVm CurrentTaskListVm
        {
            get => _currentTaskListVm;
            set
            {
                _currentTaskListVm = value;
                OnPropertyChanged();
            }
        }

        private TaskListVm _currentTaskListVm = null;

        /// <summary>
        /// Gets the workspace.
        /// </summary>
        public Workspace Workspace { get; private set; }

        #endregion Properties

        #region Constructor

        public MainWindowVm()
        {
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CheckTree");
            Workspace = new Workspace(defaultPath);

            if (Workspace.TaskLists.Count > 0)
            {
                CurrentTaskListVm = Workspace.TaskLists.ElementAt(0);
            }
            else
            {
                CurrentTaskListVm = null;
            }
        }

        #endregion Constructor

        #region Commands

        #region New task list

        public ICommand NewTaskListCommand => _newTaskListCommand ?? (_newTaskListCommand = new RelayCommand(NewTaskListExecute));

        private RelayCommand _newTaskListCommand;

        private void NewTaskListExecute(object sender)
        {
            Workspace.CloseList(CurrentTaskListVm);
            string newID = IDManager.GetNewName("lst");
            CurrentTaskListVm = new TaskListVm(Workspace, newID, "New task list");

            Workspace.RefreshWorkspace();
        }

        #endregion New task list

        #region Import task list

        public ICommand ImportTaskListCommand => _importTaskListCommand ?? (_importTaskListCommand = new RelayCommand(ImportTaskListExecute));

        private RelayCommand _importTaskListCommand;

        private void ImportTaskListExecute(object sender)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Workspace.WorkspacePath,
                Filter = "Task list files (*.lst)|*.lst",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the path of specified file
                string filePath = openFileDialog.FileName;

                Workspace.CloseList(CurrentTaskListVm);

                // Open the list package
                FilePackageReader reader = new FilePackageReader(filePath);
                reader.OpenPackage(Workspace.WorkspacePath);

                Workspace.RefreshWorkspace();

                if (Workspace.TaskLists.Count > 0)
                {
                    CurrentTaskListVm = Workspace.TaskLists.ElementAt(0);
                }
                else
                {
                    CurrentTaskListVm = null;
                }
            }
        }

        #endregion Import task list

        #region Export task list

        public ICommand ExportTaskListCommand => _exportTaskListCommand ?? (_exportTaskListCommand = new RelayCommand(ExportTaskListExecute));

        private RelayCommand _exportTaskListCommand;

        private void ExportTaskListExecute(object sender)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Workspace.WorkspacePath,
                FileName = $"{CurrentTaskListVm.Title}",
                DefaultExt = ".lst",
                Filter = "Task list files (*.lst)|*.lst",
                FilterIndex = 1
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Workspace.ExportList(CurrentTaskListVm, saveFileDialog.FileName);
            }
        }

        #endregion Export task list

        #region Exit

        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new RelayCommand(ExitExecute));

        private RelayCommand _exitCommand;

        private void ExitExecute(object sender)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #endregion Exit

        #endregion Commands
    }
}