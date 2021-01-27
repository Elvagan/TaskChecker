using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;
using TaskChecker.Models;
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

        #region Commands

        #region New task list

        public ICommand NewTaskListCommand => _newTaskListCommand ?? (_newTaskListCommand = new RelayCommand(NewTaskListExecute));

        private RelayCommand _newTaskListCommand;

        private void NewTaskListExecute(object sender)
        {
        }

        #endregion New task list

        #region Open task list

        public ICommand OpenTaskListCommand => _openTaskListCommand ?? (_openTaskListCommand = new RelayCommand(OpenTaskListExecute));

        private RelayCommand _openTaskListCommand;

        private void OpenTaskListExecute(object sender)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Task list files (*.xml)|*.xml";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                string filePath = openFileDialog.FileName;
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                XmlSerializer xs = new XmlSerializer(typeof(List<Models.Task>));
                using (StreamReader rd = new StreamReader(filePath))
                {
                    if (!(xs.Deserialize(rd) is List<Task> import)) return;

                    CurrentTaskListVm = new TaskListVm(fileName);
                    foreach (var task in import)
                    {
                        CurrentTaskListVm.AddTask(task);
                    }

                    OnPropertyChanged(nameof(CurrentTaskListVm));
                }
            }
        }

        #endregion Open task list

        #region Save task list

        public ICommand SaveTaskListCommand => _saveTaskListCommand ?? (_saveTaskListCommand = new RelayCommand(SaveTaskListExecute));

        private RelayCommand _saveTaskListCommand;

        private void SaveTaskListExecute(object sender)
        {
            List<Models.Task> export = CurrentTaskListVm.Tasks.Where(t => !t.IsFake).Select(t => t.Model).ToList();

            XmlSerializer xs = new XmlSerializer(typeof(List<Models.Task>));
            using (StreamWriter wr = new StreamWriter($"{CurrentTaskListVm.Title}.xml"))
            {
                xs.Serialize(wr, export);
            }
        }

        #endregion Save task list

        #endregion Commands
    }
}