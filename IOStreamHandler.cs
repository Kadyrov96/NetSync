using System;
using System.IO;
using System.Net.Security;
using System.Text;

namespace TEST2
{
    class IOStreamHandler: IStreamHandler
    {
        private static int bufSize;
        private int readBytesSize;
        private string fileName;
        private long fileSize;
        FileStream downWriter;

        private byte[] receiveBuffer;
        private byte[] sendBuffer;
        private byte[] bytedFileName;
        private byte[] bytedFileSize;

        private SslStream ssl_stream;
        public SslStream SSL_Stream
        {
            get
            {
                return ssl_stream;
            }
            private set
            {
                ssl_stream = value;
            }
        }

        public IOStreamHandler(SslStream _SSL_Stream)
        {
            this.SSL_Stream = _SSL_Stream;
            this.bufSize = 2048;
            this.receiveBuffer = new byte[bufSize];
        }

        void IStreamHandler.ClearBuffers()
        {
            fileName = "";
            fileSize = 0;
            readBytesSize = 0;
            receiveBuffer = new byte[bufSize];
            downWriter.Close();
            downWriter.Dispose();
        }

        void IStreamHandler.ReceiveData(string savingFolderPath)
        {
            if (SSL_Stream.CanRead)
            {
                //Reading first field of incoming packet - name of the file
                readBytesSize = SSL_Stream.Read(receiveBuffer, 0, bufSize);
                fileName = Encoding.Unicode.GetString(receiveBuffer, 0, readBytesSize);

                //Reading second field of incoming packet - size of the file
                receiveBuffer = new byte[bufSize];
                readBytesSize = SSL_Stream.Read(receiveBuffer, 0, bufSize);
                fileSize = Convert.ToInt64(Encoding.Unicode.GetString(receiveBuffer, 0, receiveBuffer.Length));

                //Reading file content
                receiveBuffer = new byte[fileSize];
                downWriter = File.Create(savingFolderPath + fileName);
                readBytesSize = SSL_Stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                downWriter.Write(receiveBuffer, 0, readBytesSize); 
            }
        }

        void IStreamHandler.SendData(string filePath)
        {
            if (SSL_Stream.CanWrite)
            {
                //Getting info about selected file
                FileInfo fInfo = new FileInfo(filePath);
                fileName = fInfo.Name;
                fileSize = fInfo.Length;

                //Sending first field of the packet - name of the file
                bytedFileName = new byte[bufSize];
                bytedFileName = Encoding.Unicode.GetBytes(fileName.ToCharArray());
                SSL_Stream.Write(bytedFileName, 0, bufSize);

                //Sending second field of the packet - size of the file
                bytedFileSize = new byte[bufSize];
                bytedFileSize = Encoding.Unicode.GetBytes(fileSize.ToString().ToCharArray());
                SSL_Stream.Write(bytedFileSize, 0, bufSize);

                //System.Windows.Forms.MessageBox.Show("Sending the file " + fileName + " (" + fileSize + " bytes)\r\n");
                //Sending file content
                sendBuffer = new byte[fileSize];
                sendBuffer = File.ReadAllBytes(filePath);
                SSL_Stream.Write(sendBuffer, 0, sendBuffer.Length);
            }
        }
    }
}
