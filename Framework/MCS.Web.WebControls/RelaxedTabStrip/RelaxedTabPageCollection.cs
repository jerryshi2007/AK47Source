using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 表示<see cref="RelaxedTabPage"/>的集合
	/// </summary>
	public class RelaxedTabPageCollection : ControlCollection
	{
		/// <summary>
		/// 使用指定的拥有者初始化<see cref="RelaxedTabPageCollection"/>
		/// </summary>
		/// <param name="owner">拥有者<see cref="Control"/>对象，应为一个<see cref="RelaxedTabStrip"/>对象。</param>
		public RelaxedTabPageCollection(Control owner)
			: base(owner)
		{
		}

		/// <summary>
		/// 获取此集合的拥有者<see cref="RelaxedTabStrip"/>
		/// </summary>
		public new RelaxedTabStrip Owner
		{
			get
			{
				return base.Owner as RelaxedTabStrip;
			}
		}

		/// <summary>
		/// 获取指定下标处的<see cref="RelaxedTabPage"/>。
		/// </summary>
		/// <param name="i">从0开始的索引，表示要获取的<see cref="RelaxedTabPage"/>在集合中的位置。</param>
		/// <returns></returns>
		public new RelaxedTabPage this[int i]
		{
			get
			{
				return (RelaxedTabPage)base[i];
			}
		}

		/// <summary>
		/// 将指定的<see cref="RelaxedTabPage"/>添加到集合
		/// </summary>
		/// <param name="v">一个<see cref="RelaxedTabPage"/>对象</param>
		public override void Add(Control v)
		{
			if (!(v is RelaxedTabPage))
			{
				throw new ArgumentException("RelaxedTabPageCollection只能包含RelaxedTabPage");
			}

			base.Add(v);
		}

		/// <summary>
		/// 将指定的<see cref="RelaxedTabPage"/>添加到集合的指定下标处
		/// </summary>
		/// <param name="index">从0开始的索引，表示将<see cref="RelaxedTabPage"/>添加到集合中的位置。</param>
		/// <param name="v">一个<see cref="RelaxedTabPage"/>对象</param>
		public override void AddAt(int index, Control v)
		{
			if (!(v is RelaxedTabPage))
			{
				throw new ArgumentException("RelaxedTabPageCollection只能包含RelaxedTabPage");
			}

			base.AddAt(index, v);
		}
	}
}