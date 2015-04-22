
/*
 *  Copyright © Northwoods Software Corporation, 1998-2011. All Rights Reserved.
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

namespace Northwoods.GoXam.Model {  // don't clutter the regular namespace with these huge parameterized types

  /// <summary>
  /// The generic implementation of a diagram model consisting of only nodes, 
  /// with implicit links between a single parent and zero or more children.
  /// </summary>
  /// <typeparam name="NodeType">the Type of node data</typeparam>
  /// <typeparam name="NodeKey">the Type of a value uniquely identifying a node data</typeparam>
  /// <seealso cref="IDiagramModel"/>
  public class TreeModel<NodeType, NodeKey> : DiagramModel, ITreeModel {

    // model state
    private System.Collections.IEnumerable _NodesSource;

    private PropPathInfo<NodeKey> _NodeKeyPathPPI;
    private bool _NodeKeyIsNodeData;
    private bool _NodeKeyReferenceAutoInserts;
    private PropPathInfo<String> _NodeCategoryPathPPI;
    private PropPathInfo<NodeKey> _FromNodesPathPPI;
    private PropPathInfo<System.Collections.IEnumerable> _ToNodesPathPPI;

    private Dictionary<NodeType, NodeInfo> _NodeInfos; // nodedata --> NodeInfo
    private Dictionary<NodeKey, NodeInfo> _IndexedNodes;  // key --> NodeInfo
    private List<NodeType> _BindingListNodes;  // list of NodeType, only used if NodesSource is IBindingList
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedFromInfos;  // if child referred to non-existing node
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedToInfos; // unresolved to key --> from infos
    private Dictionary<System.Collections.IEnumerable, NodeInfo> _ObservedToKeyCollections;
    private NodeInfo _LastRemovedNodeInfo;

    private ModelDelegates _Delegates;

    
    private static readonly IEnumerable<NodeType> NoNodes = new NodeType[0] { };


    // this sets all fields that require new values
    // this does not set any fields whose values want to be retained by CreateInitializedCopy
    private void Init() {
      this.Initializing = true;
      Reinitialize();

      var nodescoll = new ObservableCollection<NodeType>();
      nodescoll.CollectionChanged += NodesSource_CollectionChanged;
      this.NodesSourceNotifies = true;
      _NodesSource = nodescoll;

      _NodeKeyPathPPI = new PropPathInfo<NodeKey>("");
      _NodeCategoryPathPPI = new PropPathInfo<String>("");
      _FromNodesPathPPI = new PropPathInfo<NodeKey>("");
      _ToNodesPathPPI = new PropPathInfo<System.Collections.IEnumerable>("");

      _NodeInfos = new Dictionary<NodeType, NodeInfo>();
      _IndexedNodes = new Dictionary<NodeKey, NodeInfo>();
      _BindingListNodes = new List<NodeType>();
      _DelayedFromInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _DelayedToInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _ObservedToKeyCollections = new Dictionary<System.Collections.IEnumerable, NodeInfo>();

      _Delegates = new ModelDelegates();
    }


    /// <summary>
    /// The default constructor produces an empty model.
    /// </summary>
    public TreeModel() {
      Init();
      if (typeof(TreeModelNodeData<NodeKey>).IsAssignableFrom(typeof(NodeType))) {
        this.NodeKeyPath = "Key";
        this.NodeCategoryPath = "Category";
        this.ParentNodePath = "ParentKey";
        this.ChildNodesPath = "ChildKeys";
      }
      this.Initializing = false;
    }


    /// <summary>
    /// Make a copy of this model, without sharing the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="init">
    /// This is a <see cref="DataCollection"/> that provides the initial node data.
    /// (Such data is not copied.)
    /// If this is null, the initial <see cref="NodesSource"/> value is an empty collection.
    /// </param>
    /// <returns>a model just like this one, but with different data</returns>
    public virtual TreeModel<NodeType, NodeKey> CreateInitializedCopy(DataCollection init) {
      TreeModel<NodeType, NodeKey> m = (TreeModel<NodeType, NodeKey>)MemberwiseClone();
      m.Init();
      m._Delegates = _Delegates.Clone();
      m.NodeKeyPath = this.NodeKeyPath;
      m.NodeCategoryPath = this.NodeCategoryPath;
      m.ParentNodePath = this.ParentNodePath;
      m.ChildNodesPath = this.ChildNodesPath;













      {
        ObservableCollection<NodeType> coll = (ObservableCollection<NodeType>)m.NodesSource;
        if (init != null) {
          foreach (NodeType n in init.Nodes) coll.Add(n);
        }
      }
      m.Initializing = false;
      return m;
    }
    IDiagramModel IDiagramModel.CreateInitializedCopy(IDataCollection init) { return CreateInitializedCopy((DataCollection)init); }


    // Nodes data

    /// <summary>
    /// Gets or sets the collection of node data items for the model.
    /// </summary>
    /// <value>
    /// Initially this value is an empty <c>ObservableCollection</c>.
    /// It cannot be set to a null value.
    /// For generality, this is of type <see cref="System.Collections.IEnumerable"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedNodesSource"/>.
    /// </para>
    /// </remarks>
    public System.Collections.IEnumerable NodesSource {
      get { return _NodesSource; }
      set {
        System.Collections.IEnumerable old = _NodesSource;
        if (value == null) ModelHelper.Error("Cannot set NodesSource to null");
        if (old != value && value != null) {
          INotifyCollectionChanged oldsource = _NodesSource as INotifyCollectionChanged;
          if (oldsource != null) {
            oldsource.CollectionChanged -= NodesSource_CollectionChanged;





          }
          _NodesSource = value;
          INotifyCollectionChanged newsource = _NodesSource as INotifyCollectionChanged;
          if (newsource != null) {
            newsource.CollectionChanged += NodesSource_CollectionChanged;
            this.NodesSourceNotifies = true;










          } else {
            this.NodesSourceNotifies = false;

          }
          ResetNodes(false);
          RaiseModelChanged(ModelChange.ChangedNodesSource, null, old, value);
        }
      }
    }

    private bool NodesSourceNotifies { get; set; }

    private void NodesSource_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add:
          foreach (NodeType nodedata in e.NewItems) DoNodeAdded(nodedata);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (NodeType nodedata in e.OldItems) DoNodeRemoved(nodedata);
          break;
        case NotifyCollectionChangedAction.Replace:
          foreach (NodeType nodedata in e.OldItems) DoNodeRemoved(nodedata);
          foreach (NodeType nodedata in e.NewItems) DoNodeAdded(nodedata);
          break;
        case NotifyCollectionChangedAction.Reset: {
            // don't remove and re-add an existing nodedata
            // use HashSet for faster lookup of new/desired
            var newnodes = new HashSet<NodeType>();
            foreach (NodeType nodedata in _NodesSource) newnodes.Add(nodedata);

            var oldnodes = _NodeInfos.Keys.ToArray();
            foreach (NodeType nodedata in oldnodes) {
              // only remove old nodedata if not present in new _NodesSource
              if (!newnodes.Contains(nodedata)) DoNodeRemoved(nodedata);
            }

            // add all nodedata in _NodesSource; ignore if already present in old _NodeInfos
            foreach (NodeType nodedata in _NodesSource) DoNodeAdded(nodedata);
            break;
          }
        default:
          break;
      }
    }





















































































    // Model discovery:
    // Property paths for declarative graph navigation

    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for node data.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning to use the data as the key value.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindKeyForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String NodeKeyPath {
      get { return _NodeKeyPathPPI.Path; }
      set {
        String old = _NodeKeyPathPPI.Path;
        if (old != value && value != null) {
          _NodeKeyPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeKeyPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the key for node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the (hopefully) unique key for the given node data in this model</returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeKeyPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </para>
    /// <para>
    /// If <see cref="NodeKeyIsNodeData"/> is true, this just converts
    /// the <paramref name="nodedata"/> argument to the <typeparamref name="NodeKey"/> type and returns it.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindKeyForNode(NodeType nodedata) {
      if (this.Delegates.FindKeyForNode != null) {
        return this.Delegates.FindKeyForNode(nodedata);
      }
      if (this.NodeKeyIsNodeData)
        return (NodeKey)((Object)nodedata);
      else
        return _NodeKeyPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets whether all node data are also their own keys.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// For this to be set to true, the <typeparamref name="NodeType"/> type
    /// and the <typeparamref name="NodeKey"/> types must be the same.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  It is an optimization that
    /// avoids the use of an additional hash table mapping
    /// <typeparamref name="NodeKey"/> values to their <typeparamref name="NodeType"/> data,
    /// and permits other optimizations as well.
    /// This can often be set to true when "references" to nodes
    /// are actually using .NET CLR references (i.e. "pointers") instead
    /// of values such as integer, strings, GUIDs et al.
    /// </para>
    /// </remarks>
    public bool NodeKeyIsNodeData {
      get { return _NodeKeyIsNodeData; }
      set {
        if (_NodeKeyIsNodeData != value) {
          if (value) {
            if (typeof(NodeType) == typeof(NodeKey)) {
              _NodeKeyIsNodeData = true;
              ResetNodes(true);
            } else {
              ModelHelper.Error(this, "Cannot set Model.NodeKeyIsNodeData to true when the NodeType and the NodeKey are not the same Type.");
            }
          } else {
            _NodeKeyIsNodeData = false;
            ResetNodes(true);
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets whether a <typeparamref name="NodeKey"/> reference,
    /// when <see cref="NodeKeyIsNodeData"/> is true,
    /// automatically inserts the node into <see cref="NodesSource"/>.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// This property only has an effect when <see cref="NodeKeyIsNodeData"/> is true.
    /// <see cref="DoNodeAdded"/> calls <see cref="InsertNode"/> if the referred-to
    /// node is not already known to be in the <see cref="NodesSource"/> collection.
    /// </remarks>
    public bool NodeKeyReferenceAutoInserts {
      get { return _NodeKeyReferenceAutoInserts; }
      set {
        bool old = _NodeKeyReferenceAutoInserts;
        if (old != value) {
          _NodeKeyReferenceAutoInserts = value;
          //?? RaiseModelChanged(ModelChange.ChangedNodeKeyReferenceAutoInserts, null, old, value);
        }
      }
    }

    /// <summary>
    /// This method is called when a duplicate key has been found.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>
    /// true to accept the data as a node after having made the key unique in the model;
    /// false to ignore this data.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// When a node data is added to the <see cref="NodesSource"/> collection,
    /// <see cref="DoNodeAdded"/> checks for a duplicate key value.
    /// If it is a duplicate key, this method is called to either modify the node
    /// data to have a unique key, or to return false to avoid adding the data
    /// to this model.  (However, the duplicate key node data will still be
    /// in the <see cref="NodesSource"/> collection.)
    /// </para>
    /// <para>
    /// This method is frequently overridden to implement the desired
    /// policy for your particular application model.
    /// </para>
    /// <para>
    /// When the user copies selected nodes into a diagram,
    /// it is likely that this method will be called.
    /// </para>
    /// </remarks>
    protected virtual bool MakeNodeKeyUnique(NodeType nodedata) {
      if (this.Delegates.MakeNodeKeyUnique != null) {
        return this.Delegates.MakeNodeKeyUnique(this, nodedata);
      }
      NodeKey key = FindKeyForNode(nodedata);
      String nodekeypath = this.NodeKeyPath;
      if (nodekeypath != null && nodekeypath.Length > 0) {
        if (ModelHelper.MakeNodeKeyUnique<NodeKey, NodeInfo>(typeof(NodeKey), key, nodekeypath, nodedata, _IndexedNodes, " ", 2)) {
          return true;
        }
      }
      ModelHelper.Error(this, "Found duplicate key '" + (key != null ? key.ToString() : "null") + "' for node; override MakeNodeKeyUnique to modify data");
      return false;
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for "parent" node data of a node data object.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning not to call <see cref="FindParentNodeKeyForNode"/>.
    /// Otherwise that method is called to try to find the parent node for each node.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindParentNodeKeyForNode"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindParentNodeKeyForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String ParentNodePath {
      get { return _FromNodesPathPPI.Path; }
      set {
        String old = _FromNodesPathPPI.Path;
        if (old != value) {
          _FromNodesPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedFromNodesPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the key of the parent node data for a node data object.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a <typeparamref name="NodeKey"/>, the key of the parent node</returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="ParentNodePath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindParentNodeKeyForNode(NodeType nodedata) {
      if (this.Delegates.FindParentNodeKeyForNode != null) {
        return this.Delegates.FindParentNodeKeyForNode(nodedata);
      }
      return _FromNodesPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get a list of keys for the "children" nodes of a node data object.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning not to call <see cref="FindChildNodeKeysForNode"/>.
    /// Otherwise that method is called to try to find the list of children for each node.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindChildNodeKeysForNode"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindChildNodeKeysForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <see cref="System.Collections.IEnumerable"/>,
    /// holding only instances of <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String ChildNodesPath {
      get { return _ToNodesPathPPI.Path; }
      set {
        String old = _ToNodesPathPPI.Path;
        if (old != value) {
          _ToNodesPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedToNodesPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the list of keys of the children nodes for a node data object.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the list of child keys, an <see cref="System.Collections.IEnumerable"/> of <typeparamref name="NodeKey"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This is only called if <see cref="ChildNodesPath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="ChildNodesPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </para>
    /// </remarks>
    protected virtual System.Collections.IEnumerable FindChildNodeKeysForNode(NodeType nodedata) {
      if (this.Delegates.FindChildNodeKeysForNode != null) {
        return this.Delegates.FindChildNodeKeysForNode(nodedata);
      }
      return _ToNodesPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to find the category of a node data.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindCategoryForNode"/> to return an empty string.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindCategoryForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be a string.
    /// </para>
    /// </remarks>
    public String NodeCategoryPath {
      get { return _NodeCategoryPathPPI.Path; }
      set {
        String old = _NodeCategoryPathPPI.Path;
        if (old != value && value != null) {
          _NodeCategoryPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeCategoryPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the category of a node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>by default an empty string</returns>
    /// <remarks>
    /// <para>
    /// This is called on each node data that is added to the model, to decide whether the data represents a group node.
    /// </para>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeCategoryPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual String FindCategoryForNode(NodeType nodedata) {
      if (this.Delegates.FindCategoryForNode != null) {
        return this.Delegates.FindCategoryForNode(nodedata);
      }
      if (nodedata == null) return "";
      if (this.NodeCategoryPath.Length == 0) return "";
      return _NodeCategoryPathPPI.EvalFor(nodedata);
    }


    // Keeping track of nodes












    private

    sealed class NodeInfo {
      public NodeType Data { get; set; }
      public NodeKey Key { get; set; }
      public NodeInfo FromNodeInfo { get; set; }  // link going to this node
      public List<NodeInfo> ToNodeInfos { get; set; }  // links coming from this node
      public System.Collections.IEnumerable ToKeyCollection { get; set; }  // remember for CollectionChanged handler
      public List<NodeKey> SavedToKeys { get; set; }  // remember for NotifyCollectionChangedAction.Reset
      public String Category { get; set; }
      public override String ToString() {
        return "NI:" + ((this.Data != null) ? this.Data.ToString() : "(no Data)");
      }
    }

    private NodeInfo FindNodeInfoForNode(NodeType nodedata) {
      if (nodedata == null) return null;  // don't compare with default(NodeType), e.g. to allow finding zero
      NodeInfo ni;
      _NodeInfos.TryGetValue(nodedata, out ni);
      if (ni == null) ni = _LastRemovedNodeInfo;
      return ni;
    }

    private void ResetNodes(bool clear) {
      foreach (var kvp in _NodeInfos) {
        NodeInfo ni = kvp.Value;
        INotifyPropertyChanged npc = ni.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged -= HandleNodePropertyChanged;
        SetToKeyCollectionHandler(ni, null);
      }
      _NodeInfos.Clear();
      _IndexedNodes.Clear();
      _BindingListNodes.Clear();
      _ObservedToKeyCollections.Clear();
      ClearUnresolvedReferences();
      bool oldInit = this.Initializing;
      try {
        this.Initializing = true;








        foreach (NodeType nodedata in this.NodesSource) DoNodeAdded(nodedata);
      } finally {
        if (clear) ClearUnresolvedReferences();
        this.Initializing = oldInit;
      }
    }

    /// <summary>
    /// Forget all unresolved delayed or forward references.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The model may learn about node data in any order, so references to
    /// nodes may be unresolvable until later, perhaps never.
    /// Call this method to clear the internal table that keeps track
    /// of <typeparamref name="NodeKey"/>s that are not yet defined.
    /// </para>
    /// <para>
    /// This is called when setting a number of property path properties,
    /// because a model property path change can completely alter the references
    /// each node data might be making.
    /// </para>
    /// </remarks>
    public void ClearUnresolvedReferences() {
      _DelayedFromInfos.Clear();
      _DelayedToInfos.Clear();
    }


    /// <summary>
    /// Cause <see cref="ResolveNodeKey"/> to be called on each
    /// known delayed or forward node reference.
    /// </summary>
    public void ResolveAllReferences() {
      HashSet<NodeKey> keys = new HashSet<NodeKey>(_DelayedFromInfos.Keys);
      keys.UnionWith(_DelayedToInfos.Keys);
      foreach (NodeKey key in keys) {
        ResolveNodeKey(key);
      }
    }

    /// <summary>
    /// This is called repeatedly by <see cref="ResolveAllReferences"/>,
    /// once for each known delayed or forward node reference.
    /// </summary>
    /// <param name="nodekey"></param>
    /// <remarks>
    /// Depending on the situation, you may want to create and
    /// <see cref="InsertNode"/> in order to resolve a reference.
    /// Or you may want to ignore it, and later call
    /// <see cref="ClearUnresolvedReferences"/> to make sure no
    /// future node data addition might resolve the reference.
    /// </remarks>
    protected virtual void ResolveNodeKey(NodeKey nodekey) {
      if (this.Delegates.ResolveNodeKey != null) {
        this.Delegates.ResolveNodeKey(this, nodekey);
        return;
      }
    }


    // delayed references

    private void DelayFrom(NodeInfo toni, NodeKey fromkey) {
      if (fromkey == null) return;
      List<NodeInfo> list;
      if (_DelayedFromInfos.TryGetValue(fromkey, out list)) {
        if (!list.Contains(toni)) list.Add(toni);
      } else {
        _DelayedFromInfos.Add(fromkey, new List<NodeInfo>() { toni });
      }
    }

    private void DelayTo(NodeInfo fromni, NodeKey tokey) {
      if (tokey == null) return;
      List<NodeInfo> list;
      if (_DelayedToInfos.TryGetValue(tokey, out list)) {
        if (!list.Contains(fromni)) {
          ModelHelper.Error(this, "Cannot have " + fromni.Key.ToString() + " be the second parent of " + tokey.ToString());
        }
      } else {
        _DelayedToInfos.Add(tokey, new List<NodeInfo>() { fromni });
      }
    }

    private void RemoveDelayedNodeInfo(NodeInfo ni) {



      if (this.ParentNodePath != "") {
        NodeKey fromkey = FindParentNodeKeyForNode(ni.Data);
        List<NodeInfo> list;
        if (fromkey != null && _DelayedFromInfos.TryGetValue(fromkey, out list)) {
          list.Remove(ni);
          if (list.Count == 0) _DelayedFromInfos.Remove(fromkey);
        }
      }
      if (this.ChildNodesPath != "") {
        System.Collections.IEnumerable tokeys = FindChildNodeKeysForNode(ni.Data);
        if (tokeys != null) {
          foreach (NodeKey tokey in tokeys) {
            List<NodeInfo> list;
            if (_DelayedToInfos.TryGetValue(tokey, out list)) {
              list.Remove(ni);
              if (list.Count == 0) _DelayedToInfos.Remove(tokey);
            }
          }
        }
      }
    }

    private void ResolveDelayedNodeInfo(NodeInfo ni) {
      if (ni.Key == null) return;
      List<NodeInfo> list;
      if (_DelayedFromInfos.TryGetValue(ni.Key, out list)) {
        foreach (NodeInfo toni in list) {
          AddFromToInfos(ni, toni);
        }
        _DelayedFromInfos.Remove(ni.Key);
      }
      if (_DelayedToInfos.TryGetValue(ni.Key, out list)) {
        foreach (NodeInfo fromni in list) {
          AddFromToInfos(fromni, ni);
        }
        _DelayedToInfos.Remove(ni.Key);
      }
    }

    private void SetToKeyCollectionHandler(NodeInfo ni, System.Collections.IEnumerable newcoll) {
      System.Collections.IEnumerable oldcoll = ni.ToKeyCollection;
      if (oldcoll != newcoll) {
        if (oldcoll != null) {
          INotifyCollectionChanged ncc = oldcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedToKeyCollections.Remove(oldcoll);
            ncc.CollectionChanged -= ToKey_CollectionChanged;
          }
        }
        ni.ToKeyCollection = newcoll;
        if (newcoll != null) {
          INotifyCollectionChanged ncc = newcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedToKeyCollections.Add(newcoll, ni);
            ncc.CollectionChanged += ToKey_CollectionChanged;
          }
        }
      }
      ni.SavedToKeys = null;
      if (newcoll != null) {
        ni.SavedToKeys = new List<NodeKey>(newcoll.Cast<NodeKey>());
      }
    }

    private void ToKey_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      System.Collections.IEnumerable coll = (System.Collections.IEnumerable)sender;
      NodeInfo ni;
      if (_ObservedToKeyCollections.TryGetValue(coll, out ni)) {
        switch (e.Action) {
          case NotifyCollectionChangedAction.Add:
            foreach (NodeKey nkey in e.NewItems) DoChildNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (NodeKey nkey in e.OldItems) DoChildNodeKeyRemoved(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Replace:
            foreach (NodeKey nkey in e.OldItems) DoChildNodeKeyRemoved(ni.Data, nkey);
            foreach (NodeKey nkey in e.NewItems) DoChildNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Reset:
            DoChildNodeKeysChanged(ni.Data);
            break;
          default:
            break;
        }
      }
    }

    // bookkeeping

    private bool AddFromToInfos(NodeInfo fni, NodeInfo tni) {
      if (fni == null) return false;
      if (tni == null) return false;
      if (!CheckLinkValid(fni.Data, tni.Data, false, default(NodeType), default(NodeType))) {  // avoid circularities
        if (!IsLinkedInfos(fni, tni)) {  // ignore duplicates
          ModelHelper.Trace(this, "A node cannot be a child or descendant of itself: " + fni.Data.ToString() + " and " + tni.Data.ToString());
        }
        return false;
      }
      if (fni.ToNodeInfos == null) {
        fni.ToNodeInfos = new List<NodeInfo>() { tni };
      } else if (!fni.ToNodeInfos.Contains(tni)) {  // disallow duplicates
        fni.ToNodeInfos.Add(tni);
      }
      tni.FromNodeInfo = fni;
      return true;
    }

    private void RemoveFromToInfos(NodeInfo fni, NodeInfo tni) {  // called when a link is removed
      if (fni == null) return;
      if (tni == null) return;
      if (fni.ToNodeInfos != null) {
        fni.ToNodeInfos.Remove(tni);
      }
      if (tni.FromNodeInfo == fni) {
        tni.FromNodeInfo = null;
      }
    }

    private void RemoveNodeInfos(NodeInfo ni) {  // called when a node is removed
      if (ni == null) return;
      if (ni.FromNodeInfo != null) {
        NodeInfo i = ni.FromNodeInfo;
        if (i.ToNodeInfos != null) {
          if (i.ToNodeInfos.Remove(ni)) {
            DelayTo(i, ni.Key);
          }
        }
      }
      if (ni.ToNodeInfos != null) {
        foreach (NodeInfo i in ni.ToNodeInfos) {
          if (i.FromNodeInfo != null) {
            if (i.FromNodeInfo == ni) {
              i.FromNodeInfo = null;
              DelayFrom(i, ni.Key);
            }
          }
        }
      }
    }


    /// <summary>
    /// This is the <see cref="INotifyPropertyChanged"/> event handler for node data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">preferably a <see cref="ModelChangedEventArgs"/> that describes what changed and how</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <paramref name="e"/> argument is a <see cref="ModelChangedEventArgs"/>,
    /// this first calls <see cref="DiagramModel.OnChanged"/> in order to raise a <see cref="DiagramModel.Changed"/> event
    /// to notify this model's consumers.
    /// </para>
    /// <para>
    /// If the <see cref="PropertyChangedEventArgs.PropertyName"/> is the same as the
    /// <see cref="NodeKeyPath"/>, <see cref="ParentNodePath"/>, or <see cref="ChildNodesPath"/>,
    /// this automatically calls
    /// <see cref="DoNodeKeyChanged"/>, <see cref="DoParentNodeChanged"/>, or <see cref="DoChildNodeKeysChanged"/>,
    /// respectively.
    /// </para>
    /// </remarks>
    protected virtual void HandleNodePropertyChanged(Object sender, PropertyChangedEventArgs e) {
      if (e == null) return;
      ModelChangedEventArgs m = e as ModelChangedEventArgs;
      if (m != null) {
        m.Model = this;
        OnChanged(m);
      }

      String pname = e.PropertyName;
      if (pname == null || pname.Length == 0) return;

      NodeType nodedata = (NodeType)sender;
      if (pname == this.ParentNodePath) {
        DoParentNodeChanged(nodedata);
      } else if (pname == this.ChildNodesPath) {
        DoChildNodeKeysChanged(nodedata);
      } else if (pname == this.NodeKeyPath) {
        DoNodeKeyChanged(nodedata);
      } else if (pname == this.NodeCategoryPath) {
        DoNodeCategoryChanged(nodedata);
      }
    }


    /// <summary>
    /// This should be called when a node data object is added to the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <see cref="NodesSource"/> collection implements <see cref="INotifyCollectionChanged"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="NodesSource"/> has been augmented.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedNode"/>.
    /// </para>
    /// </remarks>
    public void DoNodeAdded(NodeType nodedata) {
      if (nodedata == null) return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni == null) {
        // it is a new object, but is its key unique?
        NodeKey key = FindKeyForNode(nodedata);
        bool uniquekey = true;
        if (key == null || (!this.NodeKeyIsNodeData && _IndexedNodes.ContainsKey(key))) {
          uniquekey = false;
          // not unique: give opportunity to make the node's key unique
          // or to reject this nodedata as a node
          if (MakeNodeKeyUnique(nodedata)) {
            key = FindKeyForNode(nodedata);
            // key now ought to be unique
          } else {
            return;
          }
        }
        // save in model
        ni = new NodeInfo();
        ni.Data = nodedata;
        _NodeInfos.Add(nodedata, ni);
        INotifyPropertyChanged npc = ni.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged += HandleNodePropertyChanged;

        ni.Key = key;
        // just in case MakeNodeKeyUnique failed to make the key unique but returned true:
        // allow duplicate keys in NodesSource, but not in _IndexedNodes
        if (key != null && !this.NodeKeyIsNodeData && (uniquekey || !_IndexedNodes.ContainsKey(key))) {
          _IndexedNodes.Add(key, ni);
        }

        // connect up with parent node
        if (this.ParentNodePath != "") {
          NodeKey fromkey = FindParentNodeKeyForNode(nodedata);
          NodeType fromdata = FindNodeByKey(fromkey);
          if (fromdata != null) {
            NodeInfo fromni = FindNodeInfoForNode(fromdata);
            if (fromni == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
              InsertNode(fromdata);
              fromni = FindNodeInfoForNode(fromdata);
            }
            if (fromni != null) {
              AddFromToInfos(fromni, ni);
            } else {
              DelayFrom(ni, fromkey);
            }
          } else {  // maybe node hasn't yet been added to model
            DelayFrom(ni, fromkey);
          }
        }

        if (this.ChildNodesPath != "") {
          System.Collections.IEnumerable tokeys = FindChildNodeKeysForNode(nodedata);
          if (tokeys != null) {
            foreach (NodeKey tokey in tokeys) {
              NodeType todata = FindNodeByKey(tokey);
              if (todata != null) {
                NodeInfo toni = FindNodeInfoForNode(todata);
                if (toni == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
                  InsertNode(todata);
                  toni = FindNodeInfoForNode(todata);
                }
                if (toni != null) {
                  AddFromToInfos(ni, toni);
                } else {
                  DelayTo(ni, tokey);
                }
              } else {
                DelayTo(ni, tokey);
              }
            }
            SetToKeyCollectionHandler(ni, tokeys);
          }
        }

        ni.Category = FindCategoryForNode(nodedata) ?? "";

        // maybe fix up unresolved parent references
        ResolveDelayedNodeInfo(ni);
        // notify
        if (!this.Initializing) {
          Object cols = null;




          RaiseModelChanged(ModelChange.AddedNode, nodedata, null, cols);
        }
      } else {
        // duplicate object according to hashcode, or already present -- ignore
      }
    }
    void IDiagramModel.DoNodeAdded(Object nodedata) { DoNodeAdded((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data object is removed from the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <see cref="NodesSource"/> collection implements <see cref="INotifyCollectionChanged"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="NodesSource"/> has been diminished.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedNode"/>.
    /// </para>
    /// </remarks>
    public void DoNodeRemoved(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {  // already known by model
        // save for use by GetConnectedNodesForNode et al.
        _LastRemovedNodeInfo = ni;

        // remove from model
        SetToKeyCollectionHandler(ni, null);
        _NodeInfos.Remove(ni.Data);
        INotifyPropertyChanged npc = ni.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged -= HandleNodePropertyChanged;

        // in case of duplicate keys, only remove from key index when
        // this nodedata's NodeInfo is the one in the index
        NodeKey key = ni.Key;
        NodeInfo kni;
        if (key != null && !this.NodeKeyIsNodeData && _IndexedNodes.TryGetValue(key, out kni) && ni == kni) {
          _IndexedNodes.Remove(key);
        }
        // forget delayed references to this nodedata
        RemoveDelayedNodeInfo(ni);
        // remove any references
        RemoveNodeInfos(ni);
        // notify for side-effects
        RaiseModelChanged(ModelChange.RemovedNode, nodedata, null, null);

        _LastRemovedNodeInfo = null;
      } else {
        // not present, or already gone -- OK
      }
    }
    void IDiagramModel.DoNodeRemoved(Object nodedata) { DoNodeRemoved((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data's key value may have changed.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="NodeKeyPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindKeyForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoNodeKeyChanged(NodeType nodedata) {
      if (this.NodeKeyIsNodeData) return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeKey oldkey = ni.Key;  // saved key
        NodeKey newkey = FindKeyForNode(nodedata);  // current key
        if (oldkey == null || !oldkey.Equals(newkey)) {  // if key has actually changed
          if (oldkey != null) {
            _IndexedNodes.Remove(oldkey);
          }
          ni.Key = newkey;
          if (newkey != null) {
            NodeInfo existing;
            if (_IndexedNodes.TryGetValue(newkey, out existing)) {
              ModelHelper.Trace(this, "DoNodeKeyChanged detected duplicate node data key value: " + newkey.ToString());
            } else {  // only save in index if nothing already present with that key
              _IndexedNodes.Add(newkey, ni);
            }
          }
          // Notify
          RaiseModelChanged(ModelChange.ChangedNodeKey, nodedata, oldkey, newkey);
        }
      }
    }
    void IDiagramModel.DoNodeKeyChanged(Object nodedata) { DoNodeKeyChanged((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data's category value may have changed.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="NodeCategoryPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindCategoryForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedNodeCategory"/>.
    /// </para>
    /// </remarks>
    public void DoNodeCategoryChanged(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        String oldcategory = ni.Category;
        String newcategory = FindCategoryForNode(nodedata);
        if (oldcategory != newcategory) {
          ni.Category = newcategory;
          // Notify
          RaiseModelChanged(ModelChange.ChangedNodeCategory, nodedata, oldcategory, newcategory);
        }
      }
    }


    /// <summary>
    /// This should be called when a node data's tree parent may have changed.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="ParentNodePath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindParentNodeKeyForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedParentNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoParentNodeChanged(NodeType nodedata) {
      if (this.ParentNodePath == "") return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeInfo oldpi = ni.FromNodeInfo;
        NodeKey oldkey = (oldpi != null ? oldpi.Key : default(NodeKey));
        NodeKey newkey = FindParentNodeKeyForNode(nodedata);
        NodeType newparent = FindNodeByKey(newkey);
        NodeInfo newpi = FindNodeInfoForNode(newparent);
        if (oldpi != newpi) {
          RemoveFromToInfos(oldpi, ni);
          if (!AddFromToInfos(newpi, ni)) newpi = null;  // avoid circularities
          if (oldpi != newpi) {
            RaiseModelChanged(ModelChange.ChangedParentNodeKey, nodedata, oldkey, newkey);
          }
        }
      }
    }
    void ITreeModel.DoParentNodeChanged(Object nodedata) { DoParentNodeChanged((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a child node data has been added to the collection of child nodes.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="childkey">the key for the added child node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of child keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindChildNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedChildNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoChildNodeKeyAdded(NodeType nodedata, NodeKey childkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(childkey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        AddFromToInfos(ni, toni);
        if (ni.SavedToKeys != null && !ni.SavedToKeys.Contains(childkey)) ni.SavedToKeys.Add(childkey);
        // Notify
        RaiseModelChanged(ModelChange.AddedChildNodeKey, nodedata, default(NodeKey), childkey);
      }
    }
    void ITreeModel.DoChildNodeKeyAdded(Object nodedata, Object childkey) { DoChildNodeKeyAdded((NodeType)nodedata, (NodeKey)childkey); }

    /// <summary>
    /// This should be called when a child node data has been removed from the collection of child nodes.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="childkey">the key for the removed child node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of child keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindChildNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedChildNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoChildNodeKeyRemoved(NodeType nodedata, NodeKey childkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(childkey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        RemoveFromToInfos(ni, toni);
        if (ni.SavedToKeys != null) ni.SavedToKeys.Remove(childkey);
        // Notify
        RaiseModelChanged(ModelChange.RemovedChildNodeKey, nodedata, childkey, default(NodeKey));
      }
    }
    void ITreeModel.DoChildNodeKeyRemoved(Object nodedata, Object childkey) { DoChildNodeKeyRemoved((NodeType)nodedata, (NodeKey)childkey); }

    /// <summary>
    /// This should be called when a node data's list of children nodes may have changed.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="ChildNodesPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindChildNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedChildNodeKeys"/>.
    /// </para>
    /// </remarks>
    public void DoChildNodeKeysChanged(NodeType nodedata) {
      if (this.ChildNodesPath == "") return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        System.Collections.IEnumerable oldtokeys = ni.ToKeyCollection;
        IEnumerable<NodeKey> oldtokeys2 = ni.SavedToKeys;
        System.Collections.IEnumerable newtokeys = FindChildNodeKeysForNode(nodedata);
        IEnumerable<NodeKey> newtokeys2 = (newtokeys != null ? newtokeys.Cast<NodeKey>() : null);
        if (!EqualSequences<NodeKey>(oldtokeys2, newtokeys2)) {
          if (oldtokeys2 != null) {
            foreach (NodeKey removed in (newtokeys2 != null ? oldtokeys2.Except(newtokeys2) : oldtokeys2)) {
              NodeType todata = FindNodeByKey(removed);
              if (todata != null) {
                NodeInfo toni = FindNodeInfoForNode(todata);
                RemoveFromToInfos(ni, toni);
              }
            }
          }
          SetToKeyCollectionHandler(ni, newtokeys);
          if (newtokeys2 != null) {
            foreach (NodeKey added in (oldtokeys2 != null ? newtokeys2.Except(oldtokeys2) : newtokeys2)) {
              NodeType todata = FindNodeByKey(added);
              if (todata != null) {
                NodeInfo toni = FindNodeInfoForNode(todata);
                AddFromToInfos(ni, toni);
              }
            }
          }
          RaiseModelChanged(ModelChange.ChangedChildNodeKeys, nodedata, oldtokeys, newtokeys);
        }
      }
    }
    void ITreeModel.DoChildNodeKeysChanged(Object nodedata) { DoChildNodeKeysChanged((NodeType)nodedata); }


    private static bool EqualSequences<T>(IEnumerable<T> a, IEnumerable<T> b) {
      if (a == b) return true;
      if (a == null || b == null) return false;
      return a.SequenceEqual(b);
    }


    // Model navigation:
    // Remember that these methods do not operate on or return FrameworkElements, or even DependencyObjects, but data instances.

    /// <summary>
    /// Return the <typeparamref name="NodeType"/>.
    /// </summary>
    /// <returns>a <see cref="Type"/>, not a node data object, nor a string</returns>
    /// <remarks>
    /// This is useful for data transfer.
    /// </remarks>
    public Type GetNodeType() {
      return typeof(NodeType);
    }

    /// <summary>
    /// This predicate is true when the argument is an instance of <typeparamref name="NodeType"/>.
    /// </summary>
    /// <param name="nodedata">the arbitrary object to be checked for compatibility to be a node data</param>
    /// <returns>
    /// true if the <paramref name="nodedata"/> can be cast to the <typeparamref name="NodeType"/>;
    /// false otherwise
    /// </returns>
    public bool IsNodeType(Object nodedata) {
      return nodedata is NodeType;
    }

    /// <summary>
    /// This predicate is true if the argument is a node data in this model.
    /// </summary>
    /// <param name="nodedata">the <typeparamref name="NodeType"/> object to be checked to see if it is a known node data in this model</param>
    /// <returns>
    /// true if the <paramref name="nodedata"/> is in the <see cref="NodesSource"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation.
    /// </para>
    /// <para>
    /// This uses a hash table lookup.
    /// </para>
    /// </remarks>
    public bool IsNodeData(NodeType nodedata) {
      if (nodedata == null) return false;
      return _NodeInfos.ContainsKey(nodedata);
    }
    bool IDiagramModel.IsNodeData(Object nodedata) {
      if (nodedata is NodeType)
        return IsNodeData((NodeType)nodedata);
      else
        return false;
    }


    /// <summary>
    /// Given a key, find the node data with that key.
    /// </summary>
    /// <param name="key">
    /// a value of null for this argument will result in the default value for <typeparamref name="NodeType"/>
    /// </param>
    /// <returns>
    /// a <typeparamref name="NodeType"/>;
    /// the value will be the default for the type if no such node data is known to be in this model
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation.
    /// </para>
    /// <para>
    /// If <see cref="NodeKeyIsNodeData"/> is true,
    /// this just converts the <paramref name="key"/> argument
    /// to the <typeparamref name="NodeType"/> type and returns it.
    /// </para>
    /// </remarks>
    public NodeType FindNodeByKey(NodeKey key) {
      if (key == null) return default(NodeType);  // don't compare with default(NodeKey), e.g. to allow use of zero
      if (this.NodeKeyIsNodeData) {
        return (NodeType)((Object)key);
      } else {
        NodeInfo ni;
        if (_IndexedNodes.TryGetValue(key, out ni)) return ni.Data;
        return default(NodeType);
      }
    }
    Object IDiagramModel.FindNodeByKey(Object key) { return FindNodeByKey((NodeKey)key); }


    /// <summary>
    /// This predicate is true if there is a link from one node data to another one.
    /// </summary>
    /// <param name="fromnodedata"></param>
    /// <param name="tonodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public bool IsLinked(NodeType fromnodedata, NodeType tonodedata) {
      NodeInfo fni = FindNodeInfoForNode(fromnodedata);
      NodeInfo tni = FindNodeInfoForNode(tonodedata);
      return IsLinkedInfos(fni, tni);
    }
    bool IDiagramModel.IsLinked(Object fromdata, Object fromparam, Object todata, Object toparam) { return IsLinked((NodeType)fromdata, (NodeType)todata); }

    private bool IsLinkedInfos(NodeInfo fni, NodeInfo tni) {
      return fni != null && tni != null && tni.FromNodeInfo == fni;
    }


    /// <summary>
    /// Return a sequence of node data that are directly connected to a given node, in either direction.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetConnectedNodesForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        if (ni.FromNodeInfo != null && ni.ToNodeInfos != null) {
          return ni.ToNodeInfos.Select(i => i.Data).Concat(new NodeType[1] { ni.FromNodeInfo.Data });
        } else if (ni.FromNodeInfo != null) {
          return new NodeType[1] { ni.FromNodeInfo.Data };
        } else if (ni.ToNodeInfos != null) {
          return ni.ToNodeInfos.Select(i => i.Data);
        }
      }
      return NoNodes;
    }
    IEnumerable<Object> IDiagramModel.GetConnectedNodesForNode(Object nodedata) { return GetConnectedNodesForNode((NodeType)nodedata).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of node data that are directly connected by links going into a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetFromNodesForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.FromNodeInfo != null) return new NodeType[1] { ni.FromNodeInfo.Data };
      return NoNodes;
    }
    IEnumerable<Object> IDiagramModel.GetFromNodesForNode(Object nodedata) { return GetFromNodesForNode((NodeType)nodedata).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of node data that are directly connected by links coming out from a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetToNodesForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ToNodeInfos != null) return ni.ToNodeInfos.Select(i => i.Data);
      return NoNodes;
    }
    IEnumerable<Object> IDiagramModel.GetToNodesForNode(Object nodedata) { return GetChildrenForNode((NodeType)nodedata).Cast<Object>(); }


    /// <summary>
    /// Return a parent node data for a given node data, if there is one.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a <typeparamref name="NodeType"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetParentForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.FromNodeInfo != null) return ni.FromNodeInfo.Data;
      return default(NodeType);
    }
    Object ITreeModel.GetParentForNode(Object nodedata) { return GetParentForNode((NodeType)nodedata); }

    /// <summary>
    /// Return a sequence of node data that are immediate children of a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/> of child node data; an empty sequence if there are none</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// It is the same as <see cref="GetToNodesForNode"/>.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetChildrenForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ToNodeInfos != null) return ni.ToNodeInfos.Select(i => i.Data);
      return NoNodes;
    }
    IEnumerable<Object> ITreeModel.GetChildrenForNode(Object nodedata) { return GetChildrenForNode((NodeType)nodedata).Cast<Object>(); }


    /// <summary>
    /// This method gets the category of a node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a String, defaulting to the empty string</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// This model assumes that the value will never change.
    /// </para>
    /// </remarks>
    public String GetCategoryForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) return ni.Category;
      return "";
    }
    String IDiagramModel.GetCategoryForNode(Object nodedata) { return GetCategoryForNode((NodeType)nodedata); }


    // Additional services

    // Copying

    /// <summary>
    /// This is the first pass of copying node data, responsible for constructing
    /// a copy and copying most of its properties.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// By default this handles <c>ICloneable</c> or serializable objects.
    /// But you may want to override this method to customize which properties
    /// get copied or how they are copied or to provide a faster implementation.
    /// </para>
    /// <para>
    /// This does NOT ensure that the copied node data has a unique key.
    /// You must do that before adding it to the <see cref="NodesSource"/> collection,
    /// either by overriding the data's Clone method,
    /// by declaring the data Serializable (WPF), or
    /// by overriding this method.
    /// Or you can override the <see cref="MakeNodeKeyUnique"/> method that is called when
    /// data is added to the <see cref="NodesSource"/> collection of the model.
    /// </para>
    /// </remarks>
    protected virtual NodeType CopyNode1(NodeType nodedata, CopyDictionary env) {
      if (this.Delegates.CopyNode1 != null) {
        return this.Delegates.CopyNode1(nodedata, env);
      }
      if (nodedata == null) return default(NodeType);  // don't compare with default(NodeType), e.g. to allow copying zero
      if (typeof(ValueType).IsAssignableFrom(typeof(NodeType))) return nodedata;
      ICloneable cloneable = nodedata as ICloneable;
      if (cloneable != null) {
        return (NodeType)cloneable.Clone();
      }


















      //try {
      //  return ModelHelper.CopyByXmlSerialization<NodeType>(nodedata);
      //} catch (Exception ex) {
      //  ModelHelper.Error(this, "CopyNode1: override this method to copy nodes, or have node data implement ICloneable or be Serializable or be XmlSerializable: ", ex);
      //}
      ModelHelper.Error(this, "CopyNode1: override this method to copy nodes, or have node data implement ICloneable or be Serializable");
      return default(NodeType);
    }


    /// <summary>
    /// This is the second pass of copying node data, responsible for fixing up
    /// references to other objects.
    /// </summary>
    /// <param name="oldnodedata">the original node data</param>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    /// <param name="newnodedata">the copied node data</param>
    /// <param name="newtreeparent">for convenience, the copied "parent" node data</param>
    /// <param name="newtreechildren">for convenience, a list of newly copied "children" node data</param>
    /// <remarks>
    /// <para>
    /// You will want to override this method if the node data should have any references
    /// to copied data.
    /// Otherwise the copied node will appear to have links to the original nodes,
    /// not to the copied nodes.
    /// </para>
    /// <para>
    /// Your overridden method may want to do something like what this method does by default:
    /// <code>
    ///   if (this.ParentNodePath != "" &amp;&amp; newtreeparent != null) {
    ///     ModifyParentNodeKey(newnodedata, FindKeyForNode(newtreeparent));
    ///   }
    ///   if (this.ChildNodesPath != "" &amp;&amp; newtreechildren != null) {
    ///     // remove any old keys; not needed if the copied property value is empty
    ///     foreach (NodeKey k in FindChildNodeKeysForNode(newnodedata).OfType&lt;NodeKey&gt;().ToList()) {
    ///       DeleteChildNodeKey(newnodedata, k);
    ///     }
    ///     // add new keys (which should be different from the old keys)
    ///     foreach (NodeType newto in newtreechildren) {
    ///       NodeKey newtokey = FindKeyForNode(newto);
    ///       InsertChildNodeKey(newnodedata, newtokey);
    ///     }
    ///   }
    /// </code>
    /// </para>
    /// </remarks>
    protected virtual void CopyNode2(NodeType oldnodedata, CopyDictionary env, NodeType newnodedata, NodeType newtreeparent, IEnumerable<NodeType> newtreechildren) {
      if (this.Delegates.CopyNode2 != null) {
        this.Delegates.CopyNode2(oldnodedata, env, newnodedata, newtreeparent, newtreechildren);
        return;
      }
      if (this.ParentNodePath != "" && newtreeparent != null) {
        ModifyParentNodeKey(newnodedata, FindKeyForNode(newtreeparent));
      }
      if (this.ChildNodesPath != "" && newtreechildren != null) {
        // remove any old keys
        foreach (NodeKey k in FindChildNodeKeysForNode(newnodedata).OfType<NodeKey>().ToList()) {
          DeleteChildNodeKey(newnodedata, k);
        }
        // add new keys
        foreach (NodeType newto in newtreechildren) {
          NodeKey newtokey = FindKeyForNode(newto);
          InsertChildNodeKey(newnodedata, newtokey);
        }
      }
      //ModelHelper.Error(this, "Implement ICopyNode2 or override CopyNode2");
    }


    // Data Collection

    /// <summary>
    /// Create an empty <see cref="IDataCollection"/> for this model.
    /// </summary>
    /// <returns></returns>
    public virtual DataCollection CreateDataCollection() {
      DataCollection coll = new DataCollection();
      coll.Model = this;
      return coll;
    }
    IDataCollection IDiagramModel.CreateDataCollection() { return CreateDataCollection(); }


    /// <summary>
    /// This nested class is a serializable collection of node data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This <see cref="IDataCollection"/> is used in various circumstances where
    /// there is a collection of node data, particularly for copying.
    /// If the node data is serializable, this collection can be serialized,
    /// which is useful when copying to the clipboard or pasting from it.
    /// </para>
    /// <para>
    /// Although this nested type is not a generic class, it is parameterized
    /// by the NodeType and NodeKey type parameters of the containing generic model class.
    /// </para>
    /// <para>
    /// This collection class does not support separate link data.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class DataCollection : IDataCollection {  // nested class
      /// <summary>
      /// The default constructor produces an empty collection.
      /// </summary>
      public DataCollection() { }

      [NonSerialized]
      TreeModel<NodeType, NodeKey> _Model;

      /// <summary>
      /// Gets or sets the model that owns all of the nodes in this collection.
      /// </summary>
      public TreeModel<NodeType, NodeKey> Model {
        get { return _Model; }
        set { _Model = value; }
      }
      IDiagramModel IDataCollection.Model {
        get { return this.Model; }
        set { this.Model = (TreeModel<NodeType, NodeKey>)value; }
      }


      /// <summary>
      /// Gets or sets the collection of node data, each of type <typeparamref name="NodeType"/>.
      /// </summary>
      /// <value>
      /// Setting this property will enumerate all of the node data
      /// that the argument value collection contains.
      /// If the new value is null, this collection is cleared.
      /// </value>
      public IEnumerable<NodeType> Nodes {
        get { return _Nodes; }
        set {
          if (value != null)
            _Nodes = new HashSet<NodeType>(value);
          else
            _Nodes.Clear();
        }
      }
      private HashSet<NodeType> _Nodes = new HashSet<NodeType>();
      IEnumerable<Object> IDataCollection.Nodes {
        get { return this.Nodes.Cast<Object>(); }
        set { this.Nodes = value.Cast<NodeType>(); }
      }

      /// <summary>
      /// This predicate is true if the given node data is in the collection of <see cref="Nodes"/>.
      /// </summary>
      /// <param name="nodedata">the data type is the model's NodeType type parameter</param>
      /// <returns></returns>
      public bool ContainsNode(NodeType nodedata) {
        return nodedata != null && _Nodes.Contains(nodedata);
      }
      bool IDataCollection.ContainsNode(Object nodedata) { return ContainsNode((NodeType)nodedata); }

      /// <summary>
      /// Add a node data to this collection.
      /// </summary>
      /// <param name="nodedata">the data type is the model's NodeType type parameter</param>
      /// <remarks>
      /// This is a no-op if the node data is already in the collection.
      /// </remarks>
      public void AddNode(NodeType nodedata) {
        if (nodedata != null && !_Nodes.Contains(nodedata)) _Nodes.Add(nodedata);
      }
      void IDataCollection.AddNode(Object nodedata) { AddNode((NodeType)nodedata); }

      /// <summary>
      /// Remove a node data from this collection.
      /// </summary>
      /// <param name="nodedata">the data type is the model's NodeType type parameter</param>
      /// <remarks>
      /// This is a no-op if the node data was not in the collection.
      /// </remarks>
      public void RemoveNode(NodeType nodedata) {
        if (nodedata != null) _Nodes.Remove(nodedata);
      }
      void IDataCollection.RemoveNode(Object nodedata) { RemoveNode((NodeType)nodedata); }


      IEnumerable<Object> IDataCollection.Links {
        get { return ModelHelper.NoObjects; }
        set { ModelHelper.Error(this.Model, "this model does not support link data"); }
      }

      bool IDataCollection.ContainsLink(Object linkdata) { return false; }

      void IDataCollection.AddLink(Object linkdata) { ModelHelper.Error(this.Model, "this model does not support link data"); }

      void IDataCollection.RemoveLink(Object linkdata) { ModelHelper.Error(this.Model, "this model does not support link data"); }
    }  // end of DataCollection class


    // CopyDictionary

    /// <summary>
    /// Create an <see cref="ICopyDictionary"/> initialized for this model.
    /// </summary>
    /// <returns>Normally this will be an empty dictionary.</returns>
    public virtual CopyDictionary CreateCopyDictionary() {
      CopyDictionary env = new CopyDictionary();
      env.DestinationModel = this;
      return env;
    }
    ICopyDictionary IDiagramModel.CreateCopyDictionary() { return CreateCopyDictionary(); }

    /// <summary>
    /// This nested class is used during copying to hold a mapping of original nodes
    /// to newly copied nodes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This <see cref="ICopyDictionary"/> is used during the two-pass copying process
    /// to keep track of which newly copied node data correspond to which original node data.
    /// </para>
    /// <para>
    /// Although this nested type is not a generic class, it is parameterized
    /// by the NodeType and NodeKey type parameters of the containing generic model class.
    /// </para>
    /// <para>
    /// This collection class does not support separate link data.
    /// </para>
    /// </remarks>
    public sealed class CopyDictionary : ICopyDictionary {  // nested class
      /// <summary>
      /// The default constructor builds an empty dictionary.
      /// </summary>
      public CopyDictionary() { }

      /// <summary>
      /// Gets or sets the source model for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="TreeModel{NodeType, NodeKey}.AddCollectionCopy"/> method.
      /// </remarks>
      public TreeModel<NodeType, NodeKey> SourceModel { get; set; }
      IDiagramModel ICopyDictionary.SourceModel {
        get { return this.SourceModel; }
        set { this.SourceModel = (TreeModel<NodeType, NodeKey>)value; }
      }

      /// <summary>
      /// Gets or sets the destination model for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="TreeModel{NodeType, NodeKey}.AddCollectionCopy"/> method.
      /// </remarks>
      public TreeModel<NodeType, NodeKey> DestinationModel { get; set; }
      IDiagramModel ICopyDictionary.DestinationModel {
        get { return this.DestinationModel; }
        set { this.DestinationModel = (TreeModel<NodeType, NodeKey>)value; }
      }

      /// <summary>
      /// Gets or sets the source collection of data to be copied for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="TreeModel{NodeType, NodeKey}.AddCollectionCopy"/> method.
      /// </remarks>
      public DataCollection SourceCollection { get; set; }
      IDataCollection ICopyDictionary.SourceCollection {
        get { return this.SourceCollection; }
        set { this.SourceCollection = (DataCollection)value; }
      }


      /// <summary>
      /// Gets the collection of copied nodes as a <see cref="DataCollection"/>.
      /// </summary>
      public DataCollection Copies {
        get {
          DataCollection c = this.DestinationModel.CreateDataCollection();
          c.Nodes = _NodeCopies.Select(kv => kv.Value).Where(n => n != null);
          return c;
        }
      }
      IDataCollection ICopyDictionary.Copies {
        get { return this.Copies; }
      }

      private Dictionary<NodeType, NodeType> _NodeCopies = new Dictionary<NodeType, NodeType>();

      /// <summary>
      /// This predicate is true if the given node data is in the source collection.
      /// </summary>
      /// <param name="oldnode"></param>
      /// <returns></returns>
      public bool ContainsSourceNode(NodeType oldnode) {
        if (oldnode == null) return false;
        return _NodeCopies.ContainsKey(oldnode);
      }
      bool ICopyDictionary.ContainsSourceNode(Object srcnodedata) { return ContainsSourceNode((NodeType)srcnodedata); }

      /// <summary>
      /// Look up the copied node for a given source node.
      /// </summary>
      /// <param name="oldnode">a source node data</param>
      /// <returns>the copied node data</returns>
      public NodeType FindCopiedNode(NodeType oldnode) {
        if (oldnode == null) return default(NodeType);  // don't compare with default(NodeType), e.g. to allow looking up zero
        NodeType newnode;
        _NodeCopies.TryGetValue(oldnode, out newnode);
        return newnode;
      }
      Object ICopyDictionary.FindCopiedNode(Object srcnodedata) { return FindCopiedNode((NodeType)srcnodedata); }

      /// <summary>
      /// Declare the mapping of a source node data to a copied node data.
      /// </summary>
      /// <param name="oldnode">a node data in the source collection</param>
      /// <param name="newnode">
      /// a copied node data,
      /// or null to indicate that there is no copied node data for a given source node data
      /// </param>
      /// <remarks>
      /// This will add a node data to the copies collection, associated with the source node data.
      /// </remarks>
      public void AddCopiedNode(NodeType oldnode, NodeType newnode) {
        if (oldnode != null) _NodeCopies[oldnode] = newnode;
      }
      void ICopyDictionary.AddCopiedNode(Object srcnodedata, Object dstnodedata) { AddCopiedNode((NodeType)srcnodedata, (NodeType)dstnodedata); }

      /// <summary>
      /// Remove any association between a source node data and any copied node data.
      /// </summary>
      /// <param name="srcnodedata">a node data in the source collection</param>
      public void RemoveSourceNode(NodeType srcnodedata) {
        if (srcnodedata != null) _NodeCopies.Remove(srcnodedata);
      }
      void ICopyDictionary.RemoveSourceNode(Object srcnodedata) { RemoveSourceNode((NodeType)srcnodedata); }


      bool ICopyDictionary.ContainsSourceLink(Object srclinkdata) { return false; }

      Object ICopyDictionary.FindCopiedLink(Object srclinkdata) { return null; }

      void ICopyDictionary.AddCopiedLink(Object srclinkdata, Object dstlinkdata) { ModelHelper.Error(this.SourceModel, "this model does not support link data"); }

      void ICopyDictionary.RemoveSourceLink(Object srclinkdata) { ModelHelper.Error(this.SourceModel, "this model does not support link data"); }
    }  // end of CopyDictionary class


    /// <summary>
    /// Add a copy of a node data to this model.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the copied node data</returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that just calls <see cref="AddCollectionCopy"/>.
    /// </para>
    /// </remarks>
    public NodeType AddNodeCopy(NodeType nodedata) {
      if (nodedata == null) return default(NodeType);
      DataCollection coll = CreateDataCollection();
      coll.AddNode(nodedata);
      return AddCollectionCopy(coll, null).Copies.Nodes.FirstOrDefault();
    }
    Object IDiagramModel.AddNodeCopy(Object nodedata) { return AddNodeCopy((NodeType)nodedata); }


    /// <summary>
    /// Copy existing node data and add to this model.
    /// </summary>
    /// <param name="coll">the collection of data to be copied</param>
    /// <param name="env">
    /// the <see cref="ICopyDictionary"/> used to keep track of copied objects;
    /// if null, the method will call <see cref="CreateCopyDictionary"/>, use it, and return it
    /// </param>
    /// <returns>the updated copy dictionary, mapping original data to copied data</returns>
    /// <remarks>
    /// <para>
    /// The primary purpose of this method is to perform a two-pass copy of a part of a diagram,
    /// and add the resulting data to this model.
    /// </para>
    /// <para>
    /// Of course you can add data without copying them by calling <see cref="AddNode"/>
    /// or by just adding them directly to the <see cref="NodesSource"/>.
    /// </para>
    /// <para>
    /// This calls <see cref="AugmentCopyCollection"/> on the source model to allow it to extend the
    /// collection to include parts that it thinks should be in the collection.
    /// </para>
    /// <para>
    /// Then it calls <see cref="AugmentCopyDictionary"/> on this, the destination model, to allow it
    /// to prepopulate the <see cref="CopyDictionary"/> if it wants to guide the copying process to
    /// control the sharing of references in the copied parts.
    /// </para>
    /// <para>
    /// The first pass copies all of the nodes that are not already in the <paramref name="env"/>
    /// copy dictionary, by calling <see cref="CopyNode1"/>.  If the call returns a node data,
    /// it is added to this model by calling <see cref="AddNode"/> and remembered in the
    /// <paramref name="env"/> copy dictionary, mapped to the original node data.
    /// </para>
    /// <para>
    /// The second pass fixes up references in all of the copied nodes by calling <see cref="CopyNode2"/>.
    /// It passes as arguments both the original node data and the copied node data, as well as the
    /// newly copied tree-parent node, if any.
    /// </para>
    /// </remarks>
    public CopyDictionary AddCollectionCopy(DataCollection coll, CopyDictionary env) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot CopyCollection");
      if (coll == null) return env;
      TreeModel<NodeType, NodeKey> srcmodel = coll.Model;
      if (env == null) env = new CopyDictionary();
      env.SourceModel = srcmodel;
      env.DestinationModel = this;
      env.SourceCollection = coll;
      // ask the source model to extend the collection to include expected parts
      srcmodel.AugmentCopyCollection(coll);
      // allow this destination model to prepopulate the CopyDictionary
      AugmentCopyDictionary(env);
      // first pass: individual copies
      foreach (NodeType n in coll.Nodes) {
        // already in environment? don't need to make a new copy
        if (env.ContainsSourceNode(n)) continue;
        // otherwise make a copy of the node, and save it in the CopyDictionary
        NodeType copy = CopyNode1(n, env);
        if (copy == null) continue;  // if no copy created, never mind
        AddNode(copy);  // this should add it to the NodesSource collection
        env.AddCopiedNode(n, copy);
      }
      // second pass: fix up references
      foreach (NodeType n in coll.Nodes) {
        if (!env.ContainsSourceNode(n)) continue;
        NodeType copy = env.FindCopiedNode(n);
        if (copy == null) continue;
        NodeType oldparent = srcmodel.GetParentForNode(n);
        NodeType newparent = env.FindCopiedNode(oldparent);
        IEnumerable<NodeType> newchildren = srcmodel.GetToNodesForNode(n).Select(t => env.FindCopiedNode(t)).Where(c => c != null);
        CopyNode2(n, env, copy, newparent, newchildren);
        bool needsnotification = !(copy is INotifyPropertyChanged);



        if (needsnotification && IsNodeData(copy)) {
          DoParentNodeChanged(copy);
          DoChildNodeKeysChanged(copy);
        }
      }
      return env;
    }
    ICopyDictionary IDiagramModel.AddCollectionCopy(IDataCollection coll, ICopyDictionary env) {
      return AddCollectionCopy((DataCollection)coll, (CopyDictionary)env);
    }


    /// <summary>
    /// Override this method to add more (related) data to be copied.
    /// </summary>
    /// <param name="coll">the collection of data to be copied</param>
    protected virtual void AugmentCopyCollection(DataCollection coll) {
      if (this.Delegates.AugmentCopyCollection != null) {
        this.Delegates.AugmentCopyCollection(coll);
        return;
      }
    }

    /// <summary>
    /// Override this method to avoid copying some data, or to change how copied references are resolved.
    /// </summary>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    protected virtual void AugmentCopyDictionary(CopyDictionary env) {
      if (this.Delegates.AugmentCopyDictionary != null) {
        this.Delegates.AugmentCopyDictionary(env);
        return;
      }
    }


    // Insertion

    /// <summary>
    /// This method actually implements the addition of a node data to the
    /// <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="NodesSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void InsertNode(NodeType nodedata) {
      if (this.Delegates.InsertNode != null) {
        this.Delegates.InsertNode(this, nodedata);
        return;
      }
      if (nodedata == null) return;
      if (IsNodeData(nodedata)) return;  // already present
      System.Collections.IList list = this.NodesSource as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {














        list.Add(nodedata);
        if (!this.NodesSourceNotifies) DoNodeAdded(nodedata);
      } else {
        IList<NodeType> nlist = this.NodesSource as IList<NodeType>;
        if (nlist != null && !nlist.IsReadOnly) {
          nlist.Add(nodedata);
          if (!this.NodesSourceNotifies) DoNodeAdded(nodedata);
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertNode to support adding node data to NodesSource, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Add a node data to <see cref="NodesSource"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This just calls <see cref="InsertNode"/> to actually perform the addition.
    /// This is a no-op if the node data is already in the model.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddNode(NodeType nodedata) {
      if (nodedata == null) return;
      if (IsNodeData(nodedata)) return;  // already in model
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddNode");
      InsertNode(nodedata);
    }
    void IDiagramModel.AddNode(Object nodedata) { AddNode((NodeType)nodedata); }


    /// <summary>
    /// This method actually implements the addition of a link between two nodes.
    /// </summary>
    /// <param name="parentdata"></param>
    /// <param name="childdata"></param>
    /// <remarks>
    /// If <see cref="ParentNodePath"/> is not an empty string,
    /// this calls <see cref="SetParentNodeKey"/>.
    /// If <see cref="ChildNodesPath"/> is not an empty string,
    /// this calls <see cref="AddChildNodeKey"/>.
    /// This method can be overridden in case the creation of a link
    /// is more complex than the default implementation.
    /// </remarks>
    protected virtual void InsertLink(NodeType parentdata, NodeType childdata) {
      if (this.Delegates.InsertLink != null) {
        this.Delegates.InsertLink(this, parentdata, childdata);
        return;
      }
      if (this.ParentNodePath != "" || this.ChildNodesPath != "") {
        if (this.ParentNodePath != "") {
          SetParentNodeKey(childdata, FindKeyForNode(parentdata));
        }
        if (this.ChildNodesPath != "") {
          AddChildNodeKey(parentdata, FindKeyForNode(childdata));
        }
      } else {
        ModelHelper.Error(this, "Override InsertLink(NodeType, NodeType) to support creating a new link");
      }
    }


    /// <summary>
    /// Create a link between two nodes.
    /// </summary>
    /// <param name="parentdata"></param>
    /// <param name="childdata"></param>
    /// <remarks>
    /// This is a no-op if the two nodes are already linked, according to <see cref="IsLinked"/>.
    /// This calls <see cref="InsertLink"/> to actually create the link.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddLink(NodeType parentdata, NodeType childdata) {
      if (IsLinked(parentdata, childdata)) return;  // already linked
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddLink");
      InsertLink(parentdata, childdata);
    }
    Object IDiagramModel.AddLink(Object fromdata, Object fromparam, Object todata, Object toparam) { AddLink((NodeType)fromdata, (NodeType)todata); return null; }


    // Deletion

    /// <summary>
    /// This method actually implements the removal of a node data from the
    /// <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="NodesSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void DeleteNode(NodeType nodedata) {
      if (this.Delegates.DeleteNode != null) {
        this.Delegates.DeleteNode(this, nodedata);
        return;
      }
      if (nodedata == null) return;
      if (!IsNodeData(nodedata)) return;  // not present
      System.Collections.IList list = this.NodesSource as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        Object cols = null;




        RaiseModelChanged(ModelChange.RemovingNode, nodedata, cols, null);
        list.Remove(nodedata);
        if (!this.NodesSourceNotifies) DoNodeRemoved(nodedata);
      } else {
        IList<NodeType> nlist = this.NodesSource as IList<NodeType>;
        if (nlist != null && !nlist.IsReadOnly) {
          RaiseModelChanged(ModelChange.RemovingNode, nodedata, null, null);
          nlist.Remove(nodedata);
          if (!this.NodesSourceNotifies) DoNodeRemoved(nodedata);
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteNode to support removing node data from NodesSource, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Remove a node data from <see cref="NodesSource"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This also removes any links connected to this node.
    /// This just calls <see cref="DeleteNode"/> to actually perform the removal.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveNode(NodeType nodedata) {
      if (nodedata == null) return;
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveNode");
      RemoveConnectedLinks(nodedata);
      DeleteNode(nodedata);
    }
    void IDiagramModel.RemoveNode(Object nodedata) { RemoveNode((NodeType)nodedata); }


    private void RemoveConnectedLinks(NodeType nodedata) {
      NodeType parentdata = GetParentForNode(nodedata);
      if (parentdata != null) RemoveLink(parentdata, nodedata);
      foreach (NodeType childdata in GetChildrenForNode(nodedata).ToArray()) {  // work on copy, to allow modification of collection
        RemoveLink(nodedata, childdata);
      }
    }


    /// <summary>
    /// This method actually implements the removal of a link between two nodes.
    /// </summary>
    /// <param name="parentdata"></param>
    /// <param name="childdata"></param>
    /// <remarks>
    /// If <see cref="ParentNodePath"/> is not an empty string,
    /// this calls <see cref="SetParentNodeKey"/>.
    /// If <see cref="ChildNodesPath"/> is not an empty string,
    /// this calls <see cref="RemoveChildNodeKey"/>.
    /// This method can be overridden in case the deletion of a link
    /// is more complex than the default implementation.
    /// </remarks>
    protected virtual void DeleteLink(NodeType parentdata, NodeType childdata) {
      if (this.Delegates.DeleteLink != null) {
        this.Delegates.DeleteLink(this, parentdata, childdata);
        return;
      }
      if (this.ParentNodePath != "" || this.ChildNodesPath != "") {
        if (this.ParentNodePath != "") {
          SetParentNodeKey(childdata, default(NodeKey));
        }
        if (this.ChildNodesPath != "") {
          RemoveChildNodeKey(parentdata, FindKeyForNode(childdata));
        }
      } else {
        ModelHelper.Error(this, "Override DeleteLink to support the removal of links");
      }
    }

    /// <summary>
    /// Remove any link between two nodes.
    /// </summary>
    /// <param name="parentdata"></param>
    /// <param name="childdata"></param>
    /// <remarks>
    /// This is a no-op if there is no existing link between the nodes, according to <see cref="IsLinked"/>.
    /// This calls <see cref="DeleteLink"/> to actually delete the link.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveLink(NodeType parentdata, NodeType childdata) {
      if (!IsLinked(parentdata, childdata)) return;  // no existing link to remove
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveLink");
      DeleteLink(parentdata, childdata);
    }
    void IDiagramModel.RemoveLink(Object fromdata, Object fromparam, Object todata, Object toparam) { RemoveLink((NodeType)fromdata, (NodeType)todata); }


    // Modification

    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that its reference to a parent node data (if any) is the given <paramref name="parentkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="parentkey">the key value of the "parent" node data</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="ParentNodePath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyParentNodeKey(NodeType nodedata, NodeKey parentkey) {
      if (this.Delegates.SetParentNodeKey != null) {
        this.Delegates.SetParentNodeKey(this, nodedata, parentkey);
        return;
      }
      String path = this.ParentNodePath;
      if (path != null && path.Length > 0) {
        _FromNodesPathPPI.SetFor(nodedata, parentkey);
      } else {
        ModelHelper.Error(this, "Override ModifyParentNodeKey to support defining tree relationship of nodes");
      }
    }

    /// <summary>
    /// Change a node data so that it refers to a different parent node data, by node key.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="parentkey">the key value of the "parent" node data</param>
    /// <remarks>
    /// This calls <see cref="ModifyParentNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetParentNodeKey(NodeType nodedata, NodeKey parentkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetParentNodeKey");
      ModifyParentNodeKey(nodedata, parentkey);
    }
    void ITreeModel.SetParentNodeKey(Object nodedata, Object parentkey) { SetParentNodeKey((NodeType)nodedata, (NodeKey)parentkey); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of children node data includes a given <paramref name="childkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="childkey">the key value of the "child" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindChildNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeKey}"/>.
    /// </remarks>
    protected virtual void InsertChildNodeKey(NodeType nodedata, NodeKey childkey) {
      if (this.Delegates.InsertChildNodeKey != null) {
        this.Delegates.InsertChildNodeKey(this, nodedata, childkey);
        return;
      }
      if (childkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable tokeys = FindChildNodeKeysForNode(nodedata);
      if (tokeys == null) {
        ModelHelper.Error(this, "Override InsertChildNodeKey to support adding node key to the ChildNodeKeys of node data, which is now null");
      }
      System.Collections.IList list = tokeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (!list.Contains(childkey)) {
          list.Add(childkey);
          if (!(tokeys is INotifyCollectionChanged)) DoChildNodeKeyAdded(nodedata, childkey);
        }
      } else {
        IList<NodeKey> nlist = tokeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (!nlist.Contains(childkey)) {
            nlist.Add(childkey);
            if (!(tokeys is INotifyCollectionChanged)) DoChildNodeKeyAdded(nodedata, childkey);
          }
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertChildNodeKey to support adding node key to the ChildNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Add a "child" node data's key value to a node data's list of "children".
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkey">the key value of the "child" node data</param>
    /// <remarks>
    /// This calls <see cref="InsertChildNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddChildNodeKey(NodeType nodedata, NodeKey childkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddChildNodeKey");
      InsertChildNodeKey(nodedata, childkey);
    }
    void ITreeModel.AddChildNodeKey(Object nodedata, Object childkey) { AddChildNodeKey((NodeType)nodedata, (NodeKey)childkey); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of children node data does not include a given <paramref name="childkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkey">the key value of the "child" node data</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindChildNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeKey}"/>.
    /// </remarks>
    protected virtual void DeleteChildNodeKey(NodeType nodedata, NodeKey childkey) {
      if (this.Delegates.DeleteChildNodeKey != null) {
        this.Delegates.DeleteChildNodeKey(this, nodedata, childkey);
        return;
      }
      if (childkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable tokeys = FindChildNodeKeysForNode(nodedata);
      System.Collections.IList list = tokeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (list.Contains(childkey)) {
          list.Remove(childkey);
          if (!(tokeys is INotifyCollectionChanged)) DoChildNodeKeyRemoved(nodedata, childkey);
        }
      } else {
        IList<NodeKey> nlist = tokeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (nlist.Contains(childkey)) {
            nlist.Remove(childkey);
            if (!(tokeys is INotifyCollectionChanged)) DoChildNodeKeyRemoved(nodedata, childkey);
          }
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteChildNodeKey to support removing node key from the ChildNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Remove a child node data's key value from a node data's list of "children" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkey">the key value of the "child" node data</param>
    /// <remarks>
    /// This calls <see cref="DeleteChildNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveChildNodeKey(NodeType nodedata, NodeKey childkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveChildNodeKey");
      DeleteChildNodeKey(nodedata, childkey);
    }
    void ITreeModel.RemoveChildNodeKey(Object nodedata, Object childkey) { RemoveChildNodeKey((NodeType)nodedata, (NodeKey)childkey); }


    /// <summary>
    /// This method actually implements the replacement of a node data's
    /// collection of child node keys.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkeys">a sequence of "child" node data key values</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="ChildNodesPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyChildNodeKeys(NodeType nodedata, System.Collections.IEnumerable childkeys) {
      if (this.Delegates.SetChildNodeKeys != null) {
        this.Delegates.SetChildNodeKeys(this, nodedata, childkeys);
        return;
      }
      String path = this.ChildNodesPath;
      if (path != null && path.Length > 0) {
        _ToNodesPathPPI.SetFor(nodedata, childkeys);
      } else {
        ModelHelper.Error(this, "Override ModifyChildNodeKeys to support reconnecting existing node data");
      }
    }

    /// <summary>
    /// Replace a node data's list of "children" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="childkeys">a sequence of "child" node data key values</param>
    /// <remarks>
    /// This calls <see cref="ModifyChildNodeKeys"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetChildNodeKeys(NodeType nodedata, System.Collections.IEnumerable childkeys) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetChildNodeKeys");
      ModifyChildNodeKeys(nodedata, childkeys);
    }
    void ITreeModel.SetChildNodeKeys(Object nodedata, System.Collections.IEnumerable childkeys) { SetChildNodeKeys((NodeType)nodedata, childkeys); }


    // ValidCycle

    /// <summary>
    /// This predicate is true if adding a link between two nodes would result in a valid graph.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckLinkValid"/> to do the actual graph structure check.
    /// </remarks>
    public bool IsLinkValid(NodeType fromdata, NodeType todata) {
      return CheckLinkValid(fromdata, todata, false, default(NodeType), default(NodeType));
    }
    bool IDiagramModel.IsLinkValid(Object fromdata, Object fromparam, Object todata, Object toparam) {
      return CheckLinkValid((NodeType)fromdata, (NodeType)todata, false, default(NodeType), default(NodeType));
    }

    /// <summary>
    /// This predicate is true if replacing a link between two nodes would result in a valid graph.
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the link would come</param>
    /// <param name="newtodata">a node key identify the node data to which the link would go</param>
    /// <param name="oldfromdata">a node key identifying the node data from which the existing link comes</param>
    /// <param name="oldtodata">a node key identify the node data to which the existing link goes</param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckLinkValid"/> to do the actual graph structure check.
    /// </remarks>
    public bool IsRelinkValid(NodeType newfromdata, NodeType newtodata, NodeType oldfromdata, NodeType oldtodata) {
      return CheckLinkValid(newfromdata, newtodata, true, oldfromdata, oldtodata);
    }
    bool ITreeModel.IsRelinkValid(Object newfromdata, Object newtodata, Object oldfromdata, Object oldtodata) {
      return CheckLinkValid((NodeType)newfromdata, (NodeType)newtodata, true, (NodeType)oldfromdata, (NodeType)oldtodata);
    }

    /// <summary>
    /// This predicate is true if adding a link between two nodes/ports would result in a valid tree-structured graph.
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the link would come</param>
    /// <param name="newtodata">a node key identify the node data to which the link would go</param>
    /// <param name="ignoreexistinglink">true if relinking; false if adding a new link</param>
    /// <param name="oldfromdata">a node key identifying the node data from which the existing link comes</param>
    /// <param name="oldtodata">a node key identify the node data to which the existing link goes</param>
    /// <returns>
    /// This returns false for links that would break the tree structure of the model's graph,
    /// due to either cycles or multiple "parent"s.
    /// This also returns false for duplicate links, even if they would otherwise be valid.
    /// </returns>
    protected virtual bool CheckLinkValid(NodeType newfromdata, NodeType newtodata, bool ignoreexistinglink, NodeType oldfromdata, NodeType oldtodata) {
      if (this.Delegates.CheckLinkValid != null) {
        return this.Delegates.CheckLinkValid(newfromdata, newtodata, ignoreexistinglink, oldfromdata, oldtodata);
      }
      if (!IsNodeData(newfromdata) || !IsNodeData(newtodata)) return false;
      NodeInfo from = FindNodeInfoForNode(newfromdata);
      NodeInfo to = FindNodeInfoForNode(newtodata);
      if (!ignoreexistinglink && to.FromNodeInfo != null) return false;
      return to != null && !MakesDirectedCycleFast(from, to);
    }


    private static bool MakesDirectedCycleFast(NodeInfo a, NodeInfo b) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (MakesDirectedCycleFast(a.FromNodeInfo, b)) return true;
      return false;
    }


    // Undo/redo

    /// <summary>
    /// This is called during undo or redo to effect state changes to this model.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    protected override void ChangeModelValue(ModelChangedEventArgs e, bool undo) {
      if (this.Delegates.ChangeModelValue != null) {
        this.Delegates.ChangeModelValue(this, e, undo);
      } else {
        base.ChangeModelValue(e, undo);
      }
    }

    /// <summary>
    /// This is called during undo or redo to effect state changes to model data.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    protected override void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      if (e.Data is NodeType && this.Delegates.ChangeDataValue != null) {
        this.Delegates.ChangeDataValue((NodeType)e.Data, e, undo);
      } else {
        base.ChangeDataValue(e, undo);
      }
    }

    internal override void ChangeModelState(ModelChangedEventArgs e, bool undo) {
      switch (e.Change) {
        // contents
        case ModelChange.AddedNode:
          if (undo) {
            DeleteNode((NodeType)e.Data);
          } else {









            InsertNode((NodeType)e.Data);
          }
          break;
        case ModelChange.RemovingNode:











          break;
        case ModelChange.RemovedNode:
          if (undo) {




            InsertNode((NodeType)e.Data);
          } else {
            DeleteNode((NodeType)e.Data);
          }
          break;

        case ModelChange.ChangedNodeKey: break;

        case ModelChange.ChangedParentNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyParentNodeKey(nodedata, (NodeKey)e.GetValue(undo));
            break;
          }

        case ModelChange.ChangedChildNodeKeys: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyChildNodeKeys(nodedata, (System.Collections.IEnumerable)e.GetValue(undo));
            break;
          }
        case ModelChange.AddedChildNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              DeleteChildNodeKey(nodedata, (NodeKey)e.NewValue);
            else
              InsertChildNodeKey(nodedata, (NodeKey)e.NewValue);
            break;
          }
        case ModelChange.RemovedChildNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              InsertChildNodeKey(nodedata, (NodeKey)e.OldValue);
            else
              DeleteChildNodeKey(nodedata, (NodeKey)e.OldValue);
            break;
          }

        case ModelChange.ChangedNodesSource:
          this.NodesSource = (System.Collections.IEnumerable)e.GetValue(undo);
          break;

        case ModelChange.ChangedNodeCategory: break;  // handled by HandleNodePropertyChanged

        case ModelChange.ChangedNodeKeyPath:
          this.NodeKeyPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedNodeCategoryPath:
          this.NodeCategoryPath = (String)e.GetValue(undo);
          break;

        default:
          base.ChangeModelState(e, undo);
          break;
      }
    }








    // Save and Load to/from XElement

    /// <summary>
    /// Generate a Linq for XML <c>XElement</c> holding all of the node data.
    /// </summary>
    /// <typeparam name="NodeDataType">this must be a <see cref="TreeModelNodeData{NodeKey}"/></typeparam>
    /// <param name="rootname">the name of the returned <c>XElement</c></param>
    /// <param name="nodename">the name of each <c>XElement</c> holding node data</param>
    /// <returns>an <c>XElement</c></returns>
    public XElement Save<NodeDataType>(XName rootname, XName nodename)
        where NodeDataType : TreeModelNodeData<NodeKey> {
      XElement root = new XElement(rootname);
      root.Add(this.NodesSource.OfType<NodeDataType>().Select(d => d.MakeXElement(nodename)));
      return root;
    }

    /// <summary>
    /// Given a Linq for XML <c>XContainer</c> holding node data, replace this model's
    /// <see cref="NodesSource"/> collection with a collection of new node data.
    /// </summary>
    /// <typeparam name="NodeDataType">this must be a <see cref="TreeModelNodeData{NodeKey}"/> with a public zero-argument constructor</typeparam>
    /// <param name="root">the <c>XContainer</c> holding all of the data</param>
    /// <param name="nodename">the name of each <c>XElement</c> holding node data</param>
    /// <remarks>
    /// <para>
    /// All of the changes to this model are performed within a transaction.
    /// </para>
    /// <para>
    /// This does not set the <see cref="DiagramModel.IsModified"/> property to false.
    /// You may wish to do so, depending on your application requirements.
    /// You might also wish to clear the <see cref="UndoManager"/>.
    /// </para>
    /// </remarks>
    public void Load<NodeDataType>(XContainer root, XName nodename)
        where NodeDataType : TreeModelNodeData<NodeKey>, new() {
      if (root == null) return;
      StartTransaction("Load");
      var nodedata = new ObservableCollection<NodeDataType>();
      foreach (XElement xe in root.Elements(nodename)) {
        NodeDataType d = new NodeDataType();
        d.LoadFromXElement(xe);
        nodedata.Add(d);
      }
      this.NodesSource = nodedata;
      CommitTransaction("Load");
    }

    /// <summary>
    /// Given a Linq for XML <c>XContainer</c> holding node data, replace this model's
    /// <see cref="NodesSource"/> collection with a collection of new node data.
    /// </summary>
    /// <typeparam name="NodeDataType">this must be a class inheriting from <see cref="TreeModelNodeData{NodeKey}"/></typeparam>
    /// <param name="root">the <c>XContainer</c> holding all of the data</param>
    /// <param name="nodedataallocator">
    /// a function that takes an <c>XElement</c> and returns either a newly constructed object of type <typeparamref name="NodeDataType"/>
    /// or null if that <c>XElement</c> is to be ignored
    /// </param>
    /// <remarks>
    /// <para>
    /// This will iterate over all of the child elements of the <paramref name="root"/> container,
    /// calling <paramref name="nodedataallocator"/> on each one.
    /// If that function returns non-null, it calls <see cref="TreeModelNodeData{NodeKey}.LoadFromXElement"/> on the new data
    /// and then adds it to the <see cref="NodesSource"/> collection.
    /// </para>
    /// <para>
    /// All of the changes to this model are performed within a transaction.
    /// </para>
    /// <para>
    /// This does not set the <see cref="DiagramModel.IsModified"/> property to false.
    /// You may wish to do so, depending on your application requirements.
    /// You might also wish to clear the <see cref="UndoManager"/>.
    /// </para>
    /// </remarks>
    public void Load<NodeDataType>(XContainer root, Func<XElement, NodeDataType> nodedataallocator)
        where NodeDataType : TreeModelNodeData<NodeKey> {
      if (root == null) return;
      StartTransaction("Load");
      var nodedata = new ObservableCollection<NodeDataType>();
      foreach (XElement xe in root.Elements()) {
        NodeDataType d = nodedataallocator(xe);
        if (d == null) continue;
        d.LoadFromXElement(xe);
        nodedata.Add(d);
      }
      this.NodesSource = nodedata;
      CommitTransaction("Load");
    }


    internal /*?? public */ ModelDelegates Delegates {
      get { return _Delegates; }
    }

    internal /*?? public */ sealed class ModelDelegates {  // nested class
      internal ModelDelegates() { }

      internal ModelDelegates Clone() {
        return (ModelDelegates)MemberwiseClone();
      }

      public Func<NodeType, NodeKey> FindKeyForNode { get; set; }

      public Predicate<TreeModel<NodeType, NodeKey>,  NodeType> MakeNodeKeyUnique { get; set; }

      public Func<NodeType, NodeKey> FindParentNodeKeyForNode { get; set; }

      public Func<NodeType, System.Collections.IEnumerable> FindChildNodeKeysForNode { get; set; }

      public Func<NodeType, String> FindCategoryForNode { get; set; }

      public Action<TreeModel<NodeType, NodeKey>, NodeKey> ResolveNodeKey { get; set; }

      public Func<NodeType, CopyDictionary, NodeType> CopyNode1 { get; set; }

      public Action<NodeType, CopyDictionary, NodeType, NodeType, IEnumerable<NodeType>> CopyNode2 { get; set; }

      public Action<DataCollection> AugmentCopyCollection { get; set; }

      public Action<CopyDictionary> AugmentCopyDictionary { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType> InsertNode { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType, NodeType> InsertLink { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType> DeleteNode { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType, NodeType> DeleteLink { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType, NodeKey> SetParentNodeKey { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType, NodeKey> InsertChildNodeKey { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType, NodeKey> DeleteChildNodeKey { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  NodeType, System.Collections.IEnumerable> SetChildNodeKeys { get; set; }

      public Predicate<NodeType, NodeType, bool, NodeType, NodeType> CheckLinkValid { get; set; }

      public Action<TreeModel<NodeType, NodeKey>,  ModelChangedEventArgs, bool> ChangeModelValue { get; set; }

      public Action<NodeType, ModelChangedEventArgs, bool> ChangeDataValue { get; set; }
    }
  }


  /// <summary>
  /// This is a universal model, handling all kinds of datatypes representing nodes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// There are no container nodes, nor are there link labels.
  /// Since it uses Object as the type for node data, this model class supports multiple instances
  /// of different (unrelated) types.
  /// </para>
  /// <para>
  /// For reasons of both compile-time type checking and run-time efficiency,
  /// we recommend defining your own model class derived from <see cref="TreeModel{NodeType, NodeKey}"/>.
  /// </para>
  /// <para>
  /// This defines nested classes: DataCollection and CopyDictionary.
  /// </para>
  /// </remarks>
  public sealed class UniversalTreeModel : TreeModel<Object, Object> {
    /// <summary>
    /// Create a modifiable <see cref="TreeModel{NodeType, NodeKey}"/>
    /// with an empty <c>ObservableCollection</c> for the
    /// <see cref="TreeModel{NodeType, NodeKey}.NodesSource"/>.
    /// </summary>
    public UniversalTreeModel() {
      this.Initializing = true;
      this.Modifiable = true;
      this.Initializing = false;
    }
  }
}
