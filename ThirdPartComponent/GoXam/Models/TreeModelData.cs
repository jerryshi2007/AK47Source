
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Windows;  // for Point

namespace Northwoods.GoXam.Model {

  /// <summary>
  /// A simple representation of node data for <see cref="TreeModel{NodeType, NodeKey}"/>
  /// that supports property change notification, copying, and undo
  /// via the <c>INotifyPropertyChanged</c>, <c>ICloneable</c>, and <see cref="IChangeDataValue"/> interfaces.
  /// </summary>
  /// <typeparam name="NodeKey">the Type of a value uniquely identifying a node data in the model</typeparam>
  /// <remarks>
  /// <para>
  /// This provides a standard implementation of
  /// <see cref="TreeModel{NodeType, NodeKey}"/>
  /// data that represents nodes and includes a property for
  /// referring to the "parent" node, and a property for holding a list of
  /// "children" nodes.
  /// You can use this class if you do not already have your own application class
  /// holding information about nodes and if you want to inherit from an existing
  /// class so that you can just add your own properties.  Here's a simple example:
  /// </para>
  /// <para>
  /// <code>
  ///  [Serializable]
  ///  public class MyData : TreeModelNodeData&lt;String&gt; {
  ///    public MyData() { }
  ///
  ///    public String Name {
  ///      get { return _Name; }
  ///      set { if (_Name != value) { String old = _Name; _Name = value; RaisePropertyChanged("Name", old, value); } }
  ///    }
  ///    private String _Name;
  ///
  ///    public String Address {
  ///      get { return _Address; }
  ///      set { if (_Address != value) { String old = _Address; _Address = value; RaisePropertyChanged("Address", old, value); } }
  ///    }
  ///    private String _Address;
  ///  }
  /// </code>
  /// </para>
  /// <para>
  /// Note that property setters need to raise the model's Changed event,
  /// so that the model knows about changes in the data and can then update the diagram.
  /// You should call <see cref="RaisePropertyChanged"/> only when the value has actually changed,
  /// and you should pass both the previous and the new values, in order to support undo/redo.
  /// </para>
  /// <para>
  /// For both Silverlight and WPF you should override the <see cref="Clone"/> method
  /// if the fields contain data that should not be shared between copies.
  /// For WPF the properties that you define should also be serializable,
  /// in order for the data to be copiable, especially to and from the clipboard.
  /// </para>
  /// <para>
  /// If you add properties to this node data class, and if you are using the
  /// <see cref="TreeModel{NodeType, NodeKey}.Save"/> and
  /// <see cref="TreeModel{NodeType, NodeKey}.Load"/> methods,
  /// you should override the <see cref="MakeXElement"/> and <see cref="LoadFromXElement"/>
  /// methods to add new attributes and/or elements as needed,
  /// </para>
  /// <para>
  /// Normally, each <see cref="Key"/> should have a unique value within the model.
  /// You can maintain that yourself, by setting the <see cref="Key"/> to unique values
  /// before adding the node data to the model's collection of nodes.
  /// Or you can ensure this by overriding the
  /// <see cref="TreeModel{NodeType, NodeKey}.MakeNodeKeyUnique"/>
  /// method.  The override (or the setting of the same-named delegate in
  /// <see cref="TreeModel{NodeType, NodeKey}.Delegates"/>)
  /// is required if nodes might be copied within the model.
  /// </para>
  /// <para>
  /// If you want each node to keep a reference to its "parent" node,
  /// you can use the <see cref="ParentKey"/> property.
  /// If you want each node to keep a list of reference to its "children" nodes,
  /// you can use the <see cref="ChildKeys"/> property, which is a list of node keys.
  /// You can use both <see cref="ParentKey"/> and <see cref="ChildKeys"/> at the same time.
  /// </para>
  /// </remarks>
  [Serializable]
  public class TreeModelNodeData<NodeKey> : INotifyPropertyChanged, IChangeDataValue, ICloneable {
    /// <summary>
    /// The default constructor produces an empty object.
    /// </summary>
    public TreeModelNodeData() { }
    /// <summary>
    /// This constructor also initializes the <see cref="Key"/> property.
    /// </summary>
    /// <param name="key"></param>
    public TreeModelNodeData(NodeKey key) { _Key = key; }

