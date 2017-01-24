using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEST2
{
    internal interface IStreamHandler
    {
        void ReceiveData(string savingFolderPath);
        void SendData(string filePath);
        void ClearBuffers();
    }
}
