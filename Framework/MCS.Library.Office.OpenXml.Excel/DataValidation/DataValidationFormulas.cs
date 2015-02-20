using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	internal class DataValidationFormulaInt : BaseDataValidationFormulaValue<int?>, IDataValidationFormulaInt
	{
		string IDataValidationFormula.GetValueAsString()
		{
			if (this.State == ExcelDataValidationFormulaState.Value)
			{
				return Value.HasValue ? Value.Value.ToString() : string.Empty;
			}
			else
			{
				return this.Formula;
			}
		}
	}

	internal class DataValidationFormulaDecimal : BaseDataValidationFormulaValue<double?>, IDataValidationFormulaDecimal
	{
		string IDataValidationFormula.GetValueAsString()
		{
			if (this.State == ExcelDataValidationFormulaState.Value)
			{
				return Value.HasValue ? Value.Value.ToString("g15", CultureInfo.InvariantCulture) : string.Empty;
			}
			else
			{
				return this.Formula;
			}
		}
	}

	internal class DataValidationFormulaTime : BaseDataValidationFormulaValue<TimeWrapper>, IDataValidationFormulaTime
	{
		string IDataValidationFormula.GetValueAsString()
		{
			if (this.State == ExcelDataValidationFormulaState.Value)
			{
				return Value.ToExcelString();
			}
			else
			{
				return this.Formula;
			}
		}

		internal override void ResetValue()
		{
			Value = new TimeWrapper();
		}
	}

	internal class DataValidationFormulaDateTime : BaseDataValidationFormulaValue<DateTime?>, IDataValidationFormulaDateTime
	{
		string IDataValidationFormula.GetValueAsString()
		{
			if (this.State == ExcelDataValidationFormulaState.Value)
			{
				return Value.HasValue ? Value.Value.ToOADate().ToString(CultureInfo.InvariantCulture) : string.Empty;
			}
			else
			{
				return this.Formula;
			}
		}
	}

	internal class DataValidationFormulaCustom : BaseDataValidationFormula, IDataValidationFormula
	{
		string IDataValidationFormula.GetValueAsString()
		{
			return this.Formula;
		}

		internal override void ResetValue()
		{
			//this.Formula = null;
		}
	}

	internal class DataValidationFormulaList : BaseDataValidationFormula, IDataValidationFormulaList
	{
		#region class DataValidationList
		private class DataValidationList : IList<string>, ICollection
		{
			private List<string> _Items = new List<string>();
			//private EventHandler<EventArgs> _listChanged;

			//public event EventHandler<EventArgs> ListChanged
			//{
			//    add { this._listChanged += value; }
			//    remove { this._listChanged -= value; }
			//}

			//private void OnListChanged()
			//{
			//    if (this._listChanged != null)
			//    {
			//        this._listChanged(this, EventArgs.Empty);
			//    }
			//}

			#region IList members
			int IList<string>.IndexOf(string item)
			{
				return this._Items.IndexOf(item);
			}

			void IList<string>.Insert(int index, string item)
			{
				this._Items.Insert(index, item);
				//OnListChanged();
			}

			void IList<string>.RemoveAt(int index)
			{
				this._Items.RemoveAt(index);
				//OnListChanged();
			}

			string IList<string>.this[int index]
			{
				get
				{
					return this._Items[index];
				}
				set
				{
					this._Items[index] = value;
					//OnListChanged();
				}
			}

			void ICollection<string>.Add(string item)
			{
				this._Items.Add(item);
				//OnListChanged();
			}

			void ICollection<string>.Clear()
			{
				this._Items.Clear();
				//OnListChanged();
			}

			bool ICollection<string>.Contains(string item)
			{
				return this._Items.Contains(item);
			}

			void ICollection<string>.CopyTo(string[] array, int arrayIndex)
			{
				this._Items.CopyTo(array, arrayIndex);
			}

			int ICollection<string>.Count
			{
				get { return this._Items.Count; }
			}

			bool ICollection<string>.IsReadOnly
			{
				get { return false; }
			}

			bool ICollection<string>.Remove(string item)
			{
				bool retVal = this._Items.Remove(item);
				//OnListChanged();
				return retVal;
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				return this._Items.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this._Items.GetEnumerator();
			}
			#endregion

			public void CopyTo(Array array, int index)
			{
				this._Items.CopyTo((string[])array, index);
			}

			int ICollection.Count
			{
				get { return this._Items.Count; }
			}

			public bool IsSynchronized
			{
				get { return ((ICollection)this._Items).IsSynchronized; }
			}

			public object SyncRoot
			{
				get { return ((ICollection)this._Items).SyncRoot; }
			}
		}
		#endregion

		public DataValidationFormulaList()
		{

		}

		private IList<string> _Values;
		public IList<string> Values
		{
			get
			{
				if (this._Values == null)
				{
					this.Formula = string.Empty;
					DataValidationList values = new DataValidationList();
					this.State = ExcelDataValidationFormulaState.Value;
					this._Values = values;
				}
				return this._Values;
			}
			private set
			{
				this.Formula = string.Empty;
				this.State = ExcelDataValidationFormulaState.Value;
				this._Values = value;
			}
		}

		string IDataValidationFormula.GetValueAsString()
		{
			if (this.State == ExcelDataValidationFormulaState.Value)
			{
				if (this._Values != null)
				{
					StringBuilder strItems = new StringBuilder();
					foreach (string str in Values)
					{
						if (strItems.Length == 0)
						{
							strItems.Append("\"");
							strItems.Append(str);
						}
						else
						{
							strItems.AppendFormat(", {0}", str);
						}
					}
					strItems.Append("\"");
					return strItems.ToString();
				}
				else
				{
					return this.Formula;
				}
			}
			else
			{
				return this.Formula;
			}
		}

		internal override void ResetValue()
		{
			if (this._Values != null)
			{
				this.Values.Clear();
			}
		}
	}
}
