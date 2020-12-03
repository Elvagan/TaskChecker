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
    internal class MainWindowVm
    {
        public TaskListVm CurrentTaskListVm { get; set; }

        public MainWindowVm()
        {
            Task task01 = new Task("Task 01");
            Task task0101 = new Task("Task 01-01");
            Task task02 = new Task("Task 02");
            Task task0201 = new Task("Task 02-01");
            Task task0202 = new Task("Task 02-02");
            Task task03 = new Task("Task 03");

            CurrentTaskListVm = new TaskListVm("Task List");

            task01.SubTasks.Add(task0101);
            task02.SubTasks.Add(task0201);
            task02.SubTasks.Add(task0202);

            CurrentTaskListVm.AddTask(task01);
            CurrentTaskListVm.AddTask(task02);
            CurrentTaskListVm.AddTask(task03);
        }
    }
}