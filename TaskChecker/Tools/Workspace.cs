using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Serialization;
using TaskChecker.Models;
using TaskChecker.Tools.PackageManger;
using TaskChecker.ViewModels;

namespace TaskChecker.Tools
{
    public class Workspace
    {
        #region Properties

        /// <summary>
        /// Gets or sets the workspace path.
        /// </summary>
        public string WorkspacePath { get; set; }

        /// <summary>
        /// Gets the task lists.
        /// </summary>
        public List<TaskListVm> TaskLists { get; }

        #endregion Properties

        #region Constructors

        public Workspace(string path)
        {
            WorkspacePath = path;
            TaskLists = new List<TaskListVm>();

            RefreshWorkspace();
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Read the tasks available in the workspace location.
        /// </summary>
        public void RefreshWorkspace()
        {
            TaskLists.Clear();

            if (Directory.Exists(WorkspacePath))
            {
                List<string> lists = Directory.GetDirectories(WorkspacePath).ToList();

                foreach (var list in lists)
                {
                    // Only check folders starting with "lst"
                    if (!Path.GetFileName(list).StartsWith("lst")) continue;

                    // Get the task list descriptor file
                    List<string> data = Directory.GetFiles(list, "*.xml", SearchOption.AllDirectories).ToList();
                    if (data.Count == 1)
                    {
                        TaskListVm newList = OpenList(data.ElementAt(0));
                        if (newList != null) TaskLists.Add(newList);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(WorkspacePath);
            }
        }

        /// <summary>
        /// Exports a list as a packaged file (.lst).
        /// </summary>
        /// <param name="list">The list view model.</param>
        /// <param name="packageFilePath">The package file path.</param>
        public void ExportList(TaskListVm list, string packageFilePath)
        {
            // Get the list content folder
            string packageFilesRoot = Path.Combine(WorkspacePath, list.ID);

            // Create a list descriptor
            TaskListDescriptor export = SaveListHierarchy(list);

            // Create a package descriptor
            var filePackage = new FilePackage
            {
                FilePath = packageFilePath,
                ContentFilePathList = new List<string>()
                {
                    Path.Combine(packageFilesRoot, $"{list.ID}.xml")
                }
            };

            // Fill the package descriptor with its content
            foreach (Models.Task task in export.Tasks)
            {
                filePackage.ContentFilePathList.Add(Path.Combine(packageFilesRoot, $"{task.ID}.xaml"));
            }

            // Write the packaged file
            var filePackageWriter = new FilePackageWriter(filePackage);
            filePackageWriter.GeneratePackage(false);
        }

        /// <summary>
        /// Loads (or creates if it does not exists) the task description flowdocument.
        /// </summary>
        /// <param name="listID">The list identifier.</param>
        /// <param name="taskID">The task identifier.</param>
        /// <returns></returns>
        public string LoadDescriptionFile(string listID, string taskID)
        {
            FlowDocument content;

            string filePath = $"{WorkspacePath}\\{listID}\\{taskID}.xaml";
            if (!File.Exists(filePath))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var templatePath = "TaskChecker.res.desc_template.xaml";

                Encoding unicode = Encoding.Unicode;
                File.WriteAllBytes($"{WorkspacePath}\\{listID}\\{taskID}.xaml", unicode.GetBytes(Properties.Resources.desc_template));

                using (Stream template = assembly.GetManifestResourceStream(templatePath))
                {
                    content = XamlReader.Load(template) as FlowDocument;
                }
            }
            else
            {
                using (FileStream xamlFile = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // This will work, only if the top-level element in the document is FlowDocument
                    content = XamlReader.Load(xamlFile) as FlowDocument;
                }
            }

            return XamlWriter.Save(content);
        }

        /// <summary>
        /// Saves the task description flowdocument.
        /// </summary>
        /// <param name="documentText">The description text.</param>
        /// <param name="listID">The list identifier.</param>
        /// <param name="taskID">The task identifier.</param>
        public void SaveDescriptionFile(string documentText, string listID, string taskID)
        {
            string filePath = $"{WorkspacePath}\\{listID}\\{taskID}.xaml";
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            Encoding unicode = Encoding.Unicode;
            File.WriteAllBytes(filePath, unicode.GetBytes(documentText));
        }

        /// <summary>
        /// Deletes the task description flowdocument.
        /// </summary>
        /// <param name="listID">The list identifier.</param>
        /// <param name="taskID">The task identifier.</param>
        public void DeleteDescriptionFile(string listID, string taskID)
        {
            string filePath = $"{WorkspacePath}\\{listID}\\{taskID}.xaml";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Saves the task list hierarchy.
        /// </summary>
        /// <param name="taskList">The task list.</param>
        /// <returns></returns>
        public TaskListDescriptor SaveListHierarchy(TaskListVm taskList)
        {
            // Create the directory if it does not exists
            string taskListDirectory = Path.Combine(WorkspacePath, $"{taskList.ID}");
            if (!Directory.Exists(taskListDirectory))
            {
                Directory.CreateDirectory(taskListDirectory);
            }

            // Create the descriptor file if it does not exists
            string taskListFile = Path.Combine(taskListDirectory, $"{taskList.ID}.xml");
            if (!File.Exists(taskListFile))
            {
                using (File.Create(taskListFile)) { }
            }

            // Create the task list descriptor
            TaskListDescriptor export = new TaskListDescriptor()
            {
                ID = taskList.ID,
                Title = taskList.Title,
                ModificationDate = DateTime.Now,
                Tasks = taskList.Tasks.Where(t => !t.IsFake).Select(t => t.Model).ToList()
            };

            // Write the descriptor as .xml file
            XmlSerializer xs = new XmlSerializer(typeof(TaskListDescriptor));
            using (StreamWriter wr = new StreamWriter(taskListFile))
            {
                xs.Serialize(wr, export);
            }

            return export;
        }

        /// <summary>
        /// Closes the list (package the list as a .lst file and delete its temporary files in the Worksapce).
        /// </summary>
        /// <param name="list">The list.</param>
        public void CloseList(TaskListVm list)
        {
            if (list == null) return;

            ExportList(list, Path.Combine(WorkspacePath, $"{list.Title}.lst"));

            // TODO : Do not delete folder if export failed
            string listFolder = Path.Combine(WorkspacePath, $"{list.ID}");
            if (Directory.Exists(listFolder)) Directory.Delete(listFolder, true);
        }

        /// <summary>
        /// Opens the list (create an object from a list descriptor file).
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The created TaskListVm</returns>
        /// <exception cref="System.Exception">Could not read list</exception>
        public TaskListVm OpenList(string path)
        {
            TaskListVm newList;
            TaskListDescriptor import;

            // Read the xml file
            XmlSerializer xs = new XmlSerializer(typeof(TaskListDescriptor));
            using (StreamReader rd = new StreamReader(path))
            {
                try
                {
                    import = xs.Deserialize(rd) as TaskListDescriptor;
                }
                catch
                {
                    throw new Exception("Could not read list");
                }
            }

            // Create the object
            newList = new TaskListVm(this, import.ID, import.Title);
            foreach (var task in import.Tasks)
            {
                newList.AddTask(task);
            }

            return newList;
        }

        #endregion Public methods
    }
}