
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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace Northwoods.GoXam {

  /// <summary>
  /// This class implements the handlers for all of the standard diagram commands.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each <see cref="Northwoods.GoXam.Diagram"/> has an instance of this class as its
  /// <see cref="Northwoods.GoXam.Diagram.CommandHandler"/> property.
  /// If you want to customize the standard behavior, you can easily override any of its methods
  /// and substitute an instance of your custom command handler class for your diagram.
  /// <code>
  /// public class CustomCommandHandler : CommandHandler {
  ///   protected override void CopyToClipboard(IDataCollection coll) {
  ///     base.CopyToClipboard(coll);
  ///     ... maybe set some other Clipboard data formats ...
  ///   }
  /// }
  /// </code>
  /// and install it with either XAML:
  /// <code>
  ///   &lt;go:Diagram ...&gt;
  ///     &lt;go:Diagram.CommandHandler&gt;
  ///       &lt;local:CustomCommandHandler /&gt;
  ///     &lt;/go:Diagram.CommandHandler&gt;
  ///   &lt;/go:Diagram&gt;
  /// </code>
  /// or in the initialization of your Diagram control:
  /// <code>
  ///   myDiagram.CommandHandler = new CustomCommandHandler();
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
  public class CommandHandler : FrameworkElement {
    /// <summary>
    /// Create a normal <see cref="CommandHandler"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="CommandHandler"/> class does not have any of its own state,
    /// except for a back-pointer to the owner <see cref="Diagram"/>.
    /// </remarks>
    public CommandHandler() { }

    static CommandHandler() {
      DeletingInclusionsProperty = DependencyProperty.Register("DeletingInclusions", typeof(EffectiveCollectionInclusions), typeof(CommandHandler),
        new FrameworkPropertyMetadata(EffectiveCollectionInclusions.Standard));
      CopyingInclusionsProperty = DependencyProperty.Register("CopyingInclusions", typeof(EffectiveCollectionInclusions), typeof(CommandHandler),
        new FrameworkPropertyMetadata(EffectiveCollectionInclusions.Standard));
      PrototypeGroupProperty = DependencyProperty.Register("PrototypeGroup", typeof(Object), typeof(CommandHandler),
        new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> for which this <see cref="CommandHandler"/> executes commands.
    /// </summary>
    /// <value>
    /// This value is automatically set by the <see cref="Northwoods.GoXam.Diagram.CommandHandler"/> setter.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; internal set; }



    /// <summary>
    /// This is called by the <see cref="Northwoods.GoXam.Tool.ToolManager"/>.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This interprets all of the keyboard commands, such as:
    /// if Control-C and <see cref="CanCopy"/>, call <see cref="Copy"/>.
    /// </remarks>
    public virtual void DoKeyDown(KeyEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;

      bool control = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
      bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
      bool alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;
      Key key = e.Key;

      //Diagram.Debug("DoKeyDown: " + (control ? "CTRL " : "") + (shift ? "SHIFT " : "") + (alt ? "ALT " : "") + key.ToString());

      if (control && (key == Key.C || key == Key.Insert)) {
        if (CanCopy()) Copy();
      } else if ((control && key == Key.X) || (shift && key == Key.Delete)) {
        if (CanCut()) Cut();
      } else if (key == Key.Delete) {
        if (CanDelete()) Delete();
      } else if ((control && key == Key.V) || (shift && key == Key.Insert)) {
        if (CanPaste()) Paste();
      } else if ((control && key == Key.Y) || (alt && shift && key == Key.Back)) {
        if (CanRedo()) Redo();
      } else if ((control && key == Key.Z) || (alt && key == Key.Back)) {
        if (CanUndo()) Undo();
      } else if (control && key == Key.A) {
        if (CanSelectAll()) SelectAll();
      //} else if (control && key == Key.P) {  // ^P gets intercepted by browser accelerator; handled by DoKeyUp instead
      //  if (CanPrint()) Print();
      } else if (key == Key.Escape) {
        if (CanStopCommand()) StopCommand();
      } else if (key == Key.Up) {  //?? different from WPF
        DiagramPanel panel = diagram.Panel;
        if (panel != null && panel.CanVerticallyScroll) {
          if (control)
            panel.SetVerticalOffset(panel.VerticalOffset-1);
          else
            panel.LineUp();
        }
      } else if (key == Key.Down) {  //?? different from WPF
        DiagramPanel panel = diagram.Panel;
        if (panel != null && panel.CanVerticallyScroll) {
          if (control)
            panel.SetVerticalOffset(panel.VerticalOffset+1);
          else
            panel.LineDown();
        }
      } else if (key == Key.Left) {  //?? different from WPF
        DiagramPanel panel = diagram.Panel;
        if (panel != null && panel.CanHorizontallyScroll) {
          if (control)
            panel.InternalSetHorizontalOffset(panel.HorizontalOffset-1);
          else
            panel.LineLeft();
        }
      } else if (key == Key.Right) {  //?? different from WPF
        DiagramPanel panel = diagram.Panel;
        if (panel != null && panel.CanHorizontallyScroll) {
          if (control)
            panel.InternalSetHorizontalOffset(panel.HorizontalOffset+1);
          else
            panel.LineRight();
        }
      } else if (key == Key.PageUp) {  //?? different from WPF
        DiagramPanel panel = diagram.Panel;
        if (panel != null) {
          if (control && panel.CanHorizontallyScroll)
            panel.PageLeft();
          else if (!control && panel.CanVerticallyScroll)
            panel.PageUp();
        }
      } else if (key == Key.PageDown) {  //?? different from WPF
        DiagramPanel panel = diagram.Panel;
        if (panel != null) {
          if (control && panel.CanHorizontallyScroll)
            panel.PageRight();
          else if (!control && panel.CanVerticallyScroll)
            panel.PageDown();
        }
      } else if (key == Key.Home) {
        DiagramPanel panel = diagram.Panel;
        if (panel != null) {
          Rect b = panel.DiagramBounds;
          if (control && panel.CanVerticallyScroll) {
            panel.Position = new Point(panel.Position.X, b.Y);
          } else if (!control && panel.CanHorizontallyScroll) {
            panel.Position = new Point(b.X, panel.Position.Y);
          }
        }
      } else if (key == Key.End) {
        DiagramPanel panel = diagram.Panel;
        if (panel != null) {
          Rect b = panel.DiagramBounds;
          Rect v = panel.ViewportBounds;
          if (control && panel.CanVerticallyScroll) {
            panel.Position = new Point(v.X, b.Bottom - v.Height);
          } else if (!control && panel.CanHorizontallyScroll) {
            panel.Position = new Point(b.Right - v.Width, v.Y);
          }
        }
      } else if (key == Key.Subtract) {  //?? keypad: different than '-'
        if (CanDecreaseZoom(null)) DecreaseZoom(null);
      } else if (key == Key.Add) {  //?? keypad: different than '+'
        if (CanIncreaseZoom(null)) IncreaseZoom(null);
      } else if (control && key == Key.D0) {
        if (CanZoom(null)) Zoom(null);
      } else if (control && !shift && key == Key.G) {
        if (CanGroup()) Group();
      } else if (control && shift && key == Key.G) {
        if (CanUngroup()) Ungroup();
      } else if (key == Key.F2) {
        if (CanEdit()) Edit();
      }
    }

    /// <summary>
    /// This is called by the <see cref="Northwoods.GoXam.Tool.ToolManager"/>.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// By default this does nothing.
    /// </remarks>
    public virtual void DoKeyUp(KeyEventArgs e) {
      //Diagram diagram = this.Diagram;
      //if (diagram == null) return;

      //bool control = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
      //bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
      //bool alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;
      //Key key = e.Key;

      //Diagram.Debug("DoKeyUp: " + (control ? "CTRL " : "") + (shift ? "SHIFT " : "") + (alt ? "ALT " : "") + key.ToString());

      //if (control && key == Key.P) {  // gotta handle ^P in KeyUp event  //??? need to deal with reentrancy problems
      //  if (CanPrint()) Print();
      //}
    }

































































































































































































    // return true if the DiagramEventArgs.Handled was true after calling Diagram.RaiseEvent
    internal bool RaiseEvent(int evt)









    {
      if (this.Diagram != null) {
        DiagramEventArgs e = new DiagramEventArgs();
        e.RoutedEvent = evt;
        this.Diagram.RaiseEvent(e);
        return e.Handled;
      }
      return false;
    }


    /// <summary>
    /// Cancel the operation of the current <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </summary>
    /// <remarks>
    /// This may be called when the user presses ESCAPE.
    /// If the current tool was a <see cref="ToolManager"/>, this clears the diagram's selection.
    /// </remarks>
    public virtual void StopCommand() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramTool tool = diagram.CurrentTool;
      if (tool is ToolManager && diagram.AllowSelect) {
        diagram.ClearSelection();
      }
      if (tool != null) {
        tool.DoCancel();
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <see cref="StopCommand"/> is executed.
    /// </summary>
    /// <returns>by default, this returns true</returns>
    public virtual bool CanStopCommand() {
      return true;
    }


    /// <summary>
    /// Delete the currently <see cref="Northwoods.GoXam.Diagram.SelectedParts"/> from the <see cref="Diagram"/>.
    /// </summary>
    /// <remarks>
    /// This raises the <see cref="Northwoods.GoXam.Diagram.SelectionDeletingEvent"/>;
    /// if it is <c>Handled</c>, this deletion is cancelled.
    /// It then removes all of the selected parts from the diagram,
    /// by calling <see cref="PartManager.DeleteParts"/>.
    /// Finally it raises the <see cref="Northwoods.GoXam.Diagram.SelectionDeletedEvent"/>.
    /// All of the changes are performed in an undoable edit.
    /// </remarks>
    public virtual void Delete() {
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return;
      if (RaiseEvent(Diagram.SelectionDeletingEvent)) return;

      try {
        diagram.Cursor = Cursors.Wait;
        model.StartTransaction("Delete");
        // this deletes parts from whatever model they are part of
        HashSet<Part> parts = new HashSet<Part>();
        foreach (Part p in diagram.SelectedParts) {
          GatherCollection(parts, p, this.DeletingInclusions);
        }
        mgr.DeleteParts(parts);
        RaiseEvent(Diagram.SelectionDeletedEvent);
      } finally {
        model.CommitTransaction("Delete");
        diagram.Cursor = null;
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Delete</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowDelete"/> is true,
    /// and if there are some selected parts (<see cref="Northwoods.GoXam.Diagram.SelectedParts"/>).
    /// </returns>
    public virtual bool CanDelete() {
      Diagram diagram = this.Diagram;
      return (diagram != null && !diagram.IsReadOnly && diagram.AllowDelete && diagram.SelectedParts.Count > 0 &&
              diagram.Model != null && diagram.Model.Modifiable);
    }

    internal static void GatherCollection(HashSet<Part> map, Part p, EffectiveCollectionInclusions inclusions) {
      if (map.Contains(p)) return;
      Node n = p as Node;
      if (n != null) {
        map.Add(n);
        if (((int)inclusions & 1 /*?? EffectiveCollectionInclusions.Members */) != 0) {
          Group g = n as Group;
          if (g != null) {
            foreach (Node c in g.MemberNodes) {
              GatherCollection(map, c, inclusions);
            }
            foreach (Link c in g.MemberLinks) {
              GatherCollection(map, c, inclusions);
            }
          }
        }
        if (((int)inclusions & 2 /*?? EffectiveCollectionInclusions.ConnectingLinks */) != 0) {
          foreach (Link l in n.LinksConnected) {
            if (map.Contains(l)) continue;
            Node from = l.FromNode;
            Node to = l.ToNode;
            //??? NOTMOVABLELINK
            if (from != null && map.Contains(from) && to != null && map.Contains(to)) {
              GatherCollection(map, l, inclusions);
            }
          }
        }
        if ((inclusions & EffectiveCollectionInclusions.TreeChildren) != 0) {
          Diagram diagram = n.Diagram;
          var nodes = (diagram != null && diagram.TreePath == Northwoods.GoXam.Layout.TreePath.Source) ? n.NodesInto : n.NodesOutOf;
          foreach (Node c in nodes) {
            GatherCollection(map, c, inclusions);
          }
        }
      } else {
        Link l = p as Link;
        if (l != null) {
          map.Add(p);
          if (((int)inclusions & 4 /*?? EffectiveCollectionInclusions.LinkLabelNodes */) != 0) {
            Node lab = l.LabelNode;
            if (lab != null) {
              GatherCollection(map, lab, inclusions);
            }
          }
        }
      }
    }

    /// <summary>
    /// Identifies the <see cref="DeletingInclusions"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DeletingInclusionsProperty;
    /// <summary>
    /// Gets or sets whether <see cref="Delete"/> deletes only the selected parts
    /// or subtrees of selected nodes as well.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Tool.EffectiveCollectionInclusions.Standard"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// When set to <see cref="Northwoods.GoXam.Tool.EffectiveCollectionInclusions.SubTree"/>,
    /// <see cref="Delete"/> will augment the collection of
    /// selected nodes to include their tree children nodes.
    /// The resulting collection often will include many <see cref="Node"/>s and
    /// <see cref="Link"/>s that are not <see cref="Part.IsSelected"/>.
    /// </para>
    /// <para>
    /// Links are assumed to go from the parent node to the children nodes,
    /// unless <see cref="Northwoods.GoXam.Diagram.TreePath"/> is set to "Source".
    /// </para>
    /// <para>
    /// If you set this property, you might also want to set the
    /// <see cref="CopyingInclusions"/> property and the
    /// <see cref="Northwoods.GoXam.Tool.DraggingTool.Inclusions"/> property.
    /// </para>
    /// </remarks>
    public EffectiveCollectionInclusions DeletingInclusions {
      get { return (EffectiveCollectionInclusions)GetValue(DeletingInclusionsProperty); }
      set { SetValue(DeletingInclusionsProperty, value); }
    }


    /// <summary>
    /// Select all of the <see cref="Node"/>s and <see cref="Link"/>s in the <see cref="Diagram"/>.
    /// </summary>
    public virtual void SelectAll() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      try {
        diagram.Cursor = Cursors.Wait;
        foreach (Part p in diagram.Nodes) {
          p.IsSelected = true;
        }
        foreach (Part p in diagram.Links) {
          p.IsSelected = true;
        }
      } finally {
        diagram.Cursor = null;
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>SelectAll</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true if <see cref="Northwoods.GoXam.Diagram.AllowSelect"/> is true.
    /// </returns>
    public virtual bool CanSelectAll() {
      Diagram diagram = this.Diagram;
      return (diagram != null && diagram.AllowSelect);
    }


    // clipboard commands

    // internal storage for when the system clipboard is unavailable (due to either permission or serialization problems)
    private static IDataCollection InternalClipboard { get; set; }

    /// <summary>
    /// Copy the currently <see cref="Northwoods.GoXam.Diagram.SelectedParts"/> from the <see cref="Diagram"/>
    /// into the clipboard.
    /// </summary>
    /// <remarks>
    /// This makes a copy of the current selection (by calling <see cref="PartManager.CopyParts"/>)
    /// and then calls <see cref="CopyToClipboard"/>.
    /// </remarks>
    public virtual void Copy() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return;
      try {
        diagram.Cursor = Cursors.Wait;
        CommandHandler.InternalClipboard = null;
        IDataCollection copysel = null;
        try {
          HashSet<Part> parts = new HashSet<Part>();
          foreach (Part p in diagram.SelectedParts) {
            GatherCollection(parts, p, this.CopyingInclusions);
          }
          ICopyDictionary copyenv = mgr.CopyParts(parts, null);
          copysel = copyenv.Copies;
        } catch (Exception ex) {
          Diagram.Trace(ex.ToString());
          //throw;
        }
        if (copysel != null) {
          CommandHandler.InternalClipboard = copysel;
          try {
            copysel.Model = null;  // try to avoid serializing the model
            CopyToClipboard(copysel);
          } catch (Exception ex) {
            Diagram.Trace(ex.ToString());
          }
        }
      } finally {
        diagram.Cursor = null;
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Copy</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true
    /// if <see cref="Northwoods.GoXam.Diagram.AllowCopy"/> is true,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowClipboard"/> is true,
    /// and if there are some selected parts (<see cref="Northwoods.GoXam.Diagram.SelectedParts"/>).
    /// </returns>
    public virtual bool CanCopy() {
      Diagram diagram = this.Diagram;
      return (diagram != null && diagram.AllowCopy && diagram.AllowClipboard && diagram.SelectedParts.Count > 0 &&
              diagram.Model != null && diagram.Model.DataFormat != "");
    }

    /// <summary>
    /// Identifies the <see cref="CopyingInclusions"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CopyingInclusionsProperty;
    /// <summary>
    /// Gets or sets whether <see cref="Copy"/> deletes only the selected parts
    /// or subtrees of selected nodes as well.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Tool.EffectiveCollectionInclusions.Standard"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// When set to <see cref="Northwoods.GoXam.Tool.EffectiveCollectionInclusions.SubTree"/>,
    /// <see cref="Copy"/> will augment the collection of
    /// selected nodes to include their tree children nodes.
    /// The resulting collection often will include many <see cref="Node"/>s and
    /// <see cref="Link"/>s that are not <see cref="Part.IsSelected"/>.
    /// </para>
    /// <para>
    /// Links are assumed to go from the parent node to the children nodes,
    /// unless <see cref="Northwoods.GoXam.Diagram.TreePath"/> is set to "Source".
    /// </para>
    /// <para>
    /// If you set this property, you might also want to set the
    /// <see cref="DeletingInclusions"/> property and the
    /// <see cref="Northwoods.GoXam.Tool.DraggingTool.Inclusions"/> property.
    /// </para>
    /// </remarks>
    public EffectiveCollectionInclusions CopyingInclusions {
      get { return (EffectiveCollectionInclusions)GetValue(CopyingInclusionsProperty); }
      set { SetValue(CopyingInclusionsProperty, value); }
    }


    /// <summary>
    /// Execute a <see cref="Copy"/> followed by a <see cref="Delete"/>.
    /// </summary>
    public virtual void Cut() {
      Copy();
      Delete();
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Cut</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowCopy"/> is true,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowDelete"/> is true,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowClipboard"/> is true,
    /// if there are some selected parts (<see cref="Northwoods.GoXam.Diagram.SelectedParts"/>),
    /// and if the model is <see cref="IDiagramModel.Modifiable"/>.
    /// </returns>
    public virtual bool CanCut() {
      Diagram diagram = this.Diagram;
      return (diagram != null && !diagram.IsReadOnly && diagram.AllowCopy && diagram.AllowDelete && diagram.AllowClipboard && diagram.SelectedParts.Count > 0 &&
              diagram.Model != null && diagram.Model.Modifiable);
    }


    /// <summary>
    /// This writes the given data collection to the clipboard
    /// using the diagram's model's <see cref="IDiagramModel.DataFormat"/>.
    /// </summary>
    /// <param name="coll"></param>
    protected virtual void CopyToClipboard(IDataCollection coll) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      Clipboard.SetData(model.DataFormat, coll);



      UpdateCommands();
    }

    /// <summary>
    /// This reads a data collection from the clipboard
    /// using the diagram's model's <see cref="IDiagramModel.DataFormat"/>.
    /// </summary>
    /// <returns>null if the Clipboard does not contain any data of the given format</returns>
    protected virtual IDataCollection PasteFromClipboard() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      IDiagramModel model = diagram.Model;
      if (model == null) return null;
      if (!Clipboard.ContainsData(model.DataFormat)) return null;
      IDataCollection sel = Clipboard.GetData(model.DataFormat) as IDataCollection;
      return sel;
    }


    /// <summary>
    /// Copy the contents of the clipboard as new nodes and links in this diagram,
    /// and make those new parts the new selection.
    /// </summary>
    /// <remarks>
    /// This calls <see cref="PasteFromClipboard"/> to get a collection of nodes and links,
    /// copies them into this <see cref="Diagram"/>'s model
    /// (by calling <see cref="IDiagramModel.AddCollectionCopy"/>),
    /// and then selects all of the newly created parts.
    /// This also raises the <see cref="Northwoods.GoXam.Diagram.ClipboardPastedEvent"/>.
    /// All of the changes are performed in an undoable edit.
    /// </remarks>
    public virtual void Paste() {
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return;
      try {
        diagram.Cursor = Cursors.Wait;
        String format = model.DataFormat;
        // get the data from the clipboard
        IDataCollection sel = null;
        try {
          sel = PasteFromClipboard();
        } catch (Exception ex) {
          Diagram.Trace(ex.ToString());
          sel = CommandHandler.InternalClipboard;
        }
        if (sel != null) { // if no data, do nothing
          try {
            model.StartTransaction("Paste");
            // wrap a model around the data
            IDiagramModel clipmodel = model.CreateInitializedCopy(sel);
            clipmodel.Name = "clipboard";
            sel.Model = clipmodel;  // restore Model property
            // copy everything into this diagram's model
            ICopyDictionary copyenv = model.AddCollectionCopy(sel, null);
            // select all the new nodes and links
            mgr.SelectData(copyenv.Copies);
            RaiseEvent(Diagram.ClipboardPastedEvent);
          } catch (Exception ex) {
            Diagram.Trace(ex.ToString());
            throw;
          } finally {
            model.CommitTransaction("Paste");
          }
        }
      } finally {
        diagram.Cursor = null;
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Paste</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowInsert"/> is true,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowClipboard"/> is true,
    /// if the clipboard has data of the format given by the diagram's model's <see cref="IDiagramModel.DataFormat"/>,
    /// and if the model is <see cref="IDiagramModel.Modifiable"/>.
    /// </returns>
    public virtual bool CanPaste() {
      Diagram diagram = this.Diagram;
      bool can = (diagram != null && !diagram.IsReadOnly && diagram.AllowInsert && diagram.AllowClipboard);
      if (diagram != null) {
        IDiagramModel model = diagram.Model;
        can &= (model != null && model.Modifiable &&
                (CommandHandler.InternalClipboard != null || (model.DataFormat != "" && Clipboard.ContainsData(model.DataFormat))));
      }
      return can;
    }


    // undo commands

    /// <summary>
    /// Call <see cref="Diagram"/>.<see cref="Northwoods.GoXam.Diagram.Model"/>.<see cref="IDiagramModel.UndoManager"/>.<see cref="UndoManager.Undo"/>.
    /// </summary>
    /// <remarks>
    /// This also tries to scroll the diagram to the primary edited object.
    /// </remarks>
    public virtual void Undo() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return;
      UndoManager um = model.UndoManager;
      if (um == null) return;
      //Object data = um.FindPrimaryObject(um.EditToUndo);
      um.Undo();
      //?? isn't reliable in scrolling to the desired spot
      //if (data != null) {
      //  Part part = mgr.FindNodeForData(data, model);
      //  if (part == null) part = mgr.FindLinkForData(data, model);
      //  if (part != null) diagram.Panel.MakeVisible(part, Rect.Empty);
      //}
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Undo</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowUndo"/> is true,
    /// and if the diagram's model's <see cref="UndoManager"/> <see cref="UndoManager.CanUndo"/>
    /// predicate returns true.
    /// </returns>
    public virtual bool CanUndo() {
      Diagram diagram = this.Diagram;
      bool can = (diagram != null && !diagram.IsReadOnly && diagram.AllowUndo);
      if (diagram != null) {
        IDiagramModel model = diagram.Model;
        can &= (model != null && model.UndoManager != null && model.UndoManager.CanUndo());
      }
      return can;
    }


    /// <summary>
    /// Call <see cref="Diagram"/>.<see cref="Northwoods.GoXam.Diagram.Model"/>.<see cref="IDiagramModel.UndoManager"/>.<see cref="UndoManager.Redo"/>.
    /// </summary>
    /// <remarks>
    /// This also tries to scroll the diagram to the primary edited object.
    /// </remarks>
    public virtual void Redo() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return;
      UndoManager um = model.UndoManager;
      if (um == null) return;
      //Object data = um.FindPrimaryObject(um.EditToUndo);
      um.Redo();
      //?? isn't reliable in scrolling to the desired spot
      //if (data != null) {
      //  Part part = mgr.FindNodeForData(data, model);
      //  if (part == null) part = mgr.FindLinkForData(data, model);
      //  if (part != null) diagram.Panel.MakeVisible(part, Rect.Empty);
      //}
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Redo</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowUndo"/> is true,
    /// and if the diagram's model's <see cref="UndoManager"/> <see cref="UndoManager.CanRedo"/>
    /// predicate returns true.
    /// </returns>
    public virtual bool CanRedo() {
      Diagram diagram = this.Diagram;
      bool can = (diagram != null && !diagram.IsReadOnly && diagram.AllowUndo);
      if (diagram != null) {
        IDiagramModel model = diagram.Model;
        can &= (model != null && model.UndoManager != null && model.UndoManager.CanRedo());
      }
      return can;
    }


    // zooming commands

    /// <summary>
    /// Decrease the <see cref="Diagram"/>.<see cref="Northwoods.GoXam.Diagram.Panel"/>.<see cref="DiagramPanel.Scale"/>
    /// by the given factor.
    /// </summary>
    /// <param name="param">
    /// The factor by which to decrease the zoom.
    /// This should be less than one but greater than zero; it defaults to 5% (1/1.05).
    /// </param>
    public virtual void DecreaseZoom(Object param) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      double factor = ConvertToDoubleScale(param, 1/1.05);
      double newscale = panel.Scale*factor;
      if (newscale < panel.MinimumScale || newscale > panel.MaximumScale) return;
      panel.DisableOffscreen = true;  // disable removing nodes offscreen
      panel.Scale = newscale;
      panel.DisableOffscreen = false;
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>DecreaseZoom</c> command is executed.
    /// </summary>
    /// <param name="param">the factor by which to decrease the zoom; this defaults to 5% (1/1.05)</param>
    /// <returns>
    /// by default, this returns true if <see cref="Northwoods.GoXam.Diagram.AllowZoom"/> is true,
    /// and if the <paramref name="param"/> is a value greater than zero.
    /// </returns>
    public virtual bool CanDecreaseZoom(Object param) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      double factor = ConvertToDoubleScale(param, 1/1.05);
      if (factor <= 0) return false;
      double newscale = panel.Scale*factor;
      if (newscale < panel.MinimumScale || newscale > panel.MaximumScale) return false;
      return diagram.AllowZoom;
    }


    /// <summary>
    /// Increase the <see cref="Diagram"/>.<see cref="Northwoods.GoXam.Diagram.Panel"/>.<see cref="DiagramPanel.Scale"/>
    /// by the given factor.
    /// </summary>
    /// <param name="param">
    /// The factor by which to increase the zoom.
    /// This should be greater than one; it defaults to 5% (1.05).
    /// </param>
    public virtual void IncreaseZoom(Object param) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      double factor = ConvertToDoubleScale(param, 1.05);
      double newscale = panel.Scale*factor;
      if (newscale < panel.MinimumScale || newscale > panel.MaximumScale) return;
      panel.DisableOffscreen = true;  // disable removing nodes offscreen
      panel.Scale = newscale;
      panel.DisableOffscreen = false;
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>IncreaseZoom</c> command is executed.
    /// </summary>
    /// <param name="param">the factor by which to increase the zoom; this defaults to 5% (1.05)</param>
    /// <returns>
    /// by default, this returns true if <see cref="Northwoods.GoXam.Diagram.AllowZoom"/> is true,
    /// and if the <paramref name="param"/> is a value greater than zero.
    /// </returns>
    public virtual bool CanIncreaseZoom(Object param) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      double factor = ConvertToDoubleScale(param, 1.05);
      if (factor <= 0) return false;
      double newscale = panel.Scale*factor;
      if (newscale < panel.MinimumScale || newscale > panel.MaximumScale) return false;
      return diagram.AllowZoom;
    }


    /// <summary>
    /// Set the <see cref="DiagramPanel.Scale"/> property of the <see cref="Northwoods.GoXam.Diagram.Panel"/>
    /// to the given value.
    /// </summary>
    /// <param name="param">
    /// The new scale value.
    /// This should be greater than zero; it defaults to 1.
    /// </param>
    public virtual void Zoom(Object param) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      double newscale = ConvertToDoubleScale(param, 1);
      if (newscale < panel.MinimumScale || newscale > panel.MaximumScale) return;
      panel.Scale = newscale;
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Zoom</c> command is executed.
    /// </summary>
    /// <param name="param">the new <see cref="DiagramPanel.Scale"/> value; this defaults to 1</param>
    /// <returns>
    /// by default, this returns true if <see cref="Northwoods.GoXam.Diagram.AllowZoom"/> is true,
    /// and if the <paramref name="param"/> is a value greater than zero.
    /// </returns>
    public virtual bool CanZoom(Object param) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      double newscale = ConvertToDoubleScale(param, 1);
      if (newscale < panel.MinimumScale || newscale > panel.MaximumScale) return false;
      return diagram.AllowZoom;
    }

    private static double ConvertToDoubleScale(Object param, double defval) {
      double scale = defval;
      try {
        if (param == null) {
          return defval;
        } else if (param is double) {
          scale = (double)param;
        } else if (param is float) {
          scale = (float)param;
        } else if (param is int) {
          scale = (int)param;
        } else if (param is String) {
          scale = Double.Parse((String)param, System.Globalization.CultureInfo.InvariantCulture);
        } else {
          scale = System.Convert.ToDouble(param, System.Globalization.CultureInfo.InvariantCulture);
        }
      } catch (Exception) {
        return defval;
      }
      if (Double.IsNaN(scale)) return defval;
      return scale;
    }


    //public virtual void ScrollPageLeft() {
    //  Diagram diagram = this.Diagram;
    //  if (diagram == null) return;
    //  DiagramPanel panel = diagram.Panel;
    //  if (panel == null) return;
    //  panel.PageLeft();
    //}

    //public virtual bool CanScrollPageLeft() {
    //  Diagram diagram = this.Diagram;
    //  return (diagram != null && diagram.AllowScroll &&
    //    diagram.Panel != null && diagram.Panel.CanHorizontallyScroll);
    //}

    //public virtual void ScrollPageRight() {
    //  Diagram diagram = this.Diagram;
    //  if (diagram == null) return;
    //  DiagramPanel panel = diagram.Panel;
    //  if (panel == null) return;
    //  panel.PageRight();
    //}

    //public virtual bool CanScrollPageRight() {
    //  Diagram diagram = this.Diagram;
    //  return (diagram != null && diagram.AllowScroll &&
    //    diagram.Panel != null && diagram.Panel.CanHorizontallyScroll);
    //}


    //?? ExpandTree, ExpandTreeAll, CollapseTree

    // group commands

    //?? ExpandGroup, ExpandGroupAll, CollapseGroup

    //?? if this were only an IGroupsModel, this would not "reparent" the selected nodes to be members of the new group

    /// <summary>
    /// Create a new group node and add the selected parts to that new group.
    /// </summary>
    /// <remarks>
    /// This creates a new group node by adding a copy of the <see cref="PrototypeGroup"/> data
    /// to the model, which must support the <see cref="Northwoods.GoXam.Model.ISubGraphModel"/> interface.
    /// Each of the selected nodes for which <see cref="Part.CanGroup"/> is true
    /// is made a member of that new group node.
    /// If all of the selected groupable nodes were members of a pre-existing group node,
    /// the new group node also becomes a member of that pre-existing group.
    /// The new group node becomes the only selected part.
    /// This also raises the <see cref="Northwoods.GoXam.Diagram.SelectionGrouped"/>.
    /// All of the changes are performed in an undoable edit.
    /// </remarks>
    public virtual void Group() {
      if (this.PrototypeGroup == null) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      
      // find the first eligible node
      Node firstnode = diagram.SelectedParts.OfType<Node>().FirstOrDefault(n => n.CanGroup());
      if (firstnode == null) return;
      // and find its model, which should be an ISubGraphModel
      ISubGraphModel model = firstnode.Model as ISubGraphModel;
      if (model == null || !model.Modifiable) return;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return;

      try {
        diagram.Cursor = Cursors.Wait;
        model.StartTransaction("Group");
        // get all of the selected nodes that can be grouped and that are in the subgraph model
        //?? should this check IGroupsModel.IsMemberValid -- but the PrototypeGroup hasn't been added to the model yet,
        //   AND can't do so until we have selected the nodes to be grouped!
        IEnumerable<Node> selnodes = diagram.SelectedParts.OfType<Node>().Where(n => n.CanGroup() && model.IsNodeData(n.Data)).ToList();
        // throw out selected nodes that are part of other selected nodes
        IEnumerable<Node> nodes = selnodes.Where(n => selnodes.All(m => !n.IsContainedBy(m))).ToList();
        // find lowest common containing group for selected nodes
        Node firstsg = firstnode.ContainingSubGraph;
        if (firstsg != null) {
          IEnumerable<Node> rest = nodes.Skip(1).ToList();
          while (firstsg != null && !rest.All(n => n.IsContainedBy(firstsg))) {
            firstsg = firstsg.ContainingSubGraph;
          }
        }
        // create new group
        Object newgroup = model.AddNodeCopy(this.PrototypeGroup);
        if (firstsg != null) {
          // make it a member of an existing group
          model.SetGroupNode(newgroup, firstsg.Data);
        }
        foreach (Node n in nodes) {
          model.SetGroupNode(n.Data, newgroup);
        }
        Node newnode = mgr.FindNodeForData(newgroup, model);
        if (newnode != null) {
          diagram.Select(newnode);
        }
        RaiseEvent(Diagram.SelectionGroupedEvent);
      } finally {
        model.CommitTransaction("Group");
        diagram.Cursor = null;
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Group</c> command is executed.
    /// </summary>
    /// <returns>
    /// By default, this returns true:
    /// if the <see cref="PrototypeGroup"/> data is not null,
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowInsert"/> is true,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowGroup"/> is true,
    /// if the model is an <see cref="ISubGraphModel"/> that is modifiable, and
    /// if there are any selected nodes that can be <see cref="Part.CanGroup"/>'ed.
    /// </returns>
    public virtual bool CanGroup() {
      if (this.PrototypeGroup == null) return false;
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly || !diagram.AllowInsert || !diagram.AllowGroup) return false;
      Node firstnode = diagram.SelectedParts.OfType<Node>().FirstOrDefault(n => n.CanGroup());
      if (firstnode == null) return false;
      ISubGraphModel model = firstnode.Model as ISubGraphModel;
      if (model == null || !model.Modifiable) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="PrototypeGroup"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PrototypeGroupProperty;
    /// <summary>
    /// Gets or sets a data value to be copied for a new group node by <see cref="Group"/>.
    /// </summary>
    /// <value>
    /// The default value is null, which causes <see cref="CanGroup"/> predicate to return false.
    /// </value>
    public Object PrototypeGroup {
      get { return GetValue(PrototypeGroupProperty); }
      set { SetValue(PrototypeGroupProperty, value); }
    }


    // if this were an IGroupsModel, this would not change the membership of the selected nodes in other groups

    /// <summary>
    /// For each selected node that is a group, remove the group without removing its members from the diagram.
    /// </summary>
    /// <remarks>
    /// For each selected node that is a <see cref="Northwoods.GoXam.Group"/> and that is <see cref="Northwoods.GoXam.Group.Ungroupable"/>,
    /// change all of their member nodes to be members of the group that the selected group node is in.
    /// (If the selected group node is a top-level node, i.e. not a member of any group node,
    /// its members become top-level nodes too.)
    /// All of those selected group nodes are deleted.
    /// All of the reparented member nodes are selected.
    /// This also raises the <see cref="Northwoods.GoXam.Diagram.SelectionUngroupedEvent"/>.
    /// All of the changes are performed in an undoable edit.
    /// </remarks>
    public virtual void Ungroup() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      ISubGraphModel model = diagram.Model as ISubGraphModel;
      if (model == null) return;

      try {
        diagram.Cursor = Cursors.Wait;
        model.StartTransaction("Ungroup");
        // find all selected group nodes that are ungroupable
        IEnumerable<Group> groups = diagram.SelectedParts.OfType<Group>().Where(n => n.CanUngroup()).ToList();
        diagram.ClearSelection();
        foreach (Group sg in groups) {
          ISubGraphModel gmodel = sg.Model as ISubGraphModel;
          // iterate over the member nodes and reassign their GroupSubGraph to be SG's original containing group
          Node group = sg.ContainingSubGraph;
          foreach (Node member in sg.MemberNodes) {
            gmodel.SetGroupNode(member.Data, (group != null ? group.Data : null));
            member.IsSelected = true;
          }
          // then delete the empty group
          gmodel.RemoveNode(sg.Data);
        }
        RaiseEvent(Diagram.SelectionUngroupedEvent);
      } finally {
        model.CommitTransaction("Ungroup");
        diagram.Cursor = null;
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Ungroup</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true 
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowDelete"/> is true,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowUngroup"/> is true,
    /// if the model is an <see cref="ISubGraphModel"/> that is modifiable, and
    /// if there are any selected <see cref="Northwoods.GoXam.Group"/>s that can be <see cref="Northwoods.GoXam.Group.CanUngroup"/>'ed.
    /// </returns>
    public virtual bool CanUngroup() {
      Diagram diagram = this.Diagram;
      return (diagram != null && !diagram.IsReadOnly && diagram.AllowDelete && diagram.AllowUngroup &&
        diagram.Model is ISubGraphModel && diagram.Model.Modifiable &&
        diagram.SelectedParts.OfType<Group>().Any(n => n.CanUngroup()));
    }


    /// <summary>
    /// Start in-place editing of the first <c>TextBlock</c> of the selected part.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public virtual void Edit() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;

      // find the first eligible node
      Part firstpart = diagram.SelectedParts.FirstOrDefault(n => n.CanEdit());
      if (firstpart == null) return;

      TextEditingTool tool = diagram.TextEditingTool;
      if (tool != null) {
        TextBlock tb = Part.FindElementDownFrom(firstpart.VisualElement, e => e is TextBlock && Part.GetTextEditable(e)) as TextBlock;
        if (tb != null) {
          tool.TextBlock = tb;
          diagram.CurrentTool = tool;
        } else {







        }
      }
    }

    /// <summary>
    /// This overridable predicate controls whether or not the <c>Edit</c> command is executed.
    /// </summary>
    /// <returns>
    /// By default, this returns true:
    /// if the diagram is not <see cref="Northwoods.GoXam.Diagram.IsReadOnly"/>,
    /// if <see cref="Northwoods.GoXam.Diagram.AllowEdit"/> is true,
    /// if there are any selected parts that can be <see cref="Part.CanEdit"/>'ed,
    /// and if there is a <see cref="Northwoods.GoXam.Diagram.TextEditingTool"/>.
    /// </returns>
    public virtual bool CanEdit() {
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly || !diagram.AllowEdit) return false;
      Part firstpart = diagram.SelectedParts.FirstOrDefault(n => n.CanEdit());
      if (firstpart == null) return false;
      if (diagram.TextEditingTool == null) return false;
      return true;
    }


    // print commands

    /// <summary>
    /// Print the diagram by showing a <c>PrintDialog</c> and then calling the
    /// <see cref="Northwoods.GoXam.Diagram.PrintManager"/>'s <see cref="PrintManager.Print"/> method.
    /// </summary>
    /// <remarks>
    /// This method does nothing in Silverlight 3.
    /// </remarks>
    public virtual void Print() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      try {

      PrintManager mgr = diagram.PrintManager;
      if (mgr == null) return;
      mgr.Print();






      } catch (Exception ex) {
        Diagram.Error(ex.Message);
      }
    }


    /// <summary>
    /// This overridable predicate controls whether or not the <c>Print</c> command is executed.
    /// </summary>
    /// <returns>
    /// by default, this returns true if <see cref="Northwoods.GoXam.Diagram.AllowPrint"/> is true.
    /// </returns>
    /// <remarks>
    /// This predicate returns false in Silverlight 3.
    /// </remarks>
    public virtual bool CanPrint() {



      Diagram diagram = this.Diagram;
      return (diagram != null && diagram.AllowPrint && diagram.PrintManager != null);

    }


    //public virtual void PrintPreview() {  //?? NYI
    //  Diagram diagram = this.Diagram;
    //  if (diagram == null) return;
    //}

    ///// <summary>
    ///// This overridable predicate controls whether or not the <c>PrintPreview"/> command is executed.
    ///// </summary>
    ///// <returns>
    ///// by default, this returns true if <see cref="Northwoods.GoXam.Diagram.AllowPrint"/> is true.
    ///// </returns>
    //public virtual bool CanPrintPreview() {
    //  Diagram diagram = this.Diagram;
    //  return (diagram != null && diagram.AllowPrint && diagram.PrintManager != null);
    //}


    // Non-routed commands for both Silverlight and WPF

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Copy"/> and is enabled by <see cref="CanCopy"/>.
    /// </summary>
    public ICommand CopyCommand {
      get {
        if (_CopyCommand == null) _CopyCommand = new ActionCommand(Copy, CanCopy);  // AllowCopy, AllowClipboard
        return _CopyCommand;
      }
    }
    private ICommand _CopyCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Cut"/> and is enabled by <see cref="CanCut"/>.
    /// </summary>
    public ICommand CutCommand {  // AllowCopy, AllowDelete, AllowClipboard
      get {
        if (_CutCommand == null) _CutCommand = new ActionCommand(Cut, CanCut);
        return _CutCommand;
      }
    }
    private ICommand _CutCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Delete"/> and is enabled by <see cref="CanDelete"/>.
    /// </summary>
    public ICommand DeleteCommand {  // AllowDelete
      get {
        if (_DeleteCommand == null) _DeleteCommand = new ActionCommand(Delete, CanDelete);
        return _DeleteCommand;
      }
    }
    private ICommand _DeleteCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Paste"/> and is enabled by <see cref="CanPaste"/>.
    /// </summary>
    public ICommand PasteCommand {  // AllowInsert, AllowClipboard
      get {
        if (_PasteCommand == null) _PasteCommand = new ActionCommand(Paste, CanPaste);
        return _PasteCommand;
      }
    }
    private ICommand _PasteCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Print"/> and is enabled by <see cref="CanPrint"/>.
    /// </summary>
    public ICommand PrintCommand {  // AllowPrint
      get {
        if (_PrintCommand == null) _PrintCommand = new ActionCommand(Print, CanPrint);
        return _PrintCommand;
      }
    }
    private ICommand _PrintCommand;

    ///// <summary>
    ///// Gets a non-routed <c>ICommand</c> that executes <see cref="PrintPreview"/> and is enabled by <see cref="CanPrintPreview"/>.
    ///// </summary>
    //public ICommand PrintPreviewCommand {  // AllowPrint
    //  get {
    //    if (_PrintPreviewCommand == null) _PrintPreviewCommand = new ActionCommand(PrintPreview, CanPrintPreview);
    //    return _PrintPreviewCommand;
    //  }
    //}
    //private ICommand _PrintPreviewCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Redo"/> and is enabled by <see cref="CanRedo"/>.
    /// </summary>
    public ICommand RedoCommand {  // AllowUndo
      get {
        if (_RedoCommand == null) _RedoCommand = new ActionCommand(Redo, CanRedo);
        return _RedoCommand;
      }
    }
    private ICommand _RedoCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="SelectAll"/> and is enabled by <see cref="CanSelectAll"/>.
    /// </summary>
    public ICommand SelectAllCommand {  // AllowSelect
      get {
        if (_SelectAllCommand == null) _SelectAllCommand = new ActionCommand(SelectAll, CanSelectAll);
        return _SelectAllCommand;
      }
    }
    private ICommand _SelectAllCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Undo"/> and is enabled by <see cref="CanUndo"/>.
    /// </summary>
    public ICommand UndoCommand {  // AllowUndo
      get {
        if (_UndoCommand == null) _UndoCommand = new ActionCommand(Undo, CanUndo);
        return _UndoCommand;
      }
    }
    private ICommand _UndoCommand;

    /// <summary>
    /// Gets a non-routed parameterized <c>ICommand</c> that executes <see cref="DecreaseZoom"/> and is enabled by <see cref="CanDecreaseZoom"/>.
    /// </summary>
    public ICommand DecreaseZoomCommand {  // AllowZoom
      get {
        if (_DecreaseZoomCommand == null) _DecreaseZoomCommand = new ParameterizedCommand(DecreaseZoom, CanDecreaseZoom);
        return _DecreaseZoomCommand;
      }
    }
    private ICommand _DecreaseZoomCommand;

    /// <summary>
    /// Gets a non-routed parameterized <c>ICommand</c> that executes <see cref="IncreaseZoom"/> and is enabled by <see cref="CanIncreaseZoom"/>.
    /// </summary>
    public ICommand IncreaseZoomCommand {  // AllowZoom
      get {
        if (_IncreaseZoomCommand == null) _IncreaseZoomCommand = new ParameterizedCommand(IncreaseZoom, CanIncreaseZoom);
        return _IncreaseZoomCommand;
      }
    }
    private ICommand _IncreaseZoomCommand;

    /// <summary>
    /// Gets a non-routed parameterized <c>ICommand</c> that executes <see cref="Zoom"/> and is enabled by <see cref="CanZoom"/>.
    /// </summary>
    public ICommand ZoomCommand {  // AllowZoom
      get {
        if (_ZoomCommand == null) _ZoomCommand = new ParameterizedCommand(Zoom, CanZoom);
        return _ZoomCommand;
      }
    }
    private ICommand _ZoomCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Group"/> and is enabled by <see cref="CanGroup"/>.
    /// </summary>
    public ICommand GroupCommand {  // AllowInsert, AllowGroup
      get {
        if (_GroupCommand == null) _GroupCommand = new ActionCommand(Group, CanGroup);
        return _GroupCommand;
      }
    }
    private ICommand _GroupCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Ungroup"/> and is enabled by <see cref="CanUngroup"/>.
    /// </summary>
    public ICommand UngroupCommand {  // AllowDelete, AllowGroup
      get {
        if (_UngroupCommand == null) _UngroupCommand = new ActionCommand(Ungroup, CanUngroup);
        return _UngroupCommand;
      }
    }
    private ICommand _UngroupCommand;

    /// <summary>
    /// Gets a non-routed <c>ICommand</c> that executes <see cref="Edit"/> and is enabled by <see cref="CanEdit"/>.
    /// </summary>
    public ICommand EditCommand {  // AllowEdit
      get {
        if (_EditCommand == null) _EditCommand = new ActionCommand(Edit, CanEdit);
        return _EditCommand;
      }
    }
    private ICommand _EditCommand;

    internal void UpdateCommands() {
      if (_CopyCommand != null) ((ActionCommand)_CopyCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_CutCommand != null) ((ActionCommand)_CutCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_DeleteCommand != null) ((ActionCommand)_DeleteCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_PasteCommand != null) ((ActionCommand)_PasteCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_PrintCommand != null) ((ActionCommand)_PrintCommand).RaiseCanExecuteChanged(this.Diagram);
      //if (_PrintPreviewCommand != null) ((ActionCommand)_PrintPreviewCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_RedoCommand != null) ((ActionCommand)_RedoCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_SelectAllCommand != null) ((ActionCommand)_SelectAllCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_UndoCommand != null) ((ActionCommand)_UndoCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_DecreaseZoomCommand != null) ((ParameterizedCommand)_DecreaseZoomCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_IncreaseZoomCommand != null) ((ParameterizedCommand)_IncreaseZoomCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_ZoomCommand != null) ((ParameterizedCommand)_ZoomCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_GroupCommand != null) ((ActionCommand)_GroupCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_UngroupCommand != null) ((ActionCommand)_UngroupCommand).RaiseCanExecuteChanged(this.Diagram);
      if (_EditCommand != null) ((ActionCommand)_EditCommand).RaiseCanExecuteChanged(this.Diagram);
    }
  }


  internal class ActionCommand : ICommand {
    public ActionCommand(Action action, Func<bool> predicate) {
      _Action = action;
      _Predicate = predicate;
    }

    private Action _Action;
    private Func<bool> _Predicate;

    public bool CanExecute(object parameter) {
      return _Predicate();
    }

    public event EventHandler CanExecuteChanged;

    internal void RaiseCanExecuteChanged(Object sender) {
      if (this.CanExecuteChanged != null) this.CanExecuteChanged(sender, EventArgs.Empty);
    }

    public void Execute(object parameter) {
      _Action();
    }
  }

  internal class ParameterizedCommand : ICommand {
    public ParameterizedCommand(Action<object> action, Func<object, bool> predicate) {
      _Action = action;
      _Predicate = predicate;
    }

    private Action<object> _Action;
    private Func<object, bool> _Predicate;

    public bool CanExecute(object parameter) {
      return _Predicate(parameter);
    }

    public event EventHandler CanExecuteChanged;

    internal void RaiseCanExecuteChanged(Object sender) {
      if (this.CanExecuteChanged != null) this.CanExecuteChanged(sender, EventArgs.Empty);
    }

    public void Execute(object parameter) {
      _Action(parameter);
    }
  }















































































































}
