// <copyright file="KMLTreeView.Designer.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
// <summary>This file is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    /// <summary>
    /// Designer file
    /// </summary>
    public partial class KmlTreeView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// The image list for the tree nodes
        /// </summary>
        private System.Windows.Forms.ImageList imageList1;

        /// <summary>
        /// The contextMenuStripNetworkLinks item
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNetworkLinks;

        /// <summary>
        ///  The toolStripMenuItemReloadLink item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemReloadLink;

        /// <summary>
        /// The toolStripMenuItemRemoveLink item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveLink;

        /// <summary>
        /// The contextMenuStripNodes item
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNodes;

        /// <summary>
        /// The toolStripMenuItemRemoveNode item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemoveNode;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KmlTreeView));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripNetworkLinks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemReloadLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemoveLink = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNodes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemRemoveNode = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNetworkLinks.SuspendLayout();
            this.contextMenuStripNodes.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ge");
            this.imageList1.Images.SetKeyName(1, "kml");
            this.imageList1.Images.SetKeyName(2, "flag");
            this.imageList1.Images.SetKeyName(3, "overlay");
            this.imageList1.Images.SetKeyName(4, "photo");
            this.imageList1.Images.SetKeyName(5, "tour");
            this.imageList1.Images.SetKeyName(6, "folderClosed");
            this.imageList1.Images.SetKeyName(7, "folderOpen");
            this.imageList1.Images.SetKeyName(8, "linkFolderClosed");
            this.imageList1.Images.SetKeyName(9, "linkFolderOpen");
            this.imageList1.Images.SetKeyName(10, "linkFolderClosed_0");
            this.imageList1.Images.SetKeyName(11, "linkFolderClosed_1");
            this.imageList1.Images.SetKeyName(12, "linkFolderClosed_2");
            // 
            // contextMenuStripNetworkLinks
            // 
            this.contextMenuStripNetworkLinks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemReloadLink,
            this.toolStripMenuItemRemoveLink});
            this.contextMenuStripNetworkLinks.Name = "contextMenuStripNetworkLinks";
            this.contextMenuStripNetworkLinks.Size = new System.Drawing.Size(118, 48);
            // 
            // toolStripMenuItemReload
            // 
            this.toolStripMenuItemReloadLink.Name = "toolStripMenuItemReload";
            this.toolStripMenuItemReloadLink.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItemReloadLink.Text = "Reload";
            this.toolStripMenuItemReloadLink.ToolTipText = "Reloads the link content";
            this.toolStripMenuItemReloadLink.Click += new System.EventHandler(this.ToolStripMenuItemReload_Click);
            // 
            // toolStripMenuItemRemove
            // 
            this.toolStripMenuItemRemoveLink.Name = "toolStripMenuItemRemove";
            this.toolStripMenuItemRemoveLink.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItemRemoveLink.Text = "Remove";
            this.toolStripMenuItemRemoveLink.ToolTipText = "Remove the item";
            this.toolStripMenuItemRemoveLink.Click += new System.EventHandler(this.ToolStripMenuItemRemove_Click);
            // 
            // contextMenuStripNodes
            // 
            this.contextMenuStripNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRemoveNode});
            this.contextMenuStripNodes.Name = "contextMenuStripNodes";
            this.contextMenuStripNodes.Size = new System.Drawing.Size(118, 26);
            // 
            // toolStripMenuItemRemoveNode
            // 
            this.toolStripMenuItemRemoveNode.Name = "toolStripMenuItemRemoveNode";
            this.toolStripMenuItemRemoveNode.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItemRemoveNode.Text = "Remove";
            this.toolStripMenuItemRemoveNode.Click += new System.EventHandler(ToolStripMenuItemRemove_Click);
            // 
            // KmlTreeView
            // 
            this.CheckBoxes = true;
            this.ImageKey = "ge";
            this.ImageList = this.imageList1;
            this.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.SelectedImageIndex = 0;
            this.contextMenuStripNetworkLinks.ResumeLayout(false);
            this.contextMenuStripNodes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// Called when a reload context menu item is clicked
        /// </summary>
        /// <param name="sender">the kml tree view</param>
        /// <param name="e">The event arguments</param>
        private void ToolStripMenuItemReload_Click(object sender, System.EventArgs e)
        {
            KmlTreeViewNode node = this.SelectedNode as KmlTreeViewNode;
            node.Fetched = false;
            node.Collapse();
            node.Expand();
        }

        /// <summary>
        /// Called when a remove context menu item is clicked
        /// </summary>
        /// <param name="sender">the kml tree view</param>
        /// <param name="e">The event arguments</param>
        private void ToolStripMenuItemRemove_Click(object sender, System.EventArgs e)
        {
            KmlTreeViewNode node = this.SelectedNode as KmlTreeViewNode;
            dynamic parent = node.ApiObject.getParentNode();

            if (parent != null)
            {
                parent.getFeatures().removeChild(node.ApiObject);
            }
            else
            {
                this.gewb.Plugin.getFeatures().removeChild(node.ApiObject);
            }

            this.Nodes.Remove(node);
        }
    }
}