using System.Collections.Generic;
using System.IO;
using System;

namespace TEST2
{
    class Synchroniser
    {
        private FolderHandler folderToSync;
        private string syncDataStoreFolder;
        private string hashedSyncDataStoreFolder;

        private List<string> syncDataStoreRecords_List;
        private List<FileDescript> local_folderToSyncElements;

        public Synchroniser(FolderHandler _folderToSync)
        {
            folderToSync = _folderToSync;
            syncDataStoreFolder = @"C:\Users\Admin\Desktop\Hashes\";
            hashedSyncDataStoreFolder = Hasher.GetMd5Hash(folderToSync.FolderPath);

            localDelList = new List<string>();
            remoteDelList = new List<string>();
            uploadList = new List<string>();
            downloadList = new List<string>();

            local_list = new SyncRecordList();
            remote_list = new SyncRecordList();
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
                    { elementName = Path.GetFileName(folderToSync.FolderElements[i]), modificationFlag = 0 });
            }
            //System.Windows.MessageBox.Show("Full directory was succesfully synchronized with database");
            hashTableWriter.Close();
            hashTableWriter.Dispose();
        }

        public void ReadSyncDataStore()
        {
            //Two lists to temporary collect files' hashes and names of the last sync
            List<string> tmpNamesList = new List<string>();
            List<string> tmpHashList = new List<string>();

            string[] syncDataStoreRecords = File.ReadAllLines(syncDataStoreFolder + hashedSyncDataStoreFolder + @".dat");
            syncDataStoreRecords_List = new List<string>(syncDataStoreRecords);

            //Если файл, хранящий хэши есть, однако он пуст
            if (syncDataStoreRecords.Length == 0)
            {
                if (!(folderToSync.IsFolderEmpty()))
                {
                    for (int i = 0; i < folderToSync.FolderElements.Length; i++)
                    {
                        local_folderToSyncElements.Add(
                            new FileDescript { elementName = Path.GetFileName(folderToSync.FolderElements[i]), modificationFlag = 2 });
                    }
                }
            }
            //Файл с хэшами есть и он не пуст 
            else
            {
                //Checking: folder's emptiness automatically means that all files were deleted
                if (folderToSync.IsFolderEmpty())
                {
                    for (int i = 0; i < syncDataStoreRecords.Length; i++)
                    {
                        local_folderToSyncElements.Add(new FileDescript
                        {
                            elementName = syncDataStoreRecords[i].Substring(0, syncDataStoreRecords[i].Length - 38),
                            modificationFlag = 3
                        });
                    }
                }
                else
                {
                    //Загрузка хэшей предыдущей синхронизации 
                    for (int i = 0; i < syncDataStoreRecords.Length; i++)
                    {
                        tmpNamesList.Add(syncDataStoreRecords_List[i].Substring(0, syncDataStoreRecords_List[i].Length - 38));
                        tmpHashList.Add(syncDataStoreRecords_List[i].Substring(syncDataStoreRecords_List[i].Length - 32, 32));
                    }

                    //Сравнение выгруженных хэшей с хэшами нынешнего состояния директории
                    for (int i = 0; i < syncDataStoreRecords_List.Count; i++)
                    {
                        int searchIndex = tmpNamesList.BinarySearch(syncDataStoreRecords_List[i]);
                        if (searchIndex >= 0)
                        {
                            string readenHash = tmpHashList[searchIndex];
                            byte[] dirEntryBuffer = File.ReadAllBytes(syncDataStoreRecords_List[i]);

                            if (Hasher.GetMd5Hash(dirEntryBuffer) == readenHash)
                            {
                                local_folderToSyncElements.Add(
                                    new FileDescript { elementName = Path.GetFileName(syncDataStoreRecords_List[i]), modificationFlag = 0 });
                                tmpNamesList.RemoveAt(searchIndex);
                                tmpHashList.RemoveAt(searchIndex);
                            }
                            else
                            {
                                local_folderToSyncElements.Add(
                                    new FileDescript { elementName = Path.GetFileName(syncDataStoreRecords_List[i]), modificationFlag = 1 });
                                tmpNamesList.RemoveAt(searchIndex);
                                tmpHashList.RemoveAt(searchIndex);
                            }
                        }
                        else
                        {
                            local_folderToSyncElements.Add(
                                new FileDescript { elementName = Path.GetFileName(syncDataStoreRecords_List[i]), modificationFlag = 2 });
                        }
                    }
                    foreach (var i in tmpNamesList)
                    {
                        local_folderToSyncElements.Add(
                            new FileDescript { elementName = Path.GetFileName(i), modificationFlag = 3 });
                    }
                    tmpNamesList.Clear();
                    tmpHashList.Clear();
                }
            }

            foreach (var file in local_folderToSyncElements)
            {
                local_list.AddRecord(file.elementName, file.modificationFlag);
            }
        }

        private List<string> localDelList;
        private List<string> remoteDelList;

        private List<string> uploadList;
        private List<string> downloadList;

        private SyncRecordList local_list;
        private SyncRecordList remote_list;

        private void SwitchOnKeys(int remoteIndex, int localIndex, StreamWriter _statementFileWriter)
        {
            switch (local_list.keys[localIndex])
            {
                case 0:
                    switch (remote_list.keys[remoteIndex])
                    {
                        case 0:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEmpty.Source, imageOk.Source, 140));
                            remote_list.RemoveAt(remoteIndex);
                            break;
                        case 1:
                            //listView1.Items.Add(new FileItem(syncDirPath+localNamesList[i], imageImport.Source, imageWait.Source, 140));
                            //download_count++;
                            _statementFileWriter.WriteLine(remote_list.names[remoteIndex] + "*DOWN");
                            remote_list.RemoveAt(remoteIndex);
                            break;
                        case 2:
                            break;
                        case 3:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageDel.Source, imageWait.Source, 140));
                            localDelList.Add(local_list.names[localIndex]);
                            remote_list.RemoveAt(remoteIndex);
                            break;
                    }
                    break;

                case 1:
                    switch (remote_list.keys[remoteIndex])
                    {
                        case 0:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageExport.Source, imageWait.Source, 140));
                            uploadList.Add(local_list.names[localIndex]);
                            remote_list.RemoveAt(remoteIndex);
                            break;
                        case 1:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                            break;
                        case 2:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                            break;
                        case 3:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageDel.Source, imageWait.Source, 140));
                            localDelList.Add(local_list.names[localIndex]);
                            remote_list.RemoveAt(remoteIndex);
                            break;
                    }
                    break;

                default:
                    switch (remote_list.keys[remoteIndex])
                    {
                        case 0:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageDel.Source, imageWait.Source, 140));
                            remoteDelList.Add(local_list.names[localIndex]);
                            _statementFileWriter.WriteLine(local_list.names[localIndex] + "*DELT");
                            remote_list.RemoveAt(remoteIndex);
                            break;
                        case 1:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                            break;
                        case 2:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEr.Source, imageWait.Source, 140));
                            break;
                        case 3:
                            //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageEmpty.Source, imageOk.Source, 140));
                            remote_list.RemoveAt(remoteIndex);
                            break;
                    }
                    break;
            }
        }

        public void CompareDevicesSyncData()
        {
            FileStream statementFileCreator = File.Create(syncDataStoreFolder + "ST" + @".txt");
            statementFileCreator.Close();
            StreamWriter statementWriter = new StreamWriter(syncDataStoreFolder + "ST" + @".txt");
            string[] searchResults = Directory.GetFiles(syncDataStoreFolder, @"*@135.txt", SearchOption.AllDirectories);
            if (searchResults.Length == 1)
            {
                string founded = searchResults[0];
                string[] remoteSyncData = File.ReadAllLines(founded);

                for (int i = 0; i < remoteSyncData.Length; i++)
                {
                    remote_list.AddRecord(
                        remoteSyncData[i].Substring(0, remoteSyncData[i].Length - 4), 
                        Convert.ToInt32(remoteSyncData[i].Substring(remoteSyncData[i].Length - 1, 1)));
                }

                for (int i = 0; i < local_list.Count; i++)
                {
                    int searchIndex = remote_list.names.BinarySearch(local_list.names[i]);
                    if (searchIndex >= 0)
                    {
                        SwitchOnKeys(searchIndex, i, statementWriter);
                    }
                    else
                    {
                        //listView1.Items.Add(new FileItem(syncDirPath + localNamesList[i], imageExport.Source, imageWait.Source, 140));
                        uploadList.Add(local_list.names[i]);
                    }
                }
                foreach (var i in remote_list.names)
                {
                    //listView1.Items.Add(new FileItem(i, imageImport.Source, imageWait.Source, 140));
                    statementWriter.WriteLine(i + "*DOWN");
                    //download_count++;
                }
                statementWriter.WriteLine(uploadList.Count);
                statementWriter.Close();
                statementWriter.Dispose();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Файл с удаленного устройства не найден");
            }
        }
    }
}
