
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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam {

  /// <summary>
  /// This class handles the <see cref="Northwoods.GoXam.Model.IDiagramModel.Changed"/> event
  /// for the <see cref="Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Model"/> and is responsible for
  /// creating <see cref="Node"/>s and <see cref="Link"/>s for the data in the model.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each <see cref="Northwoods.GoXam.Diagram"/> has an instance of this class as its
  /// <see cref="Northwoods.GoXam.Diagram.PartManager"/> property.
  /// If you want to customize the standard behavior, you can easily override any of its methods
  /// and substitute an instance of your custom part manager class for your diagram.
  /// <code>
  /// public class CustomPartManager : PartManager {
  ///   protected override String FindCategoryForNode(Object nodedata, IDiagramModel model, bool isgroup, bool islinklabel) {
  ///     // maybe choose different category here, to dynamically determine the DataTemplate found by FindTemplateForNode
  ///   }
  /// }
  /// </code>
  /// and install it with either XAML:
  /// <code>
  ///   &lt;go:Diagram ...&gt;
  ///     &lt;go:Diagram.PartManager&gt;
  ///       &lt;local:CustomPartManager /&gt;
  ///     &lt;/go:Diagram.PartManager&gt;
  ///   &lt;/go:Diagram&gt;
  /// </code>
  /// or in the initialization of your Diagram control:
  /// <code>
  ///   myDiagram.PartManager = new CustomPartManager();
  /// </code>
  /// </para>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class PartManager : FrameworkElement {
    /// <summary>
    /// The constructor for the standard part manager that is the initial value of
    /// <see cref="Northwoods.GoXam.Diagram.PartManager"/>.
    /// </summary>
    public PartManager() {
      _NodesDictionary = new Dictionary<Object, Node>();
      _Nodes = new HashSet<Node>();
      _LinksDictionary = new Dictionary<Object, Link>();
      _Links = new HashSet<Link>();
    }


    /// <summary>
    /// Throw an exception if the current thread does not have access to this <c>DependencyObject</c>.
    /// </summary>
    protected void VerifyAccess() {
      if (!CheckAccess()) Diagram.Error("No access to thread");
    }


    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> for which this <see cref="PartManager"/>
    /// manages the lifetime of <see cref="Part"/>s depending on the contents of the
    /// <see cref="Northwoods.GoXam.Diagram.Model"/>.
    /// </summary>
    /// <value>
    /// This value is automatically set by the <see cref="Northwoods.GoXam.Diagram.PartManager"/> setter.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; internal set; }

    internal void ClearAll() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      diagram.ClearSelection();
      ClearAllNodes();
      ClearAllLinks();
      DiagramPanel panel = diagram.Panel;
      if (panel != null) panel.ClearAll();
    }

    private void ClearAllNodes() {
      // need to get rid of obsolete data bindings
      foreach (Node n in _Nodes) ClearDataBinding(n);
      _NodesDictionary.Clear();
      _Nodes.Clear();
    }

    private void ClearAllLinks() {
      // need to get rid of obsolete data bindings
      foreach (Link l in _Links) ClearDataBinding(l);
      _LinksDictionary.Clear();
      _Links.Clear();
    }

    private void ClearDataBinding(Part p) {
      if (!p.IsBoundToData) return;
      // temporarily remove Model reference, to reduce side-effects due to
      // removing data bindings causing changes in dependency property values
      IDiagramModel oldmodel = p.Model;
      p.Model = null;
      try {
        p.DataContext = null;
        Link link = p as Link;
        if (link != null) {
          FrameworkElement elt = link.Route as FrameworkElement;
          if (elt != null) elt.DataContext = null;
        } else {
          Group group = p as Group;
          if (group != null) {
            FrameworkElement elt = group.Layout as FrameworkElement;
            if (elt != null) elt.DataContext = null;
          }
        }
      } catch (Exception) {
        // ignore binding errors
      }
      p.Model = oldmodel;
    }

    /// <summary>
    /// Find the model for some data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>
    /// normally the <see cref="Northwoods.GoXam.Diagram.Model"/>,
    /// but may return the <see cref="Northwoods.GoXam.Diagram.PartsModel"/>
    /// if the <paramref name="nodedata"/> is a <see cref="Part"/>.
    /// </returns>
    public virtual IDiagramModel FindNodeDataModel(Object nodedata) {
      Diagram diagram = this.Diagram;
      if (diagram == null || nodedata == null) return null;
      IDiagramModel model = diagram.Model;
      //// if the data is already part of a model, return that model
      //if (model != null && model.IsNodeData(nodedata)) return model;
      //IDiagramModel pmodel = diagram.PartsModel;
      //if (pmodel != null && pmodel.IsNodeData(nodedata)) return pmodel;
      // otherwise if the data type matches a model's node type, return that model
      if (model != null && model.IsNodeType(nodedata)) return model;
      PartsModel pmodel = diagram.PartsModel;
      if (pmodel != null && pmodel.IsNodeType(nodedata)) return pmodel;
      return null;
    }

    /// <summary>
    /// Find the model for the two data objects representing nodes.
    /// </summary>
    /// <param name="fromnodedata"></param>
    /// <param name="tonodedata"></param>
    /// <returns>
    /// normally the <see cref="Northwoods.GoXam.Diagram.Model"/>,
    /// but may return the <see cref="Northwoods.GoXam.Diagram.PartsModel"/>
    /// if the both <paramref name="fromnodedata"/> and <paramref name="tonodedata"/>
    /// are <see cref="Node"/>s,
    /// otherwise null.
    /// </returns>
    public virtual IDiagramModel FindCommonDataModel(Object fromnodedata, Object tonodedata) {
      Diagram diagram = this.Diagram;
      if (diagram == null || fromnodedata == null || tonodedata == null) return null;
      IDiagramModel model = diagram.Model;
      //// if both data are already part of a model, return that model
      //if (model != null && model.IsNodeData(fromnodedata) && model.IsNodeData(tonodedata)) return model;
      //IDiagramModel pmodel = diagram.PartsModel;
      //if (pmodel != null && pmodel.IsNodeData(fromnodedata) && pmodel.IsNodeData(tonodedata)) return pmodel;
      // otherwise if both data types match a model's node type, return that model
      if (model != null && model.IsNodeType(fromnodedata) && model.IsNodeType(tonodedata)) return model;
      PartsModel pmodel = diagram.PartsModel;
      if (pmodel != null && pmodel.IsNodeType(fromnodedata) && pmodel.IsNodeType(tonodedata)) return pmodel;
      return null;
    }


    /// <summary>
    /// This is called for each <see cref="Northwoods.GoXam.Model.IDiagramModel.Changed"/> event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// <para>
    /// The implementation of this method and the methods that it calls should not modify the model.
    /// </para>
    /// <para>
    /// For small changes such as the addition or removal of node data from the model,
    /// this calls the <see cref="AddNodeForData"/> or <see cref="RemoveNodeForData"/> method.
    /// </para>
    /// <para>
    /// For changes in link relationships in the model, this calls
    /// <see cref="AddLinkForData(Object, Northwoods.GoXam.Model.IDiagramModel)"/>,
    /// <see cref="AddLinkForData(Object, Object, Northwoods.GoXam.Model.IDiagramModel)"/>,
    /// <see cref="RemoveLinkForData(Object, Northwoods.GoXam.Model.IDiagramModel)"/>, or
    /// <see cref="RemoveLinkForData(Object, Object, Northwoods.GoXam.Model.IDiagramModel)"/>.
    /// </para>
    /// <para>
    /// For more wholescale changes, such as a change in the <see cref="Northwoods.GoXam.Model.IDiagramModel.NodesSource"/>,
    /// this will call <see cref="RebuildNodeElements"/> to discard all existing nodes and links and reconstruct
    /// them using the appropriate (and perhaps changed) data templates.
    /// For widespread changes only involving links, this will call <see cref="RebuildLinkElements"/>.
    /// </para>
    /// </remarks>
    public virtual void OnModelChanged(ModelChangedEventArgs e) {
      if (e == null) return;
      if (e.Change == ModelChange.Property) return;
      VerifyAccess();
      switch (e.Change) {
        // data properties
        case ModelChange.ReplacedReference:
          if (FindNodeForData(e.OldValue, e.Model) != null) {
            RemoveNodeForData(e.OldValue, e.Model);
          }
          if (FindLinkForData(e.OldValue, e.Model) != null) {
            RemoveLinkForData(e.OldValue, e.Model);
          }
          break;

        // model contents and relationships
        case ModelChange.AddedNode:
          AddNodeForData(e.Data, e.Model);
          break;
        case ModelChange.RemovingNode:
          // called before node data is actually removed from the model,
          // when relationships (e.g. with containing group) are known;
          // but won't be called if Model.NodesSource is changed directly,
          // but will be called if someone calls Model.RemoveNode/DeleteNode
          RemoveNodeForData(e.Data, e.Model);
          break;
        case ModelChange.RemovedNode:
          // called after node data is actually removed from the model,
          // but old node data relationships might already be lost
          RemoveNodeForData(e.Data, e.Model);
          break;
        case ModelChange.ChangedParentNodeKey: {
            IDiagramModel model = e.Model;
            if (model != null) {
              Object child = e.Data;
              if (e.OldValue != null) RemoveLinkForData(model.FindNodeByKey(e.OldValue), child, model);
              if (e.NewValue != null) AddLinkForData(model.FindNodeByKey(e.NewValue), child, model);
            }
            break;
          }
        case ModelChange.AddedFromNodeKey: {
            IDiagramModel model = e.Model;
            if (model != null) {
              AddLinkForData(model.FindNodeByKey(e.NewValue), e.Data, model);
            }
            break;
          }
        case ModelChange.RemovedFromNodeKey: {
            IDiagramModel model = e.Model;
            if (model != null) {
              RemoveLinkForData(model.FindNodeByKey(e.OldValue), e.Data, model);
            }
            break;
          }
        case ModelChange.AddedChildNodeKey:
        case ModelChange.AddedToNodeKey: {
            IDiagramModel model = e.Model;
            if (model != null) {
              AddLinkForData(e.Data, model.FindNodeByKey(e.NewValue), model);
            }
            break;
          }
        case ModelChange.RemovedChildNodeKey:
        case ModelChange.RemovedToNodeKey: {
            IDiagramModel model = e.Model;
            if (model != null) {
              RemoveLinkForData(e.Data, model.FindNodeByKey(e.OldValue), model);
            }
            break;
          }
        case ModelChange.ChangedFromNodeKeys: {
            Object node = e.Data;
            if (node != null) {
              IDiagramModel model = e.Model;
              if (model != null) {
                System.Collections.IEnumerable oldneighborkeys = e.OldValue as System.Collections.IEnumerable;
                System.Collections.IEnumerable newneighborkeys = e.NewValue as System.Collections.IEnumerable;
                if (oldneighborkeys != null) {
                  foreach (Object n in oldneighborkeys) {
                    if (newneighborkeys == null || !ContainsKey(newneighborkeys, n)) {
                      RemoveLinkForData(model.FindNodeByKey(n), node, model);
                    }
                  }
                }
                if (newneighborkeys != null) {
                  foreach (Object n in newneighborkeys) {
                    if (oldneighborkeys == null || !ContainsKey(oldneighborkeys, n)) {
                      AddLinkForData(model.FindNodeByKey(n), node, model);
                    }
                  }
                }
              }
            }
            break;
          }
        case ModelChange.ChangedChildNodeKeys:
        case ModelChange.ChangedToNodeKeys: {
            Object node = e.Data;
            if (node != null) {
              IDiagramModel model = e.Model;
              if (model != null) {
                System.Collections.IEnumerable oldneighborkeys = e.OldValue as System.Collections.IEnumerable;
                System.Collections.IEnumerable newneighborkeys = e.NewValue as System.Collections.IEnumerable;
                if (oldneighborkeys != null && model != null) {
                  foreach (Object n in oldneighborkeys) {
                    if (newneighborkeys == null || !ContainsKey(newneighborkeys, n)) {
                      RemoveLinkForData(node, model.FindNodeByKey(n), model);
                    }
                  }
                }
                if (newneighborkeys != null && model != null) {
                  foreach (Object n in newneighborkeys) {
                    if (oldneighborkeys == null || !ContainsKey(oldneighborkeys, n)) {
                      AddLinkForData(node, model.FindNodeByKey(n), model);
                    }
                  }
                }
              }
            }
            break;
          }
        case ModelChange.ChangedGroupNodeKey: {
            IDiagramModel model = e.Model;
            if (model != null) {
              Node oldsg = FindNodeForData(model.FindNodeByKey(e.OldValue), model);
              if (oldsg != null) {
                oldsg.InvalidateRelationships("GroupNodeChanged");
              }
              Node newsg = FindNodeForData(model.FindNodeByKey(e.NewValue), model);
              if (newsg != null) {
                newsg.InvalidateRelationships("GroupNodeChanged");
                newsg.SortZOrder();
              }
              Node node = FindNodeForData(e.Data, model);
              if (node != null) {
                foreach (Link l in node.LinksConnected) UpdateCachedMembership(model, l);
                if (oldsg != null)
                  OnMemberRemoved(oldsg, node);
                else if (newsg != null)  // and oldsg == null
                  InvalidateDiagramLayout(node, LayoutChange.NodeRemoved);
                if (newsg != null)
                  OnMemberAdded(newsg, node);
                else if (oldsg != null)  // and newsg == null
                  InvalidateDiagramLayout(node, LayoutChange.NodeAdded);
              }
            }
            break;
          }
        case ModelChange.ChangedMemberNodeKeys: {
            Object node = e.Data;
            if (node != null) {
              IDiagramModel model = e.Model;
              if (model != null) {
                Node groupnode = FindNodeForData(node, model);
                System.Collections.IEnumerable oldmemberkeys = e.OldValue as System.Collections.IEnumerable;
                System.Collections.IEnumerable newmemberkeys = e.NewValue as System.Collections.IEnumerable;
                if (oldmemberkeys != null && model != null) {
                  foreach (Object mk in oldmemberkeys) {
                    if (newmemberkeys == null || !ContainsKey(newmemberkeys, mk)) {
                      Node oldmember = FindNodeForData(model.FindNodeByKey(mk), model);
                      if (oldmember != null) {
                        oldmember.InvalidateRelationships("MemberNodeChanged");
                        foreach (Link l in oldmember.LinksConnected) UpdateCachedMembership(model, l);
                        if (groupnode != null)
                          OnMemberRemoved(groupnode, oldmember);
                        else
                          InvalidateDiagramLayout(oldmember, LayoutChange.NodeRemoved);
                      }
                    }
                  }
                }
                if (newmemberkeys != null && model != null) {
                  foreach (Object n in newmemberkeys) {
                    if (oldmemberkeys == null || !ContainsKey(oldmemberkeys, n)) {
                      Node newmember = FindNodeForData(model.FindNodeByKey(n), model);
                      if (newmember != null) {
                        newmember.InvalidateRelationships("MemberNodeChanged");
                        foreach (Link l in newmember.LinksConnected) UpdateCachedMembership(model, l);
                        newmember.SortZOrder();
                        if (groupnode != null)
                          OnMemberAdded(groupnode, newmember);
                        else
                          InvalidateDiagramLayout(newmember, LayoutChange.NodeAdded);
                      }
                    }
                  }
                }
              }
            }
            break;
          }
        case ModelChange.AddedMemberNodeKey: {
            Object nkey = e.NewValue;
            IDiagramModel model = e.Model;
            if (model != null) {
              Node newmember = FindNodeForData(model.FindNodeByKey(nkey), model);
              if (newmember != null) {
                newmember.InvalidateRelationships("MemberNodeAdded");
                foreach (Link l in newmember.LinksConnected) UpdateCachedMembership(model, l);
                newmember.SortZOrder();
                Node groupnode = FindNodeForData(e.Data, model);
                if (groupnode != null)
                  OnMemberAdded(groupnode, newmember);
                else
                  InvalidateDiagramLayout(newmember, LayoutChange.NodeAdded);
              }
            }
            break;
          }
        case ModelChange.RemovedMemberNodeKey: {
            Object okey = e.OldValue;
            IDiagramModel model = e.Model;
            if (model != null) {
              Node oldmember = FindNodeForData(model.FindNodeByKey(okey), model);
              if (oldmember != null) {
                oldmember.InvalidateRelationships("MemberNodeRemoved");
                foreach (Link l in oldmember.LinksConnected) UpdateCachedMembership(model, l);
                Node groupnode = FindNodeForData(e.Data, model);
                if (groupnode != null)
                  OnMemberRemoved(groupnode, oldmember);
                else
                  InvalidateDiagramLayout(oldmember, LayoutChange.NodeRemoved);
              }
            }
            break;
          }

        case ModelChange.AddedLink:
          AddLinkForData(e.Data, e.Model);
          break;
        case ModelChange.RemovingLink:
          RemoveLinkForData(e.Data, e.Model);
          break;
        case ModelChange.RemovedLink:
          RemoveLinkForData(e.Data, e.Model);
          break;
        case ModelChange.ChangedLinkFromPort:
        case ModelChange.ChangedLinkToPort: {
            RemoveLinkForData(e.Data, e.Model);
            AddLinkForData(e.Data, e.Model);
            break;
          }
        case ModelChange.ChangedLinkGroupNodeKey: {
            Link link = FindLinkForData(e.Data, e.Model);
            if (link != null) link.InvalidateRelationships("LinkGroupChanged");
            IDiagramModel model = e.Model;
            if (model != null) {
              Node oldsg = FindNodeForData(model.FindNodeByKey(e.OldValue), model);
              if (oldsg != null) oldsg.InvalidateRelationships("LinkGroupChanged");
              Node newsg = FindNodeForData(model.FindNodeByKey(e.NewValue), model);
              if (newsg != null) {
                Group sg = newsg as Group;
                if (sg != null && link != null) link.Visible = sg.IsExpandedSubGraph;
                newsg.InvalidateRelationships("LinkGroupChanged");
              }
            }
            break;
          }
        case ModelChange.ChangedLinkLabelKey: {
            Link link = FindLinkForData(e.Data, e.Model);
            if (link != null) link.InvalidateRelationships("LinkLabelChanged");
            break;
          }
        case ModelChange.ChangedNodeKey: {
            Node n = FindNodeForData(e.Data, e.Model);
            if (n != null) {
              n.InvalidateRelationships("NodeKeyChanged");
            }
            break;
          }
        case ModelChange.ChangedNodeCategory: {
            Object nodedata = e.Data;
            IDiagramModel model = e.Model;
            Node node = FindNodeForData(nodedata, model);
            if (node != null) {
              bool wasselected = node.IsSelected;
              RemoveNodeForData(nodedata, model);
              AddNodeForData(nodedata, model);
              if (wasselected) {
                Node newnode = FindNodeForData(nodedata, model);
                if (newnode != null) newnode.IsSelected = wasselected;
              }
            }
            break;
          }
        case ModelChange.ChangedLinkCategory: {
            Object linkdata = e.Data;
            IDiagramModel model = e.Model;
            Link link = FindLinkForData(linkdata, model);
            if (link != null) {
              bool wasselected = link.IsSelected;
              RemoveLinkForData(linkdata, model);
              AddLinkForData(linkdata, model);
              if (wasselected) {
                Link newlink = FindLinkForData(linkdata, model);
                if (newlink != null) newlink.IsSelected = wasselected;
              }
            }
            break;
          }
        case ModelChange.InvalidateRelationships: {
            Node node = FindNodeForData(e.Data, e.Model);
            if (node != null) {
              node.Remeasure();
              Diagram.InvokeLater(this.Diagram, () => { node.InvalidateRelationships("ChangedNodeGeometry"); });
            }
            break;
          }

        // model discovery
        case ModelChange.ChangedNodesSource: {
            Diagram diagram = this.Diagram;
            IDiagramModel model = e.Model;
            if (diagram != null && model != null) {
              diagram.NodesSource = model.NodesSource;
            }
            RebuildNodeElements();  // also calls RebuildLinkElements
            if (diagram != null && model != null && !model.IsChangingModel) {
              diagram.RelayoutDiagram();
            }
            break;
          }
        case ModelChange.ChangedNodeKeyPath:
        case ModelChange.ChangedNodeCategoryPath:
        case ModelChange.ChangedNodeIsGroupPath:
        case ModelChange.ChangedGroupNodePath:
        case ModelChange.ChangedMemberNodesPath:
        case ModelChange.ChangedNodeIsLinkLabelPath: {
            RebuildNodeElements();  // also calls RebuildLinkElements
            Diagram diagram = this.Diagram;
            IDiagramModel model = e.Model;
            if (diagram != null && model != null && !model.IsChangingModel) {
              diagram.RelayoutDiagram();
            }
            break;
          }
        case ModelChange.ChangedLinksSource: {
            Diagram diagram = this.Diagram;
            IDiagramModel model = e.Model;
            if (diagram != null && model != null) {
              ILinksModel lmodel = model as ILinksModel;
              if (lmodel != null) {
                diagram.LinksSource = lmodel.LinksSource;
              }
            }
            RebuildLinkElements();
            if (diagram != null && model != null && !model.IsChangingModel) {
              diagram.RelayoutDiagram();
            }
            break;
          }
        case ModelChange.ChangedLinkFromPath:
        case ModelChange.ChangedLinkToPath:
        case ModelChange.ChangedFromNodesPath:
        case ModelChange.ChangedToNodesPath:
        case ModelChange.ChangedLinkLabelNodePath: {
            RebuildLinkElements();
            Diagram diagram = this.Diagram;
            IDiagramModel model = e.Model;
            if (diagram != null && model != null && !model.IsChangingModel) {
              diagram.RelayoutDiagram();
            }
            break;
          }
        //case ModelChange.ChangedParentPortParameterPath:
        //case ModelChange.ChangedChildPortParameterPath:
        case ModelChange.ChangedLinkFromParameterPath:
        case ModelChange.ChangedLinkToParameterPath:
        case ModelChange.ChangedLinkCategoryPath:
          RebuildLinkElements();
          break;

        case ModelChange.StartedTransaction:
          //?? produce routed event
          break;
        case ModelChange.CommittedTransaction:
          //?? produce routed event
          String reason = e.Data as String;
          if (reason != "Layout") {  // check for this case to avoid infinite loop
            Diagram diagram = this.Diagram;
            if (diagram == null) break;
            IDiagramModel model = diagram.Model;
            if (model != null) model.ClearUnresolvedReferences();
            DiagramPanel panel = diagram.Panel;
            if (panel != null) panel.UpdateScrollTransform();
            LayoutManager laymgr = diagram.LayoutManager;
            if (laymgr != null) laymgr.InvokeLayoutDiagram("CommittedTransaction");
            diagram.UpdateCommands();
          }
          break;
        case ModelChange.RolledBackTransaction: {
            //?? produce routed event
            Diagram diagram = this.Diagram;
            if (diagram == null) break;
            IDiagramModel model = diagram.Model;
            if (model != null) model.ClearUnresolvedReferences();
            diagram.UpdateCommands();
            break;
          }

        case ModelChange.StartingUndo:
        case ModelChange.StartingRedo:
          break;
        case ModelChange.FinishedUndo:
        case ModelChange.FinishedRedo: {
            //?? produce routed event
            Diagram diagram = this.Diagram;
            if (diagram == null) break;
            DiagramPanel panel = diagram.Panel;
            if (panel == null) break;
            // force everything to be remeasured
            foreach (UIElement elt in panel.Children) {
              NodeLayer nlay = elt as NodeLayer;
              if (nlay != null) {
                if (nlay.IsTemporary) continue;
                foreach (Node n in nlay.Nodes) {
                  n.Remeasure();
                }
              } else {
                LinkLayer llay = elt as LinkLayer;
                if (llay != null) {
                  if (llay.IsTemporary) continue;
                  foreach (Link l in llay.Links) {
                    l.Remeasure();
                  }
                }
              }
            }
            panel.UpdateScrollTransform();
            panel.InvokeUpdateDiagramBounds("Undo/Redo");
            diagram.UpdateCommands();
            break;
          }

        // model state
        case ModelChange.ChangedName:
        case ModelChange.ChangedDataFormat:
        case ModelChange.ChangedModifiable:
        case ModelChange.ChangedValidCycle: {
            Diagram diagram = this.Diagram;
            if (diagram == null) break;
            diagram.UpdateCommands();
            break;
          }
        case ModelChange.ChangedCopyingGroupCopiesMembers:
        case ModelChange.ChangedCopyingLinkCopiesLabel:
        case ModelChange.ChangedRemovingGroupRemovesMembers:
        case ModelChange.ChangedRemovingLinkRemovesLabel:
          break;
        default:
          Diagram.Error("Diagram did not handle Model change: " + e.Change.ToString());
          break;
      }
    }

    private bool ContainsKey(System.Collections.IEnumerable coll, Object nodekey) {
      foreach (Object d in coll) {
        if (d == nodekey) return true;
      }
      return false;
    }


    internal bool TemporaryParts { get; set; }

    // nodes

    // mapping node data to Node objects
    private Dictionary<Object, Node> _NodesDictionary;
    private HashSet<Node> _Nodes;

    /// <summary>
    /// Gets an <c>IEnumerable&lt;Node&gt;</c> holding all of the <see cref="Node"/>s
    /// that are in the diagram, including nodes that are not bound to data, but
    /// excluding <see cref="Adornment"/>s.
    /// </summary>
    /// <remarks>
    /// If you want to add a <see cref="Node"/> to the diagram,
    /// call <see cref="AddNodeForData"/>.
    /// </remarks>
    public IEnumerable<Node> Nodes {
      get { return _Nodes; }
    }

    /// <summary>
    /// Gets the current number of <see cref="Node"/>s in the diagram,
    /// including nodes that are not bound to data and including <see cref="Group"/>s,
    /// but excluding <see cref="Adornment"/>s.
    /// </summary>
    public int NodesCount {
      get { return _Nodes.Count; }
    }

    /// <summary>
    /// Discard all existing <see cref="Node"/>s and <see cref="Link"/>s and then
    /// make new ones for all of model data and add them to the <see cref="Diagram"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This deletes all existing <see cref="Part"/>s from the diagram and then
    /// calls <see cref="DoRebuildNodeElements"/>.
    /// </para>
    /// <para>
    /// The implementation of this method and the methods that it calls should not modify the model.
    /// </para>
    /// </remarks>
    public void RebuildNodeElements() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Initializing) return;
      bool oldchecks = this.ChecksTransactionLevel;
      this.ChecksTransactionLevel = false;
      LayoutManager laymgr = diagram.LayoutManager;
      bool oldskips = false;
      if (laymgr != null) {
        oldskips = laymgr.SkipsInvalidate;
        laymgr.SkipsInvalidate = true;
      }
      List<Object> selecteddata = new List<Object>();

      try {
        diagram.Cursor = Cursors.Wait;
        // remove all parts from SelectedParts
        // first, remember their data, so we can re-select them later
        foreach (Part p in diagram.SelectedParts) {
          if (p.IsBoundToData)
            selecteddata.Add(p.Data);
          else
            selecteddata.Add(p);
        }
        diagram.ClearSelection();

        // remove all parts from Panel
        DiagramPanel panel = diagram.Panel;
        if (panel == null) return;

        //DateTime beforeclear = DateTime.Now;
        panel.ClearAll();
        //DateTime afterclear = DateTime.Now;
        //double clear = (afterclear-beforeclear).TotalMilliseconds;

        ClearAllNodes();

        DoRebuildNodeElements();
        //DateTime afterbuild = DateTime.Now;
        //double build = (afterbuild-afterclear).TotalMilliseconds;

        panel.SortZOrder();
        //DateTime aftersort = DateTime.Now;
        //double sort = (aftersort-afterbuild).TotalMilliseconds;
        //Diagram.Debug("buildnodes: " + Diagram.Str(clear) + " " + Diagram.Str(build) + " " + Diagram.Str(sort));

        RebuildLinkElements();
        //Diagram.Debug("RebuildNodeElements for " + this.NodesCount.ToString() + " nodes: " + (DateTime.Now-beforeclear).TotalMilliseconds.ToString());
      } finally {
        // restore the Diagram.SelectedParts collection
        foreach (Object x in selecteddata) {
          Node n = x as Node;
          if (n != null && diagram.PartsModel.IsNodeData(n)) {
            n.IsSelected = true;
          } else {
            Link l = x as Link;
            if (l != null && diagram.PartsModel.IsLinkData(l)) {
              l.IsSelected = true;
            } else if (diagram.Model.IsNodeData(x)) {
              n = FindNodeForData(x, diagram.Model);
              if (n != null) n.IsSelected = true;
            } else {
              l = FindLinkForData(x, diagram.Model);
              if (l != null) l.IsSelected = true;
            }
          }
        }
        diagram.Cursor = null;
        this.ChecksTransactionLevel = oldchecks;
        if (laymgr != null && !oldskips) {
          // invoke later because changes such as LayoutChange.NodeSizeChanged may occur asynchronously
          Diagram.InvokeLater(diagram, () => { laymgr.SkipsInvalidate = false; });
        }
      }
    }

    /// <summary>
    /// Make new <see cref="Node"/>s for all of model data and add them to the <see cref="Diagram"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will call <see cref="AddNodeForData"/> for each of the node data objects in the model.
    /// It also calls <see cref="AddNodeForData"/> for each <see cref="Node"/> that is in
    /// the <see cref="Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.PartsModel"/>.
    /// </para>
    /// <para>
    /// This is called by <see cref="RebuildNodeElements"/>.
    /// This also calls <see cref="RebuildLinkElements"/>.
    /// </para>
    /// <para>
    /// The implementation of this method and the methods that it calls should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual void DoRebuildNodeElements() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;

      IDiagramModel model = diagram.Model;
      PartsModel pmodel = diagram.PartsModel;
      if (pmodel != null && pmodel != model && pmodel.NodesSource != null) {
        foreach (Object nodedata in pmodel.NodesSource) {
          AddNodeForData(nodedata, pmodel);
        }
      }

      if (model != null && model.NodesSource != null) {
        foreach (Object nodedata in model.NodesSource) {
          AddNodeForData(nodedata, model);
        }
      }
    }


    /// <summary>
    /// Make sure a <see cref="Node"/> exists for some data in the model, added to the <see cref="NodeLayer"/>
    /// specified by its <see cref="Part.LayerName"/>.
    /// </summary>
    /// <param name="nodedata">the data in the <paramref name="model"/> that holds node information</param>
    /// <param name="model">the model that the <paramref name="nodedata"/> is in</param>
    /// <returns>
    /// the <see cref="Node"/> for the <paramref name="nodedata"/>,
    /// either an existing one or a newly created one,
    /// or null if <see cref="FilterNodeForData"/> returns false or if the diagram is uninitialized
    /// </returns>
    /// <remarks>
    /// <para>
    /// If you want to add a node to your diagram programmatically, don't call this method,
    /// but create a node data object and add it to the model's <see cref="IDiagramModel.NodesSource"/>,
    /// either directly to that collection or by calling <see cref="IDiagramModel.AddNode"/>.
    /// </para>
    /// <para>
    /// If the diagram already has a <see cref="Node"/> for the data, as determined by
    /// <see cref="FindNodeForData"/>, or if <see cref="FilterNodeForData"/> is false,
    /// this method will do nothing.
    /// </para>
    /// <para>
    /// For bound data this will call <see cref="MakeNodeForData"/>, passing in whether the
    /// the data represents a group node or a link label node and what category of node it is.
    /// </para>
    /// <para>
    /// If the <paramref name="nodedata"/> is a <see cref="Node"/> and if
    /// the <paramref name="model"/> is a <see cref="Northwoods.GoXam.Model.PartsModel"/>,
    /// this method doesn't need to construct a new node by calling <see cref="MakeNodeForData"/>,
    /// but can just add the node to the diagram, in the appropriate <see cref="Layer"/>
    /// determined by the <see cref="Part.LayerName"/>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public Node AddNodeForData(Object nodedata, IDiagramModel model) {
      if (nodedata == null) return null;
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return null;
      Node oldnode = FindNodeForData(nodedata, model);
      if (oldnode != null) return oldnode;
      if (!FilterNodeForData(nodedata, model)) return null;

      Node newnode;
      if (model is PartsModel && nodedata is Node) {
        newnode = (Node)nodedata;  // check if unbound: data is itself the Node
      } else {  // no, just regular data, gotta make a Node from binding the data with a DataTemplate
        ILinksModel lmodel = model as ILinksModel;
        IGroupsModel gmodel = model as IGroupsModel;
        bool islinklabel = (lmodel != null && lmodel.GetIsLinkLabelForNode(nodedata));
        bool isgroup = (gmodel != null && gmodel.GetIsGroupForNode(nodedata));
        String category = FindCategoryForNode(nodedata, model, isgroup, islinklabel);
        if (category == null) category = "";
        DataTemplate template = FindTemplateForNode(nodedata, model, isgroup, islinklabel, category);
        newnode = MakeNodeForData(nodedata, model, isgroup, islinklabel, category, template);
        VerifyTransaction(nodedata, model);
      }
      if (newnode != null) {
        newnode.Model = model;
        // only regular Nodes get added to the collection of Nodes
        if (!(newnode is Adornment)) {
          _NodesDictionary[nodedata] = newnode;
          _Nodes.Add(newnode);
        }
        // add to a temporary layer so that ApplyTemplate will be successful
        // so that any attached properties become available on the Part's VisualElement
        NodeLayer templayer = panel.GetLayer<NodeLayer>(DiagramPanel.ToolLayerName);
        if (templayer != null) templayer.Add(newnode);
        newnode.ApplyTemplate();  //?? virtualization
        //?? LayerName might not be correct value until after ApplyTemplate
        String layername = newnode.LayerName;
        NodeLayer layer = (this.TemporaryParts ? templayer : panel.GetLayer<NodeLayer>(layername));
        if (layer != null) {
          layer.Add(newnode);
          //?? AddLogicalChild if nodedata is a Node
          OnNodeAdded(newnode);
        }
      }
      return newnode;
    }


    /// <summary>
    /// Gets or sets whether this part manager will output warnings to Trace listeners
    /// when model changes occur outside of a transaction.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// Setting this property will also set the <see cref="Northwoods.GoXam.Model.UndoManager.ChecksTransactionLevel"/>
    /// property, if the <see cref="Diagram"/>'s model has an undo manager.
    /// </remarks>
    public bool ChecksTransactionLevel {
      get { return _ChecksTransactionLevel; }
      set {
        _ChecksTransactionLevel = value;
        if (this.Diagram != null && this.Diagram.Model != null) {
          UndoManager undomgr = this.Diagram.Model.UndoManager;
          if (undomgr != null) undomgr.ChecksTransactionLevel = value;
        }
      }
    }
    private bool _ChecksTransactionLevel = true;

    private void VerifyTransaction(Object data, IDiagramModel mdl) {
      if (!this.ChecksTransactionLevel) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      DiagramModel model = diagram.Model as DiagramModel;
      if (mdl != model) return;
      if (panel.InitialLayoutCompleted) {
        if (model != null && model.UndoManager == null && model.NoUndoManagerTransactionLevel == 0) {
          Diagram.Trace("Model changes should occur within a transaction -- call StartTransation and CommitTransaction" + (data != null ? "\n  relevant data: " + data.ToString() : ""));
        }
      }
    }


    /// <summary>
    /// Given some <paramref name="nodedata"/> in a <paramref name="model"/>,
    /// find the corresponding <see cref="Node"/> in this diagram.
    /// </summary>
    /// <param name="nodedata">the data in the <paramref name="model"/> that holds node information</param>
    /// <param name="model">the model that the <paramref name="nodedata"/> is in</param>
    /// <returns>a <see cref="Node"/>, or null if such a node has not been created for that data</returns>
    public Node FindNodeForData(Object nodedata, IDiagramModel model) {
      if (nodedata == null) return null;
      VerifyAccess();
      Node node;
      _NodesDictionary.TryGetValue(nodedata, out node);
      return node;
    }

    /// <summary>
    /// Decide whether a particular <paramref name="nodedata"/> should be represented
    /// in the diagram by a <see cref="Node"/>.
    /// </summary>
    /// <param name="nodedata">the data in the <paramref name="model"/> that holds node information</param>
    /// <param name="model">the model that the <paramref name="nodedata"/> is in</param>
    /// <returns>true only if <see cref="AddNodeForData"/> should make and add a <see cref="Node"/> to the diagram</returns>
    /// <remarks>
    /// <para>
    /// Normally, this return true.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual bool FilterNodeForData(Object nodedata, IDiagramModel model) {
      if (nodedata is Node) return true;
      if (model == null) return true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return true;
      bool poptree = (diagram.Filter & PartManagerFilter.CollapsedTreeChildren) != 0;
      bool popgroup = (diagram.Filter & PartManagerFilter.CollapsedSubGraphMembers) != 0;
      if (poptree) {
        ISubGraphModel sgmodel = model as ISubGraphModel;
        Object container = (sgmodel != null ? sgmodel.GetGroupForNode(nodedata) : null);
        // look for nodes with links coming into NODEDATA
        bool foundfrom = false;
        foreach (Object neighbor in model.GetFromNodesForNode(nodedata)) {
          // exclude links from self and from nodes in other groups
          if (neighbor == nodedata) continue;
          if (sgmodel != null && sgmodel.GetGroupForNode(neighbor) != container) continue;  //??? wrong check
          Node node = FindNodeForData(neighbor, model);
          // if there's a from node that is tree-expanded, make the Node now
          if (node != null && node.IsExpandedTree) return true;
          foundfrom = true;
        }
        // always create root nodes -- nodes that don't have any from nodes connected to it
        if (!foundfrom) return true;
      }
      if (popgroup) {
        ISubGraphModel sgmodel = diagram.Model as ISubGraphModel;
        if (sgmodel == null) return true;
        Object parent = sgmodel.GetGroupForNode(nodedata);
        if (parent == null) return true;  // OK if top-level
        // if there is a parent in the model,
        Group group = FindNodeForData(parent, model) as Group;
        // if parent group exists and it is expanded, make Node now
        if (group != null && group.IsExpandedSubGraph) return true;
      }
      return (!poptree && !popgroup);
    }


    /// <summary>
    /// Determine the category for the node, to be able to decide between different templates for the <see cref="Node"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="model"></param>
    /// <param name="isgroup"></param>
    /// <param name="islinklabel"></param>
    /// <returns>
    /// the result of calling <see cref="Northwoods.GoXam.Model.IDiagramModel.GetCategoryForNode"/>
    /// </returns>
    /// <remarks>
    /// The result is used in a call to <see cref="FindTemplateForNode"/>.
    /// </remarks>
    protected virtual String FindCategoryForNode(Object nodedata, IDiagramModel model, bool isgroup, bool islinklabel) {
      if (model == null) return "";
      return model.GetCategoryForNode(nodedata);
    }

    /// <summary>
    /// Given a category and the node data, find a <c>DataTemplate</c> to use in making the <see cref="Node"/> or <see cref="Group"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="model"></param>
    /// <param name="isgroup"></param>
    /// <param name="islinklabel"></param>
    /// <param name="category"></param>
    /// <returns>
    /// a <c>DataTemplate</c> taken from either <see cref="Northwoods.GoXam.Diagram.NodeTemplate"/> or <see cref="Northwoods.GoXam.Diagram.NodeTemplateDictionary"/>
    /// (or if <paramref name="isgroup"/> is true,
    /// from either <see cref="Northwoods.GoXam.Diagram.GroupTemplate"/> or <see cref="Northwoods.GoXam.Diagram.GroupTemplateDictionary"/>),
    /// or null if no template is found
    /// </returns>
    /// <remarks>
    /// <para>
    /// The search for a data template looks in different places depending on the value of <paramref name="isgroup"/>.
    /// For the default category (the empty string ""),
    /// this uses <see cref="Northwoods.GoXam.Diagram.NodeTemplate"/> (or <see cref="Northwoods.GoXam.Diagram.GroupTemplate"/>) if it is non-null,
    /// or else the default entry ("") in the <see cref="Northwoods.GoXam.Diagram.NodeTemplateDictionary"/>
    /// (or <see cref="Northwoods.GoXam.Diagram.GroupTemplateDictionary"/>) if there is a dictionary.
    /// For any other category,
    /// this looks up the category in the <see cref="Northwoods.GoXam.Diagram.NodeTemplateDictionary"/>
    /// (or <see cref="Northwoods.GoXam.Diagram.GroupTemplateDictionary"/>) if there is a dictionary,
    /// and if that fails it uses <see cref="Northwoods.GoXam.Diagram.NodeTemplate"/>
    /// (or <see cref="Northwoods.GoXam.Diagram.GroupTemplate"/>) or the
    /// default entry in the dictionary.
    /// This may return null if no appropriate template is found for the category.
    /// The result is used in a call to <see cref="MakeNodeForData"/>.
    /// </para>
    /// <para>
    /// You may wish to override this method to customize the selection of the template based on the link data.
    /// </para>
    /// </remarks>
    protected virtual DataTemplate FindTemplateForNode(Object nodedata, IDiagramModel model, bool isgroup, bool islinklabel, String category) {
      Diagram diagram = this.Diagram;
      // try defaulting the category for link label nodes
      if (category == "" && islinklabel) {
        category = "LinkLabel";
      }
      // for the default category, "", use NodeTemplate/GroupTemplate and then the dictionary;
      // for other categories, use the dictionary, then NodeTemplate/GroupTemplate, then "" in dictionary
      DataTemplate template = null;
      if (isgroup) {
        if (category == "") template = diagram.GroupTemplate;
        if (template == null) {
          DataTemplateDictionary dict = diagram.GroupTemplateDictionary;
          if (dict != null) {
            dict.TryGetValue(category, out template);
            // but if the template is not found, use the default one, if it's not a link label
            // (note that if category == "", we've already searched)
            if (template == null && category != "" && !islinklabel) {
              template = diagram.GroupTemplate;
              if (template == null) dict.TryGetValue("", out template);
            }
          }
        }
      } else {
        if (category == "") template = diagram.NodeTemplate;
        if (template == null) {
          DataTemplateDictionary dict = diagram.NodeTemplateDictionary;
          if (dict != null) {
            dict.TryGetValue(category, out template);
            // but if the template is not found, use the default one, if it's not a link label
            // (note that if category == "", we've already searched)
            if (template == null && category != "" && !islinklabel) {
              template = diagram.NodeTemplate;
              if (template == null) dict.TryGetValue("", out template);
            }
          }
        }
      }
      return template;
    }

    /// <summary>
    /// Construct a new <see cref="Node"/>, setting its <c>Content</c> and <c>ContentTemplate</c> properties.
    /// </summary>
    /// <param name="nodedata">the data that this node is bound to; must not be a <see cref="Node"/> or any <c>UIElement</c></param>
    /// <param name="model">the model that the <paramref name="nodedata"/> is in</param>
    /// <param name="isgroup">whether the node should be a <see cref="Group"/></param>
    /// <param name="islinklabel">whether the node acts as the label node for a link</param>
    /// <param name="category">the category of the node</param>
    /// <param name="templ">the <c>DataTemplate</c> for the <c>ContentTemplate</c> property</param>
    /// <returns>a newly created <see cref="Node"/> or <see cref="Group"/>,
    /// bound to <paramref name="nodedata"/> via a <see cref="PartBinding"/></returns>
    /// <remarks>
    /// <para>
    /// You may wish to override this method in order to customize the <c>DataTemplate</c> used
    /// for the particular <paramref name="nodedata"/>.
    /// Less frequently you will want to override this method to construct a subclass of <see cref="Node"/>
    /// where you have overridden one of the node methods, such as <see cref="Node.FindPort"/>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual Node MakeNodeForData(Object nodedata, IDiagramModel model, bool isgroup, bool islinklabel, String category, DataTemplate templ) {
      Node node = (isgroup ? new Group() : new Node());  // for PartManager.MakeNodeForData
      PartBinding data = new PartBinding(node, nodedata);
      node.Content = data;
      node.DataContext = data;
      node.ContentTemplate = templ;  // for PartManager.MakeNodeForData
      node.IsLinkLabel = islinklabel;
      if (category != null && category != "") node.Category = category;
      return node;
    }

    /// <summary>
    /// Called after a <paramref name="node"/> is added to the diagram.
    /// </summary>
    /// <param name="node">a <see cref="Node"/></param>
    /// <remarks>
    /// <para>
    /// For models that are not <see cref="Northwoods.GoXam.Model.ILinksModel"/>,
    /// this will add any <see cref="Link"/>s that should be connected to the new <paramref name="node"/>.
    /// For models that are <see cref="Northwoods.GoXam.Model.ISubGraphModel"/>,
    /// this checks for any <see cref="Group"/> containing this new node is not <see cref="Group.IsExpandedSubGraph"/> --
    /// if so this node is made <c>Visibility.Collapsed</c>.
    /// </para>
    /// <para>
    /// Adding a node will request a new automatic layout if it does not have a <see cref="Node.Location"/>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the addition of the node to the diagram.
    /// </para>
    /// </remarks>
    protected virtual void OnNodeAdded(Node node) {
      if (node == null) return;
      Object nodedata = node.Data;
      IDiagramModel model = node.Model;
      ITreeModel tmodel = model as ITreeModel;
      if (tmodel != null) {
        Object parentdata = tmodel.GetParentForNode(nodedata);
        if (parentdata != null) {
          // create link from parent to this new node
          AddLinkForData(parentdata, nodedata, model);
        }
      } else if (model is IConnectedModel) {
        // create links to this new node
        foreach (Object n in model.GetFromNodesForNode(nodedata)) {
          AddLinkForData(n, nodedata, model);
        }
        // create links from this new node
        foreach (Object n in model.GetToNodesForNode(nodedata)) {
          AddLinkForData(nodedata, n, model);
        }
      } // don't need to automatically create links for ILinksModel when nodes get added

      // deal with subgraph visibility
      ISubGraphModel sgmodel = model as ISubGraphModel;
      if (sgmodel != null) {
        // maybe member of a collapsed subgraph -- make the node not visible
        Group containernode = node.ContainingSubGraph;
        if (containernode != null && node.Visible && (!containernode.IsExpandedSubGraph || !containernode.Visible)) {
          node.Visible = false;
          // this propagates to connected links
        }
        // maybe this is a collapsed subgraph -- make sure members are not visible
        Group group = node as Group;
        if (group != null && !group.IsExpandedSubGraph) {
          foreach (Node n in group.MemberNodes) n.Visible = false;
          foreach (Link l in group.MemberLinks) l.Visible = false;
        }
      }

      // deal with tree visibility
      if (node.Visible) {
        // maybe this is a child of a collapsed tree node
        foreach (Node parentnode in node.NodesInto) {
          if (!parentnode.IsExpandedTree) {
            node.Visible = false;
            // this propagates to connected links
            break;
          }
        }
      }

      // maybe this is a collapsed tree node -- make sure children are not visible
      if (!node.IsExpandedTree) {
        foreach (Link childlink in node.LinksOutOf) {
          Node other = childlink.GetOtherNode(node);
          if (other != null && other != node) {
            other.Visible = false;
            // this propagates to connected links
          }
        }
      }

      node.InvalidateLayout(LayoutChange.NodeAdded);
    }

    /// <summary>
    /// Called before a <paramref name="node"/> is removed from the diagram.
    /// </summary>
    /// <param name="node">a <see cref="Node"/></param>
    /// <remarks>
    /// <para>
    /// This remove all links that are connected to the <paramref name="node"/>.
    /// </para>
    /// <para>
    /// Removing a node will request a new automatic layout if it has a <see cref="Node.Location"/>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the removal of the node from the diagram.
    /// </para>
    /// </remarks>
    protected virtual void OnNodeRemoving(Node node) {
      if (node == null) return;
      Object nodedata = node.Data;
      IDiagramModel model = node.Model;
      if (model != null && !(model is ILinksModel)) {
        foreach (Object n in model.GetConnectedNodesForNode(nodedata).ToList()) {
          RemoveLinkForData(nodedata, n, model);
          RemoveLinkForData(n, nodedata, model);
        }
      }
      foreach (Group g in node.ContainingGroups) {
        g.Remeasure();
      }
      node.InvalidateLayout(LayoutChange.NodeRemoved);
    }

    /// <summary>
    /// Called after a <paramref name="node"/> is removed from the diagram.
    /// </summary>
    /// <param name="node">a <see cref="Node"/></param>
    /// <param name="layer">the <see cref="NodeLayer"/> that the <paramref name="node"/> was removed from</param>
    /// <remarks>
    /// <para>
    /// By default this method does nothing.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the removal of the node from the diagram.
    /// </para>
    /// </remarks>
    protected virtual void OnNodeRemoved(Node node, NodeLayer layer) {
    }


    /// <summary>
    /// Remove any <see cref="Node"/> in this diagram that exists for the <paramref name="nodedata"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="model"></param>
    /// <remarks>
    /// <para>
    /// This calls <see cref="OnNodeRemoving"/> before the node is removed from its layer,
    /// and <see cref="OnNodeRemoved"/> afterwards.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the removal of the node from the diagram.
    /// </para>
    /// </remarks>
    public void RemoveNodeForData(Object nodedata, IDiagramModel model) {
      if (nodedata == null) return;
      VerifyAccess();
      Node node = FindNodeForData(nodedata, model);
      if (node != null) {
        VerifyTransaction(nodedata, model);
        node.IsSelected = false;
        OnNodeRemoving(node);
        // update collection and visuals
        if (!(node is Adornment)) {
          _NodesDictionary.Remove(nodedata);
          _Nodes.Remove(node);
        }
        NodeLayer nlayer = node.Layer as NodeLayer;
        if (nlayer != null) nlayer.Remove(node);
        OnNodeRemoved(node, nlayer);
        ClearDataBinding(node);
      }
    }


    // links

    // mapping link data to Link objects
    private Dictionary<Object, Link> _LinksDictionary;
    private HashSet<Link> _Links;

    /// <summary>
    /// Gets a <c>IEnumerable&lt;Link&gt;</c> holding all of the <see cref="Link"/>s
    /// that are in the diagram, including links that are not bound to data.
    /// </summary>
    /// <remarks>
    /// If you want to add a <see cref="Link"/> to the diagram,
    /// call <see cref="AddLinkForData(Object, IDiagramModel)"/> or
    /// or <see cref="AddLinkForData(Object, Object, IDiagramModel)"/>.
    /// </remarks>
    public IEnumerable<Link> Links {
      get { return _Links; }
    }

    /// <summary>
    /// Gets the current number of <see cref="Link"/>s in the diagram,
    /// including links that are not bound to data.
    /// </summary>
    public int LinksCount {
      get { return _Links.Count; }
    }

    /// <summary>
    /// Discard all existing <see cref="Link"/>s and then
    /// make new ones for all of model data and add them to the <see cref="Diagram"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This removes all of the old <see cref="Link"/>s and then calls
    /// <see cref="DoRebuildLinkElements"/>.
    /// </para>
    /// <para>
    /// This does not delete existing <see cref="Node"/>s.
    /// This method is also called by <see cref="RebuildNodeElements"/>.
    /// </para>
    /// <para>
    /// The implementation of this method and the methods that it calls should not modify the model.
    /// </para>
    /// </remarks>
    public void RebuildLinkElements() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Initializing) return;
      bool oldchecks = this.ChecksTransactionLevel;
      this.ChecksTransactionLevel = false;
      LayoutManager laymgr = diagram.LayoutManager;
      bool oldskips = false;
      if (laymgr != null) {
        oldskips = laymgr.SkipsInvalidate;
        laymgr.SkipsInvalidate = true;
      }
      List<Object> selecteddata = new List<Object>();

      try {
        // remove all links from SelectedParts
        if (diagram.SelectedParts.Count > 0) {  // will be zero if called from RebuildNodeElements
          // first, remember their data, so we can re-select them later
          List<Link> sellinks = diagram.SelectedParts.OfType<Link>().ToList();
          foreach (Link link in sellinks) {
            if (link.IsBoundToData)
              selecteddata.Add(link.Data);
            else
              selecteddata.Add(link);
            link.IsSelected = false;
            link.ClearAdornments();  // remove any Adornment nodes
          }
        }

        // remove all links (but not nodes)
        DiagramPanel panel = diagram.Panel;
        if (panel == null) return;

        //DateTime beforeclear = DateTime.Now;
        panel.ClearLinks();
        foreach (Link l in _Links) RemoveCachedMembership(l);
        //DateTime afterclear = DateTime.Now;
        //double clear = (afterclear-beforeclear).TotalMilliseconds;

        ClearAllLinks();

        DoRebuildLinkElements();
      } finally {
        // restore selected links
        foreach (Object x in selecteddata) {
          Link l = x as Link;
          if (l != null && diagram.PartsModel.IsLinkData(l)) {
            l.IsSelected = true;
          } else {
            l = FindLinkForData(x, diagram.Model);
            if (l != null) l.IsSelected = true;
          }
        }
        this.ChecksTransactionLevel = oldchecks;
        if (laymgr != null && !oldskips) {
          // invoke later because changes such as LayoutChange.NodeSizeChanged may occur asynchronously
          Diagram.InvokeLater(diagram, () => { laymgr.SkipsInvalidate = false; });
        }
      }

      //DateTime afterbuild = DateTime.Now;
      //double build = (afterbuild-afterclear).TotalMilliseconds;
      //Diagram.Debug("buildlinks: " + Diagram.Str(clear) + " " + Diagram.Str(build));
    }

    /// <summary>
    /// This is called by <see cref="RebuildLinkElements"/> to construct or
    /// reconstruct <see cref="Link"/>s in the diagram.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will call <see cref="AddLinkForData(Object, IDiagramModel)"/>
    /// for each of the link data objects in the model, whether explicit
    /// link data in an <see cref="Northwoods.GoXam.Model.ILinksModel"/>,
    /// or implicit for each node in an <see cref="Northwoods.GoXam.Model.ITreeModel"/>
    /// or an <see cref="Northwoods.GoXam.Model.IConnectedModel"/>.
    /// It also calls <see cref="AddLinkForData(Object, IDiagramModel)"/>
    /// for each <see cref="Link"/> that is in
    /// the <see cref="Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.PartsModel"/>.
    /// </para>
    /// <para>
    /// The implementation of this method and the methods that it calls should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual void DoRebuildLinkElements() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      PartsModel pmodel = diagram.PartsModel;
      if (pmodel != null && pmodel != model && pmodel.LinksSource != null) {
        foreach (Object linkdata in pmodel.LinksSource) {
          AddLinkForData(linkdata, pmodel);
        }
      }

      ILinksModel lmodel = model as ILinksModel;
      if (lmodel != null && lmodel.LinksSource != null) {
        foreach (Object linkdata in lmodel.LinksSource) {
          AddLinkForData(linkdata, model);
        }
      } else if (model != null && model.NodesSource != null) {
        ITreeModel tmodel = model as ITreeModel;
        if (tmodel != null) {
          foreach (Object nodedata in model.NodesSource) {
            Object parentdata = tmodel.GetParentForNode(nodedata);
            if (parentdata != null) {
              AddLinkForData(parentdata, nodedata, model);
            }
          }
        } else if (model != null) {
          foreach (Object nodedata in model.NodesSource) {
            foreach (Object n in model.GetFromNodesForNode(nodedata)) {
              AddLinkForData(n, nodedata, model);
            }
            foreach (Object n in model.GetToNodesForNode(nodedata)) {
              AddLinkForData(nodedata, n, model);
            }
          }
        }
      }
    }


    /// <summary>
    /// Determine the category for the link, to be able to decide between different templates for the <see cref="Link"/>.
    /// </summary>
    /// <param name="linkdata">
    /// if the model is an <see cref="Northwoods.GoXam.Model.ILinksModel"/>, this is the link data;
    /// otherwise this is a <see cref="VirtualLinkData"/>
    /// </param>
    /// <param name="model"></param>
    /// <returns>
    /// If there is no category associated with the link data,
    /// this uses the category of the "from" node data if it is not an empty string,
    /// and then tries to use the category of the "to" node data.
    /// The default category is the empty string.
    /// </returns>
    /// <remarks>
    /// The result is used in a call to <see cref="FindTemplateForLink"/>.
    /// </remarks>
    protected virtual String FindCategoryForLink(Object linkdata, IDiagramModel model) {
      String category = "";
      ILinksModel lmodel = model as ILinksModel;
      if (lmodel != null) category = lmodel.GetCategoryForLink(linkdata);
      // try getting the link category from the link's FromData
      if (category == "") {
        Object fromdata = null;
        PartManager.VirtualLinkData vl = linkdata as PartManager.VirtualLinkData;
        if (vl != null) {
          fromdata = vl.From;
        } else if (lmodel != null) {
          fromdata = lmodel.GetFromNodeForLink(linkdata);
        }
        if (fromdata != null && model != null) {
          category = model.GetCategoryForNode(fromdata);
        }
        // or else from the link's ToData
        if (category == "") {
          Object todata = null;
          if (vl != null) {
            todata = vl.To;
          } else if (lmodel != null) {
            todata = lmodel.GetToNodeForLink(linkdata);
          }
          if (todata != null) {
            category = model.GetCategoryForNode(todata);
          }
        }
      }
      return category;
    }

    /// <summary>
    /// Given a category and the link data, find a <c>DataTemplate</c> to use in making the <see cref="Link"/>.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <param name="model"></param>
    /// <param name="category"></param>
    /// <returns>
    /// a <c>DataTemplate</c> taken from either <see cref="Northwoods.GoXam.Diagram.LinkTemplate"/> or <see cref="Northwoods.GoXam.Diagram.LinkTemplateDictionary"/>,
    /// or null if no template is found
    /// </returns>
    /// <remarks>
    /// <para>
    /// For the default category (the empty string ""),
    /// this uses <see cref="Northwoods.GoXam.Diagram.LinkTemplate"/> if it is non-null,
    /// or else the default entry ("") in the <see cref="Northwoods.GoXam.Diagram.LinkTemplateDictionary"/> if there is a dictionary.
    /// For any other category,
    /// this looks up the category in the <see cref="Northwoods.GoXam.Diagram.LinkTemplateDictionary"/> if there is a dictionary,
    /// and if that fails it uses <see cref="Northwoods.GoXam.Diagram.LinkTemplate"/> or the
    /// default entry ("") in the <see cref="Northwoods.GoXam.Diagram.LinkTemplateDictionary"/>.
    /// This may return null if no appropriate template is found for the category.
    /// The result is used in a call to <see cref="MakeLinkForData"/>.
    /// </para>
    /// <para>
    /// You may wish to override this method to customize the selection of the template based on the link data.
    /// </para>
    /// </remarks>
    protected virtual DataTemplate FindTemplateForLink(Object linkdata, IDiagramModel model, String category) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      // for the default category, "", use LinkTemplate and then the dictionary;
      // for other categories, use the dictionary, then LinkTemplate, then "" in dictionary
      DataTemplate template = null;
      if (category == "") template = diagram.LinkTemplate;
      if (template == null) {
        DataTemplateDictionary dict = diagram.LinkTemplateDictionary;
        if (dict != null) {
          dict.TryGetValue(category, out template);
          // but if the template is not found, use the default one
          // (note that if category == "", we've already searched)
          if (template == null && category != "") {
            template = diagram.LinkTemplate;
            if (template == null) dict.TryGetValue("", out template);
          }
        }
      }
      return template;
    }

    /// <summary>
    /// Construct a new <see cref="Link"/>, setting its <c>Content</c> and <c>ContentTemplate</c> properties.
    /// </summary>
    /// <param name="linkdata">the data that this link is bound to; must not be a <see cref="Link"/> or any <c>UIElement</c></param>
    /// <param name="model">the model that the <paramref name="linkdata"/> is in</param>
    /// <param name="templ">the <c>DataTemplate</c> for the <c>ContentTemplate</c> property</param>
    /// <param name="category">the category for the link</param>
    /// <returns>a newly created <see cref="Link"/></returns>
    /// <remarks>
    /// <para>
    /// You may wish to override this method in order to customize the <c>DataTemplate</c> used
    /// for the particular <paramref name="linkdata"/>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual Link MakeLinkForData(Object linkdata, IDiagramModel model, DataTemplate templ, String category) {
      Link link = new Link();  // for PartManager.MakeLinkForData
      PartBinding data = new PartBinding(link, linkdata);
      link.Content = data;
      link.DataContext = data;
      link.ContentTemplate = templ;  // for PartManager.MakeLinkForData
      if (category != null && category != "") link.Category = category;
      return link;
    }


    // links for ILinksModel -- real link data

    /// <summary>
    /// Given some <paramref name="linkdata"/> in a <paramref name="model"/>,
    /// find the corresponding <see cref="Link"/> in this diagram.
    /// </summary>
    /// <param name="linkdata">the data in the <paramref name="model"/> that holds link relationship information</param>
    /// <param name="model">the model that the <paramref name="linkdata"/> is in, an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <returns>a <see cref="Link"/>, or null if such a link has not been created for that data</returns>
    /// <remarks>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public Link FindLinkForData(Object linkdata, IDiagramModel model) {
      if (linkdata == null) return null;
      VerifyAccess();
      Link link;
      _LinksDictionary.TryGetValue(linkdata, out link);
      return link;
    }

    /// <summary>
    /// Decide whether a particular <paramref name="linkdata"/> should be represented
    /// in the diagram by a <see cref="Link"/>.
    /// </summary>
    /// <param name="linkdata">the data in the <paramref name="model"/> that holds link relationship information</param>
    /// <param name="model">the model that the <paramref name="linkdata"/> is in, an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <returns>true only if <see cref="AddLinkForData(Object, IDiagramModel)"/> should make and add a <see cref="Link"/> to the diagram</returns>
    /// <remarks>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual bool FilterLinkForData(Object linkdata, IDiagramModel model) {
      if (linkdata is Link) return true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return true;
      bool poptree = (diagram.Filter & PartManagerFilter.CollapsedTreeChildren) != 0;
      bool popgroup = (diagram.Filter & PartManagerFilter.CollapsedSubGraphMembers) != 0;
      if (poptree) {
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel == null) return true;
        // if its "from" Node exists and is expanded, make Link now
        Node fromnode = FindNodeForData(lmodel.GetFromNodeForLink(linkdata), lmodel);
        if (fromnode != null && fromnode.IsExpandedTree) return true;
      }
      if (popgroup) {
        ISubGraphLinksModel sgmodel = model as ISubGraphLinksModel;
        if (sgmodel == null) return true;
        // if top-level, or if parent Group exists and is expanded, make Link now
        Group group = FindNodeForData(sgmodel.GetGroupForLink(linkdata), model) as Group;
        if (group == null) return true;
        if (group != null && group.IsExpandedSubGraph) return true;
      }
      return (!poptree && !popgroup);
    }

    /// <summary>
    /// Make sure a <see cref="Link"/> exists for some data in the model, added to the <see cref="LinkLayer"/>
    /// specified by its <see cref="Part.LayerName"/>.
    /// </summary>
    /// <param name="linkdata">the data in the <paramref name="model"/> that holds link relationship information</param>
    /// <param name="model">the model that the <paramref name="linkdata"/> is in, an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <returns>
    /// the <see cref="Link"/> for the <paramref name="linkdata"/>,
    /// either an existing one or a newly created one,
    /// or null if <see cref="FilterLinkForData(Object, IDiagramModel)"/> returns false or if the diagram is uninitialized
    /// </returns>
    /// <remarks>
    /// <para>
    /// If the diagram already has a <see cref="Link"/> for the data, as determined by
    /// <see cref="FindLinkForData(Object, IDiagramModel)"/>,
    /// or if <see cref="FilterLinkForData(Object, IDiagramModel)"/> is false,
    /// this method will do nothing.
    /// </para>
    /// <para>
    /// If the <paramref name="linkdata"/> is a <see cref="Link"/> and if
    /// the <paramref name="model"/> is a <see cref="Northwoods.GoXam.Model.PartsModel"/>,
    /// this method doesn't need to construct a new link by calling <see cref="MakeLinkForData"/>,
    /// but can just add the link to the diagram, in the appropriate <see cref="Layer"/>
    /// determined by the <see cref="Part.LayerName"/>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public Link AddLinkForData(Object linkdata, IDiagramModel model) {
      if (linkdata == null) return null;
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return null;
      Link oldlink = FindLinkForData(linkdata, model);
      if (oldlink != null) return oldlink;
      if (!FilterLinkForData(linkdata, model)) return null;

      Link newlink;
      if (model is PartsModel && linkdata is Link) {  // check if unbound: data is itself the Link
        newlink = (Link)linkdata;
      } else {  // no?  create the Link by binding the linkdata with a DataTemplate
        String category = FindCategoryForLink(linkdata, model);
        if (category == null) category = "";
        DataTemplate template = FindTemplateForLink(linkdata, model, category);
        newlink = MakeLinkForData(linkdata, model, template, category);
        VerifyTransaction(linkdata, model);
      }
      if (newlink != null) {
        newlink.Model = model;
        // add to collection of Links
        _LinksDictionary[linkdata] = newlink;
        _Links.Add(newlink);
        // add to a temporary layer so that ApplyTemplate will be successful
        // so that any attached properties become available on the Part's VisualElement
        LinkLayer templayer = panel.GetLayer<LinkLayer>(DiagramPanel.ToolLayerName);
        if (templayer != null) templayer.Add(newlink);
        newlink.ApplyTemplate();  //?? virtualization
        //?? LayerName might not be correct value until after ApplyTemplate
        String layername = newlink.LayerName;
        LinkLayer layer = (this.TemporaryParts ? templayer : panel.GetLayer<LinkLayer>(layername));
        if (layer != null) {
          layer.Add(newlink);
          UpdateLinkBundleAdd(model, newlink);
          UpdatePortKnots(newlink);
          //?? AddLogicalChild
          OnLinkAdded(newlink);  // after adding to dictionary
        }
      }
      return newlink;
    }

    /// <summary>
    /// Remove any <see cref="Link"/> that exists in the diagram for the <paramref name="linkdata"/>.
    /// </summary>
    /// <param name="linkdata">the data in the <paramref name="model"/> that holds link relationship information</param>
    /// <param name="model">the model that the <paramref name="linkdata"/> is in, an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <remarks>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public void RemoveLinkForData(Object linkdata, IDiagramModel model) {
      if (linkdata == null) return;
      VerifyAccess();
      Link link = FindLinkForData(linkdata, model);
      if (link != null) {
        VerifyTransaction(linkdata, model);
        link.IsSelected = false;
        OnLinkRemoving(link);  // before removing from dictionary
        UpdateLinkBundleRemove(link);
        UpdatePortKnots(link);
        _LinksDictionary.Remove(linkdata);
        _Links.Remove(link);
        LinkLayer llayer = link.Layer as LinkLayer;
        if (llayer != null) llayer.Remove(link);
        OnLinkRemoved(link, llayer);
        ClearDataBinding(link);
      }
    }


    // links for IConnectedModel and ITreeModel -- no link data exists, so we use the VirtualLinkData class

    /// <summary>
    /// Given a pair of node data for which there is a link relationship,
    /// find the corresponding <see cref="Link"/> in this diagram.
    /// </summary>
    /// <param name="fromnodedata">the data from which the link relationship comes</param>
    /// <param name="tonodedata">the data to which the link relationship goes</param>
    /// <param name="model">the model that the node data are in, not an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <returns>a <see cref="Link"/>, or null if such a link has not been created for that pair of node data</returns>
    /// <remarks>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public Link FindLinkForData(Object fromnodedata, Object tonodedata, IDiagramModel model) {
      if (fromnodedata == null && tonodedata == null) return null;
      VerifyAccess();
      ILinksModel lmodel = model as ILinksModel;
      if (lmodel != null) {
        foreach (Object linkdata in lmodel.GetFromLinksForNode(tonodedata)) {
          if (lmodel.GetFromNodeForLink(linkdata) == fromnodedata) return FindLinkForData(linkdata, model);
        }
        return null;
      } else {
        Object linkdata = new VirtualLinkData(fromnodedata, tonodedata);
        return FindLinkForData(linkdata, model);
      }
    }

    /// <summary>
    /// Decide whether the link relationship between a particular pair of node data
    /// should be represented in the diagram by a <see cref="Link"/>.
    /// </summary>
    /// <param name="fromnodedata">the data from which the link relationship comes</param>
    /// <param name="tonodedata">the data to which the link relationship goes</param>
    /// <param name="model">the model that the node data are in, not an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <returns>true only if <see cref="AddLinkForData(Object, Object, IDiagramModel)"/> should make and add a <see cref="Link"/> to the diagram</returns>
    /// <remarks>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    protected virtual bool FilterLinkForData(Object fromnodedata, Object tonodedata, IDiagramModel model) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return true;
      bool poptree = (diagram.Filter & PartManagerFilter.CollapsedTreeChildren) != 0;
      bool popgroup = (diagram.Filter & PartManagerFilter.CollapsedSubGraphMembers) != 0;
      if (poptree) {
        if (model == null) return true;
        // if its "from" Node exists and is expanded, make Link now
        Node fromnode = FindNodeForData(fromnodedata, model);
        if (fromnode != null && fromnode.IsExpandedTree) return true;
      }
      if (popgroup) {
        ISubGraphModel sgmodel = model as ISubGraphModel;
        if (sgmodel == null) return true;
        // if top-level, or if parent Group exists and is expanded, make Link now
        Group group = FindNodeForData(CommonSubGraph(sgmodel, fromnodedata, tonodedata), sgmodel) as Group;
        if (group == null) return true;
        if (group != null && group.IsExpandedSubGraph) return true;
      }
      return (!poptree && !popgroup);
    }

    /// <summary>
    /// Make sure a <see cref="Link"/> exists for the link relationship between the given
    /// pair of node data, added to the <see cref="LinkLayer"/>
    /// specified by its <see cref="Part.LayerName"/>.
    /// </summary>
    /// <param name="fromnodedata">the data from which the link relationship comes</param>
    /// <param name="tonodedata">the data to which the link relationship goes</param>
    /// <param name="model">the model that the node data are in, not an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <returns>
    /// the <see cref="Link"/> for the connecting the two node data,
    /// either an existing one or a newly created one,
    /// or null if <see cref="FilterLinkForData(Object, Object, IDiagramModel)"/> returns false or if the diagram is uninitialized
    /// </returns>
    /// <remarks>
    /// <para>
    /// If the diagram already has a <see cref="Link"/> for the pair of node data,
    /// as determined by <see cref="FindLinkForData(Object, Object, IDiagramModel)"/>,
    /// or if <see cref="FilterLinkForData(Object, Object, IDiagramModel)"/> is false,
    /// this method will do nothing.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public Link AddLinkForData(Object fromnodedata, Object tonodedata, IDiagramModel model) {
      if (fromnodedata == null && tonodedata == null) return null;
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return null;
      if (model is ILinksModel) Diagram.Error("Diagram.AddLinkForData(Object, Object) cannot be used with an ILinksModel");
      Link oldlink = FindLinkForData(fromnodedata, tonodedata, model);
      if (oldlink != null) {
        UpdateCachedMembership(model, oldlink);
        return oldlink;
      }
      if (!FilterLinkForData(fromnodedata, tonodedata, model)) return null;

      // the "data" for this link is artificial, a VirtualLinkData
      Object linkdata = new VirtualLinkData(fromnodedata, tonodedata);
      String category = FindCategoryForLink(linkdata, model);
      DataTemplate template = FindTemplateForLink(linkdata, model, category);
      Link newlink = MakeLinkForData(linkdata, model, template, category);
      VerifyTransaction(linkdata, model);
      if (newlink != null) {
        newlink.Model = model;
        // add to collection of Links
        _LinksDictionary[linkdata] = newlink;
        _Links.Add(newlink);
        // add to a temporary layer so that ApplyTemplate will be successful
        // so that any attached properties become available on the Part's VisualElement
        LinkLayer templayer = panel.GetLayer<LinkLayer>(DiagramPanel.ToolLayerName);
        if (templayer != null) templayer.Add(newlink);
        newlink.ApplyTemplate();  //?? virtualization
        //?? LayerName might not be correct value until after ApplyTemplate
        String layername = newlink.LayerName;
        LinkLayer layer = (this.TemporaryParts ? templayer : panel.GetLayer<LinkLayer>(layername));
        if (layer != null) {
          layer.Add(newlink);
          UpdateCachedMembership(model, newlink);
          UpdatePortKnots(newlink);
          //?? AddLogicalChild
          OnLinkAdded(newlink);
        }
      }
      return newlink;
    }

    /// <summary>
    /// Remove any <see cref="Link"/> that exists for a pair of given node data.
    /// </summary>
    /// <param name="fromnodedata">the data from which the link relationship comes</param>
    /// <param name="tonodedata">the data to which the link relationship goes</param>
    /// <param name="model">the model that the node data are in, not an <see cref="Northwoods.GoXam.Model.ILinksModel"/></param>
    /// <remarks>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// </para>
    /// </remarks>
    public void RemoveLinkForData(Object fromnodedata, Object tonodedata, IDiagramModel model) {
      VerifyAccess();
      if (model is ILinksModel) Diagram.Error("Diagram.AddLinkForData(Object, Object) cannot be used with an ILinksModel");
      Link link = FindLinkForData(fromnodedata, tonodedata, model);
      if (link != null) {
        VerifyTransaction(fromnodedata, model);
        link.IsSelected = false;
        OnLinkRemoving(link);
        RemoveCachedMembership(link);
        UpdatePortKnots(link);
        _LinksDictionary.Remove(link.Data);
        _Links.Remove(link);
        LinkLayer llayer = link.Layer as LinkLayer;
        if (llayer != null) llayer.Remove(link);
        OnLinkRemoved(link, llayer);
        ClearDataBinding(link);
      }
    }


    /// <summary>
    /// Called after the <paramref name="link"/> is added to the diagram.
    /// </summary>
    /// <param name="link">a <see cref="Link"/></param>
    /// <remarks>
    /// <para>
    /// Adding a link will request a new automatic layout.
    /// If either of the link's nodes are not <c>Visible</c>, the link will also be made not <c>Visible</c>.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the addition of the link to the diagram.
    /// </para>
    /// </remarks>
    protected virtual void OnLinkAdded(Link link) {
      if (link == null) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      bool vis = true;
      Group sg = link.ContainingSubGraph;
      if (sg != null && (!sg.Visible || !sg.IsExpandedSubGraph)) vis = false;
      if (vis) {
        Node from = link.FromNode;
        if (from != null && from.FindVisibleNode(null) == null) vis = false;
      }
      if (vis) {
        Node to = link.ToNode;
        if (to != null && to.FindVisibleNode(null) == null) vis = false;
      }
      link.Visible = vis;
      link.InvalidateLayout(LayoutChange.LinkAdded);
    }

    /// <summary>
    /// Called before the <paramref name="link"/> is removed from the diagram.
    /// </summary>
    /// <param name="link">a <see cref="Link"/></param>
    /// <remarks>
    /// <para>
    /// Removing a link will request a new automatic layout.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the removal of the link from the diagram.
    /// </para>
    /// </remarks>
    protected virtual void OnLinkRemoving(Link link) {
      if (link == null) return;
      link.InvalidateLayout(LayoutChange.LinkRemoved);
    }

    /// <summary>
    /// Called after the <paramref name="link"/> is removed from the diagram.
    /// </summary>
    /// <param name="link">a <see cref="Link"/></param>
    /// <param name="layer">the <see cref="LinkLayer"/> that the <paramref name="link"/> was removed from</param>
    /// <remarks>
    /// <para>
    /// By default this method does nothing.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the removal of the link from the diagram.
    /// </para>
    /// </remarks>
    protected virtual void OnLinkRemoved(Link link, LinkLayer layer) {
    }


    // only used when the model is an ISubGraphModel but is not an ILinksModel
    private void UpdateCachedMembership(IDiagramModel model, Link link) {
      if (link == null) return;
      
      ISubGraphModel sgmodel = model as ISubGraphModel;
      if (sgmodel == null || model is ILinksModel) return;

      Group oldsg = link.CachedParentSubGraph;
      Object fromdata = link.FromData;
      Object todata = link.ToData;
      Group newsg = FindNodeForData(CommonSubGraph(sgmodel, fromdata, todata), sgmodel) as Group;
      if (newsg != oldsg) {  // either might be null
        if (oldsg != null && oldsg.CachedMemberLinks != null) oldsg.CachedMemberLinks.Remove(link);
        if (newsg != null) {
          if (newsg.CachedMemberLinks == null) newsg.CachedMemberLinks = new List<Link>();
          if (!newsg.CachedMemberLinks.Contains(link)) newsg.CachedMemberLinks.Add(link);
        }
        link.CachedParentSubGraph = newsg;
      }
    }

    private void RemoveCachedMembership(Link link) {
      if (link == null) return;
      Group sg = link.CachedParentSubGraph;
      if (sg != null && sg.CachedMemberLinks != null) sg.CachedMemberLinks.Remove(link);
    }

    private static Object CommonSubGraph(ISubGraphModel model, Object a, Object b) {  // model will not be an ILinksModel
      if (a == null) return null;
      if (b == null) return null;
      Object asg = model.GetGroupForNode(a);
      if (asg == null) return null;
      if (a == b) return asg;
      Object bsg = model.GetGroupForNode(b);
      if (bsg == null) return null;
      if (asg == bsg) return bsg;
      if (IsContainedBy(model, b, asg)) return asg;
      if (IsContainedBy(model, a, bsg)) return bsg;
      return CommonSubGraph(model, asg, bsg);
    }

    private static bool IsContainedBy(ISubGraphModel model, Object p, Object part) {  // model will not be an ILinksModel
      if (p == null) return false;
      if (p == part) return true;
      Object sg = model.GetGroupForNode(p);
      if (sg != null && sg != part && IsContainedBy(model, sg, part)) return true;
      return false;
    }


    private void UpdateLinkBundleAdd(IDiagramModel model, Link link) {
      if (model == null) return;
      if (link == null) return;
      //?? some way not to participate in link bundling?
      //?? bundling all links between nodes, not specifically between pairs of ports
      Node fromnode = link.FromNode;
      if (fromnode == null) return;
      Node tonode = link.ToNode;
      if (tonode == null) return;
      Object todata = link.ToData;
      Object fromdata = link.FromData;

      // see if there's an existing LinkBundle between the nodes/ports
      Object fromparam = link.FromPortId;
      Object toparam = link.ToPortId;
      LinkBundle bundle = fromnode.FindBundle(fromparam, tonode, toparam);
      List<Link> bundledlinks = null;  // if set to a list, we'll need a LinkBundle if one doesn't already exist

      ILinksModel lmodel = model as ILinksModel;
      if (lmodel != null) {  // could be any number of links in either direction
        var linkdatas = lmodel.GetLinksBetweenNodes(fromdata, fromparam, todata, toparam)
                     .Concat(lmodel.GetLinksBetweenNodes(todata, toparam, fromdata, fromparam))
                     .Distinct().ToList();
        List<Link> links = linkdatas.Select(d => FindLinkForData(d, lmodel)).Where(l => l != null).ToList();
        if (links.Count > 1) {
          bundledlinks = links;
        }
      } else {  // in other models, even without multiple links from one node to another, there can be one in each direction
        Link fromto = FindLinkForData(fromdata, todata, model);
        Link tofrom = FindLinkForData(todata, fromdata, model);
        if (fromto != null && tofrom != null) {
          bundledlinks = new List<Link>();
          bundledlinks.Add(fromto);
          bundledlinks.Add(tofrom);
        }
      }

      if (bundledlinks != null) {  // need a LinkBundle
        if (bundle == null) {
          bundle = new LinkBundle() { Node1 = fromnode, Param1 = fromparam, Node2 = tonode, Param2 = toparam };
          fromnode.AddBundle(bundle);
          tonode.AddBundle(bundle);
        }
        bundle.Links = bundledlinks;
        for (int i = 0; i < bundledlinks.Count; i++) {
          Link l = bundledlinks[i];
          if (l.BundleIndex == 0) {
            int idx = NextIndex(bundledlinks);  // always returns positive index
            l.Bundle = bundle;  // update the back pointers
            l.BundleIndex = ((l.FromNode == bundle.Node1) ? idx : -idx);  // negative if links go the other way
            l.InvalidateRelationships("curviness");
          }
        }
      }
    }

    // find the next unused BundleIndex >= 1; treat negative values as if they were positive
    private int NextIndex(List<Link> links) {
      int idx = 1;  // don't use zero
      while (links.Any(l => Math.Abs(l.BundleIndex) == idx)) idx++;
      return idx;
    }

    private void UpdateLinkBundleRemove(Link link) {
      if (link == null) return;
      // see if there's an existing LinkBundle for this link
      LinkBundle bundle = link.Bundle;
      if (bundle != null) {
        int oldindex = link.BundleIndex;
        link.Bundle = null;
        link.BundleIndex = 0;
        link.InvalidateRelationships("curviness");
        bundle.Links.Remove(link);
        // if one or none left, remove the LinkBundle from both nodes
        if (bundle.Links.Count < 2) {
          // if there's only one link left in the LinkBundle, assume it shouldn't have one at all
          if (bundle.Links.Count == 1) {
            Link otherlink = bundle.Links[0];
            otherlink.Bundle = null;
            otherlink.BundleIndex = 0;
            otherlink.InvalidateRelationships("curviness");
          }
          bundle.Node1.RemoveBundle(bundle);
          bundle.Node2.RemoveBundle(bundle);
        } else {  // two or more links left in bundle
          // oldindex value no longer used: shift down BundleIndex values > oldindex, ignoring sign,
          // if they are on the same side as oldindex (i.e., if even, all even BundleIndexes that are larger than oldindex)
          oldindex = Math.Abs(oldindex);
          bool even = oldindex%2 == 0;
          foreach (Link l in bundle.Links) {
            int idx = Math.Abs(l.BundleIndex);
            bool e = idx%2 == 0;
            if (idx > oldindex && even == e) {
              if (l.BundleIndex > 0) {
                l.BundleIndex -= 2;
              } else {
                l.BundleIndex += 2;
              }
              l.InvalidateRelationships("curviness");
            }
          }
        }
      }
    }


    // clear any cached port side info
    private void UpdatePortKnots(Link link) {
      Route route = link.Route;
      if (route == null) return;
      FrameworkElement fromport = link.FromPort;
      if (fromport != null) Node.SetPortInfo(fromport, null);
      FrameworkElement toport = link.ToPort;
      if (toport != null) Node.SetPortInfo(toport, null);
    }


    /// <summary>
    /// Called after a part is added to a group node's members.
    /// </summary>
    /// <param name="group">a <see cref="Group"/></param>
    /// <param name="part">a <see cref="Part"/> that has been added to the <paramref name="group"/></param>
    /// <remarks>
    /// <para>
    /// Adding a member to a group will request a new automatic layout.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the membership of the part in the group.
    /// </para>
    /// </remarks>
    protected virtual void OnMemberAdded(Node group, Part part) {
      if (group == null) return;
      Group sg = group as Group;
      if (sg != null && part != null) part.Visible = sg.IsExpandedSubGraph;
      // invalidate the GROUP's layout
      group.InvalidateLayout(LayoutChange.MemberAdded);  // MemberAdded treated specially for groups
    }

    /// <summary>
    /// Called after a part is removed from a group node's members.
    /// </summary>
    /// <param name="group">a <see cref="Group"/></param>
    /// <param name="part">a <see cref="Part"/> that has been removed from the <paramref name="group"/></param>
    /// <remarks>
    /// <para>
    /// Removing a member from a group will request a new automatic layout.
    /// </para>
    /// <para>
    /// The implementation of this method should not modify the model.
    /// This method cannot and should not prevent or alter the membership of the part in the group.
    /// </para>
    /// </remarks>
    protected virtual void OnMemberRemoved(Node group, Part part) {
      if (group == null) return;
      // there isn't any OnMemberRemoving method, so PART has already been removed from the GROUP
      // Thus we have to try to invalidate the GROUP's layout
      group.InvalidateLayout(LayoutChange.MemberRemoved);  // MemberRemoved treated specially for groups
    }

    // invalidate the Diagram.Layout when reparenting a part to or from top-level
    private void InvalidateDiagramLayout(Part p, LayoutChange why) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // no side-effects during undo/redo
      IDiagramModel model = diagram.Model;
      if (model != null && model.IsChangingModel) return;
      if (p != null && (p.Layer == null || p.Layer.IsTemporary)) return;
      LayoutManager laymgr = diagram.LayoutManager;
      if (laymgr != null) laymgr.InvalidateLayout(null, LayoutChange.All);
    }


    // Other services

    /// <summary>
    /// This is called by <see cref="Node.ExpandTree"/> to make sure all of the node's
    /// connected <see cref="Node"/>s all exist in the diagram.
    /// </summary>
    /// <param name="parent"></param>
    /// <remarks>
    /// <para>
    /// Typically this is useful for expanding a node in a tree structure,
    /// but the graph need not be tree-structured, and the model can be any
    /// kind of model, not just <see cref="Northwoods.GoXam.Model.ITreeModel"/>.
    /// </para>
    /// <para>
    /// You might want to override this method to first bring in data to the model,
    /// if the needed data isn't in the model already.
    /// </para>
    /// </remarks>
    public virtual void RealizeTreeChildren(Node parent) {
      if (parent == null) return;
      Diagram diagram = this.Diagram;
      if (diagram != null && ((diagram.Filter & PartManagerFilter.CollapsedTreeChildren) != 0) && parent.IsBoundToData) {
        IDiagramModel model = diagram.Model;
        if (model == null) return;
        foreach (Object todata in model.GetToNodesForNode(parent.Data)) {
          if (FindNodeForData(todata, model) == null) {
            AddNodeForData(todata, model);
            Node child = FindNodeForData(todata, model);
            if (child != null) {
              Point loc = child.Location;
              if (Double.IsNaN(loc.X) || Double.IsNaN(loc.Y)) child.Location = parent.Location;
            }
          }
        }
      }
    }

    /// <summary>
    /// This is called by <see cref="Node.CollapseTree"/> to allow all of the child nodes and links
    /// for a particular <see cref="Node"/> to be removed from the diagram.
    /// </summary>
    /// <param name="parent"></param>
    public virtual void ReleaseTreeChildren(Node parent) {
      if (parent == null) return;
      Diagram diagram = this.Diagram;
      if (diagram != null && ((diagram.Filter & PartManagerFilter.CollapsedTreeChildren) != 0) && parent.IsBoundToData) {
        IDiagramModel model = diagram.Model;
        if (model == null) return;
        foreach (Node child in parent.NodesOutOf) {
          RemoveNodeForData(child.Data, model);
        }
      }
    }

    /// <summary>
    /// This is called by <see cref="Group.ExpandSubGraph"/> to make sure the group's
    /// member <see cref="Node"/>s and <see cref="Link"/>s all exist in the diagram.
    /// </summary>
    /// <param name="sg">a <see cref="Group"/></param>
    /// <remarks>
    /// <para>
    /// This depends on the diagram's <see cref="Northwoods.GoXam.Diagram.Model"/>
    /// being a <see cref="Northwoods.GoXam.Model.ISubGraphModel"/>.
    /// You might want to override this method to bring in data to the model,
    /// if the needed data isn't in the model already.
    /// </para>
    /// </remarks>
    public virtual void RealizeSubGraphMembers(Group sg) {
      if (sg == null) return;
      Diagram diagram = this.Diagram;
      if (diagram != null && ((diagram.Filter & PartManagerFilter.CollapsedSubGraphMembers) != 0) && sg.IsBoundToData) {
        ISubGraphModel sgmodel = diagram.Model as ISubGraphModel;
        if (sgmodel == null) return;
        GroupPanel gp = sg.GroupPanel;
        Point pos = (gp != null ? sg.GetElementPoint(gp, Spot.TopLeft) : new Point());
        foreach (Object childdata in sgmodel.GetMemberNodesForGroup(sg.Data)) {
          if (FindNodeForData(childdata, sgmodel) == null) {
            AddNodeForData(childdata, sgmodel);
            if (gp != null) {
              Node mem = FindNodeForData(childdata, sgmodel);
              if (mem != null) {
                Point loc = mem.Position;
                if (Double.IsNaN(loc.X) || Double.IsNaN(loc.Y)) mem.Position = pos;
              }
            }
          }
        }
        ISubGraphLinksModel sglmodel = diagram.Model as ISubGraphLinksModel;
        if (sglmodel != null) {
          foreach (Object childdata in sglmodel.GetMemberLinksForGroup(sg.Data)) {
            if (FindLinkForData(childdata, sglmodel) == null) AddLinkForData(childdata, sglmodel);
          }
        }
      }
    }

    /// <summary>
    /// This is called by <see cref="Group.CollapseSubGraph"/> to allow the member <see cref="Node"/>s
    /// and <see cref="Link"/>s to be removed from the diagram.
    /// </summary>
    /// <param name="sg">a <see cref="Group"/></param>
    public virtual void ReleaseSubGraphMembers(Group sg) {
      if (sg == null) return;
      Diagram diagram = this.Diagram;
      if (diagram != null && ((diagram.Filter & PartManagerFilter.CollapsedSubGraphMembers) != 0) && sg.IsBoundToData) {
        ISubGraphModel sgmodel = diagram.Model as ISubGraphModel;
        if (sgmodel == null) return;
        foreach (Node mem in sg.MemberNodes) {
          RemoveNodeForData(mem.Data, sgmodel);
        }
      }
    }

    /// <summary>
    /// Copy a collection of <see cref="Part"/>s from this model to a given model.
    /// </summary>
    /// <param name="coll">a collection of <see cref="Part"/>s that are in this model</param>
    /// <param name="destmodel">
    /// if null, this creates a model like this one by calling <see cref="IDiagramModel.CreateInitializedCopy"/>;
    /// the new model is available from the returned <see cref="ICopyDictionary"/> as its
    /// <see cref="ICopyDictionary.DestinationModel"/> property
    /// </param>
    /// <returns>an <see cref="ICopyDictionary"/> that maps original parts to the copied ones</returns>
    /// <remarks>
    /// <para>
    /// First this creates an <see cref="IDataCollection"/> of all of the data
    /// referenced by the collection of <see cref="Part"/>s.
    /// It ignores those parts for which <see cref="Part.CanCopy"/> returns false.
    /// </para>
    /// <para>
    /// Second it calls <see cref="IDiagramModel.AddCollectionCopy"/> to actually
    /// copy the data to the destination model.
    /// </para>
    /// <para>
    /// This does not raise any <see cref="Diagram"/> events.
    /// Call <see cref="CommandHandler.Copy"/> if you want to copy the current selection into the clipboard.
    /// (It calls this method.)
    /// </para>
    /// </remarks>
    public virtual ICopyDictionary CopyParts(IEnumerable<Part> coll, IDiagramModel destmodel) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      IDiagramModel model = diagram.Model;
      if (model == null) return null;

      // get the corresponding node datas and link datas
      IDataCollection sel = model.CreateDataCollection();
      //??? need to sort to maintain Z-order
      // call .ToList() to make lists serializable:
      sel.Nodes = coll.OfType<Node>().Where(CanCopyNode).Select(n => n.Data).ToList();
      HashSet<Link> sellinks = null;
      if (model is ILinksModel) {
        sel.Links = coll.OfType<Link>().Where(CanCopyLink).Select(l => {
          // add any link label nodes
          Node lab = l.LabelNode;
          if (lab != null) sel.AddNode(lab.Data);
          return l.Data;
        }).ToList();
      } else { // cf below when NOT ILinksModel
        sellinks = new HashSet<Link>(coll.OfType<Link>());
      }

      if (destmodel == null) {
        destmodel = model.CreateInitializedCopy(null);
        destmodel.Modifiable = true;
      }
      ICopyDictionary copyenv = destmodel.AddCollectionCopy(sel, null);

      if (!(sel.Model is ILinksModel)) {
        foreach (Object nodedata in sel.Nodes) {
          Object copynode = copyenv.FindCopiedNode(nodedata);
          foreach (Object fromdata in sel.Model.GetFromNodesForNode(nodedata)) {
            Link link = FindLinkForData(fromdata, nodedata, model);
            if (IsLinkToBeRemoved(link, sellinks, sel)) {
              Object copyfrom = copyenv.FindCopiedNode(fromdata);
              if (copyfrom != null) {
                destmodel.RemoveLink(copyfrom, link.FromPortId, copynode, link.ToPortId);
              }
            }
          }
          foreach (Object todata in sel.Model.GetToNodesForNode(nodedata)) {
            Link link = FindLinkForData(nodedata, todata, model);
            if (IsLinkToBeRemoved(link, sellinks, sel)) {
              Object copyto = copyenv.FindCopiedNode(todata);
              if (copyto != null) {
                destmodel.RemoveLink(copynode, link.FromPortId, copyto, link.ToPortId);
              }
            }
          }
        }
      }

      // for external drops, need to make sure Node.Bounds has a real size
      if (destmodel == model) {
        foreach (Object data in copyenv.Copies.Nodes) {
          Node n = FindNodeForData(data, destmodel);
          if (n != null) n.RemeasureNow();
        }
      }

      destmodel.ClearUnresolvedReferences();

      return copyenv;
    }

    private bool CanCopyNode(Node n) {
      if (n == null) return false;
      if (!n.CanCopy()) return false;
      if (n.IsBoundToData) return true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      return (diagram.Model == diagram.PartsModel);
    }

    private bool CanCopyLink(Link l) {
      if (l == null) return false;
      if (!l.CanCopy()) return false;
      if (l.IsBoundToData) return true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      return (diagram.Model == diagram.PartsModel);
    }

    private bool IsLinkToBeRemoved(Link link, HashSet<Link> sellinks, IDataCollection sel) {  // only called when not ILinksModel
      if (link == null) return false;
      if (sellinks.Contains(link)) return false;
      if (link.CachedParentSubGraph != null && sel.ContainsNode(link.CachedParentSubGraph.Data)) return false;
      return true;
    }


    // respects Part.CanDelete()
    // works with parts of mixed models

    /// <summary>
    /// Remove a collection of <see cref="Part"/>s from this model.
    /// </summary>
    /// <param name="coll">a collection of <see cref="Part"/>s that are in this model</param>
    /// <remarks>
    /// <para>
    /// If the part's <see cref="Part.CanDelete"/> method returns false,
    /// the part's data is not removed from the model.
    /// </para>
    /// <para>
    /// This just calls <see cref="IDiagramModel.RemoveNode"/> and
    /// <see cref="IDiagramModel.RemoveLink(Object, Object, Object, Object)"/>
    /// (or <see cref="ILinksModel.RemoveLink(Object)"/>) as needed.
    /// </para>
    /// <para>
    /// This does not raise any <see cref="Diagram"/> events.
    /// Call <see cref="CommandHandler.Delete"/> if you want to delete the current selection with the normal events.
    /// (It calls this method.)
    /// </para>
    /// </remarks>
    public virtual void DeleteParts(IEnumerable<Part> coll) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return;
      foreach (Part p in coll.ToList()) {  // work on copy of collection
        if (!p.CanDelete()) continue;  // not removable?
        IDiagramModel dmodel = p.Model;
        if (dmodel == null) return;
        Node n = p as Node;
        if (n != null) {
          dmodel.RemoveNode(p.Data);
        } else {
          Link l = p as Link;
          if (l != null) {
            ILinksModel lmodel = dmodel as ILinksModel;
            if (lmodel != null) {
              lmodel.RemoveLink(p.Data);
            } else {
              dmodel.RemoveLink(l.FromData, l.FromPortId, l.ToData, l.ToPortId);
            }
          }
        }
      }
    }


    /// <summary>
    /// Select the <see cref="Part"/>s in this diagram corresponding to a collection of model data.
    /// </summary>
    /// <param name="datacoll">an <see cref="IDataCollection"/></param>
    /// <remarks>
    /// First this clears this <see cref="Diagram"/>'s selection.
    /// Then it finds each <see cref="Node"/> or <see cref="Link"/> corresponding
    /// to the data in <paramref name="datacoll"/> and
    /// sets its <see cref="Part.IsSelected"/> to true.
    /// </remarks>
    public virtual void SelectData(IDataCollection datacoll) {
      if (datacoll == null) return;
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      // select all the new nodes and links
      diagram.ClearSelection();
      foreach (Object nodedata in datacoll.Nodes) {
        Node n = FindNodeForData(nodedata, model);
        if (n != null) n.IsSelected = true;
      }
      if (model is ILinksModel) {
        foreach (Object linkdata in datacoll.Links) {
          Link l = FindLinkForData(linkdata, model);
          if (l != null) l.IsSelected = true;
        }
      } else {
        foreach (Object nodedata in datacoll.Nodes) {
          foreach (Object fromdata in model.GetFromNodesForNode(nodedata)) {
            Link link = FindLinkForData(fromdata, nodedata, model);
            if (link != null) link.IsSelected = true;
          }
          foreach (Object todata in model.GetToNodesForNode(nodedata)) {
            Link link = FindLinkForData(nodedata, todata, model);
            if (link != null) link.IsSelected = true;
          }
        }
      }
    }

    //?? make it easier to map Parts to data and vice-versa

    internal IDataCollection MakeDataCollection(IEnumerable<Part> parts) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      IDiagramModel model = diagram.Model;
      if (model == null) return null;
      IDataCollection coll = model.CreateDataCollection();
      foreach (Part p in parts) {
        Node n = p as Node;
        if (n != null && n.IsBoundToData) coll.AddNode(n.Data);
      }
      if (model is ILinksModel) {
        foreach (Part p in parts) {
          Link l = p as Link;
          if (l != null && l.IsBoundToData) coll.AddLink(l.Data);
        }
      }
      return coll;
    }

    /// <summary>
    /// Return a collection of <see cref="Link"/>s in this diagram corresponding to some model data.
    /// </summary>
    /// <param name="datacoll">an <see cref="IDataCollection"/></param>
    /// <returns>
    /// If this model is an <see cref="ILinksModel"/>, this just calls <see cref="FindLinkForData(Object, IDiagramModel)"/>
    /// and collects the <see cref="Link"/>s that are found.
    /// Otherwise it looks for <see cref="Link"/>s that are connected to the nodes in the data collection.
    /// </returns>
    public virtual IEnumerable<Link> FindLinksForData(IDataCollection datacoll) {
      if (datacoll == null) return Part.NoLinks;
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return Part.NoLinks;
      IDiagramModel model = diagram.Model;
      if (model == null) return Part.NoLinks;
      if (model is ILinksModel) {
        return datacoll.Links.Select(l => FindLinkForData(l, model)).Where(l => l != null);
      } else {
        return datacoll.Nodes.SelectMany(n => model.GetFromNodesForNode(n).Select(f => FindLinkForData(f, n, model)).Where(l => l != null))
          .Concat(datacoll.Nodes.SelectMany(n => model.GetToNodesForNode(n).Select(t => FindLinkForData(n, t, model)).Where(l => l != null)));
      }
    }


    /// <summary>
    /// Provide access to the <see cref="Part"/> for data binding, e.g. inside a <c>DataTemplate</c>,
    /// as well as access to the model data itself.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="PartBinding"/> is created as the <c>ContentPresenter.Content</c> value
    /// for each new <see cref="Northwoods.GoXam.Node"/> and <see cref="Northwoods.GoXam.Link"/>.
    /// The presence of a <see cref="PartBinding"/> also indicates that the <see cref="Part"/>
    /// is bound to data.
    /// </para>
    /// <para>
    /// You probably do not have a reason to explicitly create a <c>PartBinding</c>.
    /// </para>
    /// </remarks>
    public sealed class PartBinding {  // nested class
      /// <summary>
      /// Construct and initialize a <see cref="PartBinding"/>,
      /// matching up a <see cref="Node"/> or <see cref="Link"/> with the data to which it is bound,
      /// to be used as the <c>Content</c> of a <c>ContentPresenter</c>.
      /// </summary>
      /// <param name="part"></param>
      /// <param name="data"></param>
      public PartBinding(Part part, Object data) { this.Part = part; this.Data = data; }

      /// <summary>
      /// Gets the <see cref="Northwoods.GoXam.Node"/> or <see cref="Northwoods.GoXam.Link"/>
      /// that is bound to model data.
      /// </summary>
      /// <remarks>
      /// This allows for access to <see cref="Northwoods.GoXam.Part"/> properties
      /// when data binding in the <c>DataTemplate</c> for the <see cref="Northwoods.GoXam.Node"/>
      /// or <see cref="Northwoods.GoXam.Link"/>.
      /// </remarks>
      public Part Part { get; private set; }

      /// <summary>
      /// Gets the model data that this <see cref="Northwoods.GoXam.Node"/> or
      /// <see cref="Northwoods.GoXam.Link"/> is bound to.
      /// </summary>
      /// <remarks>
      /// For example a minimal node template might be:
      /// <code>
      ///   &lt;DataTemplate&gt;
      ///     &lt;TextBlock Text="{Binding Path=Data}" /&gt;
      ///   &lt;/DataTemplate&gt;
      /// </code>
      /// where the text string will the result of calling <c>ToString</c> on your node data object.
      /// </remarks>
      public Object Data { get; private set; }

      /// <summary>
      /// This returns <see cref="Part"/> as a <see cref="Northwoods.GoXam.Node"/>.
      /// </summary>
      public Node Node {
        get { return this.Part as Node; }
      }

      /// <summary>
      /// This returns <see cref="Part"/> as a <see cref="Northwoods.GoXam.Group"/>.
      /// </summary>
      public Group Group {
        get { return this.Part as Group; }
      }

      /// <summary>
      /// This returns <see cref="Part"/> as a <see cref="Northwoods.GoXam.Link"/>.
      /// </summary>
      public Link Link {
        get { return this.Part as Link; }
      }
    }

    /// <summary>
    /// The value of <see cref="Part.Data"/> for those <see cref="Link"/>s
    /// that are bound to data in a model that does not support separate link data.
    /// </summary>
    /// <remarks>
    /// Because there are no data objects for links in models such as GraphModel and TreeModel,
    /// we keep track of links by instead remembering the two nodes that they connect.
    /// This also becomes the value for the <see cref="PartBinding.Data"/>
    /// property for links, so that link data templates can have access to the node data
    /// at either end of the link relationship.
    /// This class is not used when the model is <see cref="Northwoods.GoXam.Model.ILinksModel"/>.
    /// </remarks>
    public sealed class VirtualLinkData {  // nested class
      internal VirtualLinkData(Object from, Object to) { this.From = from; this.To = to; }

      /// <summary>
      /// Gets the data object for the "from" end of this link relationship.
      /// </summary>
      /// <value>
      /// The value will not be a <see cref="Node"/> unless the model is a <see cref="PartsModel"/>.
      /// </value>
      public Object From { get; private set; }

      /// <summary>
      /// Gets the data object for the "to" end of this link relationship.
      /// </summary>
      /// <value>
      /// The value will not be a <see cref="Node"/> unless the model is a <see cref="PartsModel"/>.
      /// </value>
      public Object To { get; private set; }

      /// <summary>
      /// Two <see cref="VirtualLinkData"/> objects are effectively the same if the <see cref="From"/>
      /// and <see cref="To"/> data objects are equal.
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals(object obj) {
        VirtualLinkData other = obj as VirtualLinkData;
        Object fd = this.From;
        Object td = this.To;
        return other != null &&
          (fd != null ? fd.Equals(other.From) : other.From == null) &&
          (td != null ? td.Equals(other.To) : other.To == null);
      }

      /// <summary>
      /// This is just the combination of the hash codes for the <see cref="From"/> and <see cref="To"/> data objects.
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode() {
        int h = 0;
        if (this.From != null) h = this.From.GetHashCode();
        if (this.To != null) h ^= this.To.GetHashCode();
        return h;
      }
      
      /// <summary>
      /// This is might be useful for debugging.
      /// </summary>
      /// <returns></returns>
      public override string ToString() {
        return (this.From != null ? this.From.ToString() : "(null)") + " --> " + (this.To != null ? this.To.ToString() : "(null)");
      }
    }

  }  // end of PartManager


  /// <summary>
  /// This enumeration lists ways in which the <see cref="PartManager"/>
  /// filter methods can automatically restrict which model data get
  /// realized by actual <see cref="Node"/>s and <see cref="Link"/>s.
  /// </summary>
  /// <remarks>
  /// This is used as the value of the <see cref="Diagram.Filter"/> property.
  /// </remarks>
  internal /*?? public */ enum PartManagerFilter {
    /// <summary>
    /// All model data get realized by nodes and links;
    /// this is the default value for <see cref="Diagram.Filter"/>.
    /// </summary>
    None = 0,
    /// <summary>
    /// The children of collapsed tree nodes are not realized.
    /// </summary>
    /// <remarks>
    /// When one expands a tree <see cref="Node"/>,
    /// <see cref="PartManager.RealizeTreeChildren"/> is called.
    /// </remarks>
    CollapsedTreeChildren = 1,
    /// <summary>
    /// The members of collapsed group nodes are not realized.
    /// </summary>
    /// <remarks>
    /// When one expands a <see cref="Group"/>,
    /// <see cref="PartManager.RealizeSubGraphMembers"/> is called.
    /// </remarks>
    CollapsedSubGraphMembers = 2
  }
}
