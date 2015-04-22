
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
using System.Collections.ObjectModel;
using System.Windows;
using Northwoods.GoXam.Layout;

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
  /// It also re-layouts automatically whenever the viewport size changes,
  /// because the size of the <see cref="DiagramPanel"/> changes or because the scale changes.
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
    /// This override replaces the default <see cref="Diagram.Layout"/> with a <see cref="GridLayout"/>
    /// that executes under <see cref="DiagramLayout.Conditions"/> that include <see cref="LayoutChange.ViewportSizeChanged"/>.
    /// </summary>
    public override void OnApplyTemplate() {
      // use a different default Layout than what the Diagram constructor might set
      if (this.Layout == null || this.Layout.GetType() == typeof(Northwoods.GoXam.Layout.DiagramLayout)) {
        var layout = new GridLayout();
        layout.Conditions = LayoutChange.Standard | LayoutChange.ViewportSizeChanged;
        this.Layout = layout;
      } else {
        DiagramLayout layout = this.Layout as DiagramLayout;
        if (layout != null) {
          layout.Conditions |= LayoutChange.ViewportSizeChanged;
        }
      }
      base.OnApplyTemplate();
    }
  }
}
