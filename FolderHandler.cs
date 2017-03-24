using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NetSync_WinDesktop
{
    class FolderHandler
    {
        private List<string> folderElementsList;
        public string FolderPath { get; private set; }
        public string[] FolderElements { get; private set; }

        public FolderHandler()
        {
            folderElementsList = new List<string>();
        }

        public FolderHandler(string _folderPath)
        {
            folderElementsList = new List<string>();
            FolderPath = _folderPath;
            FolderElements = Directory.GetFileSystemEntries(FolderPath);
        }
        public void SelectFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = folderBrowserDialog.SelectedPath;
                FolderElements = Directory.GetFileSystemEntries(FolderPath);
            }
        }
        
        public bool IsFolderEmpty()
        {
            return FolderElements.Length == 0;
        }

        public void CreateServiceFile(string _full_file_path)
        {
            FileStream serviceFileCreator = File.Create(_full_file_path);
            serviceFileCreator.Close();
        }
    }
}
