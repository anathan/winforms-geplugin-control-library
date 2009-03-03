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

        #endregion

        #region Public methods

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance</param>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewb = browser;
            this.geplugin = browser.GetPlugin();
            this.htmlDocument = browser.Document;
            this.Enabled = true;
        }

        #endregion

        #region Protected methods

        #endregion

        #region Private methods

        /// <summary>
        /// Invokes the javascript function 'doGeocode'
        /// Automatically flys to the location if one is found
        /// </summary>
        /// <param name="input">the location to geocode</param>
        /// <returns>the point object (if any)</returns>
        private IKmlPoint InvokeDoGeocode(string input)
        {
            if (this.htmlDocument == null) 
            { 
                return null; 
            }

            return (IKmlPoint)this.htmlDocument.InvokeScript("jsDoGeocode", new object[] { input });
        }

        /// <summary>
        /// Invokes the javascitp function 'LoadKml'
        /// </summary>
        /// <param name="url">The url of the file to load</param>
        /// <returns>The resulting kml object (if any)</returns>
        private IKmlObject InvokeLoadKml(string url)
        {
            if (this.htmlDocument == null) 
            { 
                return null;
            }

            return (IKmlObject)this.htmlDocument.InvokeScript("jsFetchKml", new object[] { url });
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
        /// <param name="sender">The sending object</param>
         /// <param name="e">The Eveny arguments</param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (this.gewb != null)
            {
                this.gewb.Refresh();
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
        /// Called when an item in the Screen grab button is clicked 
        /// </summary>
        /// <param name="sender">Screen grab button</param>
        /// <param name="e">Event arguments</param>
        private void ScreenGrabButton_Click(object sender, System.EventArgs e)
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
