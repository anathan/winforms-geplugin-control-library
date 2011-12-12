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
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// The GEStatusStrip shows various information about the plug-in
    /// </summary>
    public sealed partial class GEStatusStrip : StatusStrip, IGEControls
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
        private GEWebBrowser browser = null;

        /// <summary>
        /// Timer interval in miliseconds
        /// </summary>
        private int interval = 100;

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
            this.InitializeComponent();
            this.Enabled = false;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the timer interval for polling data
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the timer interval for polling data."),
        DefaultValueAttribute(100)]
        public int Interval
        {
            get
            {
                return this.interval;
            }

            set
            {
                this.interval = value;

                if (null != this.timer)
                {
                    this.timer.Interval = value;
                }
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
        Description("Specifies the visibility of the progress streaming label."),
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
        Description("Specifies the visibility of the browser version label."),
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
                    this.apiVersionStatusLabel.Spring = true;
                }
                else
                {
                    this.apiVersionStatusLabel.Spring = false;
                }

                this.browserVersionStatusLabelVisible = value;
                this.browserVersionStatusLabel.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the api version label is visible
        /// </summary>
        [Category("Control Options"),
        Description("Specifies the visibility of the api version label."),
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
        Description("Specifies the visibility of the plugin version label."),
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
        /// <param name="instance">The GEWebBrowser instance</param>
        /// <example>Example: GEToolStrip.SetBrowserInstance(GEWebBrowser)</example>
        public void SetBrowserInstance(GEWebBrowser instance)
        {
            this.browser = instance;
            this.geplugin = instance.Plugin;

            if (!GEHelpers.IsGE(this.geplugin))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            if (this.browser.PluginIsReady)
            {
                this.Enabled = true;
                this.timer = new Timer();
                this.timer.Interval = this.interval;
                this.timer.Start();
                this.timer.Tick += (o, e) => this.Timer_Tick(o, e);

                try
                {
                    this.browserVersionStatusLabel.Text = "ie " + this.browser.Version.ToString();
                    this.apiVersionStatusLabel.Text = "api " + this.geplugin.getApiVersion();
                    this.pluginVersionStatusLabel.Text = "plugin " + this.geplugin.getPluginVersion();
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("SetBrowserInstance: " + rbex.Message, "StatusStrip");
                    ////throw;
                }
            }

            this.browser.PluginReady += (o, e) => this.Enabled = true;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Timer tick callback function
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.browser.PluginIsReady)
            {
                float percent = 0;

                try
                {
                    percent = this.geplugin.getStreamingPercent();
                }
                catch (COMException)
                {
                    this.timer.Stop();
                    return;
                }

                try
                {
                    if (100 == percent || 0 == percent)
                    {
                        this.streamingStatusLabel.ForeColor = Color.Gray;
                        this.streamingStatusLabel.Text = "idle";
                        this.streamingProgressBar.Value = 0;
                    }
                    else
                    {
                        this.streamingStatusLabel.ForeColor = Color.Black;
                        this.streamingProgressBar.Value = (int)percent;
                        this.streamingStatusLabel.Text = percent + "%";
                    }
                }
                catch (NullReferenceException)
                {
                    // TODO sometimes on application close...need to look into this...
                }
            }
            else
            {
                this.timer.Stop();
            }
        }

        #endregion
    }
}
