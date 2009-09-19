// <copyright file="GEToolStrip.cs" company="FC">
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
        private GEWebBrowser gewb = null;

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
        /// Indicates whether the imagery items are visible
        /// </summary>
        private bool imageryDropDownVisiblity = true;

        /// <summary>
        /// Indicates whether the screen grab button is visible
        /// </summary>
        private bool screenGrabButtonVisiblity = true;

        /// <summary>
        /// Indicates whether the view in maps button is visible
        /// </summary>
        private bool viewInMapsButtonVisiblity = true;

        /// <summary>
        /// Indicates if auto compleate should be used in the navigaton text box
        /// </summary>
        private bool useAutoCompleteSugestions = true;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEToolStrip class.
        /// </summary>
        public GEToolStrip()
            : base()
        {
            this.InitializeComponent();
            this.Enabled = false;
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
                this.submitButton.Visible = value;
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

        /// <summary>
        /// Gets or sets a value indicating whether the Imagery drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the Imagery drop down."),
        DefaultValueAttribute(true)]
        public bool ShowImageryDropDown
        {
            get
            {
                return this.imageryDropDownVisiblity;
            }

            set
            {
                this.imageryDropDownVisiblity = value;
                this.imageryDropDownButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the screen grab button is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the screen grab button."),
        DefaultValueAttribute(true)]
        public bool ShowScreenGrabButton
        {
            get
            {
                return this.screenGrabButtonVisiblity;
            }

            set
            {
                this.screenGrabButtonVisiblity = value;
                this.imageryDropDownButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view in maps button is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the show view in maps button."),
        DefaultValueAttribute(true)]
        public bool ShowViewInMapsButton
        {
            get
            {
                return this.viewInMapsButtonVisiblity;
            }

            set
            {
                this.viewInMapsButtonVisiblity = value;
                this.viewInMapsButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation textbox uses autocomplete suggestions
        /// </summary>
        [Category("Control Options"),
        Description("Specifies whether the navigation textbox uses autocomplete suggestions."),
        DefaultValueAttribute(true)]
        public bool UseAutoCompleteSugestions
        {
            get
            {
                return useAutoCompleteSugestions;
            }
            set
            {
                useAutoCompleteSugestions = value;
                if (value)
                {
                    navigationTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                }
                else
                {
                    navigationTextBox.AutoCompleteMode = AutoCompleteMode.None;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds multiple entries to the Auto Compleate suggestion list
        /// </summary>
        /// <param name="suggestions">The suggestions to be entered</param>
        public void AddAutoCompleteSuggestions(string[] suggestions)
        {
            this.navigationTextBoxStringCollection.AddRange(suggestions);
        }

        /// <summary>
        /// Adds an entry to the Auto Compleate suggestion list
        /// </summary>
        /// <param name="suggestion">The suggestion entry</param>
        public void AddAutoCompleteSuggestions(string suggestion)
        {
            this.navigationTextBoxStringCollection.Add(suggestion);
        }

        /// <summary>
        /// Removes all entries from the Auto Compleate suggestion list
        /// </summary>
        public void ClearAutoCompleteSuggestions()
        {
            this.navigationTextBoxStringCollection.Clear();
        }

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance</param>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewb = browser;
            this.geplugin = browser.GetPlugin();
            if (gewb.PluginIsReady)
            {
                this.SynchronizeOptions();
                this.htmlDocument = browser.Document;
                this.Enabled = true;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Resets the toolstrips menu items to match the default initialization state of the plug-in.
        /// </summary>
        private void ResetToolStripDefaults()
        {
            this.statusBarMenuItem.Checked = false;
            this.gridMenuItem.Checked = false;
            this.overviewMapMenuItem.Checked = false;
            this.scaleLegendMenuItem.Checked = false;
            this.atmosphereMenuItem.Checked = true;
            this.mouseNavigationMenuItem.Checked = true;
            this.scaleLegendMenuItem.Checked = false;
            this.overviewMapMenuItem.Checked = false;
            this.skyMenuItem.Checked = false;
            this.sunMenuItem.Checked = false;
            this.controlsMenuItem.Checked = false;
        }

        /// <summary>
        /// synchronizes the plug-in and tool strip options
        /// </summary>
        private void SynchronizeOptions()
        {
            if (gewb.PluginIsReady)
            {
                // options

                IGEOptions options = this.geplugin.getOptions();

                options.setStatusBarVisibility(Convert.ToInt16(this.statusBarMenuItem.Checked));

                options.setGridVisibility(Convert.ToInt16(this.gridMenuItem.Checked));

                options.setOverviewMapVisibility(Convert.ToInt16(this.overviewMapMenuItem.Checked));

                options.setScaleLegendVisibility(Convert.ToInt16(this.scaleLegendMenuItem.Checked));

                options.setAtmosphereVisibility(Convert.ToInt16(this.atmosphereMenuItem.Checked));

                options.setMouseNavigationEnabled(Convert.ToInt16(this.mouseNavigationMenuItem.Checked));

                options.setScaleLegendVisibility(Convert.ToInt16(this.scaleLegendMenuItem.Checked));

                options.setOverviewMapVisibility(Convert.ToInt16(this.overviewMapMenuItem.Checked));

                options.setMapType(Convert.ToInt16(this.skyMenuItem.Checked) + 1);

                this.geplugin.getSun().setVisibility(Convert.ToInt16(this.sunMenuItem.Checked));

                this.geplugin.getNavigationControl().setVisibility(Convert.ToInt16(this.controlsMenuItem.Checked));

                // layers 

                if (this.gewb.ImageyBase == ImageryBase.Earth)
                {
                    this.geplugin.getLayerRoot().enableLayerById(
                        this.geplugin.LAYER_BORDERS, Convert.ToInt16(this.bordersMenuItem.Checked));

                    this.geplugin.getLayerRoot().enableLayerById(
                        this.geplugin.LAYER_BUILDINGS, Convert.ToInt16(this.buildingsMenuItem.Checked));

                    this.geplugin.getLayerRoot().enableLayerById(
                        this.geplugin.LAYER_BUILDINGS_LOW_RESOLUTION, Convert.ToInt16(this.buildingsGreyMenuItem.Checked));

                    this.geplugin.getLayerRoot().enableLayerById(
                        this.geplugin.LAYER_ROADS, Convert.ToInt16(this.roadsMenuItem.Checked));

                    this.geplugin.getLayerRoot().enableLayerById(
                        this.geplugin.LAYER_TERRAIN, Convert.ToInt16(this.terrainMenuItem.Checked));
                }
            }
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
                if (UseAutoCompleteSugestions)
                {
                    // add the user input to the custom 'per-session' string collection
                    this.navigationTextBoxStringCollection.Add(input);
                }
                
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

                    this.gewb.FetchKml(input);
                }
                else
                {
                    this.gewb.InvokeDoGeocode(input);
                }
            }
        }

        /// <summary>
        /// Called when the refresh button is clicked
        /// </summary>
        /// <param name="sender">The sending object</param>
         /// <param name="e">The Eveny arguments</param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            this.gewb.Refresh();   
        }

        /// <summary>
        /// Called when an item in the layers menu is clicked 
        /// </summary>
        /// <param name="sender">layers menu</param>
        /// <param name="e">Event arguments</param>
        private void LayersItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (gewb.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();
                int value = Convert.ToInt32(item.Checked);

                switch (type)
                {
                    case "BORDERS":
                        this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_BORDERS, value);
                        break;
                    case "BUILDINGS":
                        this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_BUILDINGS, value);
                        break;
                    case "BUILDINGS_GREY":
                        this.geplugin.getLayerRoot().enableLayerById(this.geplugin.LAYER_BUILDINGS_LOW_RESOLUTION, value);
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

        /// <summary>
        /// Called when an item in the options menu is clicked 
        /// </summary>
        /// <param name="sender">options menu</param>
        /// <param name="e">Event arguments</param>
        private void OptionsItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (gewb.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();
                int value = Convert.ToInt32(item.Checked);

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

        /// <summary>
        /// Called when an item in the view menu is clicked 
        /// </summary>
        /// <param name="sender">view  menu</param>
        /// <param name="e">Event arguments</param>
        private void ViewItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (gewb.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();
                int value = Convert.ToInt32(item.Checked);

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
        }

        /// <summary>
        /// Called when an item in the imagery menu is clicked 
        /// </summary>
        /// <param name="sender">imagery menu</param>
        /// <param name="e">Event arguments</param>
        private void ImageryItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;

            if (gewb.PluginIsReady && (selectedItem != null))
            {
                string type = selectedItem.Tag.ToString();
                ToolStripItemCollection imageryItems = imageryDropDownButton.DropDownItems;

                foreach (ToolStripMenuItem menuItem in imageryItems)
                {
                    // uncheck and disable all items
                    menuItem.Checked = false; 
                    menuItem.Enabled = true;
                }

                // check and enable the selected item
                selectedItem.Checked = true;
                selectedItem.Enabled = false;

                switch (type)
                {
                    case "MARS":
                    case "MOON":
                        this.layersDropDownButton.Enabled = false;
                        this.viewInMapsButton.Enabled = false;
                        ImageryBase b = (ImageryBase)Enum.Parse(typeof(ImageryBase), type, true);
                        this.gewb.CreateInstance(b);
                        break;
                    case "EARTH":
                    default:
                        this.layersDropDownButton.Enabled = true;
                        this.viewInMapsButton.Enabled = true;
                        this.gewb.CreateInstance(ImageryBase.Earth);
                        break;
                }

                // reset the default options to match the default view
                ////ResetToolStripDefaults();
            }
        }

        /// <summary>
        /// Called when the Screen grab button is clicked 
        /// </summary>
        /// <param name="sender">Screen grab button</param>
        /// <param name="e">Event arguments</param>
        private void ScreenGrabButton_Click(object sender, EventArgs e)
        {
            if (gewb.PluginIsReady)
            {
                // Take a 'screen grab' of the plugin
                System.Drawing.Bitmap image = this.gewb.ScreenGrab();

                // Save the file with a dialog
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    image.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        /// Called when the view in maps button is clicked
        /// </summary>
        /// <param name="sender">View in maps button</param>
        /// <param name="e">Event arguments</param>
        private void ViewInMapsButton_Click(object sender, EventArgs e)
        {
            if (gewb.PluginIsReady)
            {
                GEHelpers.ShowCurrentViewInMaps(geplugin);
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
