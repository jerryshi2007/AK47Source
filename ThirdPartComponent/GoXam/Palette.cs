
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
using System.Collections.ObjectModel;
using System.Windows;

namespace Northwoods.GoXam {

  /// <summary>
  /// The <c>Palette</c> control is a <see cref="Diagram"/> that holds parts that can be dragged to a <see cref="Diagram"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You will need to initialize the palette's <see cref="Diagram.Model"/> with the node data with which
  /// you wish to populate the palette.  For example, for the (simple) palette:
  /// <code>
  ///   &lt;go:Palette x:Name="myPalette" NodeTemplate="..." /&gt;
  /// </code>
  /// you would need to perform something like the following at initialization:
  /// <code>
  ///   myPalette.Model.NodesSource = new String[] {
  ///     "Alpha", "Beta", "Gamma"
  ///   };
  /// </code>
  /// You also need to set <c>AllowDrop="True"</c> on the <see cref="Diagram"/>(s)
  /// into which users may drop the items from the palette.
  /// </para>
  /// <para>
  /// It is commonplace to have a different and probably simpler node <c>DataTemplate</c> for
  /// <see cref="Palette"/>s than for the destination <see cref="Diagram"/>s.
  /// </para>
  /// <para>
  /// By default a palette uses a <see cref="Northwoods.GoXam.Layout.GridLayout"/>
  /// as its <see cref="Diagram.Layout"/>.  That <c>GridLayout</c> defaults to sorting
  /// all of the nodes by the <see cref="Part.Text"/> (attached) property.
  /// </para>
  /// <para>
  /// The default style for a palette also makes it <see cref="Diagram.IsReadOnly"/>,
  /// and sets <see cref="Diagram.AllowDragOut"/> to be true, to permit drag-and-drop
  /// operations to start from this palette and to end on other diagrams.
  /// </para>
  /// </remarks>
  public class Palette : Diagram {
    /// <summary>
    /// Create an empty <see cref="Palette"/> control.
    /// </summary>
    public Palette() {

      this.DefaultStyleKey = typeof(Palette);

    }

    static Palette() {



    }

    /// <summary>
    /// This override establishes an event handler that notices when the <see cref="Palette"/>'s
    /// <see cref="DiagramPanel"/>'s <see cref="DiagramPanel.ViewportBounds"/> changes,
    /// which then causes the layout to be performed again.
    /// </summary>
    public override void OnApplyTemplate() {
      // use a different default Layout than what Diagram.OnApplyTemplate might set
      if (this.Layout == null) this.Layout = new Northwoods.GoXam.Layout.GridLayout();
      base.OnApplyTemplate();
      DiagramPanel panel = this.Panel;
      if (panel != null && !this.EventHandlerEstablished) {
        panel.ViewportBoundsChanged += Panel_ViewportBoundsChanged;
        this.EventHandlerEstablished = true;
      }
    }
    private bool EventHandlerEstablished { get; set; }

    private void Panel_ViewportBoundsChanged(Object sender, RoutedPropertyChangedEventArgs<Rect> e) {
      // only call LayoutDiagram when the size of the viewport has changed (in model coordinates),
      // because the panel changed size or because the panel.Scale changed
      if (!Geo.IsApprox(e.OldValue.Width, e.NewValue.Width) || !Geo.IsApprox(e.OldValue.Height, e.NewValue.Height)) {
        LayoutDiagram();
      }
    }

  }
}
