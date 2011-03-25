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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// The GEToolStrip provides a quick way to access and set the plugin options
    /// </summary>
    public sealed partial class GEToolStrip : ToolStrip, IGEControls
    {
        #region Private fields

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private dynamic geplugin = null;

        /// <summary>
        /// An instance of the current browser
        /// </summary>
        private GEWebBrowser gewb = null;

        /// <summary>
        /// An instance of the options wrapper class
        /// </summary>
        private GEOptions geoptions = null;

        /// <summary>
        /// The plugin navigation cotrol 
        /// </summary>
        private GENavigationControl control = null;

        /// <summary>
        /// Indicates whether the navigation items are visible
        /// </summary>
        private bool navigationItemsVisible = true;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEToolStrip class.
        /// </summary>
        public GEToolStrip()
            : base()
        {
            this.InitializeComponent();
            this.BuildLanguageOptions();
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
                return this.navigationItemsVisible;
            }

            set
            {
                this.navigationItemsVisible = value;
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
            get { return this.layersDropDownButton.Visible; }
            set { this.layersDropDownButton.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Options drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the Options drop down menu."),
        DefaultValueAttribute(true)]
        public bool ShowOptionsDropDown
        {
            get { return this.optionsDropDownButton.Visible; }
            set { this.optionsDropDownButton.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the View drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the View drop down."),
        DefaultValueAttribute(true)]
        public bool ShowViewDropDown
        {
            get { return this.viewDropDownButton.Visible; }
            set { this.viewDropDownButton.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Imagery drop down is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the Imagery drop down."),
        DefaultValueAttribute(true)]
        public bool ShowImageryDropDown
        {
            get { return this.imageryDropDownButton.Visible; }
            set { this.imageryDropDownButton.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the screen grab button is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the screen grab button."),
        DefaultValueAttribute(true)]
        public bool ShowScreenGrabButton
        {
            get { return this.screenGrabButton.Visible; }
            set { this.screenGrabButton.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view in maps button is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the show view in maps button."),
        DefaultValueAttribute(true)]
        public bool ShowViewInMapsButton
        {
            get { return this.viewInMapsButton.Visible; }
            set { this.viewInMapsButton.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the language combobox is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the language combobox."),
        DefaultValueAttribute(true)]
        public bool ShowLanguageCombobox
        {
            get { return this.languageComboBox.Visible; }
            set { this.languageComboBox.Visible = value; }
        }

        /// <summary>
        /// Gets or sets the AutoCompleteMode of the navigation textbox
        /// Default is AutoCompleteMode.Append
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the AutoCompleteMode of the navigation textbox. Default is AutoCompleteMode.Append"),
        DefaultValueAttribute(AutoCompleteMode.Append)]
        public AutoCompleteMode NavigationAutoCompleteMode
        {
            get { return this.navigationTextBox.AutoCompleteMode; }
            set { this.navigationTextBox.AutoCompleteMode = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds multiple entries to the Auto Compleate suggestion list
        /// </summary>
        /// <param name="suggestions">The suggestions to be entered</param>
        /// <example>GEToolStrip.AddAutoCompleteSuggestions(new string[] { "London", "Paris", "Rome" });</example>
        public void AddAutoCompleteSuggestions(string[] suggestions)
        {
            this.navigationTextBoxStringCollection.AddRange(suggestions);
        }

        /// <summary>
        /// Adds an entry to the Auto Compleate suggestion list
        /// </summary>
        /// <param name="suggestion">The suggestion entry</param>
        /// <example>GEToolStrip.AddAutoCompleteSuggestions("London");</example>
        public void AddAutoCompleteSuggestions(string suggestion)
        {
            this.navigationTextBoxStringCollection.Add(suggestion);
        }

        /// <summary>
        /// Removes all entries from the Auto Compleate suggestion list
        /// </summary>
        /// <example>GEToolStrip.ClearAutoCompleteSuggestions()</example>
        public void ClearAutoCompleteSuggestions()
        {
            this.navigationTextBoxStringCollection.Clear();
        }

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance</param>
        /// <example>GEToolStrip.SetBrowserInstance(GEWebBrowser)</example>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewb = browser;
            this.geplugin = browser.Plugin;

            if (this.gewb.PluginIsReady)
            {
                this.geoptions = new GEOptions(this.geplugin);
                this.control = new GENavigationControl(this.geplugin);
                this.SynchronizeOptions();
                this.Enabled = true;

                // sycn the tool stip options whenever the Ready event is fired by the browser
                this.gewb.PluginReady += (o, e) =>
                {
                    this.Enabled = true;
                    this.SynchronizeOptions();
                };
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Resets the toolstrips menu items to match the default initialization state of the plug-in.
        /// </summary>
        private void ResetToolStripDefaults()
        {
            this.earthMenuItem.Checked = true;
            this.marsMenuItem.Checked = false;
            this.moonMenuItem.Checked = false;
            this.statusBarMenuItem.Checked = false;
            this.gridMenuItem.Checked = false;
            this.overviewMapMenuItem.Checked = false;
            this.scaleLegendMenuItem.Checked = false;
            this.atmosphereMenuItem.Checked = true;
            this.mouseNavigationMenuItem.Checked = true;
            this.imperialUnitsMenuItem.Checked = false;
            this.scaleLegendMenuItem.Checked = false;
            this.overviewMapMenuItem.Checked = false;
            this.skyMenuItem.Checked = false;
            this.sunMenuItem.Checked = false;
            this.controlsMenuItem.Checked = false;
        }

        /// <summary>
        /// Force the plug-in to confom to the tool-strip settings
        /// </summary>
        private void SynchronizeOptions()
        {
            if (this.gewb.PluginIsReady)
            {
                this.geoptions.StatusBarVisibility = this.statusBarMenuItem.Checked;
                this.geoptions.StatusBarVisibility = this.statusBarMenuItem.Checked;
                this.geoptions.GridVisibility = this.gridMenuItem.Checked;
                this.geoptions.OverviewMapVisibility = this.overviewMapMenuItem.Checked;
                this.geoptions.ScaleLegendVisibility = this.scaleLegendMenuItem.Checked;
                this.geoptions.AtmosphereVisibility = this.atmosphereMenuItem.Checked;
                this.geoptions.MouseNavigationEnabled = this.mouseNavigationMenuItem.Checked;
                this.geoptions.ScaleLegendVisibility = this.scaleLegendMenuItem.Checked;
                this.geoptions.OverviewMapVisibility = this.overviewMapMenuItem.Checked;
                this.geoptions.UnitsFeetMiles = this.imperialUnitsMenuItem.Checked;

                // checked: t(1) + 1 = 2 = MapType.Sky
                // unchecked: f(0) + 1 = 1 = MapType.Earth
                this.geoptions.SetMapType(((MapType)Convert.ToInt16(this.skyMenuItem.Checked) + 1));

                // sun
                this.geplugin.getSun().setVisibility(Convert.ToInt16(this.sunMenuItem.Checked));

                // controls
                this.geplugin.getNavigationControl().setVisibility(Convert.ToInt16(this.controlsMenuItem.Checked));

                if (this.gewb.ImageyBase == ImageryBase.Earth)
                {
                    GEHelpers.EnableLayerById(this.geplugin, GELayer.Borders, this.bordersMenuItem.Checked);
                    GEHelpers.EnableLayerById(this.geplugin, GELayer.Buildings, this.buildingsMenuItem.Checked);
                    GEHelpers.EnableLayerById(this.geplugin, GELayer.BuildingsLowRes, this.buildingsGreyMenuItem.Checked);
                    GEHelpers.EnableLayerById(this.geplugin, GELayer.Roads, this.roadsMenuItem.Checked);
                    GEHelpers.EnableLayerById(this.geplugin, GELayer.Terrain, this.terrainMenuItem.Checked);
                    GEHelpers.EnableLayerById(this.geplugin, GELayer.Trees, this.treesMenuItem.Checked);

                    // imagery 
                    foreach (ToolStripMenuItem item in this.imageryDropDownButton.DropDownItems)
                    {
                        item.Enabled = true;
                        item.Checked = false;
                    }

                    this.earthMenuItem.Checked = true;
                    this.earthMenuItem.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Builds the language combobox options from the Languages class
        /// </summary>
        private void BuildLanguageOptions()
        {
            this.languageComboBox.Items.Clear();

            Dictionary<string, string> languages = Languages.GetList();
            foreach (KeyValuePair<string, string> entry in languages)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = entry.Value;
                item.Tag = entry.Key;
                this.languageComboBox.Items.Add(item);
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
                if (this.NavigationAutoCompleteMode == AutoCompleteMode.Append ||
                    this.NavigationAutoCompleteMode == AutoCompleteMode.SuggestAppend)
                {
                    // add the user input to the custom 'per-session' string collection
                    this.navigationTextBoxStringCollection.Add(input);
                }

                if (System.IO.File.Exists(input))
                {
                    if (input.EndsWith("kml", true, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        // input is a local kml file
                        this.gewb.FetchKmlLocal(input);
                    }
                }
                else if (input.StartsWith("http", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    try
                    {
                        new Uri(input);
                    }
                    catch (UriFormatException)
                    {
                        return;
                    }

                    // input is a remote file
                    this.gewb.FetchKml(input);
                }
                else
                {
                    // attempt to gecode the input
                    this.gewb.InvokeDoGeocode(input);
                }
            }
        }

        /// <summary>
        /// Called when the refresh button is clicked
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The Event arguments</param>
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

            if (this.gewb.PluginIsReady && (item != null))
            {
                GEHelpers.EnableLayerById(this.geplugin, (string)item.Tag, item.Checked);
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

            if (this.gewb.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();

                try
                {
                    switch (type)
                    {
                        case "FADEINOUT":
                            this.geoptions.FadeInOutEnabled = item.Checked;
                            break;
                        case "IMPERIAL":
                            this.geoptions.UnitsFeetMiles = item.Checked;
                            break;
                        case "ATMOSPHERE":
                            this.geoptions.AtmosphereVisibility = item.Checked;
                            break;
                        case "CONTROLS":
                            this.control.Visiblity = (Visiblity)Convert.ToInt16(item.Checked);
                            break;
                        case "GRID":
                            this.geoptions.GridVisibility = item.Checked;
                            break;
                        case "MOUSE":
                            this.geoptions.MouseNavigationEnabled = item.Checked;
                            break;
                        case "OVERVIEW":
                            this.geoptions.OverviewMapVisibility = item.Checked;
                            break;
                        case "SCALE":
                            this.geoptions.ScaleLegendVisibility = item.Checked;
                            break;
                        case "STATUS":
                            this.geoptions.StatusBarVisibility = item.Checked;
                            break;
                        default:
                            break;
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("OptionsItem_Clicked: " + rbex.ToString(), "ToolStrip");
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

            if (this.gewb.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();
                int value = Convert.ToInt16(item.Checked);

                try
                {
                    switch (type)
                    {
                        case "SKY":
                            this.layersDropDownButton.Enabled = !item.Checked;
                            if (item.Checked)
                            {
                                this.geoptions.SetMapType(MapType.Sky);
                            }
                            else
                            {
                                this.geoptions.SetMapType(MapType.Earth);
                            }

                            break;
                        case "SUN":
                            this.geplugin.getSun().setVisibility(value);
                            break;
                        case "HISTORY":
                            this.geplugin.getTime().setHistoricalImageryEnabled(value);
                            break;
                        default:
                            break;
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("ViewItem_Clicked: " + rbex.ToString(), "ToolStrip");
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

            if (this.gewb.PluginIsReady && (selectedItem != null))
            {
                ImageryBase type = (ImageryBase)selectedItem.Tag;

                ToolStripItemCollection imageryItems = this.imageryDropDownButton.DropDownItems;

                foreach (ToolStripMenuItem menuItem in imageryItems)
                {
                    if (menuItem != selectedItem)
                    {
                        // uncheck and disable all items
                        menuItem.Checked = false;
                        menuItem.Enabled = true;
                    }
                    else
                    {
                        // check and enable the selected item
                        selectedItem.Checked = true;
                        selectedItem.Enabled = false;
                    }
                }

                try
                {
                    switch (type)
                    {
                        case ImageryBase.Mars:
                        case ImageryBase.Moon:
                            this.layersDropDownButton.Enabled = false;
                            this.viewInMapsButton.Enabled = false;
                            this.historyMenuItem.Enabled = false;
                            this.Enabled = false;
                            this.gewb.CreateInstance(type);
                            break;
                        case ImageryBase.Earth:
                        default:
                            this.layersDropDownButton.Enabled = true;
                            this.viewInMapsButton.Enabled = true;
                            this.historyMenuItem.Enabled = true;
                            this.gewb.CreateInstance(ImageryBase.Earth);
                            break;
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("ImageryItem_Clicked: " + rbex.ToString(), "ToolStrip");
                    ////throw;
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
            if (this.gewb.PluginIsReady)
            {
                // Take a 'screen grab' of the plugin
                Bitmap image = this.gewb.ScreenGrab();

                // Save the file with a dialog
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All files (*.*)|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    image.Save(dialog.FileName, ImageFormat.Jpeg);
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
            if (this.gewb.PluginIsReady)
            {
                GEHelpers.ShowCurrentViewInMaps(this.geplugin);
            }
        }

        /// <summary>
        /// Called when the tool strip layout changes
        /// </summary>
        /// <param name="sender">The ToolStrip</param>
        /// <param name="e">LayoutEvent arguments</param>
        private void GEToolStrip_Layout(object sender, LayoutEventArgs e)
        {
            this.navigationTextBox.Width = this.Width / 3;
        }

        /// <summary>
        /// Called whenever the language combobox selected index is changed
        /// </summary>
        /// <param name="sender">The language combobox</param>
        /// <param name="e">The event arguments</param>
        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = this.languageComboBox.SelectedItem as ToolStripMenuItem;
            this.gewb.SetLanguage(item.Tag.ToString());
        }

        #endregion
    }
}