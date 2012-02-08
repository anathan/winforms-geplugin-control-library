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
    using System.Globalization;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// The GEToolStrip provides a quick way to access and set the plugin options
    /// </summary>
    public sealed partial class GEToolStrip : ToolStrip, IGEControls
    {
        #region Private fields

        /// <summary>
        /// An instance of the current browser
        /// </summary>
        private GEWebBrowser browser = null;

        /// <summary>
        /// An instance of the options wrapper class
        /// </summary>
        private GEOptions options = null;

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
        Description("Specifies the visibility of the Navigation items."),
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
        Description("Specifies the visibility of the Layers drop down menu."),
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
        Description("Specifies the visibility of the Options drop down menu."),
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
        Description("Specifies the visibility of the View drop down."),
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
        Description("Specifies the visibility of the Imagery drop down."),
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
        Description("Specifies the visibility of the screen grab button."),
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
        Description("Specifies the visibility of the show view in maps button."),
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
        Description("Specifies the visibility of the language combobox."),
        DefaultValueAttribute(true)]
        public bool ShowLanguageComboBox
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
        DefaultValueAttribute(AutoCompleteMode.SuggestAppend)]
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
        /// <example>Example: GEToolStrip.AddAutoCompleteSuggestions(new string[] { "London", "Paris", "Rome" });</example>
        public void AddAutoCompleteSuggestions(string[] suggestions)
        {
            this.navigationTextBoxStringCollection.AddRange(suggestions);
        }

        /// <summary>
        /// Adds an entry to the Auto Compleate suggestion list
        /// </summary>
        /// <param name="suggestion">The suggestion entry</param>
        /// <example>Example: GEToolStrip.AddAutoCompleteSuggestions("London");</example>
        public void AddAutoCompleteSuggestions(string suggestion)
        {
            this.navigationTextBoxStringCollection.Add(suggestion);
        }

        /// <summary>
        /// Removes all entries from the Auto Compleate suggestion list
        /// </summary>
        /// <example>Example: GEToolStrip.ClearAutoCompleteSuggestions()</example>
        public void ClearAutoCompleteSuggestions()
        {
            this.navigationTextBoxStringCollection.Clear();
        }

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="instance">The GEWebBrowser instance</param>
        /// <example>Example: GEToolStrip.SetBrowserInstance(GEWebBrowser)</example>
        public void SetBrowserInstance(GEWebBrowser instance)
        {
            this.browser = instance;

            if (this.browser.PluginIsReady)
            {
                this.options = new GEOptions(this.browser.Plugin);
                this.control = new GENavigationControl(this.browser.Plugin);
                this.SynchronizeOptions();
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
            if (this.browser.PluginIsReady)
            {
                // checked: t(1)+1=2 = MapType.Sky - unchecked: f(0)+1=1 = MapType.Earth
                this.options.SetMapType(((MapType)Convert.ToUInt16(this.skyMenuItem.Checked) + 1));

                this.options.StatusBarVisibility = this.statusBarMenuItem.Checked;
                this.options.StatusBarVisibility = this.statusBarMenuItem.Checked;
                this.options.GridVisibility = this.gridMenuItem.Checked;
                this.options.OverviewMapVisibility = this.overviewMapMenuItem.Checked;
                this.options.ScaleLegendVisibility = this.scaleLegendMenuItem.Checked;
                this.options.AtmosphereVisibility = this.atmosphereMenuItem.Checked;
                this.options.MouseNavigationEnabled = this.mouseNavigationMenuItem.Checked;
                this.options.ScaleLegendVisibility = this.scaleLegendMenuItem.Checked;
                this.options.OverviewMapVisibility = this.overviewMapMenuItem.Checked;
                this.options.UnitsFeetMiles = this.imperialUnitsMenuItem.Checked;
                this.control.Visibility = (Visibility)Convert.ToUInt16(this.controlsMenuItem.Checked);

                // no wrapper for the sun - so make a direct api call to GEPlugin.getSun().setVisibility()
                this.browser.Plugin.getSun().setVisibility(Convert.ToUInt16(this.sunMenuItem.Checked));

                if (this.browser.ImageryBase == ImageryBase.Earth)
                {
                    GEHelpers.EnableLayer(this.browser.Plugin, Layer.Borders, this.bordersMenuItem.Checked);
                    GEHelpers.EnableLayer(this.browser.Plugin, Layer.Buildings, this.buildingsMenuItem.Checked);
                    GEHelpers.EnableLayer(this.browser.Plugin, Layer.BuildingsLowRes, this.buildingsGreyMenuItem.Checked);
                    GEHelpers.EnableLayer(this.browser.Plugin, Layer.Roads, this.roadsMenuItem.Checked);
                    GEHelpers.EnableLayer(this.browser.Plugin, Layer.Terrain, this.terrainMenuItem.Checked);
                    GEHelpers.EnableLayer(this.browser.Plugin, Layer.Trees, this.treesMenuItem.Checked);

                    foreach (ToolStripMenuItem item in this.imageryDropDownButton.DropDownItems)
                    {
                        // every imagery item is enabled and unchecked
                        item.Enabled = true;
                        item.Checked = false;
                    }

                    // the Earth item is checked and disabled
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

            Dictionary<string, string> languageList = Languages.Codes();
            foreach (KeyValuePair<string, string> entry in languageList)
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
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
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
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
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
                    this.browser.FetchKmlLocal(input);
                }
                else if (GEHelpers.IsUri(input, UriKind.Absolute))
                {
                    // input is a remote file...
                    this.browser.FetchKml(input);
                }
                else if (input.Contains(","))
                {
                    // input is possibly decimal coordinates
                    string[] parts = input.Split(',');

                    if (parts.Length == 2)
                    {
                        double latitude;
                        double longitude;

                        if (double.TryParse(parts[0], out latitude) &&
                            double.TryParse(parts[1], out longitude))
                        {
                            KmlHelpers.CreateLookAt(this.browser.Plugin, latitude, longitude);
                        }
                    }
                }
                else
                {
                    // finally attempt to geocode the input
                    // fly to the point here or in javascript? 
                    dynamic point = this.browser.InvokeDoGeocode(input);
                }
            }
        }

        /// <summary>
        /// Called when the refresh button is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            this.browser.Refresh();
        }

        /// <summary>
        /// Called when an item in the layers menu is clicked 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void LayersItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (this.browser.PluginIsReady && (item != null))
            {
                GEHelpers.EnableLayer(this.browser.Plugin, (Layer)item.Tag, item.Checked);
            }
        }

        /// <summary>
        /// Called when an item in the options menu is clicked 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OptionsItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (this.browser.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();

                try
                {
                    switch (type)
                    {
                        case "FADEINOUT":
                            this.options.FadeInOutEnabled = item.Checked;
                            break;
                        case "IMPERIAL":
                            this.options.UnitsFeetMiles = item.Checked;
                            break;
                        case "ATMOSPHERE":
                            this.options.AtmosphereVisibility = item.Checked;
                            break;
                        case "CONTROLS":
                            this.control.Visibility = (Visibility)Convert.ToUInt16(item.Checked);
                            break;
                        case "GRID":
                            this.options.GridVisibility = item.Checked;
                            break;
                        case "MOUSE":
                            this.options.MouseNavigationEnabled = item.Checked;
                            break;
                        case "OVERVIEW":
                            this.options.OverviewMapVisibility = item.Checked;
                            break;
                        case "SCALE":
                            this.options.ScaleLegendVisibility = item.Checked;
                            break;
                        case "STATUS":
                            this.options.StatusBarVisibility = item.Checked;
                            break;
                        default:
                            break;
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("OptionsItem_Clicked: " + rbex.Message, "ToolStrip");
                }
            }
        }

        /// <summary>
        /// Called when an item in the view menu is clicked 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ViewItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            if (this.browser.PluginIsReady && (item != null))
            {
                string type = item.Tag.ToString();
                int value = Convert.ToUInt16(item.Checked);

                try
                {
                    switch (type)
                    {
                        case "SKY":
                            this.layersDropDownButton.Enabled = !item.Checked;
                            if (item.Checked)
                            {
                                this.options.SetMapType(MapType.Sky);
                            }
                            else
                            {
                                this.options.SetMapType(MapType.Earth);
                            }

                            break;
                        case "SUN":
                            this.browser.Plugin.getSun().setVisibility(value);
                            break;
                        case "HISTORY":
                            this.browser.Plugin.getTime().setHistoricalImageryEnabled(value);
                            break;
                        default:
                            break;
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("ViewItem_Clicked: " + rbex.Message, "ToolStrip");
                }
            }
        }

        /// <summary>
        /// Called when an item in the imagery menu is clicked 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ImageryItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;

            if (this.browser.PluginIsReady && (selectedItem != null))
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
                            this.browser.CreateInstance(type);
                            break;
                        case ImageryBase.Earth:
                        default:
                            this.layersDropDownButton.Enabled = true;
                            this.viewInMapsButton.Enabled = true;
                            this.historyMenuItem.Enabled = true;
                            this.browser.CreateInstance(ImageryBase.Earth);
                            break;
                    }
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("ImageryItem_Clicked: " + rbex.Message, "ToolStrip");
                    ////throw;
                }

                // reset the default options to match the default view
                ////ResetToolStripDefaults();
            }
        }

        /// <summary>
        /// Called when the Screen grab button is clicked 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenGrabButton_Click(object sender, EventArgs e)
        {
            if (this.browser.PluginIsReady)
            {
                // Take a 'screen grab' of the plugin
                Bitmap image = this.browser.ScreenGrab();

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
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ViewInMapsButton_Click(object sender, EventArgs e)
        {
            if (this.browser.PluginIsReady)
            {
                GEHelpers.ShowCurrentViewInMaps(this.browser.Plugin);
            }
        }

        /// <summary>
        /// Called when the tool strip layout changes
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void GEToolStrip_Layout(object sender, LayoutEventArgs e)
        {
            this.navigationTextBox.Width = this.Width / 3;
        }

        /// <summary>
        /// Called whenever the language combobox selected index is changed
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = this.languageComboBox.SelectedItem as ToolStripMenuItem;
            this.browser.SetLanguage(item.Tag.ToString());
        }

        #endregion
    }
}