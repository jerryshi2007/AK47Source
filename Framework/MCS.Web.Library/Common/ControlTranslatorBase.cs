using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Web.Library
{
	/// <summary>
	/// 
	/// </summary>
	public interface IControlTranslator
	{
		/// <summary>
		/// 
		/// </summary>
		void Translate();
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class ControlTranslatorGenericBase<T> : IControlTranslator
	{
		private T _Control = default(T);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		public ControlTranslatorGenericBase(T control)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(control != null, "control");

			this._Control = control;
		}

		/// <summary>
		/// 
		/// </summary>
		public T Control
		{
			get
			{
				return this._Control;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IAttributeAccessor AttributeAccessor
		{
			get
			{
				return this._Control as IAttributeAccessor;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Category
		{
			get
			{
				string result = string.Empty;

				if (AttributeAccessor != null)
					result = AttributeAccessor.GetAttribute("Category");

				return result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CategoryDefined
		{
			get
			{
				return string.IsNullOrEmpty(this.Category) == false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract void Translate();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="type"></param>
		protected static void CheckControlType(T control, System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(control != null, "control");
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			ExceptionHelper.FalseThrow(control is Type, "Control Type {0} is not {1}", control.GetType().FullName, type.FullName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="objParams"></param>
		/// <returns></returns>
		protected string Translate(string text, params object[] objParams)
		{
			string result = text;

			if (CategoryDefined)
				result = Translator.Translate(Category, text, objParams);

			return result;
		}
	}
}
