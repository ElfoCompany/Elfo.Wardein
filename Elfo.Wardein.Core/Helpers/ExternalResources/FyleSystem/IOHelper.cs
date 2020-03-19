using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public class IOHelper
    {
        private readonly string filePath;

        public IOHelper(string filePath)
        {
            this.filePath = filePath;
        }

        public string GetFileContent() => System.IO.File.ReadAllText(this.filePath);

        public bool CheckIfFileExist() => System.IO.File.Exists(this.filePath);

        public void PersistFileOnDisk(string fileContent) => System.IO.File.WriteAllText(this.filePath, fileContent);
    }
}
