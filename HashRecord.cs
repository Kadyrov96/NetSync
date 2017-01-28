using System.Collections.Generic;

namespace TEST2
{
    class SyncRecordList
    {
        private int count;
        public int Count
        {
            private set
            {
                count = namesList.Count;
            }
            get
            {
                return count;
            }
        }
        public List<string> names;
        public List<int> keys;
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
