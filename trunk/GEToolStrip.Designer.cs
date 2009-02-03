// <copyright file="GEToolStrip.Designer.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-01-30</date>
// <summary>This program is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    /// <summary>
    /// Designer file
    /// </summary>
    public partial class GEToolStrip
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// The navigation text box
        /// </summary>
        private System.Windows.Forms.ToolStripTextBox navigationTextBox;

        /// <summary>
        /// The navigation 'go' button
        /// </summary>
        private System.Windows.Forms.ToolStripButton submitButton;

        /// <summary>
        /// The naviagtion seperator
        /// </summary>
        private System.Windows.Forms.ToolStripSeparator navigationSeparator;

        /// <summary>
        /// The view drop down button
        /// </summary>
        private System.Windows.Forms.ToolStripDropDownButton viewDropDownButton;

        /// <summary>
        /// The sky menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem skyMenuItem;

        /// <summary>
        /// The sun menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem sunMenuItem;

        /// <summary>
        /// The options drop down button
        /// </summary>
        private System.Windows.Forms.ToolStripDropDownButton optionsDropDownButton;

        /// <summary>
        /// The borders menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem bordersMenuItem;

        /// <summary>
        /// The buildings menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem buildingsMenuItem;

        /// <summary>
        /// The roads menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem roadsMenuItem;

        /// <summary>
        /// The terrain menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem terrainMenuItem;

        /// <summary>
        /// The layers menu item
        /// </summary>
        private System.Windows.Forms.ToolStripDropDownButton layersDropDownButton;

        /// <summary>
        /// The staus menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem statusBarMenuItem;

        /// <summary>
        /// The grid menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem gridMenuItem;

        /// <summary>
        /// The overview map menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem overviewMapMenuItem;

        /// <summary>
        /// The scale legend menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem scaleLegendMenuItem;

        /// <summary>
        /// The atmosphere menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem atmosphereMenuItem;

        /// <summary>
        /// The refresh button
        /// </summary>
        private System.Windows.Forms.ToolStripButton refreshButton;

        /// <summary>
        /// The default image list
        /// </summary>
        private System.Windows.Forms.ImageList imageList1;

        /// <summary>
        /// The scale mouse navigation menu item
        /// </summary>
        private System.Windows.Forms.ToolStripMenuItem mouseNavigationMenuItem;

        /// <summary>
        /// The dropDownSeparator seperator
        /// </summary>
        private System.Windows.Forms.ToolStripSeparator dropDownSeparator;

        /// <summary>
        /// The print screen button
        /// </summary>
        private System.Windows.Forms.ToolStripButton screenGrabButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GEToolStrip));
            this.navigationTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.submitButton = new System.Windows.Forms.ToolStripButton();
            this.navigationSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.viewDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.skyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sunMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.statusBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overviewMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleLegendMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.atmosphereMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseNavigationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.bordersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roadsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terrainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.dropDownSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.screenGrabButton = new System.Windows.Forms.ToolStripButton();
            this.SuspendLayout();
            // 
            // navigationTextBox
            // 
            this.navigationTextBox.AutoSize = false;
            this.navigationTextBox.Name = "navigationTextBox";
            this.navigationTextBox.Size = new System.Drawing.Size(100, 21);
            this.navigationTextBox.Tag = "NAVIGATION";
            this.navigationTextBox.ToolTipText = "Enter a location or the url of a kml\\kmz file";
            this.navigationTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NavigationTextBox_KeyUp);
            // 
            // submitButton
            // 
            this.submitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.submitButton.ImageKey = "go";
            this.submitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(23, 20);
            this.submitButton.ToolTipText = "Go!";
            this.submitButton.Click += new System.EventHandler(this.NavigationButton_Click);
            // 
            // navigationSeparator
            // 
            this.navigationSeparator.Name = "navigationSeparator";
            this.navigationSeparator.Size = new System.Drawing.Size(6, 6);
            // 
            // viewDropDownButton
            // 
            this.viewDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skyMenuItem,
            this.sunMenuItem});
            this.viewDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewDropDownButton.Name = "viewDropDownButton";
            this.viewDropDownButton.Size = new System.Drawing.Size(42, 17);
            this.viewDropDownButton.Tag = "VIEW";
            this.viewDropDownButton.Text = "View";
            this.viewDropDownButton.ToolTipText = "Change the View settings";
            // 
            // skyMenuItem
            // 
            this.skyMenuItem.CheckOnClick = true;
            this.skyMenuItem.Name = "skyMenuItem";
            this.skyMenuItem.Size = new System.Drawing.Size(131, 22);
            this.skyMenuItem.Tag = "SKY";
            this.skyMenuItem.Text = "Sky Mode";
            this.skyMenuItem.ToolTipText = "Toggle Sky and Earth mode";
            this.skyMenuItem.Click += new System.EventHandler(this.ViewItem_Clicked);
            // 
            // sunMenuItem
            // 
            this.sunMenuItem.CheckOnClick = true;
            this.sunMenuItem.Name = "sunMenuItem";
            this.sunMenuItem.Size = new System.Drawing.Size(131, 22);
            this.sunMenuItem.Tag = "SUN";
            this.sunMenuItem.Text = "Sun";
            this.sunMenuItem.ToolTipText = "Toggle the sun visiblity";
            this.sunMenuItem.Click += new System.EventHandler(this.ViewItem_Clicked);
            // 
            // optionsDropDownButton
            // 
            this.optionsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optionsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarMenuItem,
            this.gridMenuItem,
            this.overviewMapMenuItem,
            this.scaleLegendMenuItem,
            this.atmosphereMenuItem,
            this.mouseNavigationMenuItem});
            this.optionsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsDropDownButton.Name = "optionsDropDownButton";
            this.optionsDropDownButton.Size = new System.Drawing.Size(57, 17);
            this.optionsDropDownButton.Tag = "OPTIONS";
            this.optionsDropDownButton.Text = "Options";
            this.optionsDropDownButton.ToolTipText = "Toggle the various options";
            // 
            // statusBarMenuItem
            // 
            this.statusBarMenuItem.CheckOnClick = true;
            this.statusBarMenuItem.Name = "statusBarMenuItem";
            this.statusBarMenuItem.Size = new System.Drawing.Size(169, 22);
            this.statusBarMenuItem.Tag = "STATUS";
            this.statusBarMenuItem.Text = "Status bar";
            this.statusBarMenuItem.ToolTipText = "Toggle the Status bar visiblity";
            this.statusBarMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // gridMenuItem
            // 
            this.gridMenuItem.CheckOnClick = true;
            this.gridMenuItem.Name = "gridMenuItem";
            this.gridMenuItem.Size = new System.Drawing.Size(169, 22);
            this.gridMenuItem.Tag = "GRID";
            this.gridMenuItem.Text = "Grid";
            this.gridMenuItem.ToolTipText = "Toggle the Grid visiblity";
            this.gridMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // overviewMapMenuItem
            // 
            this.overviewMapMenuItem.CheckOnClick = true;
            this.overviewMapMenuItem.Name = "overviewMapMenuItem";
            this.overviewMapMenuItem.Size = new System.Drawing.Size(169, 22);
            this.overviewMapMenuItem.Tag = "OVERVIEW";
            this.overviewMapMenuItem.Text = "Overview map";
            this.overviewMapMenuItem.ToolTipText = "Toggle the Overview map visiblity";
            this.overviewMapMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // scaleLegendMenuItem
            // 
            this.scaleLegendMenuItem.CheckOnClick = true;
            this.scaleLegendMenuItem.Name = "scaleLegendMenuItem";
            this.scaleLegendMenuItem.Size = new System.Drawing.Size(169, 22);
            this.scaleLegendMenuItem.Tag = "SCALE";
            this.scaleLegendMenuItem.Text = "Scale legend";
            this.scaleLegendMenuItem.ToolTipText = "Toggle the Scale legend visiblity";
            this.scaleLegendMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // atmosphereMenuItem
            // 
            this.atmosphereMenuItem.Checked = true;
            this.atmosphereMenuItem.CheckOnClick = true;
            this.atmosphereMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.atmosphereMenuItem.Name = "atmosphereMenuItem";
            this.atmosphereMenuItem.Size = new System.Drawing.Size(169, 22);
            this.atmosphereMenuItem.Tag = "ATMOSPHERE";
            this.atmosphereMenuItem.Text = "Atmosphere";
            this.atmosphereMenuItem.ToolTipText = "Toggle the Atmosphere visiblity";
            this.atmosphereMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // mouseNavigationMenuItem
            // 
            this.mouseNavigationMenuItem.Checked = true;
            this.mouseNavigationMenuItem.CheckOnClick = true;
            this.mouseNavigationMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mouseNavigationMenuItem.Name = "mouseNavigationMenuItem";
            this.mouseNavigationMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mouseNavigationMenuItem.Tag = "MOUSE";
            this.mouseNavigationMenuItem.Text = "Mouse navigation";
            this.mouseNavigationMenuItem.ToolTipText = "Toggle Mouse navigation enabled";
            this.mouseNavigationMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // layersDropDownButton
            // 
            this.layersDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.layersDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bordersMenuItem,
            this.buildingsMenuItem,
            this.roadsMenuItem,
            this.terrainMenuItem});
            this.layersDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.layersDropDownButton.Name = "layersDropDownButton";
            this.layersDropDownButton.Size = new System.Drawing.Size(52, 17);
            this.layersDropDownButton.Tag = "LAYERS";
            this.layersDropDownButton.Text = "Layers";
            this.layersDropDownButton.ToolTipText = "Toggle the in-built layers";
            // 
            // bordersMenuItem
            // 
            this.bordersMenuItem.CheckOnClick = true;
            this.bordersMenuItem.Name = "bordersMenuItem";
            this.bordersMenuItem.Size = new System.Drawing.Size(126, 22);
            this.bordersMenuItem.Tag = "BORDERS";
            this.bordersMenuItem.Text = "Borders";
            this.bordersMenuItem.ToolTipText = "Toggle the Borders layer";
            this.bordersMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // buildingsMenuItem
            // 
            this.buildingsMenuItem.CheckOnClick = true;
            this.buildingsMenuItem.Name = "buildingsMenuItem";
            this.buildingsMenuItem.Size = new System.Drawing.Size(126, 22);
            this.buildingsMenuItem.Tag = "BUILDINGS";
            this.buildingsMenuItem.Text = "Buildings";
            this.buildingsMenuItem.ToolTipText = "Toggle the Buildings layer";
            this.buildingsMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // roadsMenuItem
            // 
            this.roadsMenuItem.CheckOnClick = true;
            this.roadsMenuItem.Name = "roadsMenuItem";
            this.roadsMenuItem.Size = new System.Drawing.Size(126, 22);
            this.roadsMenuItem.Tag = "ROADS";
            this.roadsMenuItem.Text = "Roads";
            this.roadsMenuItem.ToolTipText = "Toggle the Roads layer";
            this.roadsMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // terrainMenuItem
            // 
            this.terrainMenuItem.Checked = true;
            this.terrainMenuItem.CheckOnClick = true;
            this.terrainMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.terrainMenuItem.Name = "terrainMenuItem";
            this.terrainMenuItem.Size = new System.Drawing.Size(126, 22);
            this.terrainMenuItem.Tag = "TERRAIN";
            this.terrainMenuItem.Text = "Terrain";
            this.terrainMenuItem.ToolTipText = "Toggle the Terrain layer";
            this.terrainMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.ImageKey = "refresh";
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 20);
            this.refreshButton.Tag = "REFRESH";
            this.refreshButton.Text = "refresh";
            this.refreshButton.ToolTipText = "Refresh the plugin";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "go");
            this.imageList1.Images.SetKeyName(1, "refresh");
            this.imageList1.Images.SetKeyName(2, "jpg");
            // 
            // dropDownSeparator
            // 
            this.dropDownSeparator.Name = "dropDownSeparator";
            this.dropDownSeparator.Size = new System.Drawing.Size(6, 6);
            // 
            // screenGrabButton
            // 
            this.screenGrabButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.screenGrabButton.ImageKey = "jpg";
            this.screenGrabButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.screenGrabButton.Name = "screenGrabButton";
            this.screenGrabButton.Size = new System.Drawing.Size(23, 20);
            this.screenGrabButton.Tag = "SCREENGRAB";
            this.screenGrabButton.Text = "PrtScr";
            this.screenGrabButton.ToolTipText = "Screen Grab";
            this.screenGrabButton.Click += new System.EventHandler(this.ScreenGrabButton_Click);
            // 
            // GEToolStrip
            // 
            this.ImageList = this.imageList1;
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigationTextBox,
            this.submitButton,
            this.refreshButton,
            this.navigationSeparator,
            this.viewDropDownButton,
            this.optionsDropDownButton,
            this.layersDropDownButton,
            this.dropDownSeparator,
            this.screenGrabButton});
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.GEToolStrip_Layout);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
