using System.Collections.Generic;
using System.IO;

namespace TEST2
{
    class Synchroniser
    {
        private FolderHandler folderToSync;
        private string syncDataStoreFolder;
        private string hashedSyncDataStoreFolder;

        private List<FileDescript> local_folderToSyncElements;

        Synchroniser(FolderHandler _folderToSync)
        {
            folderToSync = _folderToSync;
            syncDataStoreFolder = @"C:\Users\Admin\Desktop\Hashes\";
            hashedSyncDataStoreFolder = Hasher.GetMd5Hash(folderToSync.FolderPath);
        }

        public void CreateSyncDataStore()
        {
            //Creating the file, that includes synchronization data of the concrete folder
            FileStream hashTableCreator = File.Create(syncDataStoreFolder + hashedSyncDataStoreFolder + @".dat");
            hashTableCreator.Close();

            //Filling the file with synchronization data of the concrete folder
            StreamWriter hashTableWriter = new StreamWriter(syncDataStoreFolder + hashedSyncDataStoreFolder + @".dat");

            byte[] folderElementBuffer;
            for (int i = 0; i < folderToSync.FolderElements.Length; i++)
            {
                folderElementBuffer = File.ReadAllBytes(folderToSync.FolderElements[i]);
                hashTableWriter.WriteLine(folderToSync.FolderElements[i] + "***" + Hasher.GetMd5Hash(folderElementBuffer));
                local_folderToSyncElements.Add(new FileDescript
                    { name = Path.GetFileName(folderToSync.FolderElements[i]), modFlag = 0 });
            }
            //System.Windows.MessageBox.Show("Full directory was succesfully synchronized with database");
            hashTableWriter.Close();
            hashTableWriter.Dispose();
        }
    }
}
