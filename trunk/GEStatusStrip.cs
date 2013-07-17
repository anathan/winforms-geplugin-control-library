// <copyright file="GEStatusStrip.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-09-19</date>
// <summary>This file is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.using System.Diagnostics;
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
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms;

    /// <summary>
    /// The GEStatusStrip shows various information about the plug-in
    /// </summary>
    public sealed partial class GEStatusStrip : StatusStrip, IGEControls
    {
        #region Private fields

        /// <summary>
        /// An instance of the current browser
        /// </summary>
        private GEWebBrowser browser;

        /// <summary>
        /// Timer interval in milliseconds
        /// </summary>
        private int interval = 500;

        /// <summary>
        /// Indicates whether the streaming status label is visible
        /// </summary>
        private bool streamingStatusLabelVisible = true;

        /// <summary>
        /// Indicates whether the streaming progress bar is visible
        /// </summary>
        private bool streamingProgressBarVisible = true;

        /// <summary>
        /// Indicates whether the Google API version label is visible
        /// </summary>
        private bool apiVersionStatusLabelVisible = true;

        /// <summary>
        /// Indicates whether the plug-in version label is visible
        /// </summary>
        private bool pluginVersionStatusLabelVisible = true;

        /// <summary>
        /// Indicates whether the browser version label is visible
        /// </summary>
        private bool browserVersionStatusLabelVisible = true;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEStatusStrip class.
        /// </summary>
        public GEStatusStrip()
        {
            this.InitializeComponent();
            this.timer.Interval = this.interval;
            this.Enabled = false;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the timer interval for polling data in milliseconds
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the timer interval for polling data. Default value is 500ms"),
        DefaultValue(500)]
        public int Interval
        {
            get
            {
                return this.interval;
            }

            set
            {
                this.interval = value;

                if (null != timer)
                {
                    timer.Interval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the progress streaming progress bar."),
        DefaultValue(true)]
        public bool ShowStreamingProgressBar
        {
            get
            {
                return this.streamingProgressBarVisible;
            }

            set
            {
                this.streamingProgressBarVisible = value;
                this.streamingProgressBar.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the streaming label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visibility of the progress streaming label."),
        DefaultValue(true)]
        public bool ShowStreamingStatusLabel
        {
            get
            {
                return this.streamingStatusLabelVisible;
            }

            set
            {
                this.streamingStatusLabelVisible = value;
                this.streamingStatusLabel.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the browser version label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visibility of the browser version label."),
        DefaultValue(true)]
        public bool ShowBrowserVersionStatusLabel
        {
            get
            {
                return this.browserVersionStatusLabelVisible;
            }

            set
            {
                this.apiVersionStatusLabel.Spring = !value;
                this.browserVersionStatusLabelVisible = value;
                this.browserVersionStatusLabel.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Google API version label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visibility of the Google API version label."),
        DefaultValue(true)]
        public bool ShowApiVersionStatusLabel
        {
            get
            {
                return this.apiVersionStatusLabelVisible;
            }

            set
            {
                if (!value && !this.browserVersionStatusLabelVisible)
                {
                    this.pluginVersionStatusLabel.Spring = true;
                }
                else
                {
                    this.pluginVersionStatusLabel.Spring = false;
                }

                this.apiVersionStatusLabelVisible = value;
                this.apiVersionStatusLabel.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plug-in version label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visibility of the plug-in version label."),
        DefaultValue(true)]
        public bool ShowPluginVersionStatusLabel
        {
            get
            {
                return this.pluginVersionStatusLabelVisible;
            }

            set
            {
                this.pluginVersionStatusLabelVisible = value;
                this.pluginVersionStatusLabel.Visible = value;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="instance">The GEWebBrowser instance</param>
        /// <example>Example: GEToolStrip.SetBrowserInstance(GEWebBrowser)</example>
        [SecurityCritical]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void SetBrowserInstance(GEWebBrowser instance)
        {
            this.browser = instance;

            if (!this.browser.PluginIsReady)
            {
                return;
            }

            this.Enabled = true;
            this.ShowStatusLabels(true);

            this.browser.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName != "PluginIsReady")
                {
                    return;
                }

                this.ShowStatusLabels(this.browser.PluginIsReady);
            };
        }

        #endregion

        #region private methods

        private void ShowStatusLabels(bool value)
        {
            if (value)
            {
                this.Enabled = true;
                this.browserVersionStatusLabel.Text = "ie " + this.browser.Version;
                this.apiVersionStatusLabel.Text = "api " + this.browser.Plugin.getApiVersion();
                this.pluginVersionStatusLabel.Text = "plugin " + this.browser.Plugin.getPluginVersion();
                this.timer.Start();
                this.timer.Tick += this.Timer_Tick;
            }
            else
            {
                this.apiVersionStatusLabel.Text = string.Empty;
                this.browserVersionStatusLabel.Text = string.Empty;
                this.pluginVersionStatusLabel.Text = string.Empty;
                this.Enabled = false;
                timer.Stop();
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Timer tick event handler
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!this.browser.PluginIsReady)
            {
                return;
            }

            double percent;

            try
            {
                percent = this.browser.Plugin.getStreamingPercent();
            }
            catch (COMException)
            {
                timer.Stop();
                percent = 0;
            }

            this.streamingProgressBar.Value = (int)percent;
            this.streamingStatusLabel.Text = string.Concat(percent, '%');
        }

        #endregion
    }
}