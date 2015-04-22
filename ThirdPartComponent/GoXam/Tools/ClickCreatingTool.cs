
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
using System.ComponentModel;
using System.Windows;
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>ClickCreatingTool</c> lets the user create a node by clicking where they want the new node to be.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Normally this is a "mouse-up" mode-less tool.
  /// It will not run until you have set the <see cref="PrototypeData"/> property.
  /// </para>
  /// <para>
  /// This tool does not utilize any <see cref="Adornment"/>s or tool handles.
  /// </para>
  /// <para>
  /// This tool conducts a model edit in the <see cref="InsertNode"/> method.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class ClickCreatingTool : DiagramTool {

    static ClickCreatingTool() {
      PrototypeDataProperty = DependencyProperty.Register("PrototypeData", typeof(Object), typeof(ClickCreatingTool), new FrameworkPropertyMetadata(null));
      DoubleClickProperty = DependencyProperty.Register("DoubleClick", typeof(bool), typeof(ClickCreatingTool), new FrameworkPropertyMetadata(false));
    }

    /// <summary>
    /// Identifies the <see cref="PrototypeData"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PrototypeDataProperty;
    /// <summary>
    /// Gets or sets a data value for a new node.
    /// </summary>
    public Object PrototypeData {
      get { return GetValue(PrototypeDataProperty); }
      set { SetValue(PrototypeDataProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DoubleClick"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DoubleClickProperty;
    /// <summary>
    /// Gets or sets whether this tool requires a double-click to create a node at the click point.
    /// </summary>
    /// <value>
    /// The default value is false -- a single click is sufficient to insert a new node.
    /// </value>
    public bool DoubleClick {
      get { return (bool)GetValue(DoubleClickProperty); }
      set { SetValue(DoubleClickProperty, value); }
    }

    /// <summary>
    /// This tool can run when the diagram supports inserting nodes,
    /// the model is modifiable, and there is a click (or a double-click
    /// if <see cref="DoubleClick"/> is true).
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <see cref="PrototypeData"/> must be non-null, too.
    /// </remarks>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      // gotta have some node data that can be copied
      if (this.PrototypeData == null) return false;

      Diagram diagram = this.Diagram;

      // heed IsReadOnly & AllowInsert
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!diagram.AllowInsert) return false;

      // make sure the model permits adding stuff
      IDiagramModel model = diagram.Model;
      if (model == null || !model.Modifiable) return false;

      // only works with the left button
      if (!IsLeftButtonDown()) return false;

      // the mouse down point needs to be near the mouse up point
      if (IsBeyondDragSize()) return false;

      // maybe requires double-click; otherwise avoid accidental double-create
      if (this.DoubleClick ^ IsDoubleClick()) return false;

      // don't include the following check when this tool is running modally
      if (diagram.CurrentTool != this) {
        // only operates in the background, not on some Part
        Part part = FindPartAt(diagram.LastMousePointInModel, false);
        if (part != null) return false;
      }

      return true;
    }

    /// <summary>
    /// Upon a click, call <see cref="InsertNode"/> and stop this tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        InsertNode(diagram.LastMousePointInModel);
      }
      StopTool();
    }

    /// <summary>
    /// Create a node by adding a copy of the <see cref="PrototypeData"/>
    /// to the diagram's model, and assign its <see cref="Node.Location"/>
    /// to be the given point.
    /// </summary>
    /// <param name="loc"></param>
    /// <remarks>
    /// This also selects the new node and raises the "object created" event.
    /// </remarks>
    public virtual void InsertNode(Point loc) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      PartManager partmgr = diagram.PartManager;
      if (partmgr == null) return;

      StartTransaction("Create Node");
      IDiagramModel model = partmgr.FindNodeDataModel(this.PrototypeData);
      Object copy = model.AddNodeCopy(this.PrototypeData);
      Node node = partmgr.FindNodeForData(copy, model);
      if (node != null) {
        node.Location = loc;
        if (diagram.AllowSelect) {
          diagram.Select(node);
        }
      }

      diagram.Panel.UpdateDiagramBounds();
      // set the EditResult before raising event, in case it changes the result or cancels the tool
      this.TransactionResult = "Create Node";
      RaiseEvent(Diagram.NodeCreatedEvent, new DiagramEventArgs(node));
      StopTransaction();
    }

  }
}
