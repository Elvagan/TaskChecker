using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskChecker.Model;
using TaskChecker.ViewModel;
using Task = TaskChecker.Model.Task;

namespace TaskChecker.UI
{
    class MainWindowVm
    {
        public TaskListVm CurrentTaskListVm { get; set; }

        public MainWindowVm()
        {
            TaskList newTaskList = new TaskList("First task list");
            Task task01 = new Task("Task 01");
            Task task0101 = new Task("Task 01-01");
            Task task02 = new Task("Task 02");
            Task task0201 = new Task("Task 02-01");
            Task task0202 = new Task("Task 02-02");
            Task task03 = new Task("Task 03");

            newTaskList.AddTask(task01);
            newTaskList.AddTask(task0101,task01);
            newTaskList.AddTask(task02);
            newTaskList.AddTask(task0201, task02);
            newTaskList.AddTask(task0202, task02);
            newTaskList.AddTask(task03);

            CurrentTaskListVm = new TaskListVm(newTaskList);
        }
    }
}
