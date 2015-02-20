using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class Paragraph
	{
		public Paragraph()
		{

		}

		public Paragraph(string text)
		{
			this.Text = text;
		}

		public string LatinFont
		{
			get;
			set;
		}

		public string ComplexFont
		{
			get;
			set;
		}

		public bool Bold
		{
			get;
			set;
		}

		private ExcelUnderLineType _UnderLine = ExcelUnderLineType.None;
		public ExcelUnderLineType UnderLine
		{
			get
			{
				return this._UnderLine;
			}
			set
			{
				this._UnderLine = value;
			}
		}

		private Color _UnderLineColor = Color.Empty;
		public Color UnderLineColor
		{
			get
			{
				return this._UnderLineColor;
			}
			set
			{
				this._UnderLineColor = value;
			}
		}

		public bool Italic
		{
			get;
			set;
		}

		private ExcelStrikeType _Strike = ExcelStrikeType.No;
		public ExcelStrikeType Strike
		{
			get
			{
				return this._Strike;
			}
			set
			{
				this._Strike = value;
			}
		}

		private float _Size = 11;
		public float Size
		{
			get
			{
				return this._Size;
			}
			set
			{
				this._Size = value;
			}
		}

		private Color _Color = Color.Empty;
		public Color Color
		{
			get
			{
				return this._Color;
			}
			set
			{
				this._Color = value;
			}
		}

		public string Text
		{
			get;
			set;
		}
	}

	public sealed class ParagraphCollection : IEnumerable<Paragraph>
	{
		private List<Paragraph> _list = new List<Paragraph>();

		public Paragraph this[int Index]
		{
			get
			{
				return this._list[Index];
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public void Add(string text)
		{
			Paragraph rt = new Paragraph(text);
			rt.ComplexFont = "Calibri";
			rt.LatinFont = "Calibri";
			rt.Size = 11;

			rt.Text = text;
			this._list.Add(rt);
		}

		public string Text
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (Paragraph item in this._list)
				{
					sb.Append(item.Text);
				}
				return sb.ToString();
			}
			set
			{
				if (Count == 0)
				{
					Add(value);
				}
				else
				{
					this[0].Text = value;
					int count = Count;
					for (int ix = Count - 1; ix > 0; ix--)
					{
						RemoveAt(ix);
					}
				}
			}
		}

		public void RemoveAt(int Index)
		{
			this._list.RemoveAt(Index);
		}

		public void Clear()
		{
			this._list.Clear();
		}

		public IEnumerator<Paragraph> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._list.GetEnumerator();
		}
	}
}
