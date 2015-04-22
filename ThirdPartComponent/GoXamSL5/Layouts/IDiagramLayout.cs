
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
using System.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// This interface specifies the methods and properties that the <see cref="Northwoods.GoXam.LayoutManager"/>
  /// of the <see cref="Northwoods.GoXam.Diagram"/> uses to perform the positioning and routing
  /// of all of the nodes and links in the diagram.
  /// </summary>
  /// <remarks>
  /// All existing layouts are actually subclasses of the class <see cref="DiagramLayout"/>.
  /// </remarks>
  public interface IDiagramLayout {
    /// <summary>
    /// Gets or sets a reference to the owner <see cref="Northwoods.GoXam.Diagram"/>.
    /// </summary>
    Diagram Diagram { get; set; }

    /// <summary>
    /// Gets or sets a reference to the owner <see cref="Northwoods.GoXam.Group"/>, if any.
    /// </summary>
    Group Group { get; set; }

    /// <summary>
    /// Gets or sets an identifier for a particular layout.
    /// </summary>
    /// <remarks>
    /// This is used by <see cref="DiagramLayout.CanLayoutPart"/> to decide
    /// whether a part should participate in this layout, normally based on the
    /// part's <see cref="Part.LayoutId"/>.
    /// </remarks>
    String Id { get; set; }

    /// <summary>
    /// Gets or sets whether this layout is valid; if not, the <see cref="LayoutManager"/>
    /// will call <see cref="DoLayout"/> sometime in order to try to make the layout valid.
    /// </summary>
    /// <remarks>
    /// As parts are added, modified, or removed, or as layouts are replaced,
    /// <see cref="Invalidate"/> will be called, and depending on the conditions
    /// will set this property to false.
    /// </remarks>
    bool ValidLayout { get; set; }

    /// <summary>
    /// Declare that this layout might no longer be valid, depending on the reason for the change.
    /// </summary>
    /// <param name="reason">
    /// a <see cref="LayoutChange"/> hint describing what change may have made the layout invalid
    /// </param>
    /// <param name="part">
    /// an optional <see cref="Northwoods.GoXam.Part"/> that has changed
    /// </param>
    /// <remarks>
    /// Depending on the arguments and on the <see cref="DiagramLayout.Conditions"/>,
    /// this should set <see cref="ValidLayout"/> to false.
    /// </remarks>
    void Invalidate(LayoutChange reason, Part part);

    /// <summary>
    /// Decide whether a given <see cref="Northwoods.GoXam.Part"/> should participate in this layout.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>
    /// Typically this should return false if the part is not visible,
    /// if the part is in a temporary <see cref="Northwoods.GoXam.Layer"/>,
    /// or if the part's <see cref="Northwoods.GoXam.Part.LayoutId"/> is "None".
    /// This should return true if the part's <see cref="Part.LayoutId"/> is "All" or
    /// if it matches the <see cref="Id"/> of this layout.
    /// </returns>
    /// <remarks>
    /// The <see cref="LayoutManager"/> will use this predicate to decide which nodes and links
    /// to pass to <see cref="DoLayout"/>.
    /// </remarks>
    bool CanLayoutPart(Part part);

    /// <summary>
    /// Actually perform the layout computations--positioning the nodes and routing the links.
    /// </summary>
    /// <param name="nodes">the <see cref="Node"/>s to operate on</param>
    /// <param name="links">the <see cref="Link"/>s to operate on</param>
    void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links);
  }
}
