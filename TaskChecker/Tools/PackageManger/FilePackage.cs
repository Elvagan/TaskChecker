using System.Collections.Generic;

namespace TaskChecker.Tools.PackageManger
{
    public class FilePackage
    {
        /// <summary>
        /// Gets or sets the package destination path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the content file path list.
        /// </summary>
        public List<string> ContentFilePathList { get; set; }
    }
}