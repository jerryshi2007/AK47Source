#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LruDictionary.cs
// Remark	��	��������������㷨(Least Recently Used Algorithm)ʵ��LruDictionary��LruDictionary���������ʹ�õ�Ԫ�ط�����󲿣�
//              �������ʹ�õ�Ԫ�ط�����ǰ���� LruDictionary�е�ÿ��Ԫ������������ɡ�Key��Value������KeyΪLruDictionary�ļ�ֵ�� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Properties;

namespace MCS.Library.Core
{
	/// <summary>
	/// ��������������㷨(Least Recently Used Algorithm)ʵ�ְ�������ʹ�õ�Ԫ�ط���LruDictionary��ǰ�������������ʹ�õ�Ԫ�ط���LruDictionary�ĺ�
	/// </summary>
	/// <typeparam name="TKey">LruDictionary�ļ�ֵKey������</typeparam>
	/// <typeparam name="TValue">LruDictionary��Value������</typeparam>
	/// <remarks>��������������㷨(Least Recently Used Algorithm)ʵ��LruDictionary��LruDictionary���������ʹ�õ�Ԫ�ط�����󲿣��������ʹ�õ�Ԫ�ط�����ǰ����
	/// LruDictionary�е�ÿ��Ԫ������������ɡ�Key��Value������KeyΪLruDictionary�ļ�ֵ��
	/// </remarks>
	public sealed class LruDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private int maxLength = 100;
		private bool isThreadSafe = false;

		private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> innerDictionary =
			new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();

		private LinkedList<KeyValuePair<TKey, TValue>> innerLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();

		#region ���췽��
		/// <summary>
		/// LruDictionaryû�в����Ĺ��캯����
		/// </summary>
		/// <remarks>�ù��캯����LruDictionary����󳤶���100���˹��췽�������ڹ��첻ָ����󳤶ȵ�LruDictionary��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="LruDictionaryTest" lang="cs" title="��ȡһ���޲��������LruDictionary"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>    
		public LruDictionary()
		{
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="threadSafe">�Ƿ����̰߳�ȫ��</param>
		public LruDictionary(bool threadSafe)
		{
			this.isThreadSafe = threadSafe;
		}

		/// <summary>
		/// ������󳤶�ΪmaxLength��LruDictionary��
		/// </summary>
		/// <param name="maxLruLength">��Ҫ���õ�LruDictionary����󳤶ȡ�</param>
		/// <remarks>��Ĭ������£�LruDictionary����󳤶�Ϊ100���˹��췽�������ڹ���ָ����󳤶ȵ�LruDictionary��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="LruDictionaryTest" lang="cs" title="��ȡ��󳤶�ΪmaxLength��LruDictionary"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public LruDictionary(int maxLruLength)
		{
			ExceptionHelper.TrueThrow<InvalidOperationException>(maxLruLength < 0, "maxLruLength must >= 0");

			this.maxLength = maxLruLength;
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="maxLruLength">>��Ҫ���õ�LruDictionary����󳤶ȡ�</param>
		/// <param name="threadSafe">�Ƿ����̰߳�ȫ��</param>
		/// <remarks>��Ĭ������£�LruDictionary����󳤶�Ϊ100���˹��췽�������ڹ���ָ����󳤶ȵ�LruDictionary��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="LruDictionaryTest" lang="cs" title="��ȡ��󳤶�ΪmaxLength��LruDictionary"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public LruDictionary(int maxLruLength, bool threadSafe)
		{
			ExceptionHelper.TrueThrow<InvalidOperationException>(maxLruLength < 0, "maxLruLength must >= 0");

			this.maxLength = maxLruLength;
			this.isThreadSafe = threadSafe;
		}
		#endregion

		#region ��������
		/// <summary>
		/// �Ƿ����̰߳�ȫ�ġ����ʵ�ʱ���Ƿ����ͬ����
		/// </summary>
		public bool IsThreadSafe
		{
			get
			{
				return this.isThreadSafe;
			}
		}

		/// <summary>
		/// ͬ������LruDictionary������
		/// </summary>
		/// <remarks>ͬ������LruDictionary�����ԣ���������ֻ���ġ�</remarks>
		public object SyncRoot
		{
			get
			{
				return ((ICollection)this.innerDictionary).SyncRoot;
			}
		}

		/// <summary>
		/// LruDictionary����󳤶�
		/// </summary>
		/// <remarks>LruDictionary����󳤶ȣ��������ǿɶ���д�ġ�</remarks>
		public int MaxLength
		{
			get { return this.maxLength; }
			set { this.maxLength = value; }
		}

		/// <summary>
		/// LruDictionary�ĵ�ǰ����
		/// </summary>
		/// <remarks>��ǰLruDictionary�ĳ��ȣ���������ֻ���ġ�</remarks>
		public int Count
		{
			get
			{
				return this.innerDictionary.Count;
			}
		}
		#endregion

		#region ���з���
		/// <summary>
		/// ��LruDictionary�����һ��Ԫ�أ��û���Ҫ�����Ԫ�ص�Key��Valueֵ���롣
		/// </summary>
		/// <param name="key">��LruDictionary������Ԫ�ص�Keyֵ</param>
		/// <param name="data">��LruDictionary������Ԫ�ص�Valueֵ</param>
		/// <remarks>��LruDictionary�����һ��Ԫ��ʱ����Ԫ������Key��Valueֵ��ɡ�����LRUԭ�򣬾���ʹ�õ�Ԫ�ط���LruDictionary��ǰ�档
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="AddTest" lang="cs" title="��LruDictionary�����һ��Ԫ��"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		///</remarks>
		public void Add(TKey key, TValue data)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-Add-key");//add by yuanyong 20071227

			//����Ѿ����ڣ��׳��쳣
			ExceptionHelper.TrueThrow<ArgumentException>(this.innerDictionary.ContainsKey(key),
				Resource.DuplicateKey, key);

			DoSyncOp(() =>
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> node =
					new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, data));

				this.innerDictionary.Add(key, node);
				this.innerLinkedList.AddFirst(node);

				if (this.innerLinkedList.Count >= MaxLength)
				{
					for (int i = 0; i < this.innerLinkedList.Count - MaxLength + 1; i++)
					{
						LinkedListNode<KeyValuePair<TKey, TValue>> lastNode = this.innerLinkedList.Last;

						if (this.innerDictionary.ContainsKey(lastNode.Value.Key))
						{
							this.innerDictionary.Remove(lastNode.Value.Key);
							this.innerLinkedList.Remove(lastNode);
						}
					}
				}
			});
		}

		/// <summary>
		/// ��ȡ������LruDictionary��Ԫ�ؼ�ֵΪkeyֵ��Valueֵ
		/// </summary>
		/// <param name="key">Ҫ��õ�Ԫ�صļ�ֵ</param>
		/// <returns>LruDictionary�м�ֵkey��Valueֵ</returns>
		/// <remarks>�������ǿɶ���д�ġ�
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" lang="cs" title="��ȡ������LruDictionary�м�ֵΪKeyֵ��Valueֵ"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public TValue this[TKey key]
		{
			get
			{
				ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-get-key");//add by yuanyong 20071227

				LinkedListNode<KeyValuePair<TKey, TValue>> result = null;

				DoSyncOp(() =>
				{
					//û���ҵ������Զ��׳��쳣
					LinkedListNode<KeyValuePair<TKey, TValue>> node = this.innerDictionary[key];
					MoveNodeToFirst(node);

					result = node;
				});

				return result.Value.Value;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-set-key");//add by yuanyong 20071227

				DoSyncOp(() =>
				{
					LinkedListNode<KeyValuePair<TKey, TValue>> node;

					if (this.innerDictionary.TryGetValue(key, out node))
					{
						MoveNodeToFirst(node);

						node.Value = new KeyValuePair<TKey, TValue>(key, value);
					}
					else
						Add(key, value);
				});
			}
		}

		/// <summary>
		/// ɾ��LruDictionary�м�ֵΪKeyֵ��Ԫ��
		/// </summary>
		/// <param name="key">��ֵKey</param>
		/// <remarks>��LruDictionary�в�������ֵΪKey��Ԫ�أ���ϵͳ�Զ����׳��쳣��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="RemoveTest" lang="cs" title="��LruDictionary��ɾ����ֵΪKey��Ԫ��"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public void Remove(TKey key)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-Remove-key");//add by yuanyong 20071227

			DoSyncOp(() =>
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> node = null;

				if (this.innerDictionary.TryGetValue(key, out node))
				{
					this.innerDictionary.Remove(key);
					this.innerLinkedList.Remove(node);
				}
			});
		}

		/// <summary>
		/// �ж�LruDictionary���Ƿ������ֵΪKeyֵ��Ԫ��
		/// </summary>
		/// <param name="key">��ֵKey</param>
		/// <returns>��LruDictionary�а�����ֵΪkeyֵ��Ԫ��,�򷵻�true�����򷵻�false</returns>
		/// <remarks>������ֵΪtrue�����ڸ�Keyֵ��Ԫ�ظ�ʹ�ù�����Ѹ�Ԫ�ط���LruDictionary����ǰ�档
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region ="ContainsKeyTest" lang="cs" title="�ж�LruDictionary���Ƿ������ֵΪkey��Ԫ��"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public bool ContainsKey(TKey key)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-ContainsKey-key");//add by yuanyong 20071227

			return this.innerDictionary.ContainsKey(key);
		}

		/// <summary>
		/// �ж�LruDictionary���Ƿ������ֵΪKeyֵ��Ԫ�ء����������򷵻�ֵ��true�����Դ�data��ȡ����ֵ�����򷵻�false��
		/// </summary>
		/// <param name="key">��ֵkey</param>
		/// <param name="data">��ֵkey��Valueֵ</param>
		/// <returns>����true��false</returns>
		/// <remarks>������ֵΪtrue�����ڸ�Keyֵ��Ԫ�ظ�ʹ�ù�����Ѹ�Ԫ�ط���LruDictionary����ǰ�档
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="TryGetValueTest" lang="cs" title="��ͼ��LruDictionary��ȡ����ֵΪkey��Ԫ��"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public bool TryGetValue(TKey key, out TValue data)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-TryGetValue-key");//add by yuanyong 20071227

			TValue returnData = default(TValue);

			bool result = false;

			DoSyncOp(() =>
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> node;

				result = this.innerDictionary.TryGetValue(key, out node);

				if (result)
				{
					MoveNodeToFirst(node);

					returnData = node.Value.Value;
				}
			});

			data = returnData;

			return result;
		}

		/// <summary>
		/// ���LruDictionary�ڵ�����ֵ
		/// </summary>
		/// <remarks>��ʱLruDictionary��û���κ�Ԫ��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region ="ClearTest" lang="cs" title="���LruDictionary�е�����Ԫ��"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		public void Clear()
		{
			DoSyncOp(() =>
			{
				this.innerDictionary.Clear();
				this.innerLinkedList.Clear();
			});
		}
		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> ��Ա
		/// <summary>
		/// ���LruDictionary������Ԫ�ص�ö����
		/// </summary>
		/// <returns>LruDictionary������Ԫ�ص�ö����</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\LruDictionaryTest.cs" region="GetEnumeratorTest" lang="cs" title="���LruDictionary������Ԫ�ص�ö����"/>
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		///</remarks>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			IEnumerator<KeyValuePair<TKey, TValue>> result = null;

			DoSyncOp(() =>
			{
				result = EnumItems();
			});

			return result;
		}

		#endregion

		#region IEnumerable ��Ա

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return EnumItems();
		}

		#endregion

		private IEnumerator<KeyValuePair<TKey, TValue>> EnumItems()
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> node = this.innerLinkedList.First;

			while (node != null)
			{
				yield return node.Value;
				node = node.Next;
			}
		}

		private void MoveNodeToFirst(LinkedListNode<KeyValuePair<TKey, TValue>> node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			LinkedList<KeyValuePair<TKey, TValue>> list = node.List;

			list.Remove(node);
			list.AddFirst(node);
		}

		private void DoSyncOp(Action action)
		{
			DoSyncOp(this.isThreadSafe, action);
		}

		private void DoSyncOp(bool threadSafe, Action action)
		{
			if (action != null)
			{
				if (threadSafe)
				{
					lock (this.SyncRoot)
					{
						action();
					}
				}
				else
					action();
			}
		}
	}
}
