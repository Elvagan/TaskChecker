using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// from https://www.pluralsight.com/guides/creating-file-packages-in-c

namespace TaskChecker.Tools.PackageManger
{
    public class FilePackageWriter
    {
        #region Fields

        private readonly string _filepath;
        private readonly IEnumerable<string> _contentFilePathList;
        private string _tempDirectoryPath;

        #endregion Fields

        #region Constructors

        public FilePackageWriter(FilePackage filePackage)
        {
            _filepath = filePackage.FilePath;
            _contentFilePathList = filePackage.ContentFilePathList;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Generates the package.
        /// </summary>
        /// <param name="deleteContents">if set to <c>true</c> [delete contents].</param>
        /// <exception cref="System.NullReferenceException">Parent directory info was empty.</exception>
        /// <exception cref="System.Exception">The input file path <> does not contain any file seperators.</exception>
        /// <exception cref="System.IO.FileNotFoundException">File path <> does not exists.</exception>
        public void GeneratePackage(bool deleteContents)
        {
            try
            {
                string parentDirectoryPath = null;
                string filename = null;

                var fileInfo = new FileInfo(_filepath);

                // Get the parent directory path of the package file and if the package file already exists delete it
                if (fileInfo.Exists)
                {
                    filename = fileInfo.Name;

                    var parentDirectoryInfo = fileInfo.Directory;
                    if (parentDirectoryInfo != null)
                    {
                        parentDirectoryPath = parentDirectoryInfo.FullName;
                    }
                    else
                    {
                        throw new NullReferenceException("Parent directory info was empty.");
                    }

                    File.Delete(_filepath);
                }
                else
                {
                    var lastIndexOfFileSeperator = _filepath.LastIndexOf("\\", StringComparison.Ordinal);
                    if (lastIndexOfFileSeperator != -1)
                    {
                        parentDirectoryPath = _filepath.Substring(0, lastIndexOfFileSeperator);
                        filename = _filepath.Substring(lastIndexOfFileSeperator + 1, _filepath.Length - (lastIndexOfFileSeperator + 1));
                    }
                    else
                    {
                        throw new Exception("The input file path '" + _filepath + "' does not contain any file seperators.");
                    }
                }

                // Create a temp directory for our package
                _tempDirectoryPath = parentDirectoryPath + "\\" + filename + "_temp";
                if (Directory.Exists(_tempDirectoryPath))
                {
                    Directory.Delete(_tempDirectoryPath, true);
                }

                Directory.CreateDirectory(_tempDirectoryPath);
                foreach (var filePath in _contentFilePathList)
                {
                    // Copy every content file into the temp directory we created before
                    var filePathInfo = new FileInfo(filePath);
                    if (filePathInfo.Exists)
                    {
                        File.Copy(filePathInfo.FullName, _tempDirectoryPath + "\\" + filePathInfo.Name);
                    }
                    else
                    {
                        throw new FileNotFoundException("File path " + filePath + " does not exists.");
                    }
                }

                // Generate the ZIP from the temp directory
                ZipFile.CreateFromDirectory(_tempDirectoryPath, _filepath);
            }
            catch (Exception e)
            {
                var errorMessage = "An error occured while generating the package. " + e.Message;
                throw new Exception(errorMessage);
            }
            finally
            {
                // Clear the temp directory and the content files
                if (Directory.Exists(_tempDirectoryPath))
                {
                    Directory.Delete(_tempDirectoryPath, true);
                }

                if (deleteContents)
                {
                    foreach (var filePath in _contentFilePathList)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }
            }
        }

        #endregion Public methods
    }
}