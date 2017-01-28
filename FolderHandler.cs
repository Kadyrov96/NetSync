using System.IO;
using System.Windows.Forms;

namespace TEST2
{
    class FolderHandler
    {
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
    }
}
