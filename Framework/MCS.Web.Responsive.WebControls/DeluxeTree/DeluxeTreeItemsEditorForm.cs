using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web.UI.Design;
using System.ComponentModel.Design;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// Summary description for TreeViewAutoFormatEditorForm.
    /// </summary>
    internal class DeluxeTreeItemsEditorForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TreeView _treeView;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.ComponentModel.IContainer components;
        private IServiceProvider _serviceProvider;

        private DeluxeTree _navBar;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private bool _firstActivate;
        private ToolStrip DeluxTreeToolStrip;
        private ToolStripButton _addRootButton;
        private ToolStripButton _addChildButton;
        private ToolStripButton _removeButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton _moveUpButton;
        private ToolStripButton _moveDownButton;
        private ToolStripButton _indentButton;
        private ToolStripButton _unindentButton;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;

        public DeluxeTreeNodeCollection Items;

        private void LoadNodes(MCS.Web.Responsive.WebControls.DeluxeTreeNode oItem, TreeNode oTreeNode)
        {
            oTreeNode.Tag = oItem;

            foreach (MCS.Web.Responsive.WebControls.DeluxeTreeNode oChild in oItem.Nodes)
            {
                TreeNode oChildNode = new TreeNode(oChild.Text);
                LoadNodes(oChild, oChildNode);

                oTreeNode.Nodes.Add(oChildNode);
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
        protected override object GetService(Type serviceType)
        {
            if (this._serviceProvider != null)
            {
                return this._serviceProvider.GetService(serviceType);
            }
            return null;
        }


        public DeluxeTreeItemsEditorForm(DeluxeTree oMenu)
        {
            InitializeComponent();
          
            this._firstActivate = true;
            _navBar = oMenu;
            Items = oMenu.Nodes;
            _serviceProvider = oMenu.Site;
            // add pre-existing nodes
            foreach (MCS.Web.Responsive.WebControls.DeluxeTreeNode oRoot in Items)
            {
                TreeNode oRootNode = new TreeNode(oRoot.Text);
                LoadNodes(oRoot, oRootNode);
                _treeView.Nodes.Add(oRootNode);
            }
            this.propertyGrid1.Site = new FormPropertyGridSite(_serviceProvider, this.propertyGrid1);
            _treeView.HideSelection = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    _serviceProvider = null;
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.SplitContainer splitContainer1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeluxeTreeItemsEditorForm));
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.DeluxTreeToolStrip = new System.Windows.Forms.ToolStrip();
            this._addRootButton = new System.Windows.Forms.ToolStripButton();
            this._addChildButton = new System.Windows.Forms.ToolStripButton();
            this._removeButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._moveUpButton = new System.Windows.Forms.ToolStripButton();
            this._moveDownButton = new System.Windows.Forms.ToolStripButton();
            this._indentButton = new System.Windows.Forms.ToolStripButton();
            this._unindentButton = new System.Windows.Forms.ToolStripButton();
            this._treeView = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.DeluxTreeToolStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            splitContainer1.Size = new System.Drawing.Size(518, 366);
            splitContainer1.SplitterDistance = 257;
            splitContainer1.TabIndex = 11;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.Controls.Add(this.DeluxTreeToolStrip, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._treeView, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(257, 366);
            this.tableLayoutPanel3.TabIndex = 11;
            // 
            // DeluxTreeToolStrip
            // 
            this.DeluxTreeToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeluxTreeToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.DeluxTreeToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.DeluxTreeToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._addRootButton,
            this._addChildButton,
            this._removeButton,
            this.toolStripSeparator1,
            this._moveUpButton,
            this._moveDownButton,
            this._indentButton,
            this._unindentButton});
            this.DeluxTreeToolStrip.Location = new System.Drawing.Point(0, 22);
            this.DeluxTreeToolStrip.Name = "DeluxTreeToolStrip";
            this.DeluxTreeToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.DeluxTreeToolStrip.Size = new System.Drawing.Size(257, 29);
            this.DeluxTreeToolStrip.TabIndex = 10;
            this.DeluxTreeToolStrip.Text = "toolStrip1";
            // 
            // _addRootButton
            // 
            this._addRootButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addRootButton.Image = ((System.Drawing.Image)(resources.GetObject("_addRootButton.Image")));
            this._addRootButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addRootButton.Name = "_addRootButton";
            this._addRootButton.Size = new System.Drawing.Size(23, 26);
            this._addRootButton.Text = "Add Root Button";
            this._addRootButton.Click += new System.EventHandler(this._addRootButton_Click);
            // 
            // _addChildButton
            // 
            this._addChildButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addChildButton.Image = ((System.Drawing.Image)(resources.GetObject("_addChildButton.Image")));
            this._addChildButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._addChildButton.Name = "_addChildButton";
            this._addChildButton.Size = new System.Drawing.Size(23, 26);
            this._addChildButton.Text = "Add Child Button";
            this._addChildButton.Click += new System.EventHandler(this._addChildButton_Click);
            // 
            // _removeButton
            // 
            this._removeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._removeButton.Image = ((System.Drawing.Image)(resources.GetObject("_removeButton.Image")));
            this._removeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(23, 26);
            this._removeButton.Text = "Remove Button";
            this._removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // _moveUpButton
            // 
            this._moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._moveUpButton.Image = ((System.Drawing.Image)(resources.GetObject("_moveUpButton.Image")));
            this._moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._moveUpButton.Name = "_moveUpButton";
            this._moveUpButton.Size = new System.Drawing.Size(23, 26);
            this._moveUpButton.Text = "Move Up Button";
            this._moveUpButton.Click += new System.EventHandler(this._moveUpButton_Click);
            // 
            // _moveDownButton
            // 
            this._moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._moveDownButton.Image = ((System.Drawing.Image)(resources.GetObject("_moveDownButton.Image")));
            this._moveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._moveDownButton.Name = "_moveDownButton";
            this._moveDownButton.Size = new System.Drawing.Size(23, 26);
            this._moveDownButton.Text = "Move Down Button";
            this._moveDownButton.Click += new System.EventHandler(this._moveDownButton_Click);
            // 
            // _indentButton
            // 
            this._indentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._indentButton.Image = ((System.Drawing.Image)(resources.GetObject("_indentButton.Image")));
            this._indentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._indentButton.Name = "_indentButton";
            this._indentButton.Size = new System.Drawing.Size(23, 26);
            this._indentButton.Text = "Indent Button";
            this._indentButton.Click += new System.EventHandler(this._indentButton_Click);
            // 
            // _unindentButton
            // 
            this._unindentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._unindentButton.Image = ((System.Drawing.Image)(resources.GetObject("_unindentButton.Image")));
            this._unindentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._unindentButton.Name = "_unindentButton";
            this._unindentButton.Size = new System.Drawing.Size(23, 26);
            this._unindentButton.Text = "Unindent Button";
            this._unindentButton.Click += new System.EventHandler(this._unindentButton_Click);
            // 
            // _treeView
            // 
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.Location = new System.Drawing.Point(3, 54);
            this._treeView.Name = "_treeView";
            this._treeView.Size = new System.Drawing.Size(251, 275);
            this._treeView.TabIndex = 0;
            this._treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeView_AfterSelect);
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
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(257, 366);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 25);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(251, 301);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_ValueChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.5F));
            this.tableLayoutPanel2.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(14, 332);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(240, 31);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(89, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(62, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "完成";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(157, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 24);
            this.button2.TabIndex = 3;
            this.button2.Text = "取消";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 474);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(462, 0);
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // DeluxeTreeItemsEditorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(518, 366);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(this.pictureBox2);
            this.Name = "DeluxeTreeItemsEditorForm";
            this.Text = "树设计器";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.DeluxTreeToolStrip.ResumeLayout(false);
            this.DeluxTreeToolStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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
            if (e.ChangedItem.Label == "Text")
            {
                _treeView.SelectedNode.Text = (string)e.ChangedItem.Value;
            }
        }
        /// <summary>
        /// new root
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _addRootButton_Click(object sender, EventArgs e)
        {
            MCS.Web.Responsive.WebControls.DeluxeTreeNode oItem = new MCS.Web.Responsive.WebControls.DeluxeTreeNode();
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

            MCS.Web.Responsive.WebControls.DeluxeTreeNode oItem = (MCS.Web.Responsive.WebControls.DeluxeTreeNode)_treeView.SelectedNode.Tag;

            MCS.Web.Responsive.WebControls.DeluxeTreeNode oNewItem = new MCS.Web.Responsive.WebControls.DeluxeTreeNode();
            oNewItem.Text = "New Item";

            oItem.Nodes.Add(oNewItem);

            TreeNode oNewTreeNode = new TreeNode("New Item");
            oNewTreeNode.Tag = oNewItem;
            _treeView.SelectedNode.Nodes.Add(oNewTreeNode);
            _treeView.SelectedNode.Expand();
        }
        /// <summary>
        /// delete the selected  node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            TreeNode node1 = this._treeView.SelectedNode;
            if (node1 != null)
            {
                DeluxeTreeNode oItem = (DeluxeTreeNode)_treeView.SelectedNode.Tag;
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
                RemoveTreeItem(oItem);
                node1.Remove();
                if (this._treeView.SelectedNode == null)
                {
                    this.propertyGrid1.SelectedObject = null;
                }
            }
        }

        private void RemoveTreeItem(DeluxeTreeNode oItem)
        {
            if (oItem.Parent == null)
            {
                // we have a root
                // oItem.Remove(oItem);
                _navBar.Nodes.Remove(oItem);

            }
            else
            {
                // we have a child
                oItem.Parent.Nodes.Remove(oItem);
            }
        }
        /// <summary>
        /// move up node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _moveUpButton_Click(object sender, EventArgs e)
        {
            TreeNode node = this._treeView.SelectedNode;
            if (node != null)
            {
                DeluxeTreeNode oItem = (DeluxeTreeNode)node.Tag;
                DeluxeTreeNode oItem2 = (DeluxeTreeNode)node.PrevNode.Tag;
                DeluxeTreeNodeCollection oItemCollection = _navBar.Nodes;
                TreeNode node2 = node.PrevNode;
                TreeNodeCollection collection1 = this._treeView.Nodes;
                if (node.Parent != null && oItem.Parent!=null)
                {
                    collection1 = node.Parent.Nodes;
                    oItemCollection = oItem.Parent.Nodes;
                }
                if (node2 != null)
                {
                    node.Remove();
                    RemoveTreeItem(oItem); 
                    collection1.Insert(node2.Index, node);
                    oItemCollection.AddAt(oItemCollection.IndexOf(oItem2), oItem);
                    this._treeView.SelectedNode = node;
                }
            }
        }
        /// <summary>
        /// move down node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _moveDownButton_Click(object sender, EventArgs e)
        {
            TreeNode node = this._treeView.SelectedNode;
            if (node != null)
            {
                DeluxeTreeNode oItem = (DeluxeTreeNode)node.Tag;
                DeluxeTreeNode oItem2 = (DeluxeTreeNode)node.NextNode.Tag;
                DeluxeTreeNodeCollection oItemCollection = _navBar.Nodes;
                TreeNode node2 = node.NextNode;
                TreeNodeCollection collection1 = this._treeView.Nodes;
                if (node.Parent != null && oItem.Parent != null)
                {
                    collection1 = node.Parent.Nodes;
                    oItemCollection = oItem.Parent.Nodes;
                }
                if (node2 != null)
                {
                    node.Remove();
                    RemoveTreeItem(oItem); 
                    collection1.Insert(node2.Index+1, node);
                    oItemCollection.AddAt(oItemCollection.IndexOf(oItem2)+1, oItem);
                    this._treeView.SelectedNode = node;
                }
            }
        }
        /// <summary>
        /// indent the node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _indentButton_Click(object sender, EventArgs e)
        {
            TreeNode node = this._treeView.SelectedNode;
            if (node != null)
            {
                TreeNode node2 = node.PrevNode;
                DeluxeTreeNode oItem = (DeluxeTreeNode)node.Tag;
                DeluxeTreeNode oItem2 = (DeluxeTreeNode)node.PrevNode.Tag;
                if (node2 != null)
                {
                    node.Remove();
                    RemoveTreeItem(oItem); 
                    node2.Nodes.Add(node);
                    oItem2.Nodes.Add(oItem);
                    this._treeView.SelectedNode = node;
                }
            }
        }
        /// <summary>
        /// unindent the node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _unindentButton_Click(object sender, EventArgs e)
        {
            TreeNode node = this._treeView.SelectedNode;
            DeluxeTreeNode oItem = (DeluxeTreeNode)_treeView.SelectedNode.Tag;
            if (node != null)
            {
                TreeNode node2 = node.Parent;
                DeluxeTreeNode oItem2 = oItem.Parent;
                if (node2 != null)
                {
                    TreeNodeCollection collection1 = this._treeView.Nodes;
                    DeluxeTreeNodeCollection oItemCollection = _navBar.Nodes;
                    if (node2.Parent != null)
                    {
                        collection1 = node2.Parent.Nodes;
                        oItemCollection = oItem2.Parent.Nodes;
                    }
                    if (node2 != null)
                    {
                        node.Remove();
                        RemoveTreeItem(oItem); 
                        collection1.Insert(node2.Index + 1, node);
                        oItemCollection.AddAt(oItemCollection.IndexOf(oItem2) + 1, oItem);
                        this._treeView.SelectedNode = node;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 基类
    /// </summary>
    public class FormPropertyGridSite : ISite, IServiceProvider
    {
        /// <summary>
        ///  Constructors
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="comp"></param>
        public FormPropertyGridSite(IServiceProvider sp, IComponent comp)
        {
            this.sp = sp;
            this.comp = comp;
        }


        // Methods
        object System.IServiceProvider.GetService(Type t)
        {
            if (t.Equals(typeof(IDesignerHost)))
            {
                return this.sp.GetService(t);
            }
            return null;
        }


        // Properties
        IComponent System.ComponentModel.ISite.Component
        {
            get
            {
                return this.comp;
            }
        }

        IContainer System.ComponentModel.ISite.Container
        {
            get
            {
                return null;
            }
        }

        bool System.ComponentModel.ISite.DesignMode
        {
            get
            {
                return false;
            }
        }

        string System.ComponentModel.ISite.Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }


        // Instance Fields
        private IServiceProvider sp;
        private IComponent comp;
    }
}


