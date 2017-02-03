namespace TEST2
{
    internal interface IStreamHandler
    {
        void ReceiveData(string savingFolderPath);
        void SendData(string filePath);
    }
}