using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.UI;
using System.Runtime;

namespace MCS.Web.Responsive.WebControls
{
    public sealed class DeluxeMenuStripItemCollection : ICollection, IList, IEnumerable, IStateManager
    {
        private DeluxeMenuStrip owner;

        public DeluxeMenuStripItemCollection(DeluxeMenuStrip deluxeMenu)
        {
            this.owner = deluxeMenu;
        }

        private ArrayList listItems = new ArrayList();
        private bool marked = false;
        private bool saveAll = false;

        // Methods
        public void Add(string item)
        {
            this.Add(new DeluxeMenuStripItem(item));
        }

        public void Add(DeluxeMenuStripItem item)
        {
            this.listItems.Add(item);
            if (this.marked)
            {
                item.Dirty = true;
            }
        }

        public void AddRange(DeluxeMenuStripItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (DeluxeMenuStripItem item in items)
            {
                this.Add(item);
            }
        }

        public void Clear()
        {
            this.listItems.Clear();
            if (this.marked)
            {
                this.saveAll = true;
            }
        }

        public bool Contains(DeluxeMenuStripItem item)
        {
            return this.listItems.Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            this.listItems.CopyTo(array, index);
        }

        public DeluxeMenuStripItem FindByText(string text)
        {
            int num = this.FindByTextInternal(text, true);
            if (num != -1)
            {
                return (DeluxeMenuStripItem)this.listItems[num];
            }
            return null;
        }

        internal int FindByTextInternal(string text, bool includeDisabled)
        {
            int num = 0;
            foreach (DeluxeMenuStripItem item in this.listItems)
            {
                if (item.Text.Equals(text) && (includeDisabled || item.Enabled))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public DeluxeMenuStripItem FindByValue(string value)
        {
            int num = this.FindByValueInternal(value, true);
            if (num != -1)
            {
                return (DeluxeMenuStripItem)this.listItems[num];
            }
            return null;
        }

        internal int FindByValueInternal(string value, bool includeDisabled)
        {
            int num = 0;
            foreach (DeluxeMenuStripItem item in this.listItems)
            {
                if (item.Value.Equals(value) && (includeDisabled || item.Enabled))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public IEnumerator GetEnumerator()
        {
            return this.listItems.GetEnumerator();
        }

        public int IndexOf(DeluxeMenuStripItem item)
        {
            return this.listItems.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            this.Insert(index, new DeluxeMenuStripItem(item));
        }

        public void Insert(int index, DeluxeMenuStripItem item)
        {
            this.listItems.Insert(index, item);
            if (this.marked)
            {
                this.saveAll = true;
            }
        }

        internal void LoadViewState(object state)
        {
            if (state != null)
            {
                if (state is Pair)
                {
                    Pair pair = (Pair)state;
                    ArrayList first = (ArrayList)pair.First;
                    ArrayList second = (ArrayList)pair.Second;
                    for (int i = 0; i < first.Count; i++)
                    {
                        int num2 = (int)first[i];
                        if (num2 < this.Count)
                        {
                            this[num2].LoadViewState(second[i]);
                        }
                        else
                        {
                            DeluxeMenuStripItem item = new DeluxeMenuStripItem();
                            item.LoadViewState(second[i]);
                            this.Add(item);
                        }
                    }
                }
                else
                {
                    Triplet triplet = (Triplet)state;
                    this.listItems = new ArrayList();
                    this.saveAll = true;
                    string[] strArray = (string[])triplet.First;
                    string[] strArray2 = (string[])triplet.Second;
                    bool[] third = (bool[])triplet.Third;
                    for (int j = 0; j < strArray.Length; j++)
                    {
                        this.Add(new DeluxeMenuStripItem(strArray[j], strArray2[j], third[j]));
                    }
                }
            }
        }

        public void Remove(string item)
        {
            int index = this.IndexOf(new DeluxeMenuStripItem(item));
            if (index >= 0)
            {
                this.RemoveAt(index);
            }
        }

        public void Remove(DeluxeMenuStripItem item)
        {
            int index = this.IndexOf(item);
            if (index >= 0)
            {
                this.RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            this.listItems.RemoveAt(index);
            if (this.marked)
            {
                this.saveAll = true;
            }
        }

        internal object SaveViewState()
        {
            if (this.saveAll)
            {
                int count = this.Count;
                object[] objArray = new string[count];
                object[] objArray2 = new string[count];
                bool[] z = new bool[count];
                for (int j = 0; j < count; j++)
                {
                    objArray[j] = this[j].Text;
                    objArray2[j] = this[j].Value;
                    z[j] = this[j].Enabled;
                }
                return new Triplet(objArray, objArray2, z);
            }
            ArrayList x = new ArrayList(4);
            ArrayList y = new ArrayList(4);
            for (int i = 0; i < this.Count; i++)
            {
                object obj2 = this[i].SaveViewState();
                if (obj2 != null)
                {
                    x.Add(i);
                    y.Add(obj2);
                }
            }
            if (x.Count > 0)
            {
                return new Pair(x, y);
            }
            return null;
        }

        int IList.Add(object item)
        {
            DeluxeMenuStripItem item2 = (DeluxeMenuStripItem)item;
            int num = this.listItems.Add(item2);
            if (this.marked)
            {
                item2.Dirty = true;
            }
            return num;
        }

        bool IList.Contains(object item)
        {
            return this.Contains((DeluxeMenuStripItem)item);
        }

        int IList.IndexOf(object item)
        {
            return this.IndexOf((DeluxeMenuStripItem)item);
        }

        void IList.Insert(int index, object item)
        {
            this.Insert(index, (DeluxeMenuStripItem)item);
        }

        void IList.Remove(object item)
        {
            this.Remove((DeluxeMenuStripItem)item);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void IStateManager.LoadViewState(object state)
        {
            this.LoadViewState(state);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        object IStateManager.SaveViewState()
        {
            return this.SaveViewState();
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        void IStateManager.TrackViewState()
        {
            this.TrackViewState();
        }

        internal void TrackViewState()
        {
            this.marked = true;
            for (int i = 0; i < this.Count; i++)
            {
                this[i].TrackViewState();
            }
        }

        // Properties
        public int Capacity
        {
            get
            {
                return this.listItems.Capacity;
            }
            set
            {
                this.listItems.Capacity = value;
            }
        }

        public int Count
        {
            get
            {
                return this.listItems.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.listItems.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.listItems.IsSynchronized;
            }
        }

        public DeluxeMenuStripItem this[int index]
        {
            get
            {
                return (DeluxeMenuStripItem)this.listItems[index];
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this.listItems[index];
            }
            set
            {
                this.listItems[index] = (DeluxeMenuStripItem)value;
            }
        }

        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return this.marked;
            }
        }
    }
}
