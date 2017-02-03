using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TEST2
{
    class FolderHandler
    {
        private List<string> folderElementsList;

        public FolderHandler()
        {
            folderElementsList = new List<string>();
        }

        private string folderPath;
        public string FolderPath
        {
            private set
            {
                folderPath = value;
            }
            get
            {
                return folderPath;
            }
        }

        private string[] folderElements;
        public string[] FolderElements
        {
            private set
            {
                folderElements = value;
            }
            get
            {
                return folderElements;
            }
        }

        public void SelectFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = folderBrowserDialog.SelectedPath;
                FolderElements = Directory.GetFileSystemEntries(folderPath);
            }
        }
        
        public bool IsFolderEmpty()
        {
            if (folderElements.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateServiceFile(string _full_file_path)
        {
            FileStream serviceFileCreator = File.Create(_full_file_path);
            serviceFileCreator.Close();
        }
    }
}