    /// <summary>
    /// Create a copy of this data; this implements the <c>ICloneable</c> interface.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// When you add your own state in a subclass, and when you expect to be able to copy the data,
    /// you should override this method in your derived class when it has some fields that are object references.
    /// Your override method should first call <c>base.Clone()</c> to get the newly copied object.
    /// The result should be the object you return,
    /// after performing any other deeper copying of referenced objects that you deem necessary,
    /// and after removing references that should not be shared (such as to cached data structures).
    /// </para>
    /// <para>
    /// The standard implementation of this method is to do a shallow copy, by <c>Object.MemberwiseClone()</c>,
    /// and reinitialize the <see cref="ParentKey"/> and <see cref="ChildKeys"/> properties.
    /// You do not need to override this method if you have only added some fields/properties
    /// that are values or are references to intentionally shared objects.
    /// </para>
    /// </remarks>
    public virtual object Clone() {
      TreeModelNodeData<NodeKey> d = (TreeModelNodeData<NodeKey>)MemberwiseClone();
      d.PropertyChanged = null;
      d._ParentKey = default(NodeKey);
      d._ChildKeys = new ObservableCollection<NodeKey>();
      return d;
    }

    /// <summary>
    /// This event implements the <see cref="INotifyPropertyChanged"/> interface,
    /// so that both the model and the dependency object system can be informed
    /// of changes to property values.
    /// </summary>
    [field: NonSerializedAttribute()]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnPropertyChanged(ModelChangedEventArgs e) {
      if (this.PropertyChanged != null) this.PropertyChanged(this, e);
    }

    /// <summary>
    /// Call this method from property setters to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="pname">the property name</param>
    /// <param name="oldval">the value before the property was set</param>
    /// <param name="newval">the new value</param>
    /// <remarks>
    /// Only call this method when the property value actually changes.
    /// The <paramref name="oldval"/> and <paramref name="newval"/> values are needed
    /// to support undo/redo.
    /// </remarks>
    protected void RaisePropertyChanged(String pname, Object oldval, Object newval) {
      OnPropertyChanged(new ModelChangedEventArgs(pname, this, oldval, newval));
    }

    /// <summary>
    /// For debugging, use the <see cref="Key"/> property as this object's default text rendering.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      return (this.Key != null) ? this.Key.ToString() : "(nokey)";
    }

    /// <summary>
    /// This method implements the <see cref="IChangeDataValue"/> interface,
    /// used to perform state changes for undo and redo.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    /// <remarks>
    /// Unless you override this method to explicitly handle each property that you define,
    /// this implementation uses reflection to set the property.
    /// </remarks>
    public virtual void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      if (e.PropertyName == "Location") {
        this.Location = (Point)e.GetValue(undo);
      } else if (e.PropertyName == "Text") {
        this.Text = (String)e.GetValue(undo);
      } else if (e.PropertyName == "Key") {
        this.Key = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "IsTreeExpanded") {
        this.IsTreeExpanded = (bool)e.GetValue(undo);
      } else if (e.PropertyName == "WasTreeExpanded") {
        this.WasTreeExpanded = (bool)e.GetValue(undo);
      } else if (e.PropertyName == "ParentKey") {
        this.ParentKey = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "ChildKeys") {
        this.ChildKeys = (IList<NodeKey>)e.GetValue(undo);
      } else if (e.PropertyName == "Category") {
        this.Category = (String)e.GetValue(undo);
      } else if (e.Change == ModelChange.Property) {
        if (!ModelHelper.SetProperty(e.PropertyName, e.Data, e.GetValue(undo))) {
          ModelHelper.Error("ERROR: Unrecognized property name: " + e.PropertyName != null ? e.PropertyName : "(noname)" + " in TreeModelNodeData.ChangeDataValue");
        }
      }
    }


    /// <summary>
    /// Constructs a Linq for XML <c>XElement</c> holding the data of this node.
    /// </summary>
    /// <param name="n">the name of the new <c>XElement</c></param>
    /// <returns>an initialized <c>XElement</c></returns>
    /// <remarks>
    /// <para>
    /// This constructs a new <c>XElement</c> and adds an <c>XAttribute</c> for each simple property
    /// that has a value different from its default value.
    /// For each property that is a collection, it adds an <c>XElement</c> with nested item elements.
    /// This does not add an element if the collection is empty.
    /// </para>
    /// <para>
    /// Because the <typeparamref name="NodeKey"/> type might be a type for which we have an implementation
    /// to convert to and from strings for XML, this calls the <see cref="ConvertNodeKeyToString"/> method,
    /// which you can override.
    /// </para>
    /// <para>
    /// This is implemented as:
    /// <code>
    /// public virtual XElement MakeXElement(XName n) {
    ///   XElement e = new XElement(n);
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("Key", this.Key, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute("Category", this.Category, ""));
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("ParentKey", this.ParentKey, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Elements&lt;NodeKey&gt;("ChildKeys", "Key", this.ChildKeys, ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute("IsTreeExpanded", this.IsTreeExpanded, true));
    ///   e.Add(XHelper.Attribute("WasTreeExpanded", this.WasTreeExpanded, false));
    ///   e.Add(XHelper.Attribute("Location", this.Location, new Point(Double.NaN, Double.NaN)));
    ///   e.Add(XHelper.Attribute("Text", this.Text, ""));
    ///   return e;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If you add properties to this node data class, and if you are using the
    /// <see cref="TreeModel{NodeType, NodeKey}.Save"/> and
    /// <see cref="TreeModel{NodeType, NodeKey}.Load"/> methods,
    /// you should override this method to add new attributes and/or elements as needed,
    /// and you should override <see cref="LoadFromXElement"/>.
    /// </para>
    /// </remarks>
    public virtual XElement MakeXElement(XName n) {
      XElement e = new XElement(n);
      e.Add(XHelper.Attribute<NodeKey>("Key", this.Key, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Attribute("Category", this.Category, ""));
      e.Add(XHelper.Attribute<NodeKey>("ParentKey", this.ParentKey, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Elements<NodeKey>("ChildKeys", "Key", this.ChildKeys, ConvertNodeKeyToString));
      e.Add(XHelper.Attribute("IsTreeExpanded", this.IsTreeExpanded, true));
      e.Add(XHelper.Attribute("WasTreeExpanded", this.WasTreeExpanded, false));
      e.Add(XHelper.Attribute("Location", this.Location, new Point(Double.NaN, Double.NaN)));
      e.Add(XHelper.Attribute("Text", this.Text, ""));
      return e;
    }

    /// <summary>
    /// Initialize this node data with data held in a Linq for XML <c>XElement</c>.
    /// </summary>
    /// <param name="e">the <c>XElement</c></param>
    /// <remarks>
    /// <para>
    /// This sets this node data's properties by reading the data from attributes and nested elements
    /// of the given <c>XElement</c>.
    /// </para>
    /// <para>
    /// Because the <typeparamref name="NodeKey"/> type might be a type for which we have an implementation
    /// to convert to and from strings for XML, this calls the <see cref="ConvertStringToNodeKey"/> method,
    /// which you can override.
    /// </para>
    /// <para>
    /// This is implemented as:
    /// <code>
    /// public virtual void LoadFromXElement(XElement e) {
    ///   this.Key = XHelper.Read&lt;NodeKey&gt;("Key", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.Category = XHelper.Read("Category", e, "");
    ///   this.ParentKey = XHelper.Read&lt;NodeKey&gt;("ParentKey", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.ChildKeys = (IList&lt;NodeKey&gt;)XHelper.ReadElements&lt;NodeKey&gt;(e.Element("ChildKeys"), "Key", new ObservableCollection&lt;NodeKey&gt;(), ConvertStringToNodeKey);
    ///   this.IsTreeExpanded = XHelper.Read("IsTreeExpanded", e, true);
    ///   this.WasTreeExpanded = XHelper.Read("WasTreeExpanded", e, false);
    ///   this.Location = XHelper.Read("Location", e, new Point(Double.NaN, Double.NaN));
    ///   this.Text = XHelper.Read("Text", e, "");
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If you add properties to this node data class, and if you are using the
    /// <see cref="TreeModel{NodeType, NodeKey}.Save"/> and
    /// <see cref="TreeModel{NodeType, NodeKey}.Load"/> methods,
    /// you should override this method to add new attributes and/or elements as needed,
    /// and you should override <see cref="MakeXElement"/>.
    /// </para>
    /// </remarks>
    public virtual void LoadFromXElement(XElement e) {
      if (e == null) return;
      this.Key = XHelper.Read<NodeKey>("Key", e, default(NodeKey), ConvertStringToNodeKey);
      this.Category = XHelper.Read("Category", e, "");
      this.ParentKey = XHelper.Read<NodeKey>("ParentKey", e, default(NodeKey), ConvertStringToNodeKey);
      this.ChildKeys = (IList<NodeKey>)XHelper.ReadElements<NodeKey>(e.Element("ChildKeys"), "Key", new ObservableCollection<NodeKey>(), ConvertStringToNodeKey);
      this.IsTreeExpanded = XHelper.Read("IsTreeExpanded", e, true);
      this.WasTreeExpanded = XHelper.Read("WasTreeExpanded", e, false);
      this.Location = XHelper.Read("Location", e, new Point(Double.NaN, Double.NaN));
      this.Text = XHelper.Read("Text", e, "");
    }


    /// <summary>
    /// Convert a <typeparamref name="NodeKey"/> key value to a string.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>a String from which <see cref="ConvertStringToNodeKey"/> can recover the original key value</returns>
    /// <remarks>
    /// Currently this handles NodeKey types that are String, Int32, Double,
    /// DateTime, TimeSpan, or Guid.
    /// Override this method to handle additional types.
    /// </remarks>
    protected virtual String ConvertNodeKeyToString(NodeKey key) {
      if (typeof(NodeKey).IsAssignableFrom(typeof(String))) {
        return XHelper.ToString((String)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(int))) {
        return XHelper.ToString((int)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(double))) {
        return XHelper.ToString((double)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        return XHelper.ToString((Guid)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(DateTime))) {
        return XHelper.ToString((DateTime)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(TimeSpan))) {
        return XHelper.ToString((TimeSpan)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Decimal))) {
        return XHelper.ToString((Decimal)(Object)key);
      } else {
        ModelHelper.Error("Cannot convert NodeKey type to String: " + typeof(NodeKey).ToString() + " -- override GraphLinksModelNodeData.ConvertNodeKeyToString");
        return key.ToString();
      }
    }

    /// <summary>
    /// Convert a string to a <typeparamref name="NodeKey"/> key value.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>a <typeparamref name="NodeKey"/></returns>
    /// <remarks>
    /// Currently this handles NodeKey types that are String, Int32, Double,
    /// DateTime, TimeSpan, or Guid.
    /// Override this method to handle additional types.
    /// </remarks>
    protected virtual NodeKey ConvertStringToNodeKey(String s) {
      if (typeof(NodeKey).IsAssignableFrom(typeof(String))) {
        return (NodeKey)(Object)XHelper.ToString(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(int))) {
        return (NodeKey)(Object)XHelper.ToInt32(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(double))) {
        return (NodeKey)(Object)XHelper.ToDouble(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        return (NodeKey)(Object)XHelper.ToGuid(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(DateTime))) {
        return (NodeKey)(Object)XHelper.ToDateTime(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(TimeSpan))) {
        return (NodeKey)(Object)XHelper.ToTimeSpan(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Decimal))) {
        return (NodeKey)(Object)XHelper.ToDecimal(s);
      } else {
        ModelHelper.Error("Cannot convert String to NodeKey type: " + typeof(NodeKey).ToString() + " -- override GraphLinksModelNodeData.ConvertStringToNodeKey");
        return default(NodeKey);
      }
    }


    /// <summary>
    /// Gets or sets the key property for this node data.
    /// </summary>
    /// <value>
    /// The type is the parameterized type <typeparamref name="NodeKey"/>,
    /// which must be compatible with and should the same as the NodeKey type parameter
    /// used for the model, <see cref="TreeModel{NodeType, NodeKey}"/>.
    /// </value>
    public NodeKey Key {
      get { return _Key; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_Key, value)) { NodeKey old = _Key; _Key = value; RaisePropertyChanged("Key", old, value); } }
    }
    private NodeKey _Key;

    /// <summary>
    /// Gets or sets a <c>String</c> that names the category to which the node data belongs.
    /// </summary>
    /// <value>
    /// <para>
    /// The default value is an empty string.
    /// </para>
    /// </value>
    public String Category {
      get { return _Category; }
      set { if (_Category != value) { String old = _Category; _Category = value; RaisePropertyChanged("Category", old, value); } }
    }
    private String _Category = "";

    /// <summary>
    /// Gets or sets the key property referring to the "parent" node, if any.
    /// </summary>
    public NodeKey ParentKey {
      get { return _ParentKey; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_ParentKey, value)) { NodeKey old = _ParentKey; _ParentKey = value; RaisePropertyChanged("ParentKey", old, value); } }
    }
    private NodeKey _ParentKey;

    /// <summary>
    /// Gets or sets the list of keys identifying the "children" nodes.
    /// </summary>
    /// <value>
    /// By default this is an empty <see cref="ObservableCollection{NodeKey}"/>.
    /// Usually you will not need to set this, but if you do, you may want to make sure the
    /// new value also implements the <see cref="INotifyCollectionChanged"/> interface.
    /// </value>
    public IList<NodeKey> ChildKeys {
      get {
        if (_ChildKeys == null) _ChildKeys = new ObservableCollection<NodeKey>();
        return _ChildKeys;
      }
      set {
        if (_ChildKeys != value) {
          IList<NodeKey> old = _ChildKeys;
          _ChildKeys = value;
          RaisePropertyChanged("ChildKeys", old, value);
        }
      }
    }
    private IList<NodeKey> _ChildKeys;

    /// <summary>
    /// Gets or sets whether this node is in the "expanded" state.
    /// </summary>
    /// <value>
    /// By default this is true.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public bool IsTreeExpanded {
      get { return _IsTreeExpanded; }
      set { if (_IsTreeExpanded != value) { bool old = _IsTreeExpanded; _IsTreeExpanded = value; RaisePropertyChanged("IsTreeExpanded", old, value); } }
    }
    private bool _IsTreeExpanded = true;

    /// <summary>
    /// Gets or sets whether this node had been "expanded" when its parent node was "collapsed".
    /// </summary>
    /// <value>
    /// By default this is true.  This is meaningful only when the subtree is not expanded.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public bool WasTreeExpanded {
      get { return _WasTreeExpanded; }
      set { if (_WasTreeExpanded != value) { bool old = _WasTreeExpanded; _WasTreeExpanded = value; RaisePropertyChanged("WasTreeExpanded", old, value); } }
    }
    private bool _WasTreeExpanded = false;

    /// <summary>
    /// Gets or sets a <c>Point</c> that is the location of the node in model coordinates.
    /// </summary>
    /// <value>
    /// The default value is (NaN, NaN).
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public Point Location {
      get { return _Location; }
      set {
        if (_Location != value) {
          // NaN != NaN, but don't treat them as different!
          if (Double.IsNaN(_Location.X) && Double.IsNaN(value.X) && Double.IsNaN(_Location.Y) && Double.IsNaN(value.Y)) return;
          Point old = _Location;
          _Location = value;
          RaisePropertyChanged("Location", old, value);
        }
      }
    }
    private Point _Location = new Point(Double.NaN, Double.NaN);

    /// <summary>
    /// Gets or sets a <c>String</c> that is associated with the node.
    /// </summary>
    /// <value>
    /// The default value is an empty string.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public String Text {
      get { return _Text; }
      set { if (_Text != value) { String old = _Text; _Text = value; RaisePropertyChanged("Text", old, value); } }
    }
    private String _Text = "";
  }
}
