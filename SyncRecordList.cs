using System.Collections.Generic;

namespace NetSync_WinDesktop
{
    class SyncRecordList
    {
        public List<string> names;
        public List<int> keys;
        public int Count
        {
            private set
            {
                Count = names.Count;
            }
            get
            {
                return Count;
            }
        }

        public SyncRecordList()
        {
            names = new List<string>();
            keys = new List<int>();
        }

        public void AddRecord(string name, int key)
        {
            names.Add(name);
            keys.Add(key);
        }

        public void RemoveAt(int index)
        {
            names.RemoveAt(index);
            keys.RemoveAt(index);
        }
    }
}
