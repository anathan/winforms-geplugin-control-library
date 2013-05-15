// <copyright file="KMLTreeView.Designer.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-12-08</date>
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
    public sealed partial class KmlTreeView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// The tree view image list for KML features
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

        #region Component Designer generated code

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
            this.imageList1.Images.SetKeyName(13, "linkFolderClosedDisconected");
            // 
            // KmlTreeView
            // 
            this.ImageKey = "ge";
            this.ImageList = this.imageList1;
            this.SelectedImageIndex = 0;
            this.ResumeLayout(false);

        }

        #endregion
    }
}