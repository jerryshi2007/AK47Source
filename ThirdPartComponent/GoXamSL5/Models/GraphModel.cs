
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
  /// The generic implementation of a diagram model consisting of nodes and subgraphs,
  /// with at most one link between any two nodes in one direction.
  /// </summary>
  /// <typeparam name="NodeType">the Type of node data</typeparam>
  /// <typeparam name="NodeKey">the Type of a value uniquely identifying a node data</typeparam>
  /// <seealso cref="IDiagramModel"/>
  public class GraphModel<NodeType, NodeKey> : DiagramModel, IConnectedModel, ISubGraphModel {

    // model state
    private System.Collections.IEnumerable _NodesSource;

    private PropPathInfo<NodeKey> _NodeKeyPathPPI;
    private bool _NodeKeyIsNodeData;
    private bool _NodeKeyReferenceAutoInserts;
    private PropPathInfo<String> _NodeCategoryPathPPI;
    private PropPathInfo<bool> _NodeIsGroupPathPPI;
    private PropPathInfo<NodeKey> _GroupNodePathPPI;
    private PropPathInfo<System.Collections.IEnumerable> _MemberNodesPathPPI;
    private PropPathInfo<System.Collections.IEnumerable> _FromNodesPathPPI;
    private PropPathInfo<System.Collections.IEnumerable> _ToNodesPathPPI;

    private Dictionary<NodeType, NodeInfo> _NodeInfos; // nodedata --> NodeInfo
    private Dictionary<NodeKey, NodeInfo> _IndexedNodes;  // key --> NodeInfo
    private List<NodeType> _BindingListNodes;  // list of NodeType, only used if NodesSource is IBindingList
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedGroupInfos;
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedMemberInfos;
    private Dictionary<System.Collections.IEnumerable, NodeInfo> _ObservedMemberKeyCollections;
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedFromInfos; // unresolved from key --> to infos
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedToInfos; // unresolved to key --> from infos
    private Dictionary<System.Collections.IEnumerable, NodeInfo> _ObservedFromKeyCollections;
    private Dictionary<System.Collections.IEnumerable, NodeInfo> _ObservedToKeyCollections;
    private NodeInfo _LastRemovedNodeInfo;

    private bool _CopyingGroupCopiesMembers = true;
    private bool _RemovingGroupRemovesMembers = true;

    private ValidCycle _ValidCycle = ValidCycle.All;

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
      _NodeIsGroupPathPPI = new PropPathInfo<bool>("");
      _GroupNodePathPPI = new PropPathInfo<NodeKey>("");
      _MemberNodesPathPPI = new PropPathInfo<System.Collections.IEnumerable>("");
      _FromNodesPathPPI = new PropPathInfo<System.Collections.IEnumerable>("");
      _ToNodesPathPPI = new PropPathInfo<System.Collections.IEnumerable>("");

      _NodeInfos = new Dictionary<NodeType, NodeInfo>();
      _IndexedNodes = new Dictionary<NodeKey, NodeInfo>();
      _BindingListNodes = new List<NodeType>();
      _DelayedGroupInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _DelayedMemberInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _ObservedMemberKeyCollections = new Dictionary<System.Collections.IEnumerable, NodeInfo>();
      _DelayedFromInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _DelayedToInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _ObservedFromKeyCollections = new Dictionary<System.Collections.IEnumerable, NodeInfo>();
      _ObservedToKeyCollections = new Dictionary<System.Collections.IEnumerable, NodeInfo>();

      _Delegates = new ModelDelegates();
    }


    /// <summary>
    /// The default constructor produces an empty model.
    /// </summary>
    public GraphModel() {
      Init();
      if (typeof(GraphModelNodeData<NodeKey>).IsAssignableFrom(typeof(NodeType))) {
        this.NodeKeyPath = "Key";
        this.NodeCategoryPath = "Category";
        this.FromNodesPath = "FromKeys";
        this.ToNodesPath = "ToKeys";
        this.NodeIsGroupPath = "IsSubGraph";
        this.GroupNodePath = "SubGraphKey";
        this.MemberNodesPath = "MemberKeys";
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
    public virtual GraphModel<NodeType, NodeKey> CreateInitializedCopy(DataCollection init) {
      GraphModel<NodeType, NodeKey> m = (GraphModel<NodeType, NodeKey>)MemberwiseClone();
      m.Init();
      m._Delegates = _Delegates.Clone();
      m.NodeKeyPath = this.NodeKeyPath;
      m.NodeCategoryPath = this.NodeCategoryPath;
      m.NodeIsGroupPath = this.NodeIsGroupPath;
      m.GroupNodePath = this.GroupNodePath;
      m.MemberNodesPath = this.MemberNodesPath;
      m.FromNodesPath = this.FromNodesPath;
      m.ToNodesPath = this.ToNodesPath;













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
    /// Gets or sets a property path that that specifies how to get the collection of keys
    /// of node data from which links come.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindFromNodeKeysForNode"/>
    /// not to be called to get a list of related nodes.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindFromNodeKeysForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object
    /// must be of type <see cref="System.Collections.IEnumerable"/>,
    /// holding only instances of <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String FromNodesPath {
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
    /// Find the collection of "from" node key values.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a collection of <typeparamref name="NodeKey"/> values</returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This is only called if <see cref="FromNodesPath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="FromNodesPath"/>
    /// property path is not flexible enough or fast enough to get the collection of "from" keys for a node.
    /// </para>
    /// </remarks>
    protected virtual System.Collections.IEnumerable FindFromNodeKeysForNode(NodeType nodedata) {
      if (this.Delegates.FindFromNodeKeysForNode != null) {
        return this.Delegates.FindFromNodeKeysForNode(nodedata);
      }
      return _FromNodesPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get the collection of keys
    /// of node data to which links go.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindToNodeKeysForNode"/>
    /// not to be called to get a list of related nodes.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindToNodeKeysForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object
    /// must be of type <see cref="System.Collections.IEnumerable"/>,
    /// holding only instances of <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String ToNodesPath {
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
    /// Find the collection of "to" node key values.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a collection of <typeparamref name="NodeKey"/> values</returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This is only called if <see cref="ToNodesPath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="ToNodesPath"/>
    /// property path is not flexible enough or fast enough to get the collection of "to" keys for a node.
    /// </para>
    /// </remarks>
    protected virtual System.Collections.IEnumerable FindToNodeKeysForNode(NodeType nodedata) {
      if (this.Delegates.FindToNodeKeysForNode != null) {
        return this.Delegates.FindToNodeKeysForNode(nodedata);
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


    /// <summary>
    /// Gets or sets a property path that that specifies how to find out whether
    /// a node data is also a "container" group.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindIsGroupForNode"/> to return false.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindIsGroupForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be a boolean.
    /// </para>
    /// </remarks>
    public String NodeIsGroupPath {
      get { return _NodeIsGroupPathPPI.Path; }
      set {
        String old = _NodeIsGroupPathPPI.Path;
        if (old != value && value != null) {
          _NodeIsGroupPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeIsGroupPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find whether a node data is a group or container of other nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is called on each node data that is added to the model, to decide whether the data represents a group node.
    /// </para>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeIsGroupPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual bool FindIsGroupForNode(NodeType nodedata) {
      if (this.Delegates.FindIsGroupForNode != null) {
        return this.Delegates.FindIsGroupForNode(nodedata);
      }
      if (nodedata == null) return false;
      if (this.NodeIsGroupPath.Length == 0) return false;
      return _NodeIsGroupPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for "container" or group node data of a node data object.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning not to call <see cref="FindGroupKeyForNode"/>.
    /// Otherwise that method is called to try to find the container node for each node.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindGroupKeyForNode"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindGroupKeyForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String GroupNodePath {
      get { return _GroupNodePathPPI.Path; }
      set {
        String old = _GroupNodePathPPI.Path;
        if (old != value) {
          _GroupNodePathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedGroupNodePath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find containing group node key for a given node data, if any.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>
    /// a <typeparamref name="NodeKey"/> for the containing group node,
    /// or the default value for that type if there is no container node for the node
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This is only called if <see cref="GroupNodePath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="GroupNodePath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindGroupKeyForNode(NodeType nodedata) {
      if (this.Delegates.FindGroupKeyForNode != null) {
        return this.Delegates.FindGroupKeyForNode(nodedata);
      }
      return _GroupNodePathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get a list of keys for the "member" nodes of a group node data object.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning not to call <see cref="FindMemberNodeKeysForNode"/>.
    /// Otherwise that method is called to try to find the list of children for each node.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindMemberNodeKeysForNode"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindMemberNodeKeysForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <see cref="System.Collections.IEnumerable"/>,
    /// holding only instances of <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String MemberNodesPath {
      get { return _MemberNodesPathPPI.Path; }
      set {
        String old = _MemberNodesPathPPI.Path;
        if (old != value) {
          _MemberNodesPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedMemberNodesPath, null, old, value);
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
    /// This is only called if <see cref="MemberNodesPath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="MemberNodesPath"/>
    /// property path is not flexible enough or fast enough to get the collection of child node keys.
    /// </para>
    /// </remarks>
    protected virtual System.Collections.IEnumerable FindMemberNodeKeysForNode(NodeType nodedata) {
      if (this.Delegates.FindMemberNodeKeysForNode != null) {
        return this.Delegates.FindMemberNodeKeysForNode(nodedata);
      }
      return _MemberNodesPathPPI.EvalFor(nodedata);
    }


    // Keeping track of nodes
















    private

    sealed class NodeInfo {
      public NodeType Data { get; set; }
      public NodeKey Key { get; set; }
      public List<NodeInfo> FromNodeInfos { get; set; }  // links going to this node
      public List<NodeInfo> ToNodeInfos { get; set; }  // links coming from this node
      public System.Collections.IEnumerable FromKeyCollection { get; set; }  // remember for CollectionChanged handler
      public System.Collections.IEnumerable ToKeyCollection { get; set; }  // remember for CollectionChanged handler
      public List<NodeKey> SavedFromKeys { get; set; }  // remember for NotifyCollectionChangedAction.Reset
      public List<NodeKey> SavedToKeys { get; set; }  // remember for NotifyCollectionChangedAction.Reset
      public String Category { get; set; }
      public bool IsGroup { get; set; }
      public NodeInfo GroupNodeInfo { get; set; }  // to containing group
      public List<NodeInfo> MemberNodeInfos { get; set; }  // nodes contained by this group node
      public System.Collections.IEnumerable MemberKeyCollection { get; set; }  // remember for CollectionChanged handler
      public List<NodeKey> SavedMemberKeys { get; set; }  // remember for NotifyCollectionChangedAction.Reset
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
        SetFromKeyCollectionHandler(ni, null);
        SetToKeyCollectionHandler(ni, null);
        SetMemberKeyCollectionHandler(ni, null);
      }
      _NodeInfos.Clear();
      _IndexedNodes.Clear();
      _BindingListNodes.Clear();
      _ObservedFromKeyCollections.Clear();
      _ObservedToKeyCollections.Clear();
      _ObservedMemberKeyCollections.Clear();
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
      _DelayedGroupInfos.Clear();
      _DelayedMemberInfos.Clear();
      _DelayedFromInfos.Clear();
      _DelayedToInfos.Clear();
    }


    /// <summary>
    /// Cause <see cref="ResolveNodeKey"/> to be called on each
    /// known delayed or forward node reference.
    /// </summary>
    public void ResolveAllReferences() {
      HashSet<NodeKey> keys = new HashSet<NodeKey>(_DelayedGroupInfos.Keys);
      keys.UnionWith(_DelayedMemberInfos.Keys);
      keys.UnionWith(_DelayedFromInfos.Keys);
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
          list.Add(fromni);
        }
      } else {
        _DelayedToInfos.Add(tokey, new List<NodeInfo>() { fromni });
      }
    }

    // called when a node is added, but its group node reference is unresolved
    private void DelayGroup(NodeInfo memberni, NodeKey groupkey) {
      if (groupkey == null) return;
      List<NodeInfo> members;
      if (_DelayedGroupInfos.TryGetValue(groupkey, out members)) {
        if (!members.Contains(memberni)) members.Add(memberni);
      } else {
        _DelayedGroupInfos.Add(groupkey, new List<NodeInfo>() { memberni });
      }
    }

    private void DelayMember(NodeInfo groupni, NodeKey memberkey) {
      if (memberkey == null) return;
      List<NodeInfo> list;
      if (_DelayedMemberInfos.TryGetValue(memberkey, out list)) {
        if (!list.Contains(groupni)) list.Add(groupni);
      } else {
        _DelayedMemberInfos.Add(memberkey, new List<NodeInfo>() { groupni });
      }
    }

    private void RemoveDelayedNodeInfo(NodeInfo ni) {



      if (this.FromNodesPath != "") {
        System.Collections.IEnumerable fromkeys = FindFromNodeKeysForNode(ni.Data);
        if (fromkeys != null) {
          foreach (NodeKey fromkey in fromkeys) {
            List<NodeInfo> list;
            if (_DelayedFromInfos.TryGetValue(fromkey, out list)) {
              list.Remove(ni);
              if (list.Count == 0) _DelayedFromInfos.Remove(fromkey);
            }
          }
        }
      }
      if (this.ToNodesPath != "") {
        System.Collections.IEnumerable tokeys = FindToNodeKeysForNode(ni.Data);
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
      if (this.GroupNodePath != "") {
        NodeKey sgkey = FindGroupKeyForNode(ni.Data);
        List<NodeInfo> members;
        if (sgkey != null && _DelayedGroupInfos.TryGetValue(sgkey, out members)) {
          members.Remove(ni);
          if (members.Count == 0) _DelayedGroupInfos.Remove(sgkey);
        }
      }
      if (this.MemberNodesPath != "") {
        System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(ni.Data);
        if (memberkeys != null) {
          foreach (NodeKey memberkey in memberkeys) {
            List<NodeInfo> list;
            if (_DelayedMemberInfos.TryGetValue(memberkey, out list)) {
              list.Remove(ni);
              if (list.Count == 0) _DelayedMemberInfos.Remove(memberkey);
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
      if (_DelayedGroupInfos.TryGetValue(ni.Key, out list)) {
        if (ni.IsGroup) {  // only add members if NI really is a group node
          foreach (NodeInfo mi in list) {
            if (mi.GroupNodeInfo == null) {
              AddGroupMemberInfos(ni, mi);
            }
          }
        }
        _DelayedGroupInfos.Remove(ni.Key);
      }
      if (_DelayedMemberInfos.TryGetValue(ni.Key, out list)) {
        foreach (NodeInfo groupni in list) {
          AddGroupMemberInfos(groupni, ni);
        }
        _DelayedMemberInfos.Remove(ni.Key);
      }
    }

    private void SetFromKeyCollectionHandler(NodeInfo ni, System.Collections.IEnumerable newcoll) {
      System.Collections.IEnumerable oldcoll = ni.FromKeyCollection;
      if (oldcoll != newcoll) {
        if (oldcoll != null) {
          INotifyCollectionChanged ncc = oldcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedFromKeyCollections.Remove(oldcoll);
            ncc.CollectionChanged -= FromKey_CollectionChanged;
          }
        }
        ni.FromKeyCollection = newcoll;
        if (newcoll != null) {
          INotifyCollectionChanged ncc = newcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedFromKeyCollections.Add(newcoll, ni);
            ncc.CollectionChanged += FromKey_CollectionChanged;
          }
        }
      }
      ni.SavedFromKeys = null;
      if (newcoll != null) {
        ni.SavedFromKeys = new List<NodeKey>(newcoll.Cast<NodeKey>());
      }
    }

    private void FromKey_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      System.Collections.IEnumerable coll = (System.Collections.IEnumerable)sender;
      NodeInfo ni;
      if (_ObservedFromKeyCollections.TryGetValue(coll, out ni)) {
        switch (e.Action) {
          case NotifyCollectionChangedAction.Add:
            foreach (NodeKey nkey in e.NewItems) DoFromNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (NodeKey nkey in e.OldItems) DoFromNodeKeyRemoved(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Replace:
            foreach (NodeKey nkey in e.OldItems) DoFromNodeKeyRemoved(ni.Data, nkey);
            foreach (NodeKey nkey in e.NewItems) DoFromNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Reset:
            DoFromNodeKeysChanged(ni.Data);
            break;
          default:
            break;
        }
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
            foreach (NodeKey nkey in e.NewItems) DoToNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (NodeKey nkey in e.OldItems) DoToNodeKeyRemoved(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Replace:
            foreach (NodeKey nkey in e.OldItems) DoToNodeKeyRemoved(ni.Data, nkey);
            foreach (NodeKey nkey in e.NewItems) DoToNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Reset:
            DoToNodeKeysChanged(ni.Data);
            break;
          default:
            break;
        }
      }
    }

    private void SetMemberKeyCollectionHandler(NodeInfo ni, System.Collections.IEnumerable newcoll) {
      System.Collections.IEnumerable oldcoll = ni.MemberKeyCollection;
      if (oldcoll != newcoll) {
        if (oldcoll != null) {
          INotifyCollectionChanged ncc = oldcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedMemberKeyCollections.Remove(oldcoll);
            ncc.CollectionChanged -= MemberKey_CollectionChanged;
          }
        }
        ni.MemberKeyCollection = newcoll;
        if (newcoll != null) {
          INotifyCollectionChanged ncc = newcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedMemberKeyCollections.Add(newcoll, ni);
            ncc.CollectionChanged += MemberKey_CollectionChanged;
          }
        }
      }
      ni.SavedMemberKeys = null;
      if (newcoll != null) {
        ni.SavedMemberKeys = new List<NodeKey>(newcoll.Cast<NodeKey>());
      }
    }

    private void MemberKey_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      System.Collections.IEnumerable coll = (System.Collections.IEnumerable)sender;
      NodeInfo ni;
      if (_ObservedMemberKeyCollections.TryGetValue(coll, out ni)) {
        switch (e.Action) {
          case NotifyCollectionChangedAction.Add:
            foreach (NodeKey nkey in e.NewItems) DoMemberNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (NodeKey nkey in e.OldItems) DoMemberNodeKeyRemoved(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Replace:
            foreach (NodeKey nkey in e.OldItems) DoMemberNodeKeyRemoved(ni.Data, nkey);
            foreach (NodeKey nkey in e.NewItems) DoMemberNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Reset:
            DoMemberNodeKeysChanged(ni.Data);
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
      if (tni.FromNodeInfos == null) {
        tni.FromNodeInfos = new List<NodeInfo>() { fni };
      } else if (!tni.FromNodeInfos.Contains(fni)) {  // disallow duplicates
        tni.FromNodeInfos.Add(fni);
      }
      return true;
    }

    private void RemoveFromToInfos(NodeInfo fni, NodeInfo tni) {  // called when a link is removed
      if (fni == null) return;
      if (tni == null) return;
      if (fni.ToNodeInfos != null) {
        fni.ToNodeInfos.Remove(tni);
      }
      if (tni.FromNodeInfos != null) {
        tni.FromNodeInfos.Remove(fni);
      }
    }

    private void RemoveNodeInfos(NodeInfo ni) {  // called when a node is removed
      if (ni == null) return;
      if (ni.FromNodeInfos != null) {
        foreach (NodeInfo i in ni.FromNodeInfos) {
          if (i.ToNodeInfos != null) {
            if (i.ToNodeInfos.Remove(ni)) {
              DelayTo(i, ni.Key);
            }
          }
        }
      }
      if (ni.ToNodeInfos != null) {
        foreach (NodeInfo i in ni.ToNodeInfos) {
          if (i.FromNodeInfos != null) {
            if (i.FromNodeInfos.Remove(ni)) {
              DelayFrom(i, ni.Key);
            }
          }
        }
      }
      // remove relationship with group, if any
      if (ni.GroupNodeInfo != null) {
        NodeInfo cni = ni.GroupNodeInfo;
        if (cni.MemberNodeInfos != null) {
          if (cni.MemberNodeInfos.Remove(ni)) {
            //??? DelayMember(cni, ni.Key);
          }
        }
      }
      // remove references to members and vice-versa
      if (ni.MemberNodeInfos != null) {
        foreach (NodeInfo mi in ni.MemberNodeInfos) {
          if (mi.GroupNodeInfo == ni) {
            mi.GroupNodeInfo = null;
            //??? DelayGroup(mi, ni.Key);
          }
        }
      }
    }

    private bool AddGroupMemberInfos(NodeInfo sgi, NodeInfo ni) {
      if (sgi == null) return false;
      if (ni == null) return false;
      if (!sgi.IsGroup) return false;
      if (!CheckMemberValid(sgi.Data, ni.Data, false)) {  // avoid circularities
        if (!IsMemberInfos(sgi, ni)) {  // ignore duplicates
          ModelHelper.Trace(this, "A group node cannot be a member of itself: " + sgi.Data.ToString() + " and " + ni.Data.ToString());
        }
        return false;
      }
      ni.GroupNodeInfo = sgi;
      if (sgi.MemberNodeInfos == null) {
        sgi.MemberNodeInfos = new List<NodeInfo>() { ni };
      } else if (!sgi.MemberNodeInfos.Contains(ni)) {
        sgi.MemberNodeInfos.Add(ni);
      }
      return true;
    }

    private void RemoveGroupMemberInfos(NodeInfo sgi, NodeInfo ni) {
      if (sgi == null) return;
      if (ni == null) return;
      if (ni.GroupNodeInfo == sgi) {
        ni.GroupNodeInfo = null;
      }
      if (sgi.MemberNodeInfos != null) {
        sgi.MemberNodeInfos.Remove(ni);
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
    /// <see cref="NodeKeyPath"/>, <see cref="ToNodesPath"/>, <see cref="FromNodesPath"/>,
    /// <see cref="GroupNodePath"/>, or <see cref="MemberNodesPath"/>,
    /// this automatically calls
    /// <see cref="DoNodeKeyChanged"/>, <see cref="DoToNodeKeysChanged"/>, <see cref="DoFromNodeKeysChanged"/>,
    /// <see cref="DoGroupNodeChanged"/>, or <see cref="DoMemberNodeKeysChanged"/>,
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
      if (pname == this.GroupNodePath) {
        DoGroupNodeChanged(nodedata);
      } else if (pname == this.MemberNodesPath) {
        DoMemberNodeKeysChanged(nodedata);
      } else if (pname == this.FromNodesPath) {
        DoFromNodeKeysChanged(nodedata);
      } else if (pname == this.ToNodesPath) {
        DoToNodeKeysChanged(nodedata);
      } else if (pname == this.NodeKeyPath) {
        DoNodeKeyChanged(nodedata);
      } else if (pname == this.NodeCategoryPath) {
        DoNodeCategoryChanged(nodedata);
      //} else if (pname == this.NodeIsGroupPath) {
      //  DoNodeIsGroupChanged(nodedata);
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

        // connect up with from nodes
        if (this.FromNodesPath != "") {
          System.Collections.IEnumerable fromkeys = FindFromNodeKeysForNode(nodedata);
          if (fromkeys != null) {
            foreach (NodeKey fromkey in fromkeys) {
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
              } else {
                DelayFrom(ni, fromkey);
              }
            }
            SetFromKeyCollectionHandler(ni, fromkeys);
          }
        }

        // connect up with to nodes
        if (this.ToNodesPath != "") {
          System.Collections.IEnumerable tokeys = FindToNodeKeysForNode(nodedata);
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

        // see if this node is a group node
        ni.Category = FindCategoryForNode(nodedata) ?? "";
        ni.IsGroup = FindIsGroupForNode(nodedata);

        // see if this node is a member of some group node
        if (this.GroupNodePath != "") {
          NodeKey groupkey = FindGroupKeyForNode(ni.Data);
          NodeType groupdata = FindNodeByKey(groupkey);
          if (groupdata != null) {
            NodeInfo groupni = FindNodeInfoForNode(groupdata);
            if (groupni == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
              InsertNode(groupdata);
              groupni = FindNodeInfoForNode(groupdata);
            }
            if (groupni != null) {
              AddGroupMemberInfos(groupni, ni);
            } else {
              DelayGroup(ni, groupkey);
            }
          } else {  // maybe group node hasn't yet been added to model
            DelayGroup(ni, groupkey);
          }
        }

        // connect up with member nodes
        if (this.MemberNodesPath != "") {
          System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(nodedata);
          if (memberkeys != null) {
            foreach (NodeKey memberkey in memberkeys) {
              NodeType memberdata = FindNodeByKey(memberkey);
              if (memberdata != null) {
                NodeInfo memberni = FindNodeInfoForNode(memberdata);
                if (memberni == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
                  InsertNode(memberdata);
                  memberni = FindNodeInfoForNode(memberdata);
                }
                if (memberni != null) {
                  AddGroupMemberInfos(ni, memberni);
                } else {
                  DelayMember(ni, memberkey);
                }
              } else {
                DelayMember(ni, memberkey);
              }
            }
            SetMemberKeyCollectionHandler(ni, memberkeys);
          }
        }

        // maybe fix up unresolved references to this node
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
        SetFromKeyCollectionHandler(ni, null);
        SetToKeyCollectionHandler(ni, null);
        SetMemberKeyCollectionHandler(ni, null);
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
    /// This should be called when a "from" node data key has been added to the collection of "from" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="fromkey">the key for the added "from" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "from" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindFromNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedFromNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoFromNodeKeyAdded(NodeType nodedata, NodeKey fromkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType fromnode = FindNodeByKey(fromkey);
        NodeInfo fromni = FindNodeInfoForNode(fromnode);
        AddFromToInfos(fromni, ni);
        if (ni.SavedFromKeys != null && !ni.SavedFromKeys.Contains(fromkey)) ni.SavedFromKeys.Add(fromkey);
        // Notify
        RaiseModelChanged(ModelChange.AddedFromNodeKey, nodedata, default(NodeKey), fromkey);
      }
    }
    void IConnectedModel.DoFromNodeKeyAdded(Object nodedata, Object fromkey) { DoFromNodeKeyAdded((NodeType)nodedata, (NodeKey)fromkey); }

    /// <summary>
    /// This should be called when a "from" node data key has been removed from the collection of "from" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="fromkey">the key for the removed "from" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "from" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindFromNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedFromNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoFromNodeKeyRemoved(NodeType nodedata, NodeKey fromkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType fromnode = FindNodeByKey(fromkey);
        NodeInfo fromni = FindNodeInfoForNode(fromnode);
        RemoveFromToInfos(fromni, ni);
        if (ni.SavedFromKeys != null) ni.SavedFromKeys.Remove(fromkey);
        // Notify
        RaiseModelChanged(ModelChange.RemovedFromNodeKey, nodedata, fromkey, default(NodeKey));
      }
    }
    void IConnectedModel.DoFromNodeKeyRemoved(Object nodedata, Object fromkey) { DoFromNodeKeyRemoved((NodeType)nodedata, (NodeKey)fromkey); }

    /// <summary>
    /// This should be called when a node data's list of "from" nodes has been replaced.
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
    /// value of <see cref="FromNodesPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindFromNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedFromNodeKeys"/>.
    /// </para>
    /// </remarks>
    public void DoFromNodeKeysChanged(NodeType nodedata) {
      if (this.FromNodesPath == "") return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        System.Collections.IEnumerable oldfromkeys = ni.FromKeyCollection;
        IEnumerable<NodeKey> oldfromkeys2 = ni.SavedFromKeys;
        System.Collections.IEnumerable newfromkeys = FindFromNodeKeysForNode(nodedata);
        IEnumerable<NodeKey> newfromkeys2 = (newfromkeys != null ? newfromkeys.Cast<NodeKey>() : null);
        if (!EqualSequences<NodeKey>(oldfromkeys2, newfromkeys2)) {
          if (oldfromkeys2 != null) {
            foreach (NodeKey removed in (newfromkeys2 != null ? oldfromkeys2.Except(newfromkeys2) : oldfromkeys2)) {
              NodeType fromdata = FindNodeByKey(removed);
              if (fromdata != null) {
                NodeInfo fromni = FindNodeInfoForNode(fromdata);
                RemoveFromToInfos(fromni, ni);
              }
            }
          }
          SetFromKeyCollectionHandler(ni, newfromkeys);
          if (newfromkeys2 != null) {
            foreach (NodeKey added in (oldfromkeys2 != null ? newfromkeys2.Except(oldfromkeys2) : newfromkeys2)) {
              NodeType fromdata = FindNodeByKey(added);
              if (fromdata != null) {
                NodeInfo fromni = FindNodeInfoForNode(fromdata);
                AddFromToInfos(fromni, ni);
              }
            }
          }
          RaiseModelChanged(ModelChange.ChangedFromNodeKeys, nodedata, oldfromkeys, newfromkeys);
        }
      }
    }
    void IConnectedModel.DoFromNodeKeysChanged(Object nodedata) { DoFromNodeKeysChanged((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a "to" node data key has been added to the collection of "to" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="tokey">the key for the added "to" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "to" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindToNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedToNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoToNodeKeyAdded(NodeType nodedata, NodeKey tokey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(tokey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        AddFromToInfos(ni, toni);
        if (ni.SavedToKeys != null && !ni.SavedToKeys.Contains(tokey)) ni.SavedToKeys.Add(tokey);
        // Notify
        RaiseModelChanged(ModelChange.AddedToNodeKey, nodedata, default(NodeKey), tokey);
      }
    }
    void IConnectedModel.DoToNodeKeyAdded(Object nodedata, Object tokey) { DoToNodeKeyAdded((NodeType)nodedata, (NodeKey)tokey); }

    /// <summary>
    /// This should be called when a "to" node data key has been removed from the collection of "to" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="tokey">the key for the removed "to" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "to" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindToNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedToNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoToNodeKeyRemoved(NodeType nodedata, NodeKey tokey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(tokey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        RemoveFromToInfos(ni, toni);
        if (ni.SavedToKeys != null) ni.SavedToKeys.Remove(tokey);
        // Notify
        RaiseModelChanged(ModelChange.RemovedToNodeKey, nodedata, tokey, default(NodeKey));
      }
    }
    void IConnectedModel.DoToNodeKeyRemoved(Object nodedata, Object tokey) { DoToNodeKeyRemoved((NodeType)nodedata, (NodeKey)tokey); }

    /// <summary>
    /// This should be called when a node data's list of "to" nodes has been replaced.
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
    /// value of <see cref="ToNodesPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindToNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedToNodeKeys"/>.
    /// </para>
    /// </remarks>
    public void DoToNodeKeysChanged(NodeType nodedata) {
      if (this.ToNodesPath == "") return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        System.Collections.IEnumerable oldtokeys = ni.ToKeyCollection;
        IEnumerable<NodeKey> oldtokeys2 = ni.SavedToKeys;
        System.Collections.IEnumerable newtokeys = FindToNodeKeysForNode(nodedata);
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
          RaiseModelChanged(ModelChange.ChangedToNodeKeys, nodedata, oldtokeys, newtokeys);
        }
      }
    }
    void IConnectedModel.DoToNodeKeysChanged(Object nodedata) { DoToNodeKeysChanged((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data's membership in a group may have changed.
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
    /// value of <see cref="GroupNodePath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindGroupKeyForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedGroupNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoGroupNodeChanged(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && this.GroupNodePath != "") {
        NodeInfo oldsgi = ni.GroupNodeInfo;
        NodeKey newkey = FindGroupKeyForNode(nodedata);
        NodeType newsg = FindNodeByKey(newkey);
        NodeInfo newsgi = FindNodeInfoForNode(newsg);
        if (oldsgi != newsgi) {
          RemoveGroupMemberInfos(oldsgi, ni);
          if (!AddGroupMemberInfos(newsgi, ni)) newsgi = null;  // avoid circularities
          if (oldsgi != newsgi) {
            RaiseModelChanged(ModelChange.ChangedGroupNodeKey, nodedata, (oldsgi != null ? oldsgi.Key : default(NodeKey)), (newsgi != null ? newsgi.Key : default(NodeKey)));
          }
        }
      }
    }
    void ISubGraphModel.DoGroupNodeChanged(Object nodedata) { DoGroupNodeChanged((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a "member" node data key has been added to the collection of "member" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="memberkey">the key for the added "member" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "member" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindMemberNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedMemberNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoMemberNodeKeyAdded(NodeType nodedata, NodeKey memberkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(memberkey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        AddGroupMemberInfos(ni, toni);
        if (ni.SavedMemberKeys != null && !ni.SavedMemberKeys.Contains(memberkey)) ni.SavedMemberKeys.Add(memberkey);
        // Notify
        RaiseModelChanged(ModelChange.AddedMemberNodeKey, nodedata, default(NodeKey), memberkey);
      }
    }
    void IGroupsModel.DoMemberNodeKeyAdded(Object nodedata, Object memberkey) { DoMemberNodeKeyAdded((NodeType)nodedata, (NodeKey)memberkey); }

    /// <summary>
    /// This should be called when a "member" node data key has been removed from the collection of "member" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="memberkey">the key for the removed "member" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "member" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindMemberNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedMemberNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoMemberNodeKeyRemoved(NodeType nodedata, NodeKey memberkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(memberkey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        RemoveGroupMemberInfos(ni, toni);
        if (ni.SavedMemberKeys != null) ni.SavedMemberKeys.Remove(memberkey);
        // Notify
        RaiseModelChanged(ModelChange.RemovedMemberNodeKey, nodedata, memberkey, default(NodeKey));
      }
    }
    void IGroupsModel.DoMemberNodeKeyRemoved(Object nodedata, Object memberkey) { DoMemberNodeKeyRemoved((NodeType)nodedata, (NodeKey)memberkey); }

    /// <summary>
    /// This should be called when a node data's list of "member" nodes has been replaced.
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
    /// value of <see cref="MemberNodesPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindMemberNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedMemberNodeKeys"/>.
    /// </para>
    /// </remarks>
    public void DoMemberNodeKeysChanged(NodeType nodedata) {
      if (this.MemberNodesPath == "") return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        System.Collections.IEnumerable oldmemberkeys = ni.MemberKeyCollection;
        IEnumerable<NodeKey> oldmemberkeys2 = ni.SavedMemberKeys;
        System.Collections.IEnumerable newmemberkeys = FindMemberNodeKeysForNode(nodedata);
        IEnumerable<NodeKey> newmemberkeys2 = (newmemberkeys != null ? newmemberkeys.Cast<NodeKey>() : null);
        if (!EqualSequences<NodeKey>(oldmemberkeys2, newmemberkeys2)) {
          if (oldmemberkeys2 != null) {
            foreach (NodeKey removed in (newmemberkeys2 != null ? oldmemberkeys2.Except(newmemberkeys2) : oldmemberkeys2)) {
              NodeType memberdata = FindNodeByKey(removed);
              if (memberdata != null) {
                NodeInfo memberni = FindNodeInfoForNode(memberdata);
                RemoveGroupMemberInfos(ni, memberni);
              }
            }
          }
          SetMemberKeyCollectionHandler(ni, newmemberkeys);
          if (newmemberkeys2 != null) {
            foreach (NodeKey added in (oldmemberkeys2 != null ? newmemberkeys2.Except(oldmemberkeys2) : newmemberkeys2)) {
              NodeType memberdata = FindNodeByKey(added);
              if (memberdata != null) {
                NodeInfo memberni = FindNodeInfoForNode(memberdata);
                AddGroupMemberInfos(ni, memberni);
              }
            }
          }
          RaiseModelChanged(ModelChange.ChangedMemberNodeKeys, nodedata, oldmemberkeys, newmemberkeys);
        }
      }
    }
    void IGroupsModel.DoMemberNodeKeysChanged(Object nodedata) { DoMemberNodeKeysChanged((NodeType)nodedata); }


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
    /// This is used for model navigation and graph traversal.
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
      return fni != null && tni != null && tni.FromNodeInfos != null && tni.FromNodeInfos.Contains(fni);
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
        if (ni.FromNodeInfos != null && ni.ToNodeInfos != null) {
          return ni.FromNodeInfos.Select(i => i.Data).Concat(ni.ToNodeInfos.Select(i => i.Data));
        } else if (ni.FromNodeInfos != null) {
          return ni.FromNodeInfos.Select(i => i.Data);
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
      if (ni != null && ni.FromNodeInfos != null) return ni.FromNodeInfos.Select(i => i.Data);
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
    IEnumerable<Object> IDiagramModel.GetToNodesForNode(Object nodedata) { return GetToNodesForNode((NodeType)nodedata).Cast<Object>(); }


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


    /// <summary>
    /// This predicate is true if a given node data may be a group (i.e. a container) of other nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// This model assumes that the value will never change.
    /// </para>
    /// </remarks>
    public bool GetIsGroupForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) return ni.IsGroup;
      return false;
    }
    bool IGroupsModel.GetIsGroupForNode(Object nodedata) { return GetIsGroupForNode((NodeType)nodedata); }

    /// <summary>
    /// This predicate is true if the <paramref name="membernodedata"/> is a member
    /// of the <paramref name="groupnodedata"/> container group.
    /// </summary>
    /// <param name="groupnodedata"></param>
    /// <param name="membernodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public bool IsMember(NodeType groupnodedata, NodeType membernodedata) {
      NodeInfo ci = FindNodeInfoForNode(groupnodedata);
      NodeInfo mi = FindNodeInfoForNode(membernodedata);
      return IsMemberInfos(ci, mi);
    }
    bool IGroupsModel.IsMember(Object groupnodedata, Object membernodedata) { return IsMember((NodeType)groupnodedata, (NodeType)membernodedata); }

    private bool IsMemberInfos(NodeInfo ci, NodeInfo mi) {
      return ci != null && mi != null && mi.GroupNodeInfo == ci;
    }


    /// <summary>
    /// Return the container group node data for a given node data, if there is one.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a <typeparamref name="NodeType"/> node data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetGroupForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.GroupNodeInfo != null) return ni.GroupNodeInfo.Data;
      return default(NodeType);
    }
    Object ISubGraphModel.GetGroupForNode(Object nodedata) { return GetGroupForNode((NodeType)nodedata); }

    /// <summary>
    /// Return a sequence of node data that are immediate members of a given node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/> of member node data; an empty sequence if there are no members</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetMemberNodesForGroup(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.MemberNodeInfos != null) return ni.MemberNodeInfos.Select(mi => mi.Data);
      return NoNodes;
    }
    IEnumerable<Object> IGroupsModel.GetMemberNodesForGroup(Object nodedata) { return GetMemberNodesForGroup((NodeType)nodedata).Cast<Object>(); }


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
    /// either by overriding the data's Clone method, or
    /// by declaring the data Serializable (WPF),
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
    /// <param name="newgroup">for convenience, the copied "parent" node data</param>
    /// <param name="newmembers">for convenience, a list of newly copied member nodes</param>
    /// <param name="newfroms">for convenience, a list of newly copied nodes that the new data is connected from</param>
    /// <param name="newtos">for convenience, a list of newly copied nodes that the new data is connected to</param>
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
    ///   // this assumes the node data has a modifiable collection of "from" node references
    ///   // if not, you could instead construct your own list of keys and call ModifyFromNodeKeys
    ///   if (this.FromNodesPath != "" &amp;&amp; newfroms != null) {
    ///     // remove any old keys; not needed if the copied property value is empty
    ///     foreach (NodeKey k in FindFromNodeKeysForNode(newnodedata).OfType&lt;NodeKey&gt;().ToList()) {
    ///       DeleteFromNodeKey(newnodedata, k);
    ///     }
    ///     // add new keys (which should be different from the old keys)
    ///     foreach (NodeType newfrom in newfroms) {
    ///       NodeKey newfromkey = FindKeyForNode(newfrom);
    ///       InsertFromNodeKey(newnodedata, newfromkey);
    ///     }
    ///   }
    ///   // this assumes the node data has a modifiable collection of "to" node references;
    ///   // if not, you could instead construct your own list of keys and call ModifyToNodeKeys
    ///   if (this.ToNodesPath != "" &amp;&amp; newtos != null) {
    ///     // remove any old keys; not needed if the copied property value is empty
    ///     foreach (NodeKey k in FindToNodeKeysForNode(newnodedata).OfType&lt;NodeKey&gt;().ToList()) {
    ///       DeleteToNodeKey(newnodedata, k);
    ///     }
    ///     // add new keys (which should be different from the old keys)
    ///     foreach (NodeType newto in newtos) {
    ///       NodeKey newtokey = FindKeyForNode(newto);
    ///       InsertToNodeKey(newnodedata, newtokey);
    ///     }
    ///   }
    ///   // this assumes the node data has a reference to its container group
    ///   if (this.GroupNodePath != "" &amp;&amp; newgroup != null) {
    ///     ModifyGroupNodeKey(newnodedata, FindKeyForNode(newgroup));
    ///   }
    ///   // this assumes the node data has a modifiable collection that InsertMemberNodeKey can work with;
    ///   // if not, you could instead construct your own list of keys and call ModifyMemberNodeKeys.
    ///   if (this.MemberNodesPath != "" &amp;&amp; newmembers != null) {
    ///     // remove any old keys; not needed if the copied property value is empty
    ///     foreach (NodeKey k in FindMemberNodeKeysForNode(newnodedata).OfType&lt;NodeKey&gt;().ToList()) {
    ///       DeleteMemberNodeKey(newnodedata, k);
    ///     }
    ///     // add new keys (which should be different from the old keys)
    ///     foreach (NodeType newmem in newmembers) {
    ///       NodeKey newmemkey = FindKeyForNode(newmem);
    ///       InsertMemberNodeKey(newnodedata, newmemkey);
    ///     }
    ///   }
    /// </code>
    /// </para>
    /// </remarks>
    protected virtual void CopyNode2(NodeType oldnodedata, CopyDictionary env, NodeType newnodedata, NodeType newgroup, IEnumerable<NodeType> newmembers, IEnumerable<NodeType> newfroms, IEnumerable<NodeType> newtos) {
      if (this.Delegates.CopyNode2 != null) {
        this.Delegates.CopyNode2(oldnodedata, env, newnodedata, newgroup, newmembers, newfroms, newtos);
        return;
      }
      if (this.FromNodesPath != "" && newfroms != null) {
        // remove any old keys
        foreach (NodeKey k in FindFromNodeKeysForNode(newnodedata).OfType<NodeKey>().ToList()) {
          DeleteFromNodeKey(newnodedata, k);
        }
        // add new keys
        foreach (NodeType newfrom in newfroms) {
          NodeKey newfromkey = FindKeyForNode(newfrom);
          InsertFromNodeKey(newnodedata, newfromkey);
        }
      }
      if (this.ToNodesPath != "" && newtos != null) {
        // remove any old keys
        foreach (NodeKey k in FindToNodeKeysForNode(newnodedata).OfType<NodeKey>().ToList()) {
          DeleteToNodeKey(newnodedata, k);
        }
        // add new keys
        foreach (NodeType newto in newtos) {
          NodeKey newtokey = FindKeyForNode(newto);
          InsertToNodeKey(newnodedata, newtokey);
        }
      }
      if (this.GroupNodePath != "" && newgroup != null) {
        ModifyGroupNodeKey(newnodedata, FindKeyForNode(newgroup));
      }
      if (this.MemberNodesPath != "" && newmembers != null) {
        // remove any old keys
        foreach (NodeKey k in FindMemberNodeKeysForNode(newnodedata).OfType<NodeKey>().ToList()) {
          DeleteMemberNodeKey(newnodedata, k);
        }
        // add new keys
        foreach (NodeType newmem in newmembers) {
          NodeKey newmemkey = FindKeyForNode(newmem);
          InsertMemberNodeKey(newnodedata, newmemkey);
        }
      }
      //ModelHelper.Error(this, "Override CopyNode2");
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
      GraphModel<NodeType, NodeKey> _Model;

      /// <summary>
      /// Gets or sets the model that owns all of the nodes in this collection.
      /// </summary>
      public GraphModel<NodeType, NodeKey> Model {
        get { return _Model; }
        set { _Model = value; }
      }
      IDiagramModel IDataCollection.Model {
        get { return this.Model; }
        set { this.Model = (GraphModel<NodeType, NodeKey>)value; }
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
      /// This will be set by the <see cref="GraphModel{NodeType, NodeKey}.AddCollectionCopy"/> method.
      /// </remarks>
      public GraphModel<NodeType, NodeKey> SourceModel { get; set; }
      IDiagramModel ICopyDictionary.SourceModel {
        get { return this.SourceModel; }
        set { this.SourceModel = (GraphModel<NodeType, NodeKey>)value; }
      }

      /// <summary>
      /// Gets or sets the destination model for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="GraphModel{NodeType, NodeKey}.AddCollectionCopy"/> method.
      /// </remarks>
      public GraphModel<NodeType, NodeKey> DestinationModel { get; set; }
      IDiagramModel ICopyDictionary.DestinationModel {
        get { return this.DestinationModel; }
        set { this.DestinationModel = (GraphModel<NodeType, NodeKey>)value; }
      }

      /// <summary>
      /// Gets or sets the source collection of data to be copied for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="GraphModel{NodeType, NodeKey}.AddCollectionCopy"/> method.
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
    /// newly copied group node, if any, and a list of member nodes.
    /// </para>
    /// </remarks>
    public CopyDictionary AddCollectionCopy(DataCollection coll, CopyDictionary env) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddCollectionCopy");
      if (coll == null) return env;
      GraphModel<NodeType, NodeKey> srcmodel = coll.Model;
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
        NodeType oldsg = srcmodel.GetGroupForNode(n);
        NodeType newsg = env.FindCopiedNode(oldsg);
        IEnumerable<NodeType> newmembers = srcmodel.GetMemberNodesForGroup(n).Select(m => env.FindCopiedNode(m)).Where(c => c != null);
        IEnumerable<NodeType> newfroms = srcmodel.GetFromNodesForNode(n).Select(f => env.FindCopiedNode(f)).Where(c => c != null);
        IEnumerable<NodeType> newtos = srcmodel.GetToNodesForNode(n).Select(t => env.FindCopiedNode(t)).Where(c => c != null);
        CopyNode2(n, env, copy, newsg, newmembers, newfroms, newtos);
        bool needsnotification = !(copy is INotifyPropertyChanged);



        if (needsnotification && IsNodeData(copy)) {
          DoFromNodeKeysChanged(copy);
          DoToNodeKeysChanged(copy);
          DoGroupNodeChanged(copy);
          DoMemberNodeKeysChanged(copy);
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
      // recursively add all of the member nodes and links for each group to the COLL collection
      if (this.CopyingGroupCopiesMembers) AddGroupParts(coll);
    }


    internal /*?? public */ bool CopyingGroupCopiesMembers {
      get { return _CopyingGroupCopiesMembers; }
      set {
        bool old = _CopyingGroupCopiesMembers;
        if (old != value) {
          _CopyingGroupCopiesMembers = value;
          RaiseModelChanged(ModelChange.ChangedCopyingGroupCopiesMembers, null, old, value);
        }
      }
    }


    private void AddGroupParts(DataCollection coll) {
      foreach (NodeType n in coll.Nodes.ToArray()) {  // work on copy, to allow modification of COLL
        AddGroupParts1(n, coll);
      }
    }

    private void AddGroupParts1(NodeType n, DataCollection coll) {
      coll.AddNode(n);
      if (GetIsGroupForNode(n)) {
        foreach (NodeType c in GetMemberNodesForGroup(n)) {
          AddGroupParts1(c, coll);
        }
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
    /// <param name="fromdata"></param>
    /// <param name="todata"></param>
    /// <remarks>
    /// If <see cref="FromNodesPath"/> is not an empty string,
    /// this calls <see cref="AddFromNodeKey"/>.
    /// If <see cref="ToNodesPath"/> is not an empty string,
    /// this calls <see cref="AddToNodeKey"/>.
    /// This method can be overridden in case the creation of a link
    /// is more complex than the default implementation.
    /// </remarks>
    protected virtual void InsertLink(NodeType fromdata, NodeType todata) {
      if (this.Delegates.InsertLink != null) {
        this.Delegates.InsertLink(this, fromdata, todata);
        return;
      }
      if (this.FromNodesPath != "" || this.ToNodesPath != "") {
        if (this.FromNodesPath != "") {
          AddFromNodeKey(todata, FindKeyForNode(fromdata));
        }
        if (this.ToNodesPath != "") {
          AddToNodeKey(fromdata, FindKeyForNode(todata));
        }
      } else {
        ModelHelper.Error(this, "Override InsertLink(NodeType, NodeType) to support creating a new link");
      }
    }


    /// <summary>
    /// Create a link between two nodes.
    /// </summary>
    /// <param name="fromdata"></param>
    /// <param name="todata"></param>
    /// <remarks>
    /// This is a no-op if the two nodes are already linked, according to <see cref="IsLinked"/>.
    /// This calls <see cref="InsertLink"/> to actually create the link.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddLink(NodeType fromdata, NodeType todata) {
      if (IsLinked(fromdata, todata)) return;  // already linked
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddLink");
      InsertLink(fromdata, todata);
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
      if (this.RemovingGroupRemovesMembers) RemoveGroupMembers(nodedata);
      RemoveConnectedLinks(nodedata);
      DeleteNode(nodedata);
    }
    void IDiagramModel.RemoveNode(Object nodedata) { RemoveNode((NodeType)nodedata); }


    private void RemoveConnectedLinks(NodeType nodedata) {
      foreach (NodeType todata in GetToNodesForNode(nodedata).ToArray()) {  // work on copy, to allow modification of collection
        RemoveLink(nodedata, todata);
      }
      foreach (NodeType fromdata in GetFromNodesForNode(nodedata).ToArray()) {  // work on copy, to allow modification of collection
        RemoveLink(fromdata, nodedata);
      }
    }


    internal /*?? public */ bool RemovingGroupRemovesMembers {
      get { return _RemovingGroupRemovesMembers; }
      set {
        bool old = _RemovingGroupRemovesMembers;
        if (old != value) {
          _RemovingGroupRemovesMembers = value;
          RaiseModelChanged(ModelChange.ChangedRemovingGroupRemovesMembers, null, old, value);
        }
      }
    }


    private void RemoveGroupMembers(NodeType nodedata) {
      if (GetIsGroupForNode(nodedata)) {
        foreach (NodeType node in GetMemberNodesForGroup(nodedata).ToArray()) {  // work on copy, to allow modification of collection
          RemoveNode(node);
        }
      }
    }


    /// <summary>
    /// This method actually implements the removal of a link between two nodes.
    /// </summary>
    /// <param name="fromdata"></param>
    /// <param name="todata"></param>
    /// <remarks>
    /// If <see cref="FromNodesPath"/> is not an empty string,
    /// this calls <see cref="RemoveFromNodeKey"/>.
    /// If <see cref="ToNodesPath"/> is not an empty string,
    /// this calls <see cref="RemoveToNodeKey"/>.
    /// This method can be overridden in case the deletion of a link
    /// is more complex than the default implementation.
    /// </remarks>
    protected virtual void DeleteLink(NodeType fromdata, NodeType todata) {
      if (this.Delegates.DeleteLink != null) {
        this.Delegates.DeleteLink(this, fromdata, todata);
        return;
      }
      if (this.FromNodesPath != "" || this.ToNodesPath != "") {
        if (this.FromNodesPath != "") {
          RemoveFromNodeKey(todata, FindKeyForNode(fromdata));
        }
        if (this.ToNodesPath != "") {
          RemoveToNodeKey(fromdata, FindKeyForNode(todata));
        }
      } else {
        ModelHelper.Error(this, "Override DeleteLink to support the removal of links");
      }
    }


    /// <summary>
    /// Remove any link between two nodes.
    /// </summary>
    /// <param name="fromdata"></param>
    /// <param name="todata"></param>
    /// <remarks>
    /// This is a no-op if there is no existing link between the nodes, according to <see cref="IsLinked"/>.
    /// This calls <see cref="DeleteLink"/> to actually delete the link.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveLink(NodeType fromdata, NodeType todata) {
      if (!IsLinked(fromdata, todata)) return;  // no existing link to remove
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveLink");
      DeleteLink(fromdata, todata);
    }
    void IDiagramModel.RemoveLink(Object fromdata, Object fromparam, Object todata, Object toparam) { RemoveLink((NodeType)fromdata, (NodeType)todata); }


    // Modification

    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of "from" node data keys includes a given <paramref name="fromkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="fromkey">the key value of the "from" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindFromNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeKey}"/>.
    /// </remarks>
    protected virtual void InsertFromNodeKey(NodeType nodedata, NodeKey fromkey) {
      if (this.Delegates.InsertFromNodeKey != null) {
        this.Delegates.InsertFromNodeKey(this, nodedata, fromkey);
        return;
      }
      if (fromkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable fromkeys = FindFromNodeKeysForNode(nodedata);
      if (fromkeys == null) {
        ModelHelper.Error(this, "Override InsertFromNodeKey to support adding node key to the FromNodeKeys of node data, which is now null");
      }
      System.Collections.IList list = fromkeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (!list.Contains(fromkey)) {
          list.Add(fromkey);
          if (!(fromkeys is INotifyCollectionChanged)) DoFromNodeKeyAdded(nodedata, fromkey);
        }
      } else {
        IList<NodeKey> nlist = fromkeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (!nlist.Contains(fromkey)) {
            nlist.Add(fromkey);
            if (!(fromkeys is INotifyCollectionChanged)) DoFromNodeKeyAdded(nodedata, fromkey);
          }
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertFromNodeKey to support adding node key to the FromNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Add a "from" node data's key value to a node data's list of nodes from which links come.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="fromkey">the key value of the "from" node data</param>
    /// <remarks>
    /// This calls <see cref="InsertFromNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddFromNodeKey(NodeType nodedata, NodeKey fromkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddFromNodeKey");
      InsertFromNodeKey(nodedata, fromkey);
    }
    void IConnectedModel.AddFromNodeKey(Object nodedata, Object fromkey) { AddFromNodeKey((NodeType)nodedata, (NodeKey)fromkey); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of "from" node data keys does not include a given <paramref name="fromkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="fromkey">the key value of the "from" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindFromNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeKey}"/>.
    /// </remarks>
    protected virtual void DeleteFromNodeKey(NodeType nodedata, NodeKey fromkey) {
      if (this.Delegates.DeleteFromNodeKey != null) {
        this.Delegates.DeleteFromNodeKey(this, nodedata, fromkey);
        return;
      }
      if (fromkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable fromkeys = FindFromNodeKeysForNode(nodedata);
      System.Collections.IList list = fromkeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (list.Contains(fromkey)) {
          list.Remove(fromkey);
          if (!(fromkeys is INotifyCollectionChanged)) DoFromNodeKeyRemoved(nodedata, fromkey);
        }
      } else {
        IList<NodeKey> nlist = fromkeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (nlist.Contains(fromkey)) {
            nlist.Remove(fromkey);
            if (!(fromkeys is INotifyCollectionChanged)) DoFromNodeKeyRemoved(nodedata, fromkey);
          }
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteFromNodeKey to support removing node key from the FromNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Remove a "from" node data's key value from a node data's list of nodes from which links come.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="fromkey">the key value of the "from" node data</param>
    /// <remarks>
    /// This calls <see cref="DeleteFromNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveFromNodeKey(NodeType nodedata, NodeKey fromkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveFromNodeKey");
      DeleteFromNodeKey(nodedata, fromkey);
    }
    void IConnectedModel.RemoveFromNodeKey(Object nodedata, Object fromkey) { RemoveFromNodeKey((NodeType)nodedata, (NodeKey)fromkey); }


    /// <summary>
    /// This method actually implements the replacement of a node data's
    /// collection of "from" node keys.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="fromnodekeys">a sequence of "from" node data key values</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FromNodesPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyFromNodeKeys(NodeType nodedata, System.Collections.IEnumerable fromnodekeys) {
      if (this.Delegates.SetFromNodeKeys != null) {
        this.Delegates.SetFromNodeKeys(this, nodedata, fromnodekeys);
        return;
      }
      String path = this.FromNodesPath;
      if (path != null && path.Length > 0) {
        _FromNodesPathPPI.SetFor(nodedata, fromnodekeys);
      } else {
        ModelHelper.Error(this, "Override ModifyFromNodeKeys to support reconnecting existing node data");
      }
    }

    /// <summary>
    /// Replace a node data's list of "from" node key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="fromnodekeys">a sequence of "from" node data key values</param>
    /// <remarks>
    /// This calls <see cref="ModifyFromNodeKeys"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetFromNodeKeys(NodeType nodedata, System.Collections.IEnumerable fromnodekeys) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetFromNodeKeys");
      ModifyFromNodeKeys(nodedata, fromnodekeys);
    }
    void IConnectedModel.SetFromNodeKeys(Object nodedata, System.Collections.IEnumerable fromkeys) { SetFromNodeKeys((NodeType)nodedata, fromkeys); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of "to" node data keys includes a given <paramref name="tokey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="tokey">the key value of the "to" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindToNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeKey}"/>.
    /// </remarks>
    protected virtual void InsertToNodeKey(NodeType nodedata, NodeKey tokey) {
      if (this.Delegates.InsertToNodeKey != null) {
        this.Delegates.InsertToNodeKey(this, nodedata, tokey);
        return;
      }
      if (tokey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable tokeys = FindToNodeKeysForNode(nodedata);
      if (tokeys == null) {
        ModelHelper.Error(this, "Override InsertToNodeKey to support adding node key to the ToNodeKeys of node data, which is now null");
      }
      System.Collections.IList list = tokeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (!list.Contains(tokey)) {
          list.Add(tokey);
          if (!(tokeys is INotifyCollectionChanged)) DoToNodeKeyAdded(nodedata, tokey);
        }
      } else {
        IList<NodeKey> nlist = tokeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (!nlist.Contains(tokey)) {
            nlist.Add(tokey);
            if (!(tokeys is INotifyCollectionChanged)) DoToNodeKeyAdded(nodedata, tokey);
          }
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertToNodeKey to support adding node key to the ToNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Add a "to" node data's key value to a node data's list of nodes to which links go.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="tokey">the key value of the "to" node data</param>
    /// <remarks>
    /// This calls <see cref="InsertToNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddToNodeKey(NodeType nodedata, NodeKey tokey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddToNodeKey");
      InsertToNodeKey(nodedata, tokey);
    }
    void IConnectedModel.AddToNodeKey(Object nodedata, Object tokey) { AddToNodeKey((NodeType)nodedata, (NodeKey)tokey); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of "to" node data keys does not include a given <paramref name="tokey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="tokey">the key value of the "to" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindToNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeKey}"/>.
    /// </remarks>
    protected virtual void DeleteToNodeKey(NodeType nodedata, NodeKey tokey) {
      if (this.Delegates.DeleteToNodeKey != null) {
        this.Delegates.DeleteToNodeKey(this, nodedata, tokey);
        return;
      }
      if (tokey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable tokeys = FindToNodeKeysForNode(nodedata);
      System.Collections.IList list = tokeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (list.Contains(tokey)) {
          list.Remove(tokey);
          if (!(tokeys is INotifyCollectionChanged)) DoToNodeKeyRemoved(nodedata, tokey);
        }
      } else {
        IList<NodeKey> nlist = tokeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (nlist.Contains(tokey)) {
            nlist.Remove(tokey);
            if (!(tokeys is INotifyCollectionChanged)) DoToNodeKeyRemoved(nodedata, tokey);
          }
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteToNodeKey to support removing node key from the ToNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Remove a "to" node data's key value from a node data's list of nodes to which links go.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="tokey">the key value of the "to" node data</param>
    /// <remarks>
    /// This calls <see cref="DeleteToNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveToNodeKey(NodeType nodedata, NodeKey tokey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveToNodeKey");
      DeleteToNodeKey(nodedata, tokey);
    }
    void IConnectedModel.RemoveToNodeKey(Object nodedata, Object tokey) { RemoveToNodeKey((NodeType)nodedata, (NodeKey)tokey); }


    /// <summary>
    /// This method actually implements the replacement of a node data's
    /// collection of "to" node keys.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="tonodekeys">a sequence of "to" node data key values</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="ToNodesPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyToNodeKeys(NodeType nodedata, System.Collections.IEnumerable tonodekeys) {
      if (this.Delegates.SetToNodeKeys != null) {
        this.Delegates.SetToNodeKeys(this, nodedata, tonodekeys);
        return;
      }
      String path = this.ToNodesPath;
      if (path != null && path.Length > 0) {
        _ToNodesPathPPI.SetFor(nodedata, tonodekeys);
      } else {
        ModelHelper.Error(this, "Override ModifyToNodeKeys to support reconnecting existing node data");
      }
    }

    /// <summary>
    /// Replace a node data's list of "to" node key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="tonodekeys">a sequence of "to" node data key values</param>
    /// <remarks>
    /// This calls <see cref="ModifyToNodeKeys"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetToNodeKeys(NodeType nodedata, System.Collections.IEnumerable tonodekeys) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetToNodeKeys");
      ModifyToNodeKeys(nodedata, tonodekeys);
    }
    void IConnectedModel.SetToNodeKeys(Object nodedata, System.Collections.IEnumerable tokeys) { SetToNodeKeys((NodeType)nodedata, tokeys); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that its reference to a containing group node data (if any) is the given <paramref name="groupkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="groupkey">the key value of the "group" node data</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="GroupNodePath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyGroupNodeKey(NodeType nodedata, NodeKey groupkey) {
      if (this.Delegates.SetGroupNodeKey != null) {
        this.Delegates.SetGroupNodeKey(this, nodedata, groupkey);
        return;
      }
      String path = this.GroupNodePath;
      if (path != null && path.Length > 0) {
        _GroupNodePathPPI.SetFor(nodedata, groupkey);
      } else {
        ModelHelper.Error(this, "Override ModifyGroupNodeKey to support changing group membership of existing node data");
      }
    }

    /// <summary>
    /// Change a node data so that it refers to a different container group node data, by node key.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="groupkey">the key value of the new "group" node data</param>
    /// <remarks>
    /// This calls <see cref="ModifyGroupNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetGroupNodeKey(NodeType nodedata, NodeKey groupkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetGroupNodeKey");
      ModifyGroupNodeKey(nodedata, groupkey);
    }
    void ISubGraphModel.SetGroupNode(Object nodedata, Object groupnodedata) { SetGroupNodeKey((NodeType)nodedata, FindKeyForNode((NodeType)groupnodedata)); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of member node data includes a given <paramref name="memberkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="memberkey">the key value of the new "member" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindMemberNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void InsertMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (this.Delegates.InsertMemberNodeKey != null) {
        this.Delegates.InsertMemberNodeKey(this, nodedata, memberkey);
        return;
      }
      if (memberkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(nodedata);
      if (memberkeys == null) {
        ModelHelper.Error(this, "Override InsertMemberNodeKey to support adding node key to the MemberNodeKeys of node data, which is now null");
      }
      System.Collections.IList list = memberkeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (!list.Contains(memberkey)) {
          list.Add(memberkey);
          if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyAdded(nodedata, memberkey);
        }
      } else {
        IList<NodeKey> nlist = memberkeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (!nlist.Contains(memberkey)) {
            nlist.Add(memberkey);
            if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyAdded(nodedata, memberkey);
          }
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertMemberNodeKey to support adding node key to the MemberNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Add a "member" node data's key value to a node data's list of "members".
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="memberkey">the key value of the new "member" node data</param>
    /// <remarks>
    /// This calls <see cref="InsertMemberNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddMemberNodeKey");
      InsertMemberNodeKey(nodedata, memberkey);
    }
    void IGroupsModel.AddMemberNodeKey(Object nodedata, Object memberkey) { AddMemberNodeKey((NodeType)nodedata, (NodeKey)memberkey); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of children node data does not include a given <paramref name="memberkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="memberkey">the key value of the "member" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindMemberNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void DeleteMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (this.Delegates.DeleteMemberNodeKey != null) {
        this.Delegates.DeleteMemberNodeKey(this, nodedata, memberkey);
        return;
      }
      if (memberkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(nodedata);
      System.Collections.IList list = memberkeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (list.Contains(memberkey)) {
          list.Remove(memberkey);
          if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyRemoved(nodedata, memberkey);
        }
      } else {
        IList<NodeKey> nlist = memberkeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (nlist.Contains(memberkey)) {
            nlist.Remove(memberkey);
            if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyRemoved(nodedata, memberkey);
          }
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteMemberNodeKey to support removing node key from the MemberNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Remove a child node data's key value from a group node data's list of "member" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="memberkey">the key value of the "member" node data</param>
    /// <remarks>
    /// This calls <see cref="DeleteMemberNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveMemberNodeKey");
      DeleteMemberNodeKey(nodedata, memberkey);
    }
    void IGroupsModel.RemoveMemberNodeKey(Object nodedata, Object memberkey) { RemoveMemberNodeKey((NodeType)nodedata, (NodeKey)memberkey); }


    /// <summary>
    /// This method actually implements the replacement of a group node data's
    /// collection of member node keys.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="membernodekeys">a sequence of "member" node data key values</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="MemberNodesPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyMemberNodeKeys(NodeType nodedata, System.Collections.IEnumerable membernodekeys) {
      if (this.Delegates.SetMemberNodeKeys != null) {
        this.Delegates.SetMemberNodeKeys(this, nodedata, membernodekeys);
        return;
      }
      String path = this.MemberNodesPath;
      if (path != null && path.Length > 0) {
        _MemberNodesPathPPI.SetFor(nodedata, membernodekeys);
      } else {
        ModelHelper.Error(this, "Override ModifyMemberNodeKeys to support reconnecting existing node data");
      }
    }

    /// <summary>
    /// Replace a node data's list of "member" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="membernodekeys">a sequence of "member" node data key values</param>
    /// <remarks>
    /// This calls <see cref="ModifyMemberNodeKeys"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetMemberNodeKeys(NodeType nodedata, System.Collections.IEnumerable membernodekeys) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetMemberNodeKeys");
      ModifyMemberNodeKeys(nodedata, membernodekeys);
    }
    void IGroupsModel.SetMemberNodeKeys(Object nodedata, System.Collections.IEnumerable memberkeys) { SetMemberNodeKeys((NodeType)nodedata, memberkeys); }


    // ValidCycle

    /// <summary>
    /// Specify what kinds of graphs this model allows.
    /// </summary>
    /// <remarks>
    /// Changing this property will not remove or add any links,
    /// but will only affect future attempts to add or modify links.
    /// </remarks>
    public ValidCycle ValidCycle {
      get { return _ValidCycle; }
      set {
        ValidCycle old = _ValidCycle;
        if (old != value) {
          _ValidCycle = value;
          RaiseModelChanged(ModelChange.ChangedValidCycle, null, old, value);
        }
      }
    }


    /// <summary>
    /// This predicate is true if adding a link between two nodes would result in a valid graph.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckLinkValid"/> to do the actual graph structure check.
    /// </remarks>
    /// <seealso cref="ValidCycle"/>
    public bool IsLinkValid(NodeType fromdata, NodeType todata) {
      return CheckLinkValid(fromdata, todata, false, default(NodeType), default(NodeType));
    }
    bool IDiagramModel.IsLinkValid(Object fromdata, Object fromparam, Object todata, Object toparam) {
      return CheckLinkValid((NodeType)fromdata, (NodeType)todata, false, default(NodeType), default(NodeType));
    }

    /// <summary>
    /// This predicate is true if adding a link between two nodes after removing an existing one would result in a valid graph.
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the link would come</param>
    /// <param name="newtodata">a node key identify the node data to which the link would go</param>
    /// <param name="oldfromdata">a node key identifying the node data from which the existing link comes</param>
    /// <param name="oldtodata">a node key identify the node data to which the existing link goes</param>
    /// <returns></returns>
    public bool IsRelinkValid(NodeType newfromdata, NodeType newtodata, NodeType oldfromdata, NodeType oldtodata) {
      return CheckLinkValid(newfromdata, newtodata, true, oldfromdata, oldtodata);
    }
    bool IConnectedModel.IsRelinkValid(Object newfromdata, Object newtodata, Object oldfromdata, Object oldtodata) {
      return CheckLinkValid((NodeType)newfromdata, (NodeType)newtodata, true, (NodeType)oldfromdata, (NodeType)oldtodata);
    }

    /// <summary>
    /// This predicate is true if adding a link between two nodes/ports would result in a validly structured graph.
    /// </summary>
    /// <param name="newfromdata">a node key identifying the node data from which the link would come</param>
    /// <param name="newtodata">a node key identify the node data to which the link would go</param>
    /// <param name="ignoreexistinglink">true if relinking; false if adding a new link</param>
    /// <param name="oldfromdata">a node key identifying the node data from which the existing link comes</param>
    /// <param name="oldtodata">a node key identify the node data to which the existing link goes</param>
    /// <returns>
    /// The behavior of this predicate depends on the value of <see cref="ValidCycle"/>.
    /// This returns false for duplicate links.
    /// </returns>
    protected virtual bool CheckLinkValid(NodeType newfromdata, NodeType newtodata, bool ignoreexistinglink, NodeType oldfromdata, NodeType oldtodata) {
      if (this.Delegates.CheckLinkValid != null) {
        return this.Delegates.CheckLinkValid(newfromdata, newtodata, ignoreexistinglink, oldfromdata, oldtodata);
      }
      if (!IsNodeData(newfromdata) || !IsNodeData(newtodata)) return false;
      if (IsLinked(newfromdata, newtodata)) return false;  // no duplicate links in same direction
      ValidCycle vc = this.ValidCycle;
      if (vc == ValidCycle.All) return true;
      NodeInfo from = FindNodeInfoForNode(newfromdata);
      NodeInfo to = FindNodeInfoForNode(newtodata);
      NodeInfo ignorefrom = (ignoreexistinglink ? FindNodeInfoForNode(oldfromdata) : null);
      NodeInfo ignoreto = (ignoreexistinglink ? FindNodeInfoForNode(oldtodata) : null);
      switch (vc) {
        case ValidCycle.NotDirected: return !MakesDirectedCycle(from, to, ignorefrom, ignoreto);
        case ValidCycle.NotDirectedFast: return !MakesDirectedCycleFast(from, to, ignorefrom, ignoreto);
        case ValidCycle.NotUndirected: return !MakesUndirectedCycle(from, to, ignorefrom, ignoreto);
        case ValidCycle.DestinationTree: return (GetFromNodeInfos(to, ignorefrom).FirstOrDefault() == null) && !MakesDirectedCycleFast(from, to, ignorefrom, ignoreto);
        case ValidCycle.SourceTree: return (GetToNodeInfos(from, ignoreto).FirstOrDefault() == null) && !MakesDirectedCycleFast(from, to, ignorefrom, ignoreto);
        default: return true;
      }
    }


    private static IEnumerable<NodeInfo> GetConnectedNodeInfos(NodeInfo a, NodeInfo ignorefrom, NodeInfo ignoreto) {
      if (a.FromNodeInfos == null) {
        if (a.ToNodeInfos == null)
          return Enumerable.Empty<NodeInfo>();
        else
          return a.ToNodeInfos.Where(ni => ni != ignoreto);
      } else {
        if (a.ToNodeInfos == null)
          return a.FromNodeInfos.Where(ni => ni != ignorefrom);
        else
          return a.FromNodeInfos.Where(ni => ni != ignorefrom).Concat(a.ToNodeInfos.Where(ni => ni != ignoreto));
      }
    }

    private static IEnumerable<NodeInfo> GetFromNodeInfos(NodeInfo a, NodeInfo ignorefrom) {
      if (a.FromNodeInfos == null) return Enumerable.Empty<NodeInfo>();
      return a.FromNodeInfos.Where(ni => ni != ignorefrom);
    }

    private static IEnumerable<NodeInfo> GetToNodeInfos(NodeInfo a, NodeInfo ignoreto) {
      if (a.ToNodeInfos == null) return Enumerable.Empty<NodeInfo>();
      return a.ToNodeInfos.Where(ni => ni != ignoreto);
    }


    private static bool MakesDirectedCycleFast(NodeInfo a, NodeInfo b, NodeInfo ignorefrom, NodeInfo ignoreto) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      foreach (NodeInfo n in GetFromNodeInfos(a, ignorefrom)) {
        if (n == a) continue;
        if (MakesDirectedCycleFast(n, b, ignorefrom, ignoreto))
          return true;
      }
      return false;
    }


    private static bool MakesDirectedCycle(NodeInfo a, NodeInfo b, NodeInfo ignorefrom, NodeInfo ignoreto) {
      if (a == b) return true;
      HashSet<NodeInfo> seen = new HashSet<NodeInfo>();
      seen.Add(b);
      return MakesDirectedCycle1(seen, a, b, ignorefrom, ignoreto);
    }

    private static bool MakesDirectedCycle1(HashSet<NodeInfo> seen, NodeInfo a, NodeInfo b, NodeInfo ignorefrom, NodeInfo ignoreto) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (seen.Contains(a)) return false;
      seen.Add(a);
      foreach (NodeInfo n in GetFromNodeInfos(a, ignorefrom)) {
        if (n == a) continue;
        if (MakesDirectedCycle1(seen, n, b, ignorefrom, ignoreto))
          return true;
      }
      return false;
    }


    private static bool MakesUndirectedCycle(NodeInfo a, NodeInfo b, NodeInfo ignorefrom, NodeInfo ignoreto) {
      if (a == b) return true;
      HashSet<NodeInfo> seen = new HashSet<NodeInfo>();
      seen.Add(b);
      return MakesUndirectedCycle1(seen, a, b, ignorefrom, ignoreto);
    }

    private static bool MakesUndirectedCycle1(HashSet<NodeInfo> seen, NodeInfo a, NodeInfo b, NodeInfo ignorefrom, NodeInfo ignoreto) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (seen.Contains(a)) return false;
      seen.Add(a);
      foreach (NodeInfo n in GetConnectedNodeInfos(a, ignorefrom, ignoreto)) {
        if (n == a) continue;
        if (MakesUndirectedCycle1(seen, n, b, ignorefrom, ignoreto))
          return true;
      }
      return false;
    }


    // ValidMember

    /// <summary>
    /// This predicate is true if adding <paramref name="membernodedata"/> to a group node
    /// (<paramref name="groupnodedata"/>) would result in an invalid graph.
    /// </summary>
    /// <param name="groupnodedata">the node data that is a group</param>
    /// <param name="membernodedata">a node data</param>
    /// <param name="ignoreexistingmembership">
    /// whether to ignore the <paramref name="membernodedata"/>'s existing group membership,
    /// because that member is being transferred out of its existing group
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckMemberValid"/> to do the actual graph structure check.
    /// </remarks>
    /// <seealso cref="ValidCycle"/>
    public bool IsMemberValid(NodeType groupnodedata, NodeType membernodedata, bool ignoreexistingmembership) {
      return CheckMemberValid(groupnodedata, membernodedata, ignoreexistingmembership);
    }
    bool IGroupsModel.IsMemberValid(Object groupnodedata, Object membernodedata) { return IsMemberValid((NodeType)groupnodedata, (NodeType)membernodedata, true); }

    /// <summary>
    /// This predicate is true if adding a node to a group node would result in an invalid graph.
    /// </summary>
    /// <param name="groupnodedata">the node data that is a group</param>
    /// <param name="membernodedata">a node data to be considered for adding to the group</param>
    /// <param name="ignoreexistingmembership">
    /// whether to ignore the <paramref name="membernodedata"/>'s existing group membership,
    /// because that member is being transferred out of its existing group
    /// </param>
    /// <returns>
    /// This returns false for membership that would break the tree structure of the model's graph of groups.
    /// </returns>
    protected virtual bool CheckMemberValid(NodeType groupnodedata, NodeType membernodedata, bool ignoreexistingmembership) {
      if (this.Delegates.CheckMemberValid != null) {
        return this.Delegates.CheckMemberValid(groupnodedata, membernodedata, ignoreexistingmembership);
      }
      if (groupnodedata == null) return true;
      if (!IsNodeData(groupnodedata) || !IsNodeData(membernodedata)) return false;
      NodeInfo sg = FindNodeInfoForNode(groupnodedata);
      NodeInfo n = FindNodeInfoForNode(membernodedata);
      if (!ignoreexistingmembership && n.GroupNodeInfo != null) return false;
      return !MakesDirectedMember(sg, n);
    }


    private static bool MakesDirectedMember(NodeInfo a, NodeInfo b) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (MakesDirectedMember(a.GroupNodeInfo, b)) return true;
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

        case ModelChange.ChangedFromNodeKeys: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyFromNodeKeys(nodedata, (System.Collections.IEnumerable)e.GetValue(undo));
            break;
          }
        case ModelChange.AddedFromNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              DeleteFromNodeKey(nodedata, (NodeKey)e.NewValue);
            else
              InsertFromNodeKey(nodedata, (NodeKey)e.NewValue);
            break;
          }
        case ModelChange.RemovedFromNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              InsertFromNodeKey(nodedata, (NodeKey)e.OldValue);
            else
              DeleteFromNodeKey(nodedata, (NodeKey)e.OldValue);
            break;
          }

        case ModelChange.ChangedToNodeKeys: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyToNodeKeys(nodedata, (System.Collections.IEnumerable)e.GetValue(undo));
            break;
          }
        case ModelChange.AddedToNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              DeleteToNodeKey(nodedata, (NodeKey)e.NewValue);
            else
              InsertToNodeKey(nodedata, (NodeKey)e.NewValue);
            break;
          }
        case ModelChange.RemovedToNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              InsertToNodeKey(nodedata, (NodeKey)e.OldValue);
            else
              DeleteToNodeKey(nodedata, (NodeKey)e.OldValue);
            break;
          }

        case ModelChange.ChangedGroupNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyGroupNodeKey(nodedata, (NodeKey)e.GetValue(undo));
            break;
          }

        case ModelChange.ChangedMemberNodeKeys: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyMemberNodeKeys(nodedata, (System.Collections.IEnumerable)e.GetValue(undo));
            break;
          }
        case ModelChange.AddedMemberNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              DeleteMemberNodeKey(nodedata, (NodeKey)e.NewValue);
            else
              InsertMemberNodeKey(nodedata, (NodeKey)e.NewValue);
            break;
          }
        case ModelChange.RemovedMemberNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              InsertMemberNodeKey(nodedata, (NodeKey)e.OldValue);
            else
              DeleteMemberNodeKey(nodedata, (NodeKey)e.OldValue);
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
        case ModelChange.ChangedNodeIsGroupPath:
          this.NodeIsGroupPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedGroupNodePath:
          this.GroupNodePath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedMemberNodesPath:
          this.MemberNodesPath = (String)e.GetValue(undo);
          break;

        case ModelChange.ChangedFromNodesPath:
          this.FromNodesPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedToNodesPath:
          this.ToNodesPath = (String)e.GetValue(undo);
          break;

        // model state properties
        case ModelChange.ChangedCopyingGroupCopiesMembers:
          this.CopyingGroupCopiesMembers = (bool)e.GetValue(undo);
          break;
        case ModelChange.ChangedRemovingGroupRemovesMembers:
          this.RemovingGroupRemovesMembers = (bool)e.GetValue(undo);
          break;

        case ModelChange.ChangedValidCycle:
          this.ValidCycle = (ValidCycle)e.GetValue(undo);
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
    /// <typeparam name="NodeDataType">this must be a <see cref="GraphModelNodeData{NodeKey}"/></typeparam>
    /// <param name="rootname">the name of the returned <c>XElement</c></param>
    /// <param name="nodename">the name of each <c>XElement</c> holding node data</param>
    /// <returns>an <c>XElement</c></returns>
    public XElement Save<NodeDataType>(XName rootname, XName nodename)
        where NodeDataType : GraphModelNodeData<NodeKey> {
      XElement root = new XElement(rootname);
      root.Add(this.NodesSource.OfType<NodeDataType>().Select(d => d.MakeXElement(nodename)));
      return root;
    }

    /// <summary>
    /// Given a Linq for XML <c>XContainer</c> holding node data, replace this model's
    /// <see cref="NodesSource"/> collection with a collection of new node data.
    /// </summary>
    /// <typeparam name="NodeDataType">this must be a <see cref="GraphModelNodeData{NodeKey}"/> with a public zero-argument constructor</typeparam>
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
        where NodeDataType : GraphModelNodeData<NodeKey>, new() {
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
    /// <typeparam name="NodeDataType">this must be a class inheriting from <see cref="GraphModelNodeData{NodeKey}"/></typeparam>
    /// <param name="root">the <c>XContainer</c> holding all of the data</param>
    /// <param name="nodedataallocator">
    /// a function that takes an <c>XElement</c> and returns either a newly constructed object of type <typeparamref name="NodeDataType"/>
    /// or null if that <c>XElement</c> is to be ignored
    /// </param>
    /// <remarks>
    /// <para>
    /// This will iterate over all of the child elements of the <paramref name="root"/> container,
    /// calling <paramref name="nodedataallocator"/> on each one.
    /// If that function returns non-null, it calls <see cref="GraphModelNodeData{NodeKey}.LoadFromXElement"/> on the new data
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
        where NodeDataType : GraphModelNodeData<NodeKey> {
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

      public Predicate<GraphModel<NodeType, NodeKey>,  NodeType> MakeNodeKeyUnique { get; set; }

      public Func<NodeType, System.Collections.IEnumerable> FindFromNodeKeysForNode { get; set; }

      public Func<NodeType, System.Collections.IEnumerable> FindToNodeKeysForNode { get; set; }

      public Func<NodeType, String> FindCategoryForNode { get; set; }

      public Predicate<NodeType> FindIsGroupForNode { get; set; }

      public Func<NodeType, NodeKey> FindGroupKeyForNode { get; set; }

      public Func<NodeType, System.Collections.IEnumerable> FindMemberNodeKeysForNode { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeKey> ResolveNodeKey { get; set; }

      public Func<NodeType, CopyDictionary, NodeType> CopyNode1 { get; set; }

      public Action<NodeType, CopyDictionary, NodeType, NodeType, IEnumerable<NodeType>, IEnumerable<NodeType>, IEnumerable<NodeType>> CopyNode2 { get; set; }

      public Action<DataCollection> AugmentCopyCollection { get; set; }

      public Action<CopyDictionary> AugmentCopyDictionary { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType> InsertNode { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeType> InsertLink { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType> DeleteNode { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeType> DeleteLink { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> InsertFromNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> DeleteFromNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, System.Collections.IEnumerable> SetFromNodeKeys { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> InsertToNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> DeleteToNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, System.Collections.IEnumerable> SetToNodeKeys { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> SetGroupNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> InsertMemberNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, NodeKey> DeleteMemberNodeKey { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  NodeType, System.Collections.IEnumerable> SetMemberNodeKeys { get; set; }

      public Predicate<NodeType, NodeType, bool, NodeType, NodeType> CheckLinkValid { get; set; }

      public Predicate<NodeType, NodeType, bool> CheckMemberValid { get; set; }

      public Action<GraphModel<NodeType, NodeKey>,  ModelChangedEventArgs, bool> ChangeModelValue { get; set; }

      public Action<NodeType, ModelChangedEventArgs, bool> ChangeDataValue { get; set; }
    }
  }


  /// <summary>
  /// This is a universal model, handling all kinds of datatypes representing nodes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This assumes that each node can be a member of at most one subgraph node.
  /// Since it uses Object as the type for node data, this model class supports multiple instances
  /// of different (unrelated) types.
  /// </para>
  /// <para>
  /// For reasons of both compile-time type checking and run-time efficiency,
  /// we recommend defining your own model class derived from <see cref="GraphModel{NodeType, NodeKey}"/>.
  /// </para>
  /// <para>
  /// This defines nested classes: DataCollection and CopyDictionary.
  /// </para>
  /// </remarks>
  public sealed class UniversalGraphModel : GraphModel<Object, Object> {
    /// <summary>
    /// Create a modifiable <see cref="GraphModel{NodeType, NodeKey}"/>
    /// with an empty <c>ObservableCollection</c> for the
    /// <see cref="GraphModel{NodeType, NodeKey}.NodesSource"/>.
    /// </summary>
    public UniversalGraphModel() {
      this.Initializing = true;
      this.Modifiable = true;
      this.Initializing = false;
    }
  }
}
