using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace TaskChecker.Tools.PackageManger
{
    public class FilePackageReader
    {
        #region Fields

        private Dictionary<string, string> _filenameFileContentDictionary;
        private readonly string _filepath;

        #endregion Fields

        #region Constructors

        public FilePackageReader(string filepath)
        {
            _filepath = filepath;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Gets the filename file content dictionary.
        /// </summary>
        /// <returns>A dictionnary with the filename and its related content as string</returns>
        /// <exception cref="System.Exception"></exception>
        public Dictionary<string, string> GetFilenameFileContentDictionary()
        {
            try
            {
                _filenameFileContentDictionary = new Dictionary<string, string>();

                // Open the package file
                using (var fs = new FileStream(_filepath, FileMode.Open))
                {
                    // Open the package file as a ZIP
                    using (var archive = new ZipArchive(fs))
                    {
                        // Iterate through the content files and add them to a dictionary
                        foreach (var zipArchiveEntry in archive.Entries)
                        {
                            using (var stream = zipArchiveEntry.Open())
                            {
                                using (var zipSr = new StreamReader(stream))
                                {
                                    _filenameFileContentDictionary.Add(zipArchiveEntry.Name, zipSr.ReadToEnd());
                                }
                            }
                        }
                    }
                }

                return _filenameFileContentDictionary;
            }
            catch (Exception e)
            {
                var errorMessage = "Unable to open/read the package. " + e.Message;
                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Opens the package.
        /// </summary>
        /// <param name="destinationPath">The package path.</param>
        public void OpenPackage(string destinationPath)
        {
            Dictionary<string, string> content = GetFilenameFileContentDictionary();

            string lstID = "extract_tmp";
            string temporaryPath = Path.Combine(destinationPath, lstID);

            // Delete temporary folder if it exists
            if (Directory.Exists(temporaryPath))
            {
                Directory.Delete(temporaryPath, true);
            }

            // Create temporary extract folder
            Directory.CreateDirectory(temporaryPath);

            // Write all files in the package
            foreach (var file in content)
            {
                // Get the task list identifier
                if (file.Key.StartsWith("lst")) lstID = Path.GetFileNameWithoutExtension(file.Key);

                File.WriteAllText(Path.Combine(temporaryPath, file.Key), file.Value);
            }

            // Rename the temporary folder with the task list identifier
            string folderPath = Path.Combine(destinationPath, lstID);
            Directory.Move(temporaryPath, folderPath);
        }

        #endregion Public methods
    }
}