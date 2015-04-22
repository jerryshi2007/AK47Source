
/*
 *  Copyright © Northwoods Software Corporation, 1998-2010. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 *  This software is proprietary to and embodies the confidential
 *  technology of Northwoods Software Corporation. Possession, use, or
 *  copying of this software and media is authorized only pursuant to a
 *  valid written license from Northwoods or an authorized sublicensor.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

using System.Windows;
using System.Windows.Data;





namespace Northwoods.GoXam.Model {

  // Because thrown exceptions are often caught silently,
  // never just "throw new ...Exception(...)",
  // but call ModelHelper.Error(...) instead.
  // These Error methods first write out the error message to Trace listeners,
  // before throwing an InvalidOperationException.

  // But there is no System.Diagnostics.Trace in Silverlight,
  // so those messages go to System.Diagnostics.Debug instead
  // and might be lost on that platform.

  internal static class ModelHelper {



    public static void Trace(String msg) {
      System.Diagnostics.Debug.WriteLine(msg);
    }

    public static void Trace(int indent, String msg) {
      String s = "";
      for (int i = 0; i < indent; i++) s += "  ";
      System.Diagnostics.Debug.WriteLine(s + msg);
    }

    public static void Trace(IDiagramModel model, String msg) {
      System.Diagnostics.Debug.WriteLine(model.Name + "! " + msg);
    }

    public static void Error(String msg) {
      System.Diagnostics.Debug.WriteLine(msg);
      throw new InvalidOperationException(msg);
    }

    public static void Error(IDiagramModel model, String msg) {
      System.Diagnostics.Debug.WriteLine("Error in model " + model.Name);
      System.Diagnostics.Debug.WriteLine(msg);
      throw new InvalidOperationException(msg);
    }

    public static void Error(IDiagramModel model, String msg, Exception ex) {
      System.Diagnostics.Debug.WriteLine("Error in model " + model.Name);
      System.Diagnostics.Debug.WriteLine(msg);
      System.Diagnostics.Debug.WriteLine(ex);
      throw new InvalidOperationException(msg, ex);
    }
















































    //public static T CopyByXmlSerialization<T>(T x) {
    //  System.IO.StringWriter memwriter = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture);  //??? slow and unreliable
    //  System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
    //  serializer.Serialize(memwriter, x);
    //  System.IO.StringReader memreader = new System.IO.StringReader(memwriter.ToString());
    //  return (T)serializer.Deserialize(memreader);
    //}

    public static readonly IEnumerable<Object> NoObjects = new List<Object>().AsReadOnly();

    public static bool SetProperty(String pname, Object obj, Object val) {
      if (pname == null || pname.Length == 0 || obj == null) return false;
      PropertyInfo pi = obj.GetType().GetProperty(pname, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      while (pi != null && !pi.CanWrite) {
        Type parenttype = pi.DeclaringType.BaseType;
        pi = parenttype.GetProperty(pname, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      }
      if (pi != null && pi.CanWrite) {
        try {
          pi.SetValue(obj, val, null);
          return true;
        } catch (Exception) {
        }
      }











      return false;
    }


    private static bool ComputeStringKey(String key, out String prefix, out int suffix) {
      try {
        int len = key.Length-1;
        int i = len;
        while (i >= 0 && Char.IsDigit(key[i])) i--;
        if (i < len) {
          prefix = key.Substring(0, i+1);
          String digits = key.Substring(i+1);
          suffix = Int32.Parse(digits, System.Globalization.NumberFormatInfo.InvariantInfo);
          return false;
        }
      } catch (Exception) {
      }
      prefix = key;
      suffix = -1;
      return true;
    }

    public static bool MakeNodeKeyUnique<K,V>(Type valtype, Object key, String nodekeypath, Object nodedata, Dictionary<K,V> dictionary, String separator, int start) {
      if (key == null) {
        if (valtype.IsAssignableFrom(typeof(String))) {
          key = "";
        } else if (valtype.IsAssignableFrom(typeof(int))) {
          key = 0;
        } else if (valtype.IsAssignableFrom(typeof(Guid))) {
          key = new Guid();
        } else {
          return false;
        }
      }
      if (key is String) {
        String k = (String)key;
        String prefix;
        int suffix;
        if (ModelHelper.ComputeStringKey(k, out prefix, out suffix)) {
          prefix = k + separator;
          suffix = start;
        }
        String newk = prefix + suffix.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        while (dictionary.ContainsKey((K)((Object)newk))) {
          suffix++;
          newk = prefix + suffix.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }
        ModelHelper.SetProperty(nodekeypath, nodedata, newk);
        return true;
      } else if (key is int) {
        int i = (int)((Object)key);
        i++;
        while (dictionary.ContainsKey((K)((Object)i))) {
          i++;
        }
        ModelHelper.SetProperty(nodekeypath, nodedata, i);
        return true;
      } else if (key is Guid) {
        Guid newg = new Guid();
        ModelHelper.SetProperty(nodekeypath, nodedata, newg);
      }
      //??? handle other node key data types
      return false;
    }

  }


  // A convenient way to get the value of a property path for a given source object
  internal sealed class PropPathInfo<K>

    : FrameworkElement

  {
    public PropPathInfo() { }
    public PropPathInfo(String s) { this.Path = s; }

    // The property path, a string, that has the same syntax as used in Bindings.
    // The empty string will refer to the whole source object itself.
    public String Path {
      get { return _Path; }
      set {
        if (_Path != value /* && value != null */) {
          _Path = value;
          _IsSimpleProperty = (_Path != null && _Path.Length > 0 && _Path.IndexOfAny(complexSyntaxChars) < 0);
          _LastType = null;
          _LastPropInfo = null;
        }
      }
    }
    private String _Path;
    private bool _IsSimpleProperty;
    private Type _LastType;
    private PropertyInfo _LastPropInfo;



    private static readonly char[] complexSyntaxChars = new char[] { '.', '(', ')', '[', ']', ',', ':' };


    // Evaluate the property path (this.Path) on the given object.
    // When the Path is an empty string, this will just return the source object.
    // A null Path value just returns the default value for the type K.
    // If the source is null, this just returns the default value for the type K.
    public K EvalFor(Object source) {
      if (source == null) return default(K);
      String path = this.Path;
      if (path == null) return default(K);
      if (path.Length == 0) {
        K result = default(K);
        try {
          result = (K)source;
        } catch (Exception ex) {
          ModelHelper.Trace("Could not convert data: " + source.ToString() + " to Type: " + typeof(K).Name + ";\n  Perhaps the property path should not be an empty string?\n  Exception: " + ex.ToString());
        }
        return result;
      }
      if (_IsSimpleProperty) {
        PropertyInfo fni = null;



        lock (this) {
          Type stype = source.GetType();
          if (stype != _LastType) {
            _LastType = stype;
            _LastPropInfo = stype.GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);











          }
          fni = _LastPropInfo;



        }
        if (fni != null) {
          return (K)fni.GetValue(source, null);
        }









      }

      Binding binding = new Binding(path);
      binding.Source = source;
      SetBinding(DummyProperty, binding);
      K v = (K)GetValue(DummyProperty);
      ClearValue(DummyProperty);
      return v;



    }

    public void SetFor(Object source, K newval) {
      if (source == null) return;
      String path = this.Path;
      if (path == null) return;
      if (path.Length == 0) return;
      if (_IsSimpleProperty) {
        PropertyInfo fni = null;



        lock (this) {
          Type stype = source.GetType();
          if (stype != _LastType) {
            _LastType = stype;
            _LastPropInfo = stype.GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);











          }
          fni = _LastPropInfo;



        }
        if (fni != null) {
          fni.SetValue(source, newval, null);
          return;
        }






      }

      Binding binding = new Binding(path);
      binding.Source = source;
      SetBinding(DummyProperty, binding);
      SetValue(DummyProperty, newval);
      ClearValue(DummyProperty);

    }


    private static readonly DependencyProperty DummyProperty = DependencyProperty.Register("Dummy", typeof(K), typeof(PropPathInfo<K>), new FrameworkPropertyMetadata(default(K)));

  }  // end of internal PropPathInfo class




    [System.Diagnostics.Conditional("UNUSED")]
    internal class SerializableAttribute : Attribute { }

    [System.Diagnostics.Conditional("UNUSED")]
    internal class NonSerializedAttribute : Attribute { }



































































































































































































































































































































































































































}
