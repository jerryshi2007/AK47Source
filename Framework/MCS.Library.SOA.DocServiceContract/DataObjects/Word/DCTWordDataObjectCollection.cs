using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    [CollectionDataContract()]
    [Serializable]
    public class DCTWordDataObjectCollection:IList<DCTWordDataObject>
    {
        public DCTWordDataObjectCollection()
        {
            this.wordDataList = new List<DCTWordDataObject>();
        }

        private List<DCTWordDataObject> wordDataList;

        public int IndexOf(DCTWordDataObject item)
        {
            return wordDataList.IndexOf(item);
        }

        public void Insert(int index, DCTWordDataObject item)
        {
            wordDataList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            wordDataList.RemoveAt(index);
        }

        public DCTWordDataObject this[int index]
        {
            get
            {
                return wordDataList[index];
            }
            set
            {
                wordDataList[index] = value;
            }
        }

        public void Add(DCTWordDataObject item)
        {
            wordDataList.Add(item);
        }

        public void Clear()
        {
            wordDataList.Clear();
        }

        public bool Contains(DCTWordDataObject item)
        {
            return wordDataList.Contains(item);
        }

        public void CopyTo(DCTWordDataObject[] array, int arrayIndex)
        {
            wordDataList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return wordDataList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DCTWordDataObject item)
        {
            return wordDataList.Remove(item);
        }

        public IEnumerator<DCTWordDataObject> GetEnumerator()
        {
            return wordDataList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return wordDataList.GetEnumerator();
        }
    }
}
