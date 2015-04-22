
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
using System.Windows;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// This provides an abstract view of a <see cref="IEnumerable{Part}"/> as a
  /// network (graph) of nodes and directed links.  These nodes and links correspond to
  /// <see cref="Part"/>s provided in the <see cref="IEnumerable{Part}"/>.
  /// This class provides a framework for manipulating the
  /// state of nodes and links without modifying the structure of the original document.
  /// </summary>
  public class TreeNetwork : GenericNetwork<TreeVertex, TreeEdge, TreeLayout> {
    /// <summary>
    /// Constructs an empty network.
    /// </summary>
    /// <remarks>
    /// Use this default constructor to create an empty network.
    /// Call <see cref="GenericNetwork{N, L, Y}.AddNodesAndLinks"/> to automatically add
    /// network nodes and links, or call <see cref="GenericNetwork{N, L, Y}.AddNode(Node)"/> and <see cref="GenericNetwork{N, L, Y}.LinkVertexes"/>
    /// explicitly to have more detailed control over the exact graph that is laid out.
    /// </remarks>
    public TreeNetwork() { }
  }

  /********************************************************************************************/

  /// <summary>
  /// Holds auto-layout specific link data.
  /// </summary>
  public class TreeEdge : TreeNetwork.Edge {
  
    //tree specific properties

    /// <summary>
    /// Commits the position of the link to the corresponding Link.
    /// </summary>
    /// <remarks>
    /// This routes the Link's Route.
    /// </remarks>
    public override void CommitPosition() {
      Route s = this.Route;
      if (s == null) return;
      if (s.Routing == LinkRouting.AvoidsNodes) return;

      TreeLayout layout = this.Network.Layout;
      TreeVertex parent;
      TreeVertex child;
      switch (layout.Path) {
        case TreePath.Destination: parent = this.FromVertex; child = this.ToVertex; break;
        case TreePath.Source: parent = this.ToVertex; child = this.FromVertex; break;
        default: throw new InvalidOperationException("Unhandled Path value " + layout.Path.ToString());
      }
      if (parent == null || child == null) return;

      Point p = this.RelativePoint;
      if (p.X == 0 && p.Y == 0 && !parent.RouteFirstRow) {  // no rows
        AdjustRouteForAngleChange(parent, child);
        return;
      }
      bool firstrow = (p.X == 0 && p.Y == 0 && parent.RouteFirstRow);
      Node node = parent.Node;
      Rect nodebounds = node.Bounds;
      double angle = TreeLayout.OrthoAngle(parent);
      double layerspacing = TreeLayout.ComputeLayerSpacing(parent);
      double rowspacing = parent.RowSpacing;
      s.UpdatePoints();
      bool bezier = s.Curve == LinkCurve.Bezier;
      bool ortho = s.Orthogonal;
      int idx;
      Point prev;
      Point next;
      Point last;
      if (ortho || bezier) {
        idx = 2;
        while (s.PointsCount > 4) s.RemovePoint(2);
        prev = s.GetPoint(1);
        next = s.GetPoint(2);
      } else {
        idx = 1;
        while (s.PointsCount > 3) s.RemovePoint(1);
        prev = s.GetPoint(0);
        next = s.GetPoint(s.PointsCount-1);
      }
      last = s.GetPoint(s.PointsCount-1);
      if (angle == 0) {
        double c;
        if (parent.Alignment == TreeAlignment.End) {
          // route around at Y coordinate relative to the bottom of the parent node
          c = nodebounds.Bottom + p.Y;
          // try to keep the links straight from the parent node -- consider room from RowIndent and NodeIndent
          if (p.Y == 0 && prev.Y > last.Y+parent.RowIndent) {
            c = Math.Min(c, Math.Max(prev.Y, c-TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.Alignment == TreeAlignment.Start) {
          // route around at Y coordinate relative to the top of the parent node
          c = nodebounds.Top + p.Y;
          // try to keep the links straight from the parent node -- consider room from RowIndent and NodeIndent
          if (p.Y == 0 && prev.Y < last.Y-parent.RowIndent) {
            c = Math.Max(c, Math.Min(prev.Y, c+TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.RouteAroundCentered || (parent.RouteAroundLastParent && parent.MaxGenerationCount == 1)) {
          c = nodebounds.Top - parent.SubtreeOffset.Y + p.Y;
        } else {
          c = nodebounds.Y+nodebounds.Height/2 + p.Y;
        }
        if (bezier) {  // curved segments
          if (!firstrow) {
            // add a straight curve at Y-coord C
            s.InsertPoint(idx, new Point(prev.X, c)); idx++;
            s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing, c)); idx++;
            s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing + (p.X - rowspacing)/3, c)); idx++;
            s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing + (p.X - rowspacing)*2/3, c)); idx++;
          } else {
            s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing + (p.X - rowspacing), c)); idx++;
          }
          s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing + (p.X - rowspacing), c)); idx++;
          s.InsertPoint(idx, new Point(next.X, c)); idx++;
        } else {  // straight line segments
          if (ortho) { s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing/2, prev.Y)); idx++; }
          s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing/2, c)); idx++;
          s.InsertPoint(idx, new Point(nodebounds.Right + layerspacing + p.X - (ortho ? rowspacing/2 : rowspacing), c)); idx++;
          if (ortho) { s.InsertPoint(idx, new Point(s.GetPoint(idx-1).X, next.Y)); idx++; }
        }
      } else if (angle == 90) {
        double c;
        if (parent.Alignment == TreeAlignment.End) {
          c = nodebounds.Right + p.X;
          if (p.X == 0 && prev.X > last.X+parent.RowIndent) {
            c = Math.Min(c, Math.Max(prev.X, c-TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.Alignment == TreeAlignment.Start) {
          c = nodebounds.Left + p.X;
          if (p.X == 0 && prev.X < last.X-parent.RowIndent) {
            c = Math.Max(c, Math.Min(prev.X, c+TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.RouteAroundCentered || (parent.RouteAroundLastParent && parent.MaxGenerationCount == 1)) {
          c = nodebounds.Left - parent.SubtreeOffset.X + p.X;
        } else {
          c = nodebounds.X+nodebounds.Width/2 + p.X;
        }
        if (bezier) {
          if (!firstrow) {
            s.InsertPoint(idx, new Point(c, prev.Y)); idx++;
            s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing)); idx++;
            s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing + (p.Y - rowspacing)/3)); idx++;
            s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing + (p.Y - rowspacing)*2/3)); idx++;
          } else {
            s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing + (p.Y - rowspacing))); idx++;
          }
          s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing + (p.Y - rowspacing))); idx++;
          s.InsertPoint(idx, new Point(c, next.Y)); idx++;
        } else {
          if (ortho) { s.InsertPoint(idx, new Point(prev.X, nodebounds.Bottom + layerspacing/2)); idx++; }
          s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing/2)); idx++;
          s.InsertPoint(idx, new Point(c, nodebounds.Bottom + layerspacing + p.Y - (ortho ? rowspacing/2 : rowspacing))); idx++;
          if (ortho) { s.InsertPoint(idx, new Point(next.X, s.GetPoint(idx-1).Y)); idx++; }
        }
      } else if (angle == 180) {
        double c;
        if (parent.Alignment == TreeAlignment.End) {
          c = nodebounds.Bottom + p.Y;
          if (p.Y == 0 && prev.Y > last.Y+parent.RowIndent) {
            c = Math.Min(c, Math.Max(prev.Y, c-TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.Alignment == TreeAlignment.Start) {
          c = nodebounds.Top + p.Y;
          if (p.Y == 0 && prev.Y < last.Y-parent.RowIndent) {
            c = Math.Max(c, Math.Min(prev.Y, c+TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.RouteAroundCentered || (parent.RouteAroundLastParent && parent.MaxGenerationCount == 1)) {
          c = nodebounds.Top - parent.SubtreeOffset.Y + p.Y;
        } else {
          c = nodebounds.Y+nodebounds.Height/2 + p.Y;
        }
        if (bezier) {
          if (!firstrow) {
            s.InsertPoint(idx, new Point(prev.X, c)); idx++;
            s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing, c)); idx++;
            s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing + (p.X + rowspacing)/3, c)); idx++;
            s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing + (p.X + rowspacing)*2/3, c)); idx++;
          } else {
            s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing + (p.X + rowspacing), c)); idx++;
          }
          s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing + (p.X + rowspacing), c)); idx++;
          s.InsertPoint(idx, new Point(next.X, c)); idx++;
        } else {
          if (ortho) { s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing/2, prev.Y)); idx++; }
          s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing/2, c)); idx++;
          s.InsertPoint(idx, new Point(nodebounds.Left - layerspacing + p.X + (ortho ? rowspacing/2 : rowspacing), c)); idx++;
          if (ortho) { s.InsertPoint(idx, new Point(s.GetPoint(idx-1).X, next.Y)); idx++; }
        }
      } else if (angle == 270) {
        double c;
        if (parent.Alignment == TreeAlignment.End) {
          c = nodebounds.Right + p.X;
          if (p.X == 0 && prev.X > last.X+parent.RowIndent) {
            c = Math.Min(c, Math.Max(prev.X, c-TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.Alignment == TreeAlignment.Start) {
          c = nodebounds.Left + p.X;
          if (p.X == 0 && prev.X < last.X-parent.RowIndent) {
            c = Math.Max(c, Math.Min(prev.X, c+TreeLayout.ComputeNodeIndent(parent)));
          }
        } else if (parent.RouteAroundCentered || (parent.RouteAroundLastParent && parent.MaxGenerationCount == 1)) {
          c = nodebounds.Left - parent.SubtreeOffset.X + p.X;
        } else {
          c = nodebounds.X+nodebounds.Width/2 + p.X;
        }
        if (bezier) {
          if (!firstrow) {
            s.InsertPoint(idx, new Point(c, prev.Y)); idx++;
            s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing)); idx++;
            s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing + (p.Y + rowspacing)/3)); idx++;
            s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing + (p.Y + rowspacing)*2/3)); idx++;
          } else {
            s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing + (p.Y + rowspacing))); idx++;
          }
          s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing + (p.Y + rowspacing))); idx++;
          s.InsertPoint(idx, new Point(c, next.Y)); idx++;
        } else {
          if (ortho) { s.InsertPoint(idx, new Point(prev.X, nodebounds.Top - layerspacing/2)); idx++; }
          s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing/2)); idx++;
          s.InsertPoint(idx, new Point(c, nodebounds.Top - layerspacing + p.Y + (ortho ? rowspacing/2 : rowspacing))); idx++;
          if (ortho) { s.InsertPoint(idx, new Point(next.X, s.GetPoint(idx-1).Y)); idx++; }
        }
      } else {
        throw new InvalidOperationException("Invalid angle " + angle.ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }

    private void AdjustRouteForAngleChange(TreeVertex parent, TreeVertex child) {
      double angle = TreeLayout.OrthoAngle(parent);
      double childangle = TreeLayout.OrthoAngle(child);
      if (angle == childangle) return;
      double layerspacing = TreeLayout.ComputeLayerSpacing(parent);
      Rect pb = parent.Node.Bounds;
      Rect cb = child.Node.Bounds;
      // but maybe an Angle change causes the child node not to be adjacent to the layer
      // separating it with the parent node; only need to do anything if that's the case
      if ((angle == 0 && cb.Left-pb.Right < layerspacing+1) ||
          (angle == 90 && cb.Top-pb.Bottom < layerspacing+1) ||
          (angle == 180 && pb.Left-cb.Right < layerspacing+1) ||
          (angle == 270 && pb.Top-cb.Bottom < layerspacing+1)) {
        return;
      }
      
      Route s = this.Route;
      s.UpdatePoints();
      bool bezier = s.Curve == LinkCurve.Bezier;
      bool ortho = s.Orthogonal;
      if (angle == 0) {
        double x = pb.Right+layerspacing/2;
        if (bezier) {  // curved segments
          if (s.PointsCount == 4) {
            double y = s.GetPoint(3).Y;
            s.SetPoint(1, new Point(x-20, s.GetPoint(1).Y));
            s.InsertPoint(2, new Point(x-20, y));
            s.InsertPoint(3, new Point(x, y));
            s.InsertPoint(4, new Point(x+20, y));
            s.SetPoint(5, new Point(s.GetPoint(5).X, y));
          }
        } else if (ortho) {
          if (s.PointsCount == 6) {
            s.SetPoint(2, new Point(x, s.GetPoint(2).Y));
            s.SetPoint(3, new Point(x, s.GetPoint(3).Y));
          }
        } else {
          if (s.PointsCount == 4) {
            s.InsertPoint(2, new Point(x, s.GetPoint(2).Y));
          } else if (s.PointsCount == 3) {
            s.SetPoint(1, new Point(x, s.GetPoint(2).Y));
          } else if (s.PointsCount == 2) {
            s.InsertPoint(1, new Point(x, s.GetPoint(1).Y));
          }
        }
      } else if (angle == 90) {
        double y = pb.Bottom+layerspacing/2;
        if (bezier) {  // curved segments
          if (s.PointsCount == 4) {
            double x = s.GetPoint(3).X;
            s.SetPoint(1, new Point(s.GetPoint(1).X, y-20));
            s.InsertPoint(2, new Point(x, y-20));
            s.InsertPoint(3, new Point(x, y));
            s.InsertPoint(4, new Point(x, y+20));
            s.SetPoint(5, new Point(x, s.GetPoint(5).Y));
          }
        } else if (ortho) {
          if (s.PointsCount == 6) {
            s.SetPoint(2, new Point(s.GetPoint(2).X, y));
            s.SetPoint(3, new Point(s.GetPoint(3).X, y));
          }
        } else {
          if (s.PointsCount == 4) {
            s.InsertPoint(2, new Point(s.GetPoint(2).X, y));
          } else if (s.PointsCount == 3) {
            s.SetPoint(1, new Point(s.GetPoint(2).X, y));
          } else if (s.PointsCount == 2) {
            s.InsertPoint(1, new Point(s.GetPoint(1).X, y));
          }
        }
      } else if (angle == 180) {
        double x = pb.Left-layerspacing/2;
        if (bezier) {  // curved segments
          if (s.PointsCount == 4) {
            double y = s.GetPoint(3).Y;
            s.SetPoint(1, new Point(x+20, s.GetPoint(1).Y));
            s.InsertPoint(2, new Point(x+20, y));
            s.InsertPoint(3, new Point(x, y));
            s.InsertPoint(4, new Point(x-20, y));
            s.SetPoint(5, new Point(s.GetPoint(5).X, y));
          }
        } else if (ortho) {
          if (s.PointsCount == 6) {
            s.SetPoint(2, new Point(x, s.GetPoint(2).Y));
            s.SetPoint(3, new Point(x, s.GetPoint(3).Y));
          }
        } else {
          if (s.PointsCount == 4) {
            s.InsertPoint(2, new Point(x, s.GetPoint(2).Y));
          } else if (s.PointsCount == 3) {
            s.SetPoint(1, new Point(x, s.GetPoint(2).Y));
          } else if (s.PointsCount == 2) {
            s.InsertPoint(1, new Point(x, s.GetPoint(1).Y));
          }
        }
      } else if (angle == 270) {
        double y = pb.Top-layerspacing/2;
        if (bezier) {  // curved segments
          if (s.PointsCount == 4) {
            double x = s.GetPoint(3).X;
            s.SetPoint(1, new Point(s.GetPoint(1).X, y+20));
            s.InsertPoint(2, new Point(x, y+20));
            s.InsertPoint(3, new Point(x, y));
            s.InsertPoint(4, new Point(x, y-20));
            s.SetPoint(5, new Point(x, s.GetPoint(5).Y));
          }
        } else if (ortho) {
          if (s.PointsCount == 6) {
            s.SetPoint(2, new Point(s.GetPoint(2).X, y));
            s.SetPoint(3, new Point(s.GetPoint(3).X, y));
          }
        } else {
          if (s.PointsCount == 4) {
            s.InsertPoint(2, new Point(s.GetPoint(2).X, y));
          } else if (s.PointsCount == 3) {
            s.SetPoint(1, new Point(s.GetPoint(2).X, y));
          } else if (s.PointsCount == 2) {
            s.InsertPoint(1, new Point(s.GetPoint(1).X, y));
          }
        }
      }
    }
  
    // tree specific properties:

    /// <summary>
    /// Gets or sets a point, relative to the parent node,
    /// that may be useful in routing this link.
    /// </summary>
    public Point RelativePoint {
      get { return  myRelativePoint; }
      set { myRelativePoint = value; }
    }

    private Point myRelativePoint = new Point(0, 0);
  }


  /********************************************************************************************/

  /// <summary>
  /// Holds auto-layout specific node data.
  /// </summary>
  public class TreeVertex : TreeNetwork.Vertex {

    // tree specific properties:

    /// <summary>
    /// Gets or sets whether this node has been initialized as part of <see cref="TreeLayout.CreateTrees"/>.
    /// </summary>
    public bool Initialized {
      get { return (myInternalFlags & myInitializedFlag) != 0; }
      set {
        if (value)
          myInternalFlags |= myInitializedFlag;
        else
          myInternalFlags &= ~myInitializedFlag;
      }
    }

    /// <summary>
    /// Gets or sets the logical parent for this node.
    /// </summary>
    /// <remarks>
    /// This structural property is computed in <see cref="TreeLayout.WalkTree"/>.
    /// You probably should not be setting this property.
    /// </remarks>
    public TreeVertex Parent {
      get { return myParent; }
      set { myParent = value; }
    }

    /// <summary>
    /// Gets or sets the array of logical children for this node.
    /// </summary>
    /// <remarks>
    /// This structural property is computed in <see cref="TreeLayout.WalkTree"/>.
    /// You probably should not be setting this property.
    /// </remarks>
    public TreeVertex[] Children {
      get { return myChildren; }
      set { myChildren = value; }
    }

    /// <summary>
    /// Gets the number of immediate children that this node has.
    /// </summary>
    /// <seealso cref="Children"/>
    /// <seealso cref="DescendentCount"/>
    public int ChildrenCount {
      get { return this.Children.Length; }
    }

    /// <summary>
    /// Gets the number of single-parent ancestors this node has.
    /// </summary>
    /// <remarks>
    /// This could also be interpreted as which layer this node is in.
    /// A root node will have a value of zero.
    /// This informational property is computed in <see cref="TreeLayout.WalkTree"/>.
    /// You probably should not be setting this property.
    /// </remarks>
    public int Level {
      get { return myLevel; }
      set { myLevel = value; }
    }

    /// <summary>
    /// Gets the number of descendents this node has.
    /// </summary>
    /// <remarks>
    /// For a leaf node, this will be zero.
    /// This informational property is computed as part of the <see cref="TreeLayout.InitializeTreeVertexValues"/> pass.
    /// You probably should not be setting this property.
    /// </remarks>
    /// <seealso cref="ChildrenCount"/>
    public int DescendentCount {
      get { return myDescendentCount; }
      set { myDescendentCount = value; }
    }

    /// <summary>
    /// Gets the maximum number of children of any descendent of this node.
    /// </summary>
    /// <remarks>
    /// For a leaf node, this will be zero.
    /// This informational property is computed as part of the <see cref="TreeLayout.InitializeTreeVertexValues"/> pass.
    /// You probably should not be setting this property.
    /// </remarks>
    /// <seealso cref="MaxGenerationCount"/>
    /// <seealso cref="ChildrenCount"/>
    public int MaxChildrenCount {
      get { return myMaxChildrenCount; }
      set { myMaxChildrenCount = value; }
    }

    /// <summary>
    /// Gets the maximum depth of the subtrees below this node.
    /// </summary>
    /// <remarks>
    /// For a leaf node, this will be zero.
    /// This informational property is computed as part of the <see cref="TreeLayout.InitializeTreeVertexValues"/> pass.
    /// You probably should not be setting this property.
    /// </remarks>
    /// <seealso cref="MaxChildrenCount"/>
    public int MaxGenerationCount {
      get { return myMaxGenerationCount; }
      set { myMaxGenerationCount = value; }
    }

    /// <summary>
    /// Gets or sets the position of this node relative to its parent node.
    /// </summary>
    /// <remarks>
    /// This informational property is computed by <see cref="TreeLayout.LayoutTree"/>.
    /// You probably should not be setting this property.
    /// </remarks>
    public Point RelativePosition {
      get { return myRelativePosition; }
      set { myRelativePosition = value; }
    }

    /// <summary>
    /// Gets or sets the size of the subtree (including all descendents) parented by this node.
    /// </summary>
    /// <remarks>
    /// This informational property is computed by <see cref="TreeLayout.LayoutTree"/>.
    /// Of course if there are no children, this is just the same as <see cref="Size"/>.
    /// You probably should not be setting this property.
    /// </remarks>
    public Size SubtreeSize {
      get { return mySubtreeSize; }
      set { mySubtreeSize = value; }
    }

    /// <summary>
    /// Gets or sets the offset of this parent node relative to its whole subtree.
    /// </summary>
    /// <remarks>
    /// This informational property is computed by <see cref="TreeLayout.LayoutTree"/>.
    /// Of course if there are no children, this is just (0, 0).
    /// You probably should not be setting this property.
    /// </remarks>
    public Point SubtreeOffset {
      get { return mySubtreeOffset; }
      set { mySubtreeOffset = value; }
    }

    /// <summary>
    /// Gets or sets a collection of <see cref="Node"/>s that will be
    /// positioned near the node.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// These objects should not have their own <see cref="TreeVertex"/>s to be laid out.
    /// Typically these will be instances of <see cref="Northwoods.GoXam.Node"/>s
    /// whose <see cref="Northwoods.GoXam.Part.Category"/> is "Comment".
    /// This collection should be allocated and initialized in <see cref="TreeLayout.AddComments"/>.
    /// </remarks>
    public ICollection<Node> Comments {
      get { return myComments; }
      set { myComments = value; }
    }


    // Inherited properties

    /// <summary>
    /// Gets or sets whether and in what order the children should be sorted.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeSorting.Forwards"/>.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public TreeSorting Sorting {
      get { return mySorting; }
      set { mySorting = value; }
    }

    /// <summary>
    /// Gets or sets how the children should be sorted.
    /// </summary>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public IComparer<TreeVertex> Comparer {
      get { return myComparer; }
      set { myComparer = value; }
    }

    /// <summary>
    /// Gets or sets the actual absolute angle at which this node should grow.
    /// </summary>
    /// <value>
    /// The default value is zero, meaning that general tree growth should proceed rightwards along the X axis.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// By default this is the same as the <see cref="Angle"/> of the parent <see cref="TreeVertex"/>.
    /// However, after the initial propagation of property values, perhaps in
    /// an override of <see cref="TreeLayout.AssignTreeVertexValues"/>,
    /// you could just set this property to specify the angle at which this node grows
    /// it subtrees.
    /// </remarks>
    public double Angle {
      get { return myAngle; }
      set { myAngle = value; }
    }


    /// <summary>
    /// Gets or sets how this parent node should be aligned relative to its children.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeAlignment.CenterChildren"/>.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public TreeAlignment Alignment {
      get { return myAlignment; }
      set { myAlignment = value; }
    }

    /// <summary>
    /// Gets or sets the distance the first child should be indented.
    /// </summary>
    /// <value>
    /// The default value is zero.  The value should be non-negative.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// This property is only sensible when the <see cref="Alignment"/>
    /// is <see cref="TreeAlignment.Start"/> or <see cref="TreeAlignment.End"/>.
    /// </remarks>
    public double NodeIndent {
      get { return myNodeIndent; }
      set { myNodeIndent = value; }
    }

    /// <summary>
    /// Gets or sets whether the first child should be indented past the parent node's breadth.
    /// </summary>
    /// <value>
    /// The default value is 0.0 -- the only start or end spacing is provided by <see cref="NodeIndent"/>.
    /// </value>
    /// <remarks>
    /// Values must range from 0.0 to 1.0, where 1.0 means the full breadth of this node.
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// This property is only sensible when the <see cref="Alignment"/>
    /// is <see cref="TreeAlignment.Start"/> or <see cref="TreeAlignment.End"/>.
    /// </remarks>
    public double NodeIndentPastParent {
      get { return myNodeIndentPastParent; }
      set { myNodeIndentPastParent = value; }
    }

    /// <summary>
    /// Gets or sets the distance between child nodes.
    /// </summary>
    /// <value>
    /// The default value is 20.
    /// A negative value causes sibling nodes to overlap.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public double NodeSpacing {
      get { return myNodeSpacing; }
      set { myNodeSpacing = value; }
    }

    /// <summary>
    /// Gets or sets the distance there should be between this node and its layer of children.
    /// </summary>
    /// <value>
    /// The default value is 50.
    /// Negative values may cause children to overlap with the parent.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    /// <seealso cref="LayerSpacingParentOverlap"/>
    public double LayerSpacing {
      get { return myLayerSpacing; }
      set { myLayerSpacing = value; }
    }

    /// <summary>
    /// Gets or sets the fraction of this node's depth that may overlap with the children's layer.
    /// </summary>
    /// <value>
    /// Values must range from 0.0 to 1.0, where 1.0 means the full depth of this node.
    /// The default value is 0.0 -- there is overlap only if <see cref="LayerSpacing"/> is negative.
    /// </value>
    /// <remarks>
    /// When this value is greater than 0.0, there might not be overlap if <see cref="LayerSpacing"/>
    /// is larger than the depth of this node times this fraction.
    /// Even when this value is 0.0, there may be overlap when <see cref="LayerSpacing"/> is negative.
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public double LayerSpacingParentOverlap {
      get { return myLayerSpacingParentOverlap; }
      set { myLayerSpacingParentOverlap = value; }
    }

    /// <summary>
    /// Gets or sets how the children of this node should be packed together.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeCompaction.Block"/>.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public TreeCompaction Compaction {
      get { return myCompaction; }
      set { myCompaction = value; }
    }

    /// <summary>
    /// Gets or sets how broad a node and its descendents should be.
    /// </summary>
    /// <value>
    /// By default this is zero.  A value of zero imposes no limit;
    /// a positive value will specify a limit for the total width of this subtree.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public double BreadthLimit {
      get { return myBreadthLimit; }
      set { myBreadthLimit = value; }
    }

    /// <summary>
    /// Gets or sets the distance between rows within one layer, all sharing the same parent.
    /// </summary>
    /// <value>
    /// The default value is 25.
    /// Negative values may cause nodes to overlap.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public double RowSpacing {
      get { return myRowSpacing; }
      set { myRowSpacing = value; }
    }

    /// <summary>
    /// Gets or sets the distance the first child of each row should be indented.
    /// </summary>
    /// <value>
    /// The default value is 10.  The value should be non-negative.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// This property is only used when the <see cref="BreadthLimit"/> is positive,
    /// and some initial space needs to be reserved in each row of nodes for the links
    /// that are routed around those rows.
    /// </remarks>
    public double RowIndent {
      get { return myRowIndent; }
      set { myRowIndent = value; }
    }

    /// <summary>
    /// Gets or sets the space to leave between consecutive comments.
    /// </summary>
    /// <value>
    /// The default value is 10.
    /// Negative values may cause comments to overlap.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public double CommentSpacing {
      get { return myCommentSpacing; }
      set { myCommentSpacing = value; }
    }
    /// <summary>
    /// Gets or sets the space to leave between the node and the comments.
    /// </summary>
    /// <value>
    /// The default value is 20.
    /// Negative values may cause comments to overlap with the node.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// </remarks>
    public double CommentMargin {
      get { return myCommentMargin; }
      set { myCommentMargin = value; }
    }

    /// <summary>
    /// Gets or sets whether <see cref="TreeLayout.SetPortSpots"/> should set the
    /// FromSpot for this parent node port.
    /// </summary>
    /// <value>
    /// The default value is true -- this may modify the spot of the port of this node, the parent,
    /// if the node has only a single port.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// The spot used depends on the value of <see cref="PortSpot"/>.
    /// </remarks>
    public bool SetsPortSpot {
      get { return (myInternalFlags & myNoSetsPortSpotFlag) != myNoSetsPortSpotFlag; }  // clear means true, set means false
      set {
        if (value)
          myInternalFlags &= ~myNoSetsPortSpotFlag;  // true means clear flag
        else
          myInternalFlags |= myNoSetsPortSpotFlag;  // false means set flag
      }
    }

    /// <summary>
    /// Gets or sets the spot that this node's port gets as its FromSpot,
    /// if <see cref="SetsPortSpot"/> is true and the node has only a single port.
    /// </summary>
    /// <value>The default value is <c>Spot.Default</c>.</value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// A value of <c>Spot.Default</c> will cause <see cref="TreeLayout.SetPortSpots"/>
    /// to assign a FromSpot based on the parent node's
    /// <see cref="TreeVertex.Angle"/>.
    /// If the value is other than <c>NoSpot</c>, it is just assigned.
    /// When <see cref="TreeLayout.Path"/> is <see cref="TreePath.Source"/>,
    /// the port's ToSpot is set instead of the FromSpot.
    /// </remarks>
    public Spot PortSpot {
      get { return myPortSpot; }
      set { myPortSpot = value; }
    }

    /// <summary>
    /// Gets or sets whether <see cref="TreeLayout.SetPortSpots"/> should set the
    /// ToSpot for each child node port.
    /// </summary>
    /// <value>
    /// The default value is true -- this may modify the spots of the ports of the children nodes,
    /// if the node has only a single port.
    /// </value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// The spot used depends on the value of <see cref="ChildPortSpot"/>.
    /// </remarks>
    public bool SetsChildPortSpot {
      get { return (myInternalFlags & myNoSetsChildPortSpotFlag) != myNoSetsChildPortSpotFlag; }  // clear means true, set means false
      set {
        if (value)
          myInternalFlags &= ~myNoSetsChildPortSpotFlag;  // true means clear flag
        else
          myInternalFlags |= myNoSetsChildPortSpotFlag;  // false means set flag
      }
    }

    /// <summary>
    /// Gets or sets the spot that children nodes' ports get as their ToSpot,
    /// if <see cref="SetsChildPortSpot"/> is true and the node has only a single port.
    /// </summary>
    /// <value>The default value is <c>Spot.Default</c>.</value>
    /// <remarks>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// A value of <c>Spot.Default</c> will cause <see cref="TreeLayout.SetPortSpots"/>
    /// to assign a ToSpot based on the parent node's
    /// <see cref="TreeVertex.Angle"/>.
    /// If the value is other than <c>NoSpot</c>, it is just assigned.
    /// When <see cref="TreeLayout.Path"/> is <see cref="TreePath.Source"/>,
    /// the port's FromSpot is set instead of the ToSpot.
    /// </remarks>
    public Spot ChildPortSpot {
      get { return myChildPortSpot; }
      set { myChildPortSpot = value; }
    }

    /// <summary>
    /// This method just copies inheritable properties from
    /// another <see cref="TreeVertex"/>.
    /// </summary>
    /// <param name="n"></param>
    /// <remarks>
    /// The properties include:
    /// <see cref="Sorting"/>
    /// <see cref="Comparer"/>
    /// <see cref="Angle"/>
    /// <see cref="Alignment"/>
    /// <see cref="NodeIndent"/>
    /// <see cref="NodeIndentPastParent"/>
    /// <see cref="NodeSpacing"/>
    /// <see cref="LayerSpacing"/>
    /// <see cref="LayerSpacingParentOverlap"/>
    /// <see cref="Compaction"/>
    /// <see cref="BreadthLimit"/>
    /// <see cref="RowSpacing"/>
    /// <see cref="RowIndent"/>
    /// <see cref="CommentSpacing"/>
    /// <see cref="CommentMargin"/>
    /// <see cref="SetsPortSpot"/>
    /// <see cref="PortSpot"/>
    /// <see cref="SetsChildPortSpot"/>
    /// <see cref="ChildPortSpot"/>
    /// </remarks>
    public virtual void CopyInheritedPropertiesFrom(TreeVertex n) {
      if (n == null) return;
      this.InternalFlags = n.InternalFlags;
      this.Sorting = n.Sorting;
      this.Comparer = n.Comparer;
      this.Angle = n.Angle;
      this.Alignment = n.Alignment;
      this.NodeIndent = n.NodeIndent;
      this.NodeIndentPastParent = n.NodeIndentPastParent;
      this.NodeSpacing = n.NodeSpacing;
      this.LayerSpacing = n.LayerSpacing;
      this.LayerSpacingParentOverlap = n.LayerSpacingParentOverlap;
      this.Compaction = n.Compaction;
      this.BreadthLimit = n.BreadthLimit;
      this.RowSpacing = n.RowSpacing;
      this.RowIndent = n.RowIndent;
      this.CommentSpacing = n.CommentSpacing;
      this.CommentMargin = n.CommentMargin;
      this.PortSpot = n.PortSpot;
      this.ChildPortSpot = n.ChildPortSpot;
    }

    /// <summary>
    /// (Unsupported)
    /// </summary>
    public int InternalFlags {
      get { return myInternalFlags; }
      set { myInternalFlags = value; }
    }

    internal bool RouteFirstRow {
      get { return (myInternalFlags & myRouteFirstRowFlag) != 0; }
      set {
        if (value)
          myInternalFlags |= myRouteFirstRowFlag;
        else
          myInternalFlags &= ~myRouteFirstRowFlag;
      }
    }

    internal bool RouteAroundCentered {
      get { return (myInternalFlags & myRouteAroundCenteredFlag) != 0; }
      set {
        if (value)
          myInternalFlags |= myRouteAroundCenteredFlag;
        else
          myInternalFlags &= ~myRouteAroundCenteredFlag;
      }
    }

    internal bool RouteAroundLastParent {
      get { return (myInternalFlags & myRouteAroundLastParentFlag) != 0; }
      set {
        if (value)
          myInternalFlags |= myRouteAroundLastParentFlag;
        else
          myInternalFlags &= ~myRouteAroundLastParentFlag;
      }
    }

    //?? IsFixed property

    internal static readonly TreeVertex[] NoChildren = new TreeVertex[0];

    internal const int myInitializedFlag           = 0x00010000;
    internal const int myNoSetsPortSpotFlag        = 0x00020000;
    internal const int myNoSetsChildPortSpotFlag   = 0x00040000;
    internal const int myRouteFirstRowFlag         = 0x00000001;
    internal const int myRouteAroundCenteredFlag   = 0x00000002;
    internal const int myRouteAroundLastParentFlag = 0x00000004;

    // TreeNode-specific state
    // structural
    private int myInternalFlags;
    private TreeVertex myParent;
    private TreeVertex[] myChildren = NoChildren;
    private ICollection<Node> myComments;  // Nodes, not TreeVertexes!
    // informational
    private int myLevel;
    private int myDescendentCount;
    private int myMaxChildrenCount;
    private int myMaxGenerationCount;
    // positional
    private double myAngle;
    private Point myRelativePosition;
    private Size mySubtreeSize;
    private Point mySubtreeOffset;




    internal Point[] LeftFringe;
    internal Point[] RightFringe;


    // inherited properties:
    // sorting
    private TreeSorting mySorting = TreeSorting.Forwards;
    private IComparer<TreeVertex> myComparer = TreeLayout.AlphabeticNodeTextComparer;
    // alignment
    private TreeAlignment myAlignment = TreeAlignment.CenterChildren;
    private double myNodeIndent;
    private double myNodeIndentPastParent;
    // spacing and size constraint
    private double myNodeSpacing = 20;
    private double myLayerSpacing = 50;
    private double myLayerSpacingParentOverlap;
    private TreeCompaction myCompaction = TreeCompaction.Block;
    private double myBreadthLimit;
    private double myRowSpacing = 25;
    private double myRowIndent = 10;
    private double myCommentSpacing = 10;
    private double myCommentMargin = 20;
    private Spot myPortSpot = Spot.Default;
    private Spot myChildPortSpot = Spot.Default;
  }
}

