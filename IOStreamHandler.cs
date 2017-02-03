using System;
using System.IO;
using System.Net.Security;
using System.Text;

namespace TEST2
{
    class IOStreamHandler: IStreamHandler
    {
        private static int bufSize;
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
            SSL_Stream = _SSL_Stream;
            bufSize = 2048;
        }

        void IStreamHandler.ReceiveData(string savingFolderPath)
        {
            if (SSL_Stream.CanRead)
            {
                byte[] receiveBuffer = new byte[bufSize];

                //Reading first field of incoming packet - name of the file
                int readBytesSize = SSL_Stream.Read(receiveBuffer, 0, bufSize);
                string fileName = Encoding.Unicode.GetString(receiveBuffer, 0, readBytesSize);

                //Reading second field of incoming packet - size of the file
                receiveBuffer = new byte[bufSize];
                readBytesSize = SSL_Stream.Read(receiveBuffer, 0, bufSize);
                long fileSize = Convert.ToInt64(Encoding.Unicode.GetString(receiveBuffer, 0, receiveBuffer.Length));

                //Reading file content
                receiveBuffer = new byte[fileSize];
                FileStream downWriter = File.Create(savingFolderPath + fileName);
                readBytesSize = SSL_Stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                downWriter.Write(receiveBuffer, 0, readBytesSize);

                downWriter.Close();
                downWriter.Dispose();
            }
        }

        void IStreamHandler.SendData(string filePath)
        {
            if (SSL_Stream.CanWrite)
            {
                //Getting info about selected file
                FileInfo fInfo = new FileInfo(filePath);
                string fileName = fInfo.Name;
                long fileSize = fInfo.Length;

                //Sending first field of the packet - name of the file
                byte[] bytedFileName = new byte[bufSize];
                bytedFileName = Encoding.Unicode.GetBytes(fileName.ToCharArray());
                SSL_Stream.Write(bytedFileName, 0, bufSize);

                //Sending second field of the packet - size of the file
                byte[] bytedFileSize = new byte[bufSize];
                bytedFileSize = Encoding.Unicode.GetBytes(fileSize.ToString().ToCharArray());
                SSL_Stream.Write(bytedFileSize, 0, bufSize);

                //System.Windows.Forms.MessageBox.Show("Sending the file " + fileName + " (" + fileSize + " bytes)\r\n");
                //Sending file content
                byte[] sendBuffer = new byte[fileSize];
                sendBuffer = File.ReadAllBytes(filePath);
                SSL_Stream.Write(sendBuffer, 0, sendBuffer.Length);
            }
        }
    }
}
