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
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ge");
            this.imageList1.Images.SetKeyName(1, "kml");
            this.imageList1.Images.SetKeyName(2, "folderClosed");
            this.imageList1.Images.SetKeyName(3, "folderOpen");
            this.imageList1.Images.SetKeyName(4, "flag");
            this.imageList1.Images.SetKeyName(5, "overlay");
            this.imageList1.Images.SetKeyName(6, "photo");
            this.imageList1.Images.SetKeyName(7, "tour");
            // 
            // KmlTreeView
            // 
            this.CheckBoxes = true;
            this.ImageIndex = 0;
            this.ImageList = this.imageList1;
            this.LineColor = System.Drawing.Color.Black;
            this.SelectedImageIndex = 0;
            this.ShowNodeToolTips = true;
            this.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.KmlTree_AfterCheck);
            this.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.KmlTreeView_AfterCollapse);
            this.DoubleClick += new System.EventHandler(this.KmlTree_DoubleClick);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.KmlTreeView_MouseUp);
            this.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.KmlTreeView_AfterExpand);
            this.ResumeLayout(false);

        }

        #endregion
    }
}