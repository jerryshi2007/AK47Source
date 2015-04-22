
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
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Northwoods.GoXam {

  /// <summary>
  /// This <c>RoutedEventArgs</c> class adds an optional <see cref="Part"/> property.
  /// </summary>
  public class DiagramEventArgs : RoutedEventArgs {
    /// <summary>
    /// Create an empty <see cref="DiagramEventArgs"/>.
    /// </summary>
    public DiagramEventArgs() { }

    /// <summary>
    /// Create a <see cref="DiagramEventArgs"/> that refers to a <see cref="Northwoods.GoXam.Part"/>.
    /// </summary>
    /// <param name="p">a <see cref="Northwoods.GoXam.Part"/></param>
    public DiagramEventArgs(Part p) { this.Part = p; }

    /// <summary>
    /// Create a <see cref="DiagramEventArgs"/> that refers to a <see cref="Northwoods.GoXam.Part"/>
    /// and a specific <c>FrameworkElement</c> that was the subject of the event.
    /// </summary>
    /// <param name="p">a <see cref="Northwoods.GoXam.Part"/></param>
    /// <param name="e">a <c>FrameworkElement</c></param>
    public DiagramEventArgs(Part p, FrameworkElement e) { this.Part = p; this.Element = e; }

    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.Part"/>
    /// that was acted upon and that is the subject of the event.
    /// </summary>
    public Part Part { get; set; }

    /// <summary>
    /// Gets or sets the specific <c>FrameworkElement</c>
    /// that was acted upon and that is the subject of the event.
    /// </summary>
    public FrameworkElement Element { get; set; }


    internal int RoutedEvent { get; set; }

    /// <summary>
    /// Gets or sets whether this event was handled or is considered cancelled.
    /// </summary>
    public bool Handled { get; set; }

  }

}
