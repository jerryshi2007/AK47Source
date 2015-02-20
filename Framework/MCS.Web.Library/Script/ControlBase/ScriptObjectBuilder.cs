// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.Reflection;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Logging;
using System.Security.Principal;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// 脚本对象帮助类
	/// Gets the script references for a type
	/// </summary>
	public static class ScriptObjectBuilder
	{
		const string CSS_LINK = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";

		#region [ Fields ]
		private static readonly Dictionary<Type, List<ResourceEntry>> _cache = new Dictionary<Type, List<ResourceEntry>>();
		private static readonly Dictionary<Type, IList<string>> _cssCache = new Dictionary<Type, IList<string>>();
		private static readonly object _sync = new object();

		#endregion

		/// <summary>
		/// 根据一个对象属性和方法，将此对象转换到客户端对象描述ScriptComponentDescriptor中
		/// Describes an object to a ScriptComponentDescriptor based on its reflected properties and methods
		/// </summary>
		/// <param name="instance">要转换的对象。The object to be described</param>
		/// <param name="descriptor">要填充的脚本描述对象。The script descriptor to fill</param>
		/// <param name="urlResolver">Url转换器。The object used to resolve urls</param>
		/// <param name="controlResolver">Control转换器。The object used to resolve control references</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "controlResolver is checked against null before being used")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Assembly is not localized")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cyclomatic complexity issues not currently being addressed")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "value is assigned/reassigned numerous times - code below favors clarity")]
		public static void DescribeComponent(object instance, ScriptComponentDescriptor descriptor, IUrlResolutionService urlResolver, IControlResolver controlResolver)
		{
			// validate preconditions
			if (instance == null) throw new ArgumentNullException("instance");
			if (descriptor == null) throw new ArgumentNullException("descriptor");
			if (urlResolver == null) urlResolver = instance as IUrlResolutionService;
			if (controlResolver == null) controlResolver = instance as IControlResolver;

			// describe properties
			// PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);

			PropertyInfo[] properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (PropertyInfo prop in properties)
			{
				ScriptControlPropertyAttribute propAttr = null;
				ScriptControlEventAttribute eventAttr = null;
				string propertyName = prop.Name;

				System.ComponentModel.AttributeCollection attribs = new System.ComponentModel.AttributeCollection(Attribute.GetCustomAttributes(prop, false));

				// Try getting a property attribute
				propAttr = (ScriptControlPropertyAttribute)attribs[typeof(ScriptControlPropertyAttribute)];
				if (propAttr == null || !propAttr.IsScriptProperty)
				{
					// Try getting an event attribute
					eventAttr = (ScriptControlEventAttribute)attribs[typeof(ScriptControlEventAttribute)];
					if (eventAttr == null || !eventAttr.IsScriptEvent)
					{
						continue;
					}
				}

				// attempt to rename the property/event
				ClientPropertyNameAttribute nameAttr = (ClientPropertyNameAttribute)attribs[typeof(ClientPropertyNameAttribute)];
				if (nameAttr != null && !string.IsNullOrEmpty(nameAttr.PropertyName))
				{
					propertyName = nameAttr.PropertyName;
				}

				// determine whether to serialize the value of a property.  readOnly properties should always be serialized
				//bool serialize = true;// prop.ShouldSerializeValue(instance) || prop.IsReadOnly;
				//if (serialize)
				//{
				// get the value of the property, skip if it is null
				Control c = null;
				object value = prop.GetValue(instance, new object[0] { });
				if (value == null)
				{
					continue;
				}

				// convert and resolve the value
				if (eventAttr != null && prop.PropertyType != typeof(String))
				{
					throw new InvalidOperationException("ScriptControlEventAttribute can only be applied to a property with a PropertyType of System.String.");
				}
				else
				{
					if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum)
					{
						if (prop.PropertyType == typeof(Color))
						{
							value = ColorTranslator.ToHtml((Color)value);
						}
						else
						{
							// TODO: Determine if we should let ASP.NET AJAX handle this type of conversion, as it supports JSON serialization
							//TypeConverter conv = prop.Converter;
							//value = conv.ConvertToString(null, CultureInfo.InvariantCulture, value);

							//if (prop.PropertyType == typeof(CssStyleCollection))
							//    value = (new CssStyleCollectionJSCoverter()).Serialize(value, new JavaScriptSerializer());
							//if (prop.PropertyType == typeof(Style))
							//    value = (new CssStyleCollectionJSCoverter()).Serialize(((Style)value).GetStyleAttributes(null), new JavaScriptSerializer());                                

							Type valueType = value.GetType();

							JavaScriptConverterAttribute attr = (JavaScriptConverterAttribute)attribs[typeof(JavaScriptConverterAttribute)];
							JavaScriptConverter converter = attr != null ?
								(JavaScriptConverter)TypeCreator.CreateInstance(attr.ConverterType) :
								JSONSerializerFactory.GetJavaScriptConverter(valueType);

							if (converter != null)
								value = converter.Serialize(value, JSONSerializerFactory.GetJavaScriptSerializer());
							else
								value = JSONSerializerExecute.PreSerializeObject(value);

							//Dictionary<string, object> dict = value as Dictionary<string, object>;
							//if (dict != null && !dict.ContainsKey("__type"))
							//    dict["__type"] = valueType.AssemblyQualifiedName;
						}
					}
					if (attribs[typeof(IDReferencePropertyAttribute)] != null && controlResolver != null)
					{
						c = controlResolver.ResolveControl((string)value);
					}
					if (attribs[typeof(UrlPropertyAttribute)] != null && urlResolver != null)
					{
						value = urlResolver.ResolveClientUrl((string)value);
					}
				}

				// add the value as an appropriate description
				if (eventAttr != null)
				{
					if (!string.IsNullOrEmpty((string)value))
						descriptor.AddEvent(propertyName, (string)value);
				}
				else if (attribs[typeof(ElementReferenceAttribute)] != null)
				{
					if (c == null && controlResolver != null) c = controlResolver.ResolveControl((string)value);
					if (c != null) value = c.ClientID;
					descriptor.AddElementProperty(propertyName, (string)value);
				}
				else if (attribs[typeof(ComponentReferenceAttribute)] != null)
				{
					if (c == null && controlResolver != null) c = controlResolver.ResolveControl((string)value);
					if (c != null)
					{
						//ExtenderControlBase ex = c as ExtenderControlBase;
						//if (ex != null && ex.BehaviorID.Length > 0)
						//    value = ex.BehaviorID;
						//else
						value = c.ClientID;
					}
					descriptor.AddComponentProperty(propertyName, (string)value);
				}
				else
				{
					if (c != null) value = c.ClientID;
					descriptor.AddProperty(propertyName, value);
				}
			}
			//}

			// determine if we should describe methods
			foreach (MethodInfo method in instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
			{
				ScriptControlMethodAttribute methAttr = (ScriptControlMethodAttribute)Attribute.GetCustomAttribute(method, typeof(ScriptControlMethodAttribute));
				if (methAttr == null || !methAttr.IsScriptMethod)
				{
					continue;
				}

				// We only need to support emitting the callback target and registering the WebForms.js script if there is at least one valid method
				Control control = instance as Control;
				if (control != null)
				{
					// Force WebForms.js
					control.Page.ClientScript.GetCallbackEventReference(control, null, null, null);

					// Add the callback target
					descriptor.AddProperty("_callbackTarget", control.UniqueID);
				}
				break;
			}
		}


		/// <summary>
		/// 根据控件类型，通过控件Attribute声明获取脚本资源文件引用
		/// Gets the script references for a type
		/// </summary>
		/// <param name="type">控件类型</param>
		/// <returns>脚本资源文件引用集合</returns>
		public static IEnumerable<ScriptReference> GetScriptReferences(Type type)
		{
			return GetScriptReferences(type, false);
		}

		/// <summary>
		/// 根据控件类型，通过控件Attribute声明获取脚本资源文件引用
		/// Gets the script references for a type
		/// </summary>
		/// <param name="type">控件类型</param>
		/// <param name="ignoreStartingTypeReferences">是否忽略开始类型的引用</param>
		/// <returns>脚本资源文件引用集合</returns>
		public static IEnumerable<ScriptReference> GetScriptReferences(Type type, bool ignoreStartingTypeReferences)
		{
			//List<ResourceEntry> entries = GetScriptReferencesInternal((ignoreStartingTypeReferences && (null != type)) ? type.BaseType : type, new Stack<Type>());
			List<ResourceEntry> entries = GetScriptResourceEntries(type, ignoreStartingTypeReferences);

			return ScriptReferencesFromResourceEntries(entries);
		}

		/// <summary>
		/// 根据控件类型，通过控件Attribute声明获取脚本资源文件引用
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static List<ResourceEntry> GetScriptResourceEntries(Type type)
		{
			return GetScriptResourceEntries(type, false);
		}

		/// <summary>
		/// 根据控件类型，通过控件Attribute声明获取脚本资源文件引用
		/// </summary>
		/// <param name="type"></param>
		/// <param name="ignoreStartingTypeReferences"></param>
		/// <returns></returns>
		internal static List<ResourceEntry> GetScriptResourceEntries(Type type, bool ignoreStartingTypeReferences)
		{
			List<ResourceEntry> entries = GetScriptReferencesInternal((ignoreStartingTypeReferences && (null != type)) ? type.BaseType : type, new Stack<Type>());
			return entries;
		}

		/// <summary>
		/// 根据控件，通过控件Attribute声明获取Css资源文件引用
		/// Gets the embedded css file references for a type
		/// </summary>
		/// <param name="page"></param>
		/// <param name="controlType"></param>
		/// <returns>Css资源文件引用集合</returns>
		public static IEnumerable<string> GetCssReferences(Page page, Type controlType)
		{
			return GetCssReferences(page, controlType, new Stack<Type>());
		}

		/// <summary>
		/// 为控件注册Css资源文件引用
		/// Register's the Css references for this control
		/// </summary>
		/// <param name="page"></param>
		/// <param name="controlType"></param>
		public static void RegisterCssReferences(Page page, Type controlType)
		{
			foreach (string styleSheet in ScriptObjectBuilder.GetCssReferences(page, controlType))
			{
				// Add the link to the page header instead of inside the body which is not xhtml compliant
				//HtmlLink link = new HtmlLink();
				//link.Href = styleSheet;
				//link.Attributes.Add("type", "text/css");
				//link.Attributes.Add("rel", "stylesheet");
				//control.Page.Header.Controls.Add(link);
				//ClientCssManager.RegisterHeaderCss(page, styleSheet);
				ClientCssManager.RegisterHeaderEndCss(page, styleSheet);
			}
		}

		/// <summary>
		/// 为控件注册Css资源文件引用
		/// Register's the Css references for this control
		/// </summary>
		/// <param name="control">控件实例</param>
		public static void RegisterCssReferences(Control control)
		{
			RegisterCssReferences(control.Page, control.GetType());
		}

		/// <summary>
		/// 执行控件的回调方法
		/// Executes a callback capable method on a control
		/// </summary>
		/// <param name="control">控件实例</param>
		/// <param name="callInfo">回调参数</param>
		/// <returns>方法返回对象的JSON序列化结果</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Deliberate attempt to catch and pass-on all exceptions")]
		public static string ExecuteCallbackMethod(Control control, Dictionary<string, object> callInfo)
		{
			string methodName = (string)callInfo["name"];
			object[] args = (object[])callInfo["args"];
			string clientState = (string)callInfo["state"];

			// Attempt to load the client state
			IClientStateManager csm = control as IClientStateManager;
			if (csm != null && csm.SupportsClientState)
			{
				csm.LoadClientState(clientState);
			}

			// call the method
			object result = null;
			Dictionary<string, object> error = null;

			Type controlType = control.GetType();

			try
			{
				// Find a matching static or instance method.  Only public methods can be invoked
				MethodInfo mi = controlType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				if (mi == null)
				{
					throw new MissingMethodException(controlType.FullName, methodName);
				}

				// Verify that the method has the corrent number of parameters as well as the ScriptControlMethodAttribute
				ParameterInfo[] methodParams = mi.GetParameters();
				ScriptControlMethodAttribute methAttr = (ScriptControlMethodAttribute)Attribute.GetCustomAttribute(mi, typeof(ScriptControlMethodAttribute));

				if (methAttr == null || !methAttr.IsScriptMethod || args.Length != methodParams.Length)
				{
					throw new MissingMethodException(controlType.FullName, methodName);
				}

				// Convert each argument to the parameter type if possible
				// NOTE: I'd rather have the ObjectConverter from within Microsoft.Web.Script.Serialization namespace for this
				object[] targetArgs = new object[args.Length];
				for (int i = 0; i < targetArgs.Length; i++)
				{
					if (args[i] == null)
						continue;

					targetArgs[i] = JSONSerializerExecute.DeserializeObject(args[i], methodParams[i].ParameterType);
				}

				result = mi.Invoke(control, targetArgs);
			}
			catch (Exception ex)
			{
				// Catch the exception information to relay back to the client
				if (ex is TargetInvocationException)
				{
					ex = ex.InnerException;
				}
				error = new Dictionary<string, object>();
				error["name"] = ex.GetType().FullName;
				error["message"] = ex.Message;

				if (WebUtility.AllowResponseExceptionStackTrace())
					error["stackTrace"] = ex.StackTrace;
				else
					error["stackTrace"] = string.Empty;

				TryWriteLog(ex, control);
			}

			// return the result
			Dictionary<string, object> resultInfo = new Dictionary<string, object>();
			if (error == null)
			{
				resultInfo["result"] = result;
				if (csm != null && csm.SupportsClientState)
				{
					resultInfo["state"] = csm.SaveClientState();
				}
			}
			else
			{
				resultInfo["error"] = error;
			}

			// Serialize the result info into JSON
			return JSONSerializerExecute.Serialize(resultInfo);
		}

		/// <summary>
		/// 写错误日志
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="control"></param>
		private static void TryWriteLog(Exception ex, Control control)
		{
			try
			{
				Logger logger = LoggerFactory.Create("webControlInvokeError");
				LogEntity logEntity = new LogEntity(ex);
				HttpContext context = HttpContext.Current;
				logEntity.Source = control.GetType().Name;
				logEntity.Title = string.Format("{0}控件回调错误", control.GetType().Name);
				logEntity.ExtendedProperties.Add("RequestUrl", context.Request.Url.AbsoluteUri);

				if (HttpContext.Current.User != null)
					logEntity.ExtendedProperties.Add("UserLogOnName", HttpContext.Current.User.Identity.Name);

				logger.Write(logEntity);
			}
			catch
			{
			}
		}

		/// <summary>
		/// ScriptReference objects aren't immutable.  The AJAX core adds context to them, so we cant' reuse them.
		/// Therefore, we track only ReferenceEntries internally and then convert them to NEW ScriptReference objects on-demand.        
		/// </summary>
		/// <param name="entries"></param>
		/// <returns></returns>
		private static IEnumerable<ScriptReference> ScriptReferencesFromResourceEntries(IList<ResourceEntry> entries)
		{
			IList<ScriptReference> referenceList = new List<ScriptReference>(entries.Count);

			foreach (ResourceEntry re in entries)
			{
				referenceList.Add(re.ToScriptReference());
			}
			return referenceList;
		}

		/// <summary>
		/// Gets the script references for a type and walks the type's dependencies with circular-reference checking
		/// </summary>
		/// <param name="type"></param>
		/// <param name="typeReferenceStack"></param>
		/// <returns></returns>
		private static List<ResourceEntry> GetScriptReferencesInternal(Type type, Stack<Type> typeReferenceStack)
		{
			// Verify no circular references
			if (typeReferenceStack.Contains(type))
			{
				throw new InvalidOperationException("Circular reference detected.");
			}

			// Look for a cached set of references outside of the lock for perf.
			//
			List<ResourceEntry> entries;

			if (_cache.TryGetValue(type, out entries))
			{
				return entries;
			}

			// Track this type to prevent circular references
			typeReferenceStack.Push(type);
			try
			{
				lock (_sync)
				{
					// since we're inside the lock, check again just in case.
					//
					if (!_cache.TryGetValue(type, out entries))
					{
						entries = new List<ResourceEntry>();

						// Get the required scripts by type
						List<RequiredScriptAttribute> requiredScripts = new List<RequiredScriptAttribute>();
						foreach (RequiredScriptAttribute attr in type.GetCustomAttributes(typeof(RequiredScriptAttribute), true))
						{
							requiredScripts.Add(attr);
						}

						requiredScripts.Sort(delegate(RequiredScriptAttribute left, RequiredScriptAttribute right) { return left.LoadOrder.CompareTo(right.LoadOrder); });
						foreach (RequiredScriptAttribute attr in requiredScripts)
						{
							if (attr.ExtenderType != null)
							{
								// extrapolate dependant references and add them to the ref list.
								entries.AddRange(GetScriptReferencesInternal(attr.ExtenderType, typeReferenceStack));
							}
						}

						// Get the client script resource values for this type
						int order = 0;

						// create a new list so we can sort it independantly.
						//
						List<ResourceEntry> newEntries = new List<ResourceEntry>();
						for (Type current = type; current != null && current != typeof(object); current = current.BaseType)
						{
							object[] attrs = Attribute.GetCustomAttributes(current, typeof(ClientScriptResourceAttribute), false);
							order -= attrs.Length;

							foreach (ClientScriptResourceAttribute attr in attrs)
							{
								ResourceEntry re = new ResourceEntry(attr.ResourcePath, current, order + attr.LoadOrder, attr.Cacheability);

								// check for dups in the list.
								//
								if (!entries.Contains(re) && !newEntries.Contains(re))
								{
									newEntries.Add(re);
								}
							}
						}

						// sort the list and add it to the array.
						//
						newEntries.Sort(delegate(ResourceEntry l, ResourceEntry r) { return l.Order.CompareTo(r.Order); });
						entries.AddRange(newEntries);

						// Cache the reference list and return
						//
						_cache.Add(type, entries);
					}

					return entries;
				}
			}
			finally
			{
				// Remove the type as further requests will get the cached reference
				typeReferenceStack.Pop();
			}
		}

		/// <summary>
		/// Gets the css references for a type and walks the type's dependencies with circular-reference checking
		/// </summary>
		/// <param name="page"></param>
		/// <param name="type"></param>
		/// <param name="typeReferenceStack"></param>
		/// <returns></returns>
		private static IEnumerable<string> GetCssReferences(Page page, Type type, Stack<Type> typeReferenceStack)
		{
			// Verify no circular references
			if (typeReferenceStack.Contains(type))
			{
				throw new InvalidOperationException("Circular reference detected.");
			}

			// Look for a cached set of references
			IList<string> references;
			if (_cssCache.TryGetValue(type, out references))
			{
				return references;
			}

			// Track this type to prevent circular references
			typeReferenceStack.Push(type);
			try
			{
				lock (_sync)
				{
					// double-checked lock
					if (_cssCache.TryGetValue(type, out references))
					{
						return references;
					}

					// build the reference list
					List<string> referenceList = new List<string>();

					// Get the required scripts by type
					List<RequiredScriptAttribute> requiredScripts = new List<RequiredScriptAttribute>();
					foreach (RequiredScriptAttribute attr in type.GetCustomAttributes(typeof(RequiredScriptAttribute), true))
					{
						requiredScripts.Add(attr);
					}
					requiredScripts.Sort(delegate(RequiredScriptAttribute left, RequiredScriptAttribute right) { return left.LoadOrder.CompareTo(right.LoadOrder); });
					foreach (RequiredScriptAttribute attr in requiredScripts)
					{
						if (attr.ExtenderType != null)
						{
							// extrapolate dependant references
							referenceList.AddRange(GetCssReferences(page, attr.ExtenderType, typeReferenceStack));
						}
					}

					// Get the client script resource values for this type
					WebControlsSection section = ConfigSectionFactory.GetWebControlsSection();
					List<ResourceEntry> entries = new List<ResourceEntry>();
					int order = 0;
					for (Type current = type; current != null && current != typeof(object); current = current.BaseType)
					{
						//if (typeof(WebControl).IsAssignableFrom(current))
						//{
						string configCssUrl = section == null ? string.Empty : section.GetConfigCssUrl(current);
						if (!string.IsNullOrEmpty(configCssUrl))
						{
							referenceList.Add(configCssUrl);
							break;
						}
						//}

						object[] attrs = Attribute.GetCustomAttributes(current, typeof(ClientCssResourceAttribute), false);
						order -= attrs.Length;

						foreach (ClientCssResourceAttribute attr in attrs)
						{
							entries.Add(new ResourceEntry(attr.ResourcePath, current, order + attr.LoadOrder));
						}
					}
					entries.Sort(delegate(ResourceEntry l, ResourceEntry r) { return l.Order.CompareTo(r.Order); });
					foreach (ResourceEntry entry in entries)
					{
						referenceList.Add(page.ClientScript.GetWebResourceUrl(entry.ComponentType, entry.ResourcePath));
					}

					// Remove duplicates from reference list
					Dictionary<string, object> cookies = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
					List<string> newReferenceList = new List<string>();
					foreach (string refr in referenceList)
					{
						if (cookies.ContainsKey(refr))
							continue;
						cookies.Add(refr, null);
						newReferenceList.Add(refr);
					}

					// Create a readonly dictionary to hold the values
					references = new ReadOnlyCollection<string>(newReferenceList);

					// Cache the reference
					_cssCache.Add(type, references);

					// return the list
					return references;
				}
			}
			finally
			{
				// Remove the type as further requests will get the cached reference
				typeReferenceStack.Pop();
			}
		}
	}

	/// <summary>
	/// ResourceEntry
	/// </summary>
	internal struct ResourceEntry
	{
		/// <summary>
		/// 
		/// </summary>
		public string ResourcePath;
		/// <summary>
		/// 
		/// </summary>
		public Type ComponentType;
		/// <summary>
		/// 
		/// </summary>
		public int Order;

		private string RefKey
		{
			get
			{
				string key = String.Format(CultureInfo.CurrentCulture, "{0}#{1}", (ComponentType == null ? "" : ComponentType.Assembly.FullName), ResourcePath);
				return key;
			}
		}

		public ClientScriptCacheability Cacheability;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="componentType"></param>
		/// <param name="order"></param>
		public ResourceEntry(string path, Type componentType, int order)
		{
			ResourcePath = path;
			ComponentType = componentType;
			Order = order;
			Cacheability = ClientScriptCacheability.File;
		}

		public ResourceEntry(string path, Type componentType, int order, ClientScriptCacheability cacheability)
		{
			ResourcePath = path;
			ComponentType = componentType;
			Order = order;
			Cacheability = cacheability;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ScriptReference ToScriptReference()
		{
			ScriptReference refr = new ScriptReference();

			refr.Assembly = ComponentType.Assembly.FullName;
			refr.Name = ResourcePath;

			if (Cacheability == ClientScriptCacheability.None && ScriptControlSection.GetSection().UseScriptReferencesInAssembly == false)
				refr.Path = "ClientScriptCacheability.None" + refr.Name;

			return refr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ResourceEntry other = (ResourceEntry)obj;
			return String.Compare(RefKey, other.RefKey, true, CultureInfo.CurrentCulture) == 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		/// <returns></returns>
		public static bool operator ==(ResourceEntry obj1, ResourceEntry obj2)
		{
			return obj1.Equals(obj2);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		/// <returns></returns>
		public static bool operator !=(ResourceEntry obj1, ResourceEntry obj2)
		{
			return !obj1.Equals(obj2);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return RefKey.GetHashCode();
		}
	}
}
