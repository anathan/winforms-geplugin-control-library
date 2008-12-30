// <copyright file="GEToolStrip.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
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
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using GEPlugin;

    /// <summary>
    /// The GEToolStrip provides a quick way to access and set the plugin options
    /// </summary>
    public partial class GEToolStrip : ToolStrip, IGEControls
    {
        #region Private fields

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private IGEPlugin geplugin = null;

        /// <summary>
        /// An instance of the current document
        /// </summary>
        private HtmlDocument htmlDocument = null;

        /// <summary>
        /// An instance of the current browser
        /// </summary>
        private GEWebBrowser gewebborwser = null;

        /// <summary>
        /// Indicates whether the navigation items are visible
        /// </summary>
        private bool navigationItemsVisibility = true;

        /// <summary>
        /// Indicates whether the layer items are visible
        /// </summary>
        private bool layerDropDownVisiblity = true;

        /// <summary>
        /// Indicates whether the options items are visible 
        /// </summary>
        private bool optionDropDownVisiblity = true;

        /// <summary>
        /// Indicates whether the view items are visible
        /// </summary>
        private bool viewDropDownVisiblity = true;

        /// <summary>
        /// The navigation text box
        /// </summary>
        private ToolStripTextBox navigationTextBox;

        /// <summary>
        /// The navigation 'go' button
        /// </summary>
        private ToolStripButton goButton;

        /// <summary>
        /// The naviagtion seperator
        /// </summary>
        private ToolStripSeparator navigationSeparator;

        /// <summary>
        /// The view drop down button
        /// </summary>
        private ToolStripDropDownButton viewDropDownButton;

        /// <summary>
        /// The sky menu item
        /// </summary>
        private ToolStripMenuItem skyMenuItem;

        /// <summary>
        /// The sun menu item
        /// </summary>
        private ToolStripMenuItem sunMenuItem;

        /// <summary>
        /// The options drop down button
        /// </summary>
        private ToolStripDropDownButton optionsDropDownButton;

        /// <summary>
        /// The borders menu item
        /// </summary>
        private ToolStripMenuItem bordersMenuItem;

        /// <summary>
        /// The buildings menu item
        /// </summary>
        private ToolStripMenuItem buildingsMenuItem;

        /// <summary>
        /// The roads menu item
        /// </summary>
        private ToolStripMenuItem roadsMenuItem;

        /// <summary>
        /// The terrain menu item
        /// </summary>
        private ToolStripMenuItem terrainMenuItem;

        /// <summary>
        /// The layers menu item
        /// </summary>
        private ToolStripDropDownButton layersDropDownButton;

        /// <summary>
        /// The staus menu item
        /// </summary>
        private ToolStripMenuItem statusBarMenuItem;

        /// <summary>
        /// The grid menu item
        /// </summary>
        private ToolStripMenuItem gridMenuItem;

        /// <summary>
        /// The overview map menu item
        /// </summary>
        private ToolStripMenuItem overviewMapMenuItem;

        /// <summary>
        /// The scale legend menu item
        /// </summary>
        private ToolStripMenuItem scaleLegendMenuItem;

        /// <summary>
        /// The atmosphere menu item
        /// </summary>
        private ToolStripMenuItem atmosphereMenuItem;

        /// <summary>
        /// The refresh button
        /// </summary>
        private ToolStripButton refreshButton;

        /// <summary>
        /// The default image list
        /// </summary>
        private ImageList imageList1;

        /// <summary>
        /// The scale mouse navigation menu item
        /// </summary>
        private ToolStripMenuItem mouseNavigationMenuItem;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEToolStrip class.
        /// </summary>
        public GEToolStrip()
            : base()
        {
            this.InitializeComponent();
        }

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether the navigation items are visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the Navigation items."),
        DefaultValueAttribute(true)]
        public bool ShowNavigationItems
        {
            get
            {
                return this.navigationItemsVisibility;
            }

            set
            {
                this.navigationItemsVisibility = value;
                this.navigationTextBox.Visible = value;
                this.goButton.Visible = value;
                this.refreshButton.Visible = value;
                this.navigationSeparator.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Layers drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the Layers drop down menu."),
        DefaultValueAttribute(true)]
        public bool ShowLayersDropDown
        {
            get
            {
                return this.layerDropDownVisiblity;
            }

            set
            {
                this.layerDropDownVisiblity = value;
                this.layersDropDownButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Options drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the Options drop down menu."),
        DefaultValueAttribute(true)]
        public bool ShowOptionsDropDown
        {
            get
            {
                return this.optionDropDownVisiblity;
            }

            set
            {
                this.optionDropDownVisiblity = value;
                this.optionsDropDownButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the View drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the View drop down."),
        DefaultValueAttribute(true)]
        public bool ShowViewDropDown
        {
            get
            {
                return this.viewDropDownVisiblity;
            }

            set
            {
                this.viewDropDownVisiblity = value;
                this.viewDropDownButton.Visible = value;
            }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="browser"></param>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewebborwser = browser;
            this.geplugin = browser.GetPlugin();
            this.htmlDocument = browser.Document;
        }

        #endregion

        #region Protected methods

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

        #endregion

        #region Private methods

        /// <summary>
        /// Required method for Designer support
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(GEToolStrip));
            this.navigationTextBox = new ToolStripTextBox();
            this.goButton = new ToolStripButton();
            this.navigationSeparator = new ToolStripSeparator();
            this.viewDropDownButton = new ToolStripDropDownButton();
            this.skyMenuItem = new ToolStripMenuItem();
            this.sunMenuItem = new ToolStripMenuItem();
            this.optionsDropDownButton = new ToolStripDropDownButton();
            this.statusBarMenuItem = new ToolStripMenuItem();
            this.gridMenuItem = new ToolStripMenuItem();
            this.overviewMapMenuItem = new ToolStripMenuItem();
            this.scaleLegendMenuItem = new ToolStripMenuItem();
            this.atmosphereMenuItem = new ToolStripMenuItem();
            this.mouseNavigationMenuItem = new ToolStripMenuItem();
            this.layersDropDownButton = new ToolStripDropDownButton();
            this.bordersMenuItem = new ToolStripMenuItem();
            this.buildingsMenuItem = new ToolStripMenuItem();
            this.roadsMenuItem = new ToolStripMenuItem();
            this.terrainMenuItem = new ToolStripMenuItem();
            this.refreshButton = new ToolStripButton();
            this.imageList1 = new ImageList(this.components);
            this.SuspendLayout();
            // 
            // navigationTextBox
            // 
            this.navigationTextBox.AutoSize = false;
            this.navigationTextBox.Name = "navigationTextBox";
            this.navigationTextBox.Size = new Size(100, 21);
            this.navigationTextBox.Tag = "NAVIGATION";
            this.navigationTextBox.ToolTipText = "Enter a location or the url of a kml\\kmz file";
            this.navigationTextBox.KeyUp += new KeyEventHandler(this.NavigationTextBox_KeyUp);
            // 
            // goButton
            // 
            this.goButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.goButton.ImageKey = "go";
            this.goButton.ImageTransparentColor = Color.Magenta;
            this.goButton.Name = "goButton";
            this.goButton.Size = new Size(23, 20);
            this.goButton.ToolTipText = "Go!";
            this.goButton.Click += new System.EventHandler(this.NavigationButton_Click);
            // 
            // navigationSeparator
            // 
            this.navigationSeparator.Name = "navigationSeparator";
            this.navigationSeparator.Size = new Size(6, 6);
            // 
            // viewDropDownButton
            // 
            this.viewDropDownButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.viewDropDownButton.DropDownItems.AddRange(new ToolStripItem[] {
            this.skyMenuItem,
            this.sunMenuItem});
            this.viewDropDownButton.ImageTransparentColor = Color.Magenta;
            this.viewDropDownButton.Name = "viewDropDownButton";
            this.viewDropDownButton.Size = new Size(42, 17);
            this.viewDropDownButton.Tag = "VIEW";
            this.viewDropDownButton.Text = "View";
            this.viewDropDownButton.ToolTipText = "Change the View settings";
            // 
            // skyMenuItem
            // 
            this.skyMenuItem.CheckOnClick = true;
            this.skyMenuItem.Name = "skyMenuItem";
            this.skyMenuItem.Size = new Size(131, 22);
            this.skyMenuItem.Tag = "SKY";
            this.skyMenuItem.Text = "Sky Mode";
            this.skyMenuItem.ToolTipText = "Toggle Sky and Earth mode";
            this.skyMenuItem.Click += new System.EventHandler(this.ViewItem_Clicked);
            // 
            // sunMenuItem
            // 
            this.sunMenuItem.CheckOnClick = true;
            this.sunMenuItem.Name = "sunMenuItem";
            this.sunMenuItem.Size = new Size(131, 22);
            this.sunMenuItem.Tag = "SUN";
            this.sunMenuItem.Text = "Sun";
            this.sunMenuItem.ToolTipText = "Toggle the sun\'s visiblity";
            this.sunMenuItem.Click += new System.EventHandler(this.ViewItem_Clicked);
            // 
            // optionsDropDownButton
            // 
            this.optionsDropDownButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.optionsDropDownButton.DropDownItems.AddRange(new ToolStripItem[] {
            this.statusBarMenuItem,
            this.gridMenuItem,
            this.overviewMapMenuItem,
            this.scaleLegendMenuItem,
            this.atmosphereMenuItem,
            this.mouseNavigationMenuItem
            });
            this.optionsDropDownButton.ImageTransparentColor = Color.Magenta;
            this.optionsDropDownButton.Name = "optionsDropDownButton";
            this.optionsDropDownButton.Size = new Size(57, 17);
            this.optionsDropDownButton.Tag = "OPTIONS";
            this.optionsDropDownButton.Text = "Options";
            this.optionsDropDownButton.ToolTipText = "Toggle the various options";
            // 
            // statusBarMenuItem
            // 
            this.statusBarMenuItem.CheckOnClick = true;
            this.statusBarMenuItem.Name = "statusBarMenuItem";
            this.statusBarMenuItem.Size = new Size(169, 22);
            this.statusBarMenuItem.Tag = "STATUS";
            this.statusBarMenuItem.Text = "Status bar";
            this.statusBarMenuItem.ToolTipText = "Toggle the Status bar visiblity";
            this.statusBarMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // gridMenuItem
            // 
            this.gridMenuItem.CheckOnClick = true;
            this.gridMenuItem.Name = "gridMenuItem";
            this.gridMenuItem.Size = new Size(169, 22);
            this.gridMenuItem.Tag = "GRID";
            this.gridMenuItem.Text = "Grid";
            this.gridMenuItem.ToolTipText = "Toggle the Grid visiblity";
            this.gridMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // overviewMapMenuItem
            // 
            this.overviewMapMenuItem.CheckOnClick = true;
            this.overviewMapMenuItem.Name = "overviewMapMenuItem";
            this.overviewMapMenuItem.Size = new Size(169, 22);
            this.overviewMapMenuItem.Tag = "OVERVIEW";
            this.overviewMapMenuItem.Text = "Overview map";
            this.overviewMapMenuItem.ToolTipText = "Toggle the Overview map visiblity";
            this.overviewMapMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // scaleLegendMenuItem
            // 
            this.scaleLegendMenuItem.CheckOnClick = true;
            this.scaleLegendMenuItem.Name = "scaleLegendMenuItem";
            this.scaleLegendMenuItem.Size = new Size(169, 22);
            this.scaleLegendMenuItem.Tag = "SCALE";
            this.scaleLegendMenuItem.Text = "Scale legend";
            this.scaleLegendMenuItem.ToolTipText = "Toggle the Scale legend visiblity";
            this.scaleLegendMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // atmosphereMenuItem
            // 
            this.atmosphereMenuItem.Checked = true;
            this.atmosphereMenuItem.CheckOnClick = true;
            this.atmosphereMenuItem.CheckState = CheckState.Checked;
            this.atmosphereMenuItem.Name = "atmosphereMenuItem";
            this.atmosphereMenuItem.Size = new Size(169, 22);
            this.atmosphereMenuItem.Tag = "ATMOSPHERE";
            this.atmosphereMenuItem.Text = "Atmosphere";
            this.atmosphereMenuItem.ToolTipText = "Toggle the Atmosphere visiblity";
            this.atmosphereMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // mouseNavigationMenuItem
            // 
            this.mouseNavigationMenuItem.Checked = true;
            this.mouseNavigationMenuItem.CheckOnClick = true;
            this.mouseNavigationMenuItem.CheckState = CheckState.Checked;
            this.mouseNavigationMenuItem.Name = "mouseNavigationMenuItem";
            this.mouseNavigationMenuItem.Size = new Size(169, 22);
            this.mouseNavigationMenuItem.Tag = "MOUSE";
            this.mouseNavigationMenuItem.Text = "Mouse navigation";
            this.mouseNavigationMenuItem.ToolTipText = "Toggle Mouse navigation enabled";
            this.mouseNavigationMenuItem.Click += new System.EventHandler(this.OptionsItem_Clicked);
            // 
            // layersDropDownButton
            // 
            this.layersDropDownButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.layersDropDownButton.DropDownItems.AddRange(new ToolStripItem[] {
            this.bordersMenuItem,
            this.buildingsMenuItem,
            this.roadsMenuItem,
            this.terrainMenuItem
            });
            this.layersDropDownButton.ImageTransparentColor = Color.Magenta;
            this.layersDropDownButton.Name = "layersDropDownButton";
            this.layersDropDownButton.Size = new Size(52, 17);
            this.layersDropDownButton.Tag = "LAYERS";
            this.layersDropDownButton.Text = "Layers";
            this.layersDropDownButton.ToolTipText = "Toggle the in-built layers";
            // 
            // bordersMenuItem
            // 
            this.bordersMenuItem.CheckOnClick = true;
            this.bordersMenuItem.Name = "bordersMenuItem";
            this.bordersMenuItem.Size = new Size(126, 22);
            this.bordersMenuItem.Tag = "BORDERS";
            this.bordersMenuItem.Text = "Borders";
            this.bordersMenuItem.ToolTipText = "Toggle the Borders layer";
            this.bordersMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // buildingsMenuItem
            // 
            this.buildingsMenuItem.CheckOnClick = true;
            this.buildingsMenuItem.Name = "buildingsMenuItem";
            this.buildingsMenuItem.Size = new Size(126, 22);
            this.buildingsMenuItem.Tag = "BUILDINGS";
            this.buildingsMenuItem.Text = "Buildings";
            this.buildingsMenuItem.ToolTipText = "Toggle the Buildings layer";
            this.buildingsMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // roadsMenuItem
            // 
            this.roadsMenuItem.CheckOnClick = true;
            this.roadsMenuItem.Name = "roadsMenuItem";
            this.roadsMenuItem.Size = new Size(126, 22);
            this.roadsMenuItem.Tag = "ROADS";
            this.roadsMenuItem.Text = "Roads";
            this.roadsMenuItem.ToolTipText = "Toggle the Roads layer";
            this.roadsMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // terrainMenuItem
            // 
            this.terrainMenuItem.Checked = true;
            this.terrainMenuItem.CheckOnClick = true;
            this.terrainMenuItem.CheckState = CheckState.Checked;
            this.terrainMenuItem.Name = "terrainMenuItem";
            this.terrainMenuItem.Size = new Size(126, 22);
            this.terrainMenuItem.Tag = "TERRAIN";
            this.terrainMenuItem.Text = "Terrain";
            this.terrainMenuItem.ToolTipText = "Toggle the Terrain layer";
            this.terrainMenuItem.Click += new System.EventHandler(this.LayersItem_Clicked);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.refreshButton.ImageKey = "refresh";
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new Size(23, 20);
            this.refreshButton.Tag = "REFRESH";
            this.refreshButton.Text = "refresh";
            this.refreshButton.ToolTipText = "Refresh the plugin";
            this.refreshButton.Click += new EventHandler(refreshButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream =
                (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            this.imageList1.TransparentColor = Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "go");
            this.imageList1.Images.SetKeyName(1, "refresh");
            // 
            // GEToolStrip
            // 
            this.ImageList = this.imageList1;
            this.Items.AddRange(
                new ToolStripItem[] 
                {
                    this.navigationTextBox,
                    this.goButton,
                    this.refreshButton,
                    this.navigationSeparator,
                    this.viewDropDownButton,
                    this.optionsDropDownButton,
                    this.layersDropDownButton
                });

            this.Layout +=
                new LayoutEventHandler(this.GEToolStrip_Layout);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Invokes the javascitp function 'doGeocode'
        /// Automatically flys to the location if one is found
        /// </summary>
        /// <param name="input">the location to geocode</param>
        /// <returns>the point object (if any)</returns>
        private object InvokeDoGeocode(string input)
        {
            object result = null;
            if (this.htmlDocument != null)
            {
                result = this.htmlDocument.InvokeScript("doGeocode", new object[] { input });
            }

            return result;
        }

        /// <summary>
        /// Invokes the javascitp function 'LoadKml'
        /// </summary>
        /// <param name="url">The url of the file to load</param>
        /// <returns>The resulting kml object (if any)</returns>
        private IKmlObject InvokeLoadKml(string url)
        {
            IKmlObject result = null;
            if (this.htmlDocument != null)
            {
                result = (IKmlObject)this.htmlDocument.InvokeScript("LoadKml", new object[] { url });
            }
            return result;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when the KeyUp event is rasied in the navigation text box
        /// </summary>
        /// <param name="sender">The text box</param>
        /// <param name="e">KeyEvent arguments</param>
        private void NavigationTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string input = this.navigationTextBox.Text;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                // handle the same as if the go button was clicked
                this.NavigationButton_Click(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the 'go' navigation button is clicked
        /// </summary>
        /// <param name="sender">The 'go' button</param>
        /// <param name="e">Event arguments</param>
        private void NavigationButton_Click(object sender, EventArgs e)
        {
            string input = this.navigationTextBox.Text;
            if (input.Length > 1)
            {
                if (input.StartsWith("http", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    try
                    {
                        new Uri(input);
                    }
                    catch (UriFormatException)
                    {
                        return;
                    }

                    this.InvokeLoadKml(input);
                }
                else
                {
                    this.InvokeDoGeocode(input);
                }
            }
        }

        /// <summary>
        /// Called when the refresh button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void refreshButton_Click(object sender, EventArgs e)
        {
            if (this.gewebborwser != null)
            {
                this.gewebborwser.Refresh();
            }
        }

        /// <summary>
        /// Called when an item in the layers menu is clicked 
        /// </summary>
        /// <param name="sender">layers menu</param>
        /// <param name="e">Event arguments</param>
        private void LayersItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                string type = item.Tag.ToString();
                int value = Convert.ToInt32(item.Checked);

                if (this.geplugin != null && item != null)
                {
                    switch (type)
                    {
                        case "BORDERS":
                            this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_BORDERS, value);
                            break;
                        case "BUILDINGS":
                            this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_BUILDINGS, value);
                            break;
                        case "ROADS":
                            this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_ROADS, value);
                            break;
                        case "TERRAIN":
                            this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_TERRAIN, value);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Called when an item in the options menu is clicked 
        /// </summary>
        /// <param name="sender">options menu</param>
        /// <param name="e">Event arguments</param>
        private void OptionsItem_Clicked(object sender, EventArgs e)
        {
            if (sender != null)
            {
                try
                {
                    ToolStripMenuItem item = (ToolStripMenuItem)sender;
                    string type = item.Tag.ToString();
                    int value = Convert.ToInt32(item.Checked);

                    if (this.geplugin != null && item != null)
                    {
                        switch (type)
                        {
                            case "ATMOSPHERE":
                                this.geplugin.getOptions().setAtmosphereVisibility(value);
                                break;
                            case "CONTROLS":
                                this.geplugin.getNavigationControl().setVisibility(value);
                                break;
                            case "GRID":
                                this.geplugin.getOptions().setGridVisibility(value);
                                break;
                            case "MOUSE":
                                this.geplugin.getOptions().setMouseNavigationEnabled(value);
                                break;
                            case "OVERVIEW":
                                this.geplugin.getOptions().setOverviewMapVisibility(value);
                                break;
                            case "SCALE":
                                this.geplugin.getOptions().setScaleLegendVisibility(value);
                                break;
                            case "STATUS":
                                this.geplugin.getOptions().setStatusBarVisibility(value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Called when an item in the view menu is clicked 
        /// </summary>
        /// <param name="sender">view  menu</param>
        /// <param name="e">Event arguments</param>
        private void ViewItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                string type = item.Tag.ToString();
                int value = Convert.ToInt32(item.Checked);
                if (this.geplugin != null && item != null)
                {
                    switch (type)
                    {
                        case "SKY":
                            this.geplugin.getOptions().setMapType(value + 1);
                            break;
                        case "SUN":
                            this.geplugin.getSun().setVisibility(value);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                   
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Called when the tool strip layout changes
        /// </summary>
        /// <param name="sender">The ToolStrip</param>
        /// <param name="e">LayoutEvent arguments</param>
        private void GEToolStrip_Layout(object sender, LayoutEventArgs e)
        {
            this.navigationTextBox.Width = this.Width / 2;
        }

        #endregion
    }
}
