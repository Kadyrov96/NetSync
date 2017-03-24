namespace NetSync_WinDesktop
{
    internal interface IStreamHandler
    {
        void ReceiveData(string savingFolderPath);
        void SendData(string filePath);

        string ReceiveString();
        int ReceiveNum();

        void SendString();
        void SendNum();
    }
}