using System.Collections.Generic;
using System.IO;
using System;

namespace TEST2
{
    class Synchroniser
    {
        //Synchronizing folder object.
        private FolderHandler folderToSync;
        //Path of the file, that includes data of the last synchronization: name of the file and its hash.
        internal string syncDataStoreFullPath;
        private List<string> syncDataStoreRecords_List;

        private List<FileDescript> local_folderToSyncElements;

        //results of checking folders' statements to the hashes on each devices
        private SyncRecordList local_list;
        private SyncRecordList remote_list;

        #region Result names' lists of files, that must be deleted or downloaded to each device
        private List<string> localDelList;
        private List<string> remoteDelList;

        private List<string> uploadList;
        private List<string> downloadList;
        #endregion

        public Synchroniser(FolderHandler _folderToSync)
        {
            folderToSync = _folderToSync;
            syncDataStoreFullPath = Directory.GetCurrentDirectory() + Hasher.GetMd5Hash(folderToSync.FolderPath);
            local_folderToSyncElements = new List<FileDescript>();

            local_list = new SyncRecordList();
            remote_list = new SyncRecordList();

            localDelList = new List<string>();
            remoteDelList = new List<string>();
            uploadList = new List<string>();
            downloadList = new List<string>();
        }

        //Adds name and key into the storage of local folder's files
        private void AddLocalSyncElements(string elemName, int elemFlag)
        {
            local_folderToSyncElements.Add(
                new FileDescript { elementName = elemName, modificationFlag = elemFlag });
        }

        private void SwitchIfNotChanged(int _remoteIndex, int _localIndex)
        {
            switch (remote_list.keys[_remoteIndex])
            {
                case 0:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEmpty.Source, imageOk.Source, 140));
                    remote_list.RemoveAt(_remoteIndex);
                    break;
                case 1:
                    //listView1.Items.Add(new FileItem(syncDirPath+localNamesList[i], imageImport.Source, imageWait.Source, 140));
                    //download_count++;
                    downloadList.Add(remote_list.names[_remoteIndex]);
                    remote_list.RemoveAt(_remoteIndex);
                    break;
                case 2:
                    break;
                case 3:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageDel.Source, imageWait.Source, 140));
                    localDelList.Add(local_list.names[_localIndex]);
                    remote_list.RemoveAt(_remoteIndex);
                    break;
            }
        }

        private void SwitchIfModified(int _remoteIndex, int _localIndex)
        {
            switch (remote_list.keys[_remoteIndex])
            {
                case 0:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageExport.Source, imageWait.Source, 140));
                    uploadList.Add(local_list.names[_localIndex]);
                    remote_list.RemoveAt(_remoteIndex);
                    break;
                case 1:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                    break;
                case 2:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                    break;
                case 3:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageDel.Source, imageWait.Source, 140));
                    localDelList.Add(local_list.names[_localIndex]);
                    remote_list.RemoveAt(_remoteIndex);
                    break;
            }
        }

        private void SwitchIfDeleted(int _remoteIndex, int _localIndex)
        {
            switch (remote_list.keys[_remoteIndex])
            {
                case 0:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageDel.Source, imageWait.Source, 140));
                    remoteDelList.Add(local_list.names[_localIndex]);
                    remote_list.RemoveAt(_remoteIndex);
                    break;
                case 1:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                    break;
                case 2:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                    break;
                case 3:
                    //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEmpty.Source, imageOk.Source, 140));
                    remote_list.RemoveAt(_remoteIndex);
                    break;
            }
        }

        //Makes desicion of actions with files that based on keys of local and remote files.
        private void SwitchOnKeys(int remoteIndex, int localIndex)
        {
            switch (local_list.keys[localIndex])
            {
                case 0:
                    SwitchIfNotChanged(remoteIndex, localIndex);
                    break;
                case 1:
                    SwitchIfModified(remoteIndex, localIndex);
                    break;
                case 3:
                    SwitchIfDeleted(remoteIndex, localIndex);
                    break;
                default:
                    break;
            }
        }

        private void ReadRemoteStateFile(string _dataStorage)
        {
            string[] remoteSyncData = File.ReadAllLines(_dataStorage);
            for (int i = 0; i < remoteSyncData.Length; i++)
            {
                remote_list.AddRecord(
                    remoteSyncData[i].Substring(0, remoteSyncData[i].Length - 4),
                    Convert.ToInt32(remoteSyncData[i].Substring(remoteSyncData[i].Length - 1, 1)));
            }
        }

        private void CompareHashes()
        {
            //Two lists to temporary collect files' hashes and names of the last sync
            List<string> tmpNamesList = new List<string>();
            List<string> tmpHashList = new List<string>();

            for (int i = 0; i < syncDataStoreRecords_List.Count; i++)
            {
                tmpNamesList.Add(syncDataStoreRecords_List[i].Substring(0, syncDataStoreRecords_List[i].Length - 38));
                tmpHashList.Add(syncDataStoreRecords_List[i].Substring(syncDataStoreRecords_List[i].Length - 32, 32));
            }

            for (int i = 0; i < syncDataStoreRecords_List.Count; i++)
            {
                int searchIndex = tmpNamesList.BinarySearch(syncDataStoreRecords_List[i]);
                if (searchIndex >= 0)
                {
                    string readenHash = tmpHashList[searchIndex];
                    byte[] folderElemBuffer = File.ReadAllBytes(folderToSync.FolderElements[i]);

                    if (Hasher.GetMd5Hash(folderElemBuffer) == readenHash)
                    {
                        AddLocalSyncElements(Path.GetFileName(syncDataStoreRecords_List[i]), 0);
                        tmpNamesList.RemoveAt(searchIndex);
                        tmpHashList.RemoveAt(searchIndex);
                    }
                    else
                    {
                        AddLocalSyncElements(Path.GetFileName(syncDataStoreRecords_List[i]), 1);
                        tmpNamesList.RemoveAt(searchIndex);
                        tmpHashList.RemoveAt(searchIndex);
                    }
                }
                else
                {
                    AddLocalSyncElements(Path.GetFileName(syncDataStoreRecords_List[i]), 2);
                }
            }
            foreach (var i in tmpNamesList)
                AddLocalSyncElements(Path.GetFileName(i), 3);

            tmpNamesList.Clear();
            tmpHashList.Clear();
        }

        public void CreateSyncDataStore()
        {
            //Creating and filling the file, that includes synchronization data of the concrete folder
            folderToSync.CreateServiceFile(syncDataStoreFullPath + @".dat");
            StreamWriter hashTableWriter = new StreamWriter(syncDataStoreFullPath + @".dat");

            byte[] folderElementBuffer;
            for (int i = 0; i < folderToSync.FolderElements.Length; i++)
            {
                folderElementBuffer = File.ReadAllBytes(folderToSync.FolderElements[i]);
                hashTableWriter.WriteLine(folderToSync.FolderElements[i] + "***" + Hasher.GetMd5Hash(folderElementBuffer));
                AddLocalSyncElements(Path.GetFileName(folderToSync.FolderElements[i]), 0);
            }
            //System.Windows.MessageBox.Show("Full directory was succesfully synchronized with database");
            hashTableWriter.Close();
            hashTableWriter.Dispose();
        }

        public void ReadSyncDataStore()
        {
            string[] syncDataStoreRecords = File.ReadAllLines(syncDataStoreFullPath + @".dat");
            syncDataStoreRecords_List = new List<string>(syncDataStoreRecords);

            //DataStore file exists, but it is empty.
            if (syncDataStoreRecords_List.Count == 0)
            {
                //Non-emptiness of the folder means that all included files were created
                if (!(folderToSync.IsFolderEmpty()))
                {
                    for (int i = 0; i < folderToSync.FolderElements.Length; i++)
                        AddLocalSyncElements(Path.GetFileName(folderToSync.FolderElements[i]), 2);
                }
            }
            else
            {
                //Checking: folder's emptiness automatically means that all files were deleted
                if (folderToSync.IsFolderEmpty())
                {
                    for (int i = 0; i < syncDataStoreRecords.Length; i++)
                        AddLocalSyncElements(syncDataStoreRecords[i].Substring(0, syncDataStoreRecords[i].Length - 38), 3);
                }
                else
                {
                    CompareHashes();
                }
            }

            foreach (var file in local_folderToSyncElements)
                local_list.AddRecord(file.elementName, file.modificationFlag);
        }

        public void CompareDevicesSyncData()
        {
            //Searching for remote syncDataStore file
            string[] searchResults = Directory.GetFiles(Directory.GetCurrentDirectory(), @"_rem.txt", SearchOption.AllDirectories);
            if (searchResults.Length == 1)
            {
                ReadRemoteStateFile(searchResults[0]);
                for (int localIndex = 0; localIndex < local_list.Count; localIndex++)
                {
                    int searchIndex = remote_list.names.BinarySearch(local_list.names[localIndex]);
                    if (searchIndex >= 0)
                    {
                        SwitchOnKeys(searchIndex, localIndex);
                    }
                    else
                    {
                        //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageExport.Source, imageWait.Source, 140));
                        uploadList.Add(local_list.names[localIndex]);
                    }
                }

                foreach (var i in remote_list.names)
                    //listView1.Items.Add(new FileItem(i, imageImport.Source, imageWait.Source, 140));
                    downloadList.Add(i);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Файл с удаленного устройства не найден");
            }
        }

        //Create and fill configure file, which includes data about files, that have to be downloaded or deleted
        public void CreateExchangeFile()
        {
            folderToSync.CreateServiceFile(syncDataStoreFullPath + "_configure" + @".txt");
            StreamWriter configWriter = new StreamWriter(syncDataStoreFullPath + "_configure" + @".txt");

            foreach (var file in remoteDelList)
                configWriter.WriteLine(file + "*DELT");

            foreach (var file in downloadList)
                configWriter.WriteLine(file + "*DOWN");

            foreach (var file in uploadList)
                configWriter.WriteLine(file + "*UPLD");

            configWriter.Close();
            configWriter.Dispose();
        }
    }
}