using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MCS.Web.WebControls;
using System.Collections.Generic;

namespace MCS.Web.WebControls
{
  /// <summary>
  /// Summary description for TreeViewAutoFormatEditorForm.
  /// </summary>
  internal class MenuItemsEditorForm : System.Windows.Forms.Form
  {
     private System.Windows.Forms.Button button2;
     private System.ComponentModel.IContainer components;

	  private DeluxeMenu _navBar;
      private System.Windows.Forms.ToolTip toolTip1;
      private bool _firstActivate;
      private PropertyGrid propertyGrid1;
      private ToolStrip toolStrip1;
      private ToolStripButton _addRootButton;
      private ToolStripButton _addChildButton;
      private ToolStripButton _removeButton;
      private ToolStripSeparator toolStripSeparator1;
      private ToolStripButton _moveUpButton;
      private ToolStripButton _moveDownButton;
      private ToolStripButton _indentButton;
      private ToolStripButton _unindentButton;
      private TreeView _treeView;
      private Button button1;
      private SplitContainer splitContainer1;
      private TableLayoutPanel tableLayoutPanel3;
      private TableLayoutPanel tableLayoutPanel1;
      private TableLayoutPanel tableLayoutPanel2;

      public MenuItemCollection Items;


      private void LoadNodes(MCS.Web.WebControls.MenuItem oItem, TreeNode oTreeNode)
    {
      oTreeNode.Tag = oItem;

      foreach (MCS.Web.WebControls.MenuItem oChild in oItem.ChildItems)
      {
        TreeNode oChildNode = new TreeNode(oChild.Text);
        LoadNodes(oChild, oChildNode);

        oTreeNode.Nodes.Add(oChildNode);
      }
    }

	  public MenuItemsEditorForm(DeluxeMenu oMenu)
    {
      InitializeComponent();
      this._firstActivate = true;
      _navBar = oMenu;
      Items = oMenu.Items;

      // add pre-existing nodes
      foreach (MCS.Web.WebControls.MenuItem oRoot in Items)
      {
        TreeNode oRootNode = new TreeNode(oRoot.Text);
        LoadNodes(oRoot, oRootNode);
        _treeView.Nodes.Add(oRootNode);
      }
      this.propertyGrid1.Site = new FormPropertyGridSite(oMenu.Site, this.propertyGrid1);
      _treeView.HideSelection = false;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuItemsEditorForm));
        this.button2 = new System.Windows.Forms.Button();
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        this._treeView = new System.Windows.Forms.TreeView();
        this.button1 = new System.Windows.Forms.Button();
        this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        this._addRootButton = new System.Windows.Forms.ToolStripButton();
        this._addChildButton = new System.Windows.Forms.ToolStripButton();
        this._removeButton = new System.Windows.Forms.ToolStripButton();
        this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        this._moveUpButton = new System.Windows.Forms.ToolStripButton();
        this._moveDownButton = new System.Windows.Forms.ToolStripButton();
        this._indentButton = new System.Windows.Forms.ToolStripButton();
        this._unindentButton = new System.Windows.Forms.ToolStripButton();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        this.toolStrip1.SuspendLayout();
        this.splitContainer1.Panel1.SuspendLayout();
        this.splitContainer1.Panel2.SuspendLayout();
        this.splitContainer1.SuspendLayout();
        this.tableLayoutPanel3.SuspendLayout();
        this.tableLayoutPanel1.SuspendLayout();
        this.tableLayoutPanel2.SuspendLayout();
        this.SuspendLayout();
        // 
        // button2
        // 
        this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.button2.Location = new System.Drawing.Point(137, 3);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(51, 22);
        this.button2.TabIndex = 3;
        this.button2.Text = "取消";
        this.button2.Click += new System.EventHandler(this.button2_Click);
        // 
        // propertyGrid1
        // 
        this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
        this.propertyGrid1.Location = new System.Drawing.Point(3, 29);
        this.propertyGrid1.Name = "propertyGrid1";
        this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
        this.propertyGrid1.Size = new System.Drawing.Size(229, 262);
        this.propertyGrid1.TabIndex = 1;
        this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_ValueChanged);
        // 
        // _treeView
        // 
        this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
        this._treeView.Location = new System.Drawing.Point(3, 55);
        this._treeView.Name = "_treeView";
        this._treeView.Size = new System.Drawing.Size(231, 236);
        this._treeView.TabIndex = 0;
        this._treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeView_AfterSelect);
        // 
        // button1
        // 
        this.button1.Dock = System.Windows.Forms.DockStyle.Right;
        this.button1.Location = new System.Drawing.Point(76, 3);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(55, 22);
        this.button1.TabIndex = 2;
        this.button1.Text = "完成";
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // toolStrip1
        // 
        this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
        this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
        this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._addRootButton,
            this._addChildButton,
            this._removeButton,
            this.toolStripSeparator1,
            this._moveUpButton,
            this._moveDownButton,
            this._indentButton,
            this._unindentButton});
        this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
        this.toolStrip1.Location = new System.Drawing.Point(0, 20);
        this.toolStrip1.Name = "toolStrip1";
        this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
        this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        this.toolStrip1.Size = new System.Drawing.Size(237, 32);
        this.toolStrip1.TabIndex = 7;
        this.toolStrip1.Text = "toolStrip1";
        // 
        // _addRootButton
        // 
        this._addRootButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._addRootButton.Image = ((System.Drawing.Image)(resources.GetObject("_addRootButton.Image")));
        this._addRootButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._addRootButton.Name = "_addRootButton";
        this._addRootButton.Size = new System.Drawing.Size(23, 29);
        this._addRootButton.Text = "AddRootButton";
        this._addRootButton.Click += new System.EventHandler(this._addRootButton_Click);
        // 
        // _addChildButton
        // 
        this._addChildButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._addChildButton.Image = ((System.Drawing.Image)(resources.GetObject("_addChildButton.Image")));
        this._addChildButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._addChildButton.Name = "_addChildButton";
        this._addChildButton.Size = new System.Drawing.Size(23, 29);
        this._addChildButton.Text = "toolStripButton2";
        this._addChildButton.Click += new System.EventHandler(this._addChildButton_Click);
        // 
        // _removeButton
        // 
        this._removeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._removeButton.Image = ((System.Drawing.Image)(resources.GetObject("_removeButton.Image")));
        this._removeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._removeButton.Name = "_removeButton";
        this._removeButton.Size = new System.Drawing.Size(23, 29);
        this._removeButton.Text = "removeButton";
        this._removeButton.Click += new System.EventHandler(this._removeButton_Click);
        // 
        // toolStripSeparator1
        // 
        this.toolStripSeparator1.Name = "toolStripSeparator1";
        this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
        // 
        // _moveUpButton
        // 
        this._moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._moveUpButton.Image = ((System.Drawing.Image)(resources.GetObject("_moveUpButton.Image")));
        this._moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._moveUpButton.Name = "_moveUpButton";
        this._moveUpButton.Size = new System.Drawing.Size(23, 29);
        this._moveUpButton.Text = "Move Up Button";
        this._moveUpButton.Click += new System.EventHandler(this._moveUpButton_Click);
        // 
        // _moveDownButton
        // 
        this._moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._moveDownButton.Image = ((System.Drawing.Image)(resources.GetObject("_moveDownButton.Image")));
        this._moveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._moveDownButton.Name = "_moveDownButton";
        this._moveDownButton.Size = new System.Drawing.Size(23, 29);
        this._moveDownButton.Text = "Move Down Button";
        this._moveDownButton.Click += new System.EventHandler(this._moveDownButton_Click);
        // 
        // _indentButton
        // 
        this._indentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._indentButton.Image = ((System.Drawing.Image)(resources.GetObject("_indentButton.Image")));
        this._indentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._indentButton.Name = "_indentButton";
        this._indentButton.Size = new System.Drawing.Size(23, 29);
        this._indentButton.Text = "Indent Button";
        this._indentButton.Click += new System.EventHandler(this._indentButton_Click);
        // 
        // _unindentButton
        // 
        this._unindentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this._unindentButton.Image = ((System.Drawing.Image)(resources.GetObject("_unindentButton.Image")));
        this._unindentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
        this._unindentButton.Name = "_unindentButton";
        this._unindentButton.Size = new System.Drawing.Size(23, 29);
        this._unindentButton.Text = "Unindent Button";
        this._unindentButton.Click += new System.EventHandler(this._unindentButton_Click);
        // 
        // splitContainer1
        // 
        this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer1.Location = new System.Drawing.Point(0, 0);
        this.splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
        // 
        // splitContainer1.Panel2
        // 
        this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
        this.splitContainer1.Size = new System.Drawing.Size(476, 328);
        this.splitContainer1.SplitterDistance = 237;
        this.splitContainer1.TabIndex = 8;
        // 
        // tableLayoutPanel3
        // 
        this.tableLayoutPanel3.ColumnCount = 1;
        this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel3.Controls.Add(this._treeView, 0, 2);
        this.tableLayoutPanel3.Controls.Add(this.toolStrip1, 0, 1);
        this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel3.Name = "tableLayoutPanel3";
        this.tableLayoutPanel3.RowCount = 4;
        this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
        this.tableLayoutPanel3.Size = new System.Drawing.Size(237, 328);
        this.tableLayoutPanel3.TabIndex = 8;
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.ColumnCount = 1;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.Controls.Add(this.propertyGrid1, 0, 1);
        this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
        this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 3;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
        this.tableLayoutPanel1.Size = new System.Drawing.Size(235, 328);
        this.tableLayoutPanel1.TabIndex = 4;
        // 
        // tableLayoutPanel2
        // 
        this.tableLayoutPanel2.ColumnCount = 2;
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.5F));
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.5F));
        this.tableLayoutPanel2.Controls.Add(this.button1, 0, 0);
        this.tableLayoutPanel2.Controls.Add(this.button2, 1, 0);
        this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
        this.tableLayoutPanel2.Location = new System.Drawing.Point(41, 297);
        this.tableLayoutPanel2.Name = "tableLayoutPanel2";
        this.tableLayoutPanel2.RowCount = 1;
        this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
        this.tableLayoutPanel2.Size = new System.Drawing.Size(191, 28);
        this.tableLayoutPanel2.TabIndex = 2;
        // 
        // MenuItemsEditorForm
        // 
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        this.ClientSize = new System.Drawing.Size(476, 328);
        this.Controls.Add(this.splitContainer1);
        this.Name = "MenuItemsEditorForm";
        this.Text = "菜单设计器";
        this.toolStrip1.ResumeLayout(false);
        this.toolStrip1.PerformLayout();
        this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel2.ResumeLayout(false);
        this.splitContainer1.ResumeLayout(false);
        this.tableLayoutPanel3.ResumeLayout(false);
        this.tableLayoutPanel3.PerformLayout();
        this.tableLayoutPanel1.ResumeLayout(false);
        this.tableLayoutPanel2.ResumeLayout(false);
        this.ResumeLayout(false);

    }
		#endregion

    // Ok
    private void button1_Click(object sender, System.EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    // Cancel
    private void button2_Click(object sender, System.EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    // Click on node
    private void _treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
    {
      propertyGrid1.SelectedObject = e.Node.Tag;
      this.UpdateEnabledState();
    }

    private void propertyGrid1_ValueChanged(object sender, PropertyValueChangedEventArgs e)
    {
      // if we've just changed text, we need to update the visible tree
      if(e.ChangedItem.Label == "Text")
      {
        _treeView.SelectedNode.Text = (string)e.ChangedItem.Value;
      }
	  if (e.ChangedItem.Label == "IsSeparator")
	  {
		  MenuItem oItem = (MenuItem)_treeView.SelectedNode.Tag;
		  TreeNode node1 = this._treeView.SelectedNode;
		  if ((bool)e.ChangedItem.Value == true)
		  {
			 
			  if (_navBar.HasControlSeparator == true)
			  {
				  if ((oItem.Previous != null && oItem.Previous.IsSeparator == true) || (oItem.Next != null && oItem.Next.IsSeparator == true
					 ) || _navBar.Items[0] == oItem || GetLastMenuItem(_navBar.Items) == oItem || oItem.Parent.ChildItems[0] == oItem ||
					 oItem.Parent.ChildItems[oItem.Parent.ChildItems.Count-1] == oItem)
				  {

				  }
				  else
				  {
					  _treeView.SelectedNode.Text = "----";
					  oItem.Text = "----";
					  RemoveChildItems(oItem, node1);
					  
				  }

			  }
			  else
			  {
				  _treeView.SelectedNode.Text = "----";
				  oItem.Text = "----";
				  RemoveChildItems(oItem, node1);
			  }

		  }
		  else
		  {
			  _treeView.SelectedNode.Text = "New Root";
			  oItem.Text = "New Root";
		  }
	  }
    }
	  private MenuItem GetLastMenuItem(MenuItemCollection menuCollection)
	  {
		  if (menuCollection.Count > 0)
		  {
			  if (menuCollection[menuCollection.Count - 1].ChildItems.Count > 0)
			  {
				 return GetLastMenuItem(menuCollection[menuCollection.Count - 1].ChildItems);
			  }
			  else
			  {
				  return menuCollection[menuCollection.Count - 1];
			  }
		  }
		  else
		  {
			  return null;
		  }
	  }
      /// <summary>
      /// new root
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void _addRootButton_Click(object sender, EventArgs e)
      {
		  if (_navBar.Items.Count > 0 && _navBar.Items[_navBar.Items.Count - 1].IsSeparator == true)
		  {
			  return;
		  }
		  MCS.Web.WebControls.MenuItem oItem = new MCS.Web.WebControls.MenuItem();
          oItem.Text = "New Root";
          Items.Add(oItem);
          TreeNode oNewTreeNode = new TreeNode("New Root");
          oNewTreeNode.Tag = oItem;
          _treeView.Nodes.Add(oNewTreeNode);
          _treeView.SelectedNode = _treeView.Nodes[_treeView.Nodes.Count - 1];
      }
      /// <summary>
      /// new child
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void _addChildButton_Click(object sender, EventArgs e)
      {
		  if (_treeView.SelectedNode == null)
          {
              return;
          }
          MenuItem oItem = (MenuItem)_treeView.SelectedNode.Tag;
		  if (oItem.IsSeparator == true)
		  {
			  return;
		  }
          MenuItem oNewItem = new MenuItem();
          oNewItem.Text = "New Item";
          oItem.ChildItems.Add(oNewItem);
          TreeNode oNewTreeNode = new TreeNode("New Item");
          oNewTreeNode.Tag = oNewItem;
          _treeView.SelectedNode.Nodes.Add(oNewTreeNode);
          _treeView.SelectedNode.Expand();
      }
      /// <summary>
      /// delete node
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void _removeButton_Click(object sender, EventArgs e)
      {         
          TreeNode node1 = this._treeView.SelectedNode;
          if (node1 != null)
          {

              MenuItem oItem = (MenuItem)_treeView.SelectedNode.Tag;
              TreeNodeCollection collection1 = null;
              if (node1.Parent != null)
              {
                  collection1 = node1.Parent.Nodes;
              }
              else
              {
                  collection1 = this._treeView.Nodes;
              }
              if (collection1.Count == 1)
              {
                  this._treeView.SelectedNode = node1.Parent;
              }
              else if (node1.NextNode != null)
              {
                  this._treeView.SelectedNode = node1.NextNode;
              }
              else
              {
                  this._treeView.SelectedNode = node1.PrevNode;
              }

              RemoveMenuItem(oItem);
              node1.Remove();
              if (this._treeView.SelectedNode == null)
              {
                  this.propertyGrid1.SelectedObject = null;
              }
          }
      }
	  /// <summary>
	  /// 
	  /// </summary>
	  private void RemoveChildItems(MenuItem oItem,TreeNode node)
	  {
		  MenuItemCollection childItems=oItem.ChildItems;
		  TreeNodeCollection childNodes = node.Nodes;
		  while(childItems.Count>0)
		  {
			  childItems.Remove(childItems[0]);
			  childNodes.Remove(childNodes[0]);
		  }
	  }
      private void RemoveMenuItem(MenuItem oItem)
      {
          if (oItem.Parent == null)
          {
              // we have a root
              // oItem.Remove(oItem);
              _navBar.Items.Remove(oItem);
          }
          else
          {
              // we have a child
              oItem.Parent.ChildItems.Remove(oItem);
          }
      }

      private void _moveUpButton_Click(object sender, EventArgs e)
      {
          TreeNode node = this._treeView.SelectedNode;
          if (node != null)
          {
              MenuItem oItem = (MenuItem)node.Tag;
			  if (oItem.IsSeparator == false)
			  {
				  MenuItem oItem2 = (MenuItem)node.PrevNode.Tag;
				  MenuItemCollection oItemCollection = _navBar.Items;
				  TreeNode node2 = node.PrevNode;
				  TreeNodeCollection collection1 = this._treeView.Nodes;
				  if (node.Parent != null && oItem.Parent != null)
				  {
					  collection1 = node.Parent.Nodes;
					  oItemCollection = oItem.Parent.ChildItems;
				  }
				  if (node2 != null)
				  {
					  node.Remove();
					  RemoveMenuItem(oItem);
					  collection1.Insert(node2.Index, node);
					  oItemCollection.Insert(oItemCollection.IndexOf(oItem2), oItem);
					  this._treeView.SelectedNode = node;
				  }
			  }
          }
      }
      /// <summary>
      /// move down the selected node
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void _moveDownButton_Click(object sender, EventArgs e)
      {
          TreeNode node = this._treeView.SelectedNode;
          if (node != null)
          {
			  MenuItem oItem = (MenuItem)node.Tag;
			  if (oItem.IsSeparator == false)
			  {
				  MenuItem oItem2 = (MenuItem)node.NextNode.Tag;
				  MenuItemCollection oItemCollection = _navBar.Items;
				  TreeNode node2 = node.NextNode;
				  TreeNodeCollection collection1 = this._treeView.Nodes;
				  if (node.Parent != null && oItem.Parent != null)
				  {
					  collection1 = node.Parent.Nodes;
					  oItemCollection = oItem.Parent.ChildItems;
				  }
				  if (node2 != null)
				  {
					  node.Remove();
					  RemoveMenuItem(oItem);
					  collection1.Insert(node2.Index + 1, node);
					  oItemCollection.Insert(oItemCollection.IndexOf(oItem2) + 1, oItem);
					  this._treeView.SelectedNode = node;
				  }
			  }
          }
      }
      /// <summary>
      /// unindent the selected node
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void _unindentButton_Click(object sender, EventArgs e)
      {
            TreeNode node = this._treeView.SelectedNode;
            MenuItem oItem = (MenuItem)_treeView.SelectedNode.Tag;
			if (node != null && oItem.IsSeparator==false)
            {
                TreeNode node2 = node.Parent;
                MenuItem oItem2 = oItem.Parent;
                if (node2 != null)
                {
                    TreeNodeCollection collection1 = this._treeView.Nodes;
                    MenuItemCollection oItemCollection = _navBar.Items;
                    if (node2.Parent != null)
                    {
                        collection1 = node2.Parent.Nodes;
                        oItemCollection = oItem2.Parent.ChildItems;
                    }
                    if (node2 != null)
                    {
                        node.Remove();
                        RemoveMenuItem(oItem);
                        collection1.Insert(node2.Index + 1, node);
                        oItemCollection.Insert(oItemCollection.IndexOf(oItem2) + 1, oItem);
                        this._treeView.SelectedNode = node;
                    }
                }
            }
      }
      protected override void OnActivated(EventArgs e)
      {
          base.OnActivated(e);
          if (this._firstActivate)
          {
              this._firstActivate = false;
              this.OnInitialActivated(e);
          }

      }
      private void OnInitialActivated(EventArgs e)
      {
          if (this._treeView.Nodes.Count > 0)
          {
              this._treeView.SelectedNode = this._treeView.Nodes[0];
          }
          this.UpdateEnabledState();
      }
      /// <summary>
      /// Update the node State
      /// </summary>
      private void UpdateEnabledState()
      {
          TreeNode node1 = this._treeView.SelectedNode;
          if (node1 != null)
          {
              this._addChildButton.Enabled = true;
              this._removeButton.Enabled = true;
              this._moveUpButton.Enabled = node1.PrevNode != null;
              this._moveDownButton.Enabled = node1.NextNode != null;
              this._indentButton.Enabled = node1.PrevNode != null;
              this._unindentButton.Enabled = node1.Parent != null;
          }
          else
          {
              this._addChildButton.Enabled = false;
              this._removeButton.Enabled = false;
              this._moveUpButton.Enabled = false;
              this._moveDownButton.Enabled = false;
              this._indentButton.Enabled = false;
              this._unindentButton.Enabled = false;
          }
      }
      /// <summary>
      /// indent the selected node
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void _indentButton_Click(object sender, EventArgs e)
      {
          TreeNode node = this._treeView.SelectedNode;
		  MenuItem oItem = (MenuItem)node.Tag;
		  if (node != null && oItem.IsSeparator == false)
          {
              TreeNode node2 = node.PrevNode;
              MenuItem oItem2 = (MenuItem)node.PrevNode.Tag;
              if (node2 != null)
              {
                  node.Remove();
                  RemoveMenuItem(oItem);
                  node2.Nodes.Add(node);
                  oItem2.ChildItems.Add(oItem);
                  this._treeView.SelectedNode = node;
              }
          }
      }
     
  }
}


