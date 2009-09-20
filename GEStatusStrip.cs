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
    /// The GEStatusStrip shows various information about the plug-in
    /// </summary>
    public partial class GEStatusStrip : StatusStrip, IGEControls
    {
        #region Private fields

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private IGEPlugin geplugin = null;

        /// <summary>
        /// An instance of the current browser
        /// </summary>
        private GEWebBrowser gewb = null;

        /// <summary>
        /// Timer interval in miliseconds
        /// </summary>
        private int interval = 500;

        /// <summary>
        /// Timer used when polling data
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// Indicates whether the streaming status label is visible
        /// </summary>
        private bool streamingStatusLabelVisible = true;

        /// <summary>
        /// Indicates whether the streaming progress bar is visible
        /// </summary>
        private bool streamingProgressBarVisible = true;

        /// <summary>
        /// Indicates whether the api version label is visible
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
            : base()
        {
            InitializeComponent();
            this.Enabled = false;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the timer interval for polling data
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the timer interval for polling data."),
        DefaultValueAttribute(500)]
        public int Interval
        {
            get
            {
                return this.interval;
            }

            set
            {
                this.interval = value;
                this.timer = new Timer();
                this.timer.Interval = this.interval;
                this.timer.Start();
                this.timer.Tick += new EventHandler(this.Timer_Tick);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the progress streaming progress bar."),
        DefaultValueAttribute(true)]
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
        Description("Specifies the visiblity of the progress streaming label."),
        DefaultValueAttribute(true)]
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
        Description("Specifies the visiblity of the browser version label."),
        DefaultValueAttribute(true)]
        public bool ShowBrowserVersionStatusLabel
        {
            get
            {
                return this.browserVersionStatusLabelVisible;
            }

            set
            {
                if (!value)
                {
                    apiVersionStatusLabel.Spring = true;
                }
                else
                {
                    apiVersionStatusLabel.Spring = false;
                }

                this.browserVersionStatusLabelVisible = value;
                this.browserVersionStatusLabel.Visible = value;
            }
        }
               
        /// <summary>
        /// Gets or sets a value indicating whether the api version label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the api version label."),
        DefaultValueAttribute(true)]
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
                    pluginVersionStatusLabel.Spring = true;
                }
                else
                {
                    pluginVersionStatusLabel.Spring = false;
                }

                this.apiVersionStatusLabelVisible = value;
                this.apiVersionStatusLabel.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plug-in version label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visiblity of the plugin version label."),
        DefaultValueAttribute(true)]
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
        /// <param name="browser">The GEWebBrowser instance</param>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewb = browser;
            this.geplugin = browser.GetPlugin();
            this.Enabled = true;

            if (this.gewb.PluginIsReady)
            {
                this.timer = new Timer();
                this.timer.Interval = this.interval;
                this.timer.Start();
                this.timer.Tick += new EventHandler(this.Timer_Tick);

                this.Enabled = true;
                this.browserVersionStatusLabel.Text = "ie " + this.gewb.Version.ToString();
                this.apiVersionStatusLabel.Text = "api " + this.geplugin.getApiVersion();
                this.pluginVersionStatusLabel.Text = "plugin " + this.geplugin.getPluginVersion();
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Timer tick callback function
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event arguments</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.gewb.PluginIsReady)
            {
                try
                {
                    int percent = (int)this.geplugin.getStreamingPercent();
                    this.streamingProgressBar.Value = percent;
                    this.streamingStatusLabel.Text = percent + "%";

                    if (100 == percent)
                    {
                        this.streamingStatusLabel.ForeColor = Color.Gray;
                        this.streamingStatusLabel.Text = "idle";
                    }
                    else
                    {
                        this.streamingStatusLabel.ForeColor = Color.Black;
                    }
                }
                catch (System.Runtime.InteropServices.COMException cex)
                {
                    System.Diagnostics.Debug.WriteLine(cex.ToString());
                }
            }
        }

        #endregion
    }
}
