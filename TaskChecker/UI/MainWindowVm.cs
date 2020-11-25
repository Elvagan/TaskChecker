using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskChecker.Model;
using TaskChecker.ViewModel;

namespace TaskChecker.UI
{
    class MainWindowVm
    {
        public TaskListVm CurrentTaskListVm { get; set; }

        public MainWindowVm()
        {
            TaskList newTaskList = new TaskList("First task list");
            newTaskList.AddTask("Task 01");
            newTaskList.AddTask("Task 02");
            newTaskList.AddTask("Task 03");

            CurrentTaskListVm = new TaskListVm(newTaskList);
        }
    }
}
