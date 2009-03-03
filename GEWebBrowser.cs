// <copyright file="GEWebBrowser.cs" company="FC">
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
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using GEPlugin;

    /// <summary>
    /// Main delegate event handler
    /// </summary>
    /// <param name="sender">The sending object</param>
    /// <param name="e">The event arguments</param>
    public delegate void GEWebBorwserEventHandeler(object sender, GEEventArgs e);

    /// <summary>
    /// This control simplifies working with the Google Earth Plugin
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class GEWebBrowser : WebBrowser
    {
        #region Private fields

        /// <summary>
        /// External is A COM Visible class that holds all the public methods
        /// to be called from javascript. An instance of this is set
        /// to the base object's ObjectForScripting property in the constuctor.
        /// </summary>
        private External external = null;

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private IGEPlugin geplugin = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWebBrowser class.
        /// </summary>
        public GEWebBrowser()
            : base()
        {
            // External - COM visible class
            this.external = new External();
            this.external.KmlLoaded += new ExternalEventHandeler(this.External_KmlLoaded);
            this.external.PluginReady += new ExternalEventHandeler(this.External_PluginReady);
            this.external.ScriptError += new ExternalEventHandeler(this.External_ScriptError);
            this.external.KmlEvent += new ExternalEventHandeler(this.External_KmlEvent);

            // Setup the control
            this.AllowNavigation = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScrollBarsEnabled = false;
            this.WebBrowserShortcutsEnabled = false;
            this.ObjectForScripting = this.external;
            this.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(this.GEWebBrowser_DocumentCompleted);
        }

        #region Public events

        /// <summary>
        /// Raised when the plugin is ready
        /// </summary>
        public event GEWebBorwserEventHandeler PluginReady;

        /// <summary>
        /// Raised when there is a kmlEvent
        /// </summary>
        public event GEWebBorwserEventHandeler KmlEvent;

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        public event GEWebBorwserEventHandeler KmlLoaded;

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event GEWebBorwserEventHandeler ScriptError;

        #endregion

        #region Public methods

        /// <summary>
        /// Get the plugin instance associated with the control
        /// </summary>
        /// <returns>The plugin instance</returns>
        public IGEPlugin GetPlugin()
        {
            return this.geplugin;
        }

        /// <summary>
        /// Load the embeded html document into the browser 
        /// </summary>
        public void LoadEmbededPlugin()
        {
            try
            {
                // Get the html string from the embebed reasource
                string html = FC.GEPluginCtrls.Properties.Resources.Plugin;

                // Create a temp file and get the full path
                string path = Path.GetTempFileName();

                // Write the html to the temp file
                TextWriter tw = new StreamWriter(path);
                tw.Write(html);

                // Close the temp file
                tw.Close();

                // Navigate to the temp file
                this.Navigate(path);

                // NB: Windows deletes the temp file automatially when the Windows session quits.
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Load a kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        public void FetchKml(string url)
        {
            if (this.Document != null)
            {
                this.Document.InvokeScript("jsFetchKml", new object[] { url });
            }
        }

        /// <summary>
        /// Inject a javascript element into the document head
        /// </summary>
        /// <param name="javascript">the script code</param>
        public void InjectJavascript(string javascript)
        {
            if (this.Document != null)
            {
                try
                {
                    HtmlElement head = this.Document.GetElementsByTagName("head")[0];
                    HtmlElement script = this.Document.CreateElement("script");
                    script.SetAttribute("type", "text/javascript");
                    IHTMLScriptElement element = (IHTMLScriptElement)script.DomElement;
                    element.Text = "/* <![CDATA[ */ " + javascript + " /* ]]> */";
                    head.AppendChild(script);
                }
                catch (Exception e)
                {
                    this.OnScriptError(
                        this,
                        new GEEventArgs(e.Message, e.InnerException.ToString()));
                }
            }
        }

        /// <summary>
        /// Executes a script function defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <returns>The result of the evaluated function</returns>
        public object InvokeJavascript(string function)
        {
            if (this.Document != null)
            {
                // see http://msdn.microsoft.com/en-us/library/4b1a88bz.aspx
                return this.Document.InvokeScript(function, new object[] { });
            }
            else
            {
                return new object { };
            }
        }

        /// <summary>
        /// Executes a script function that is defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <param name="args">any arguments</param>
        /// <returns>The result of the evaluated function</returns>
        public object InvokeJavascript(string function, object[] args)
        {
            if (this.Document != null)
            {
                // see http://msdn.microsoft.com/en-us/library/4b1a88bz.aspx
                return this.Document.InvokeScript(function, args);
            }
            else
            {
                return new object { };
            }
        }

        /// <summary>
        /// Wrapper for the the google.earth.addEventListener method
        /// </summary>
        /// <param name="feature">The target object</param>
        /// <param name="action">The event Id</param>
        public void AddEventListener(object feature, string action)
        {
            this.InvokeJavascript(
                "jsAddEventListener",
                new object[] { feature, action });
        }

        /// <summary>
        /// Take a 'screen grab' of the current GEWebBrowser view
        /// </summary>
        /// <returns>bitmap image</returns>
        public Bitmap ScreenGrab()
        {
            try
            {
                Rectangle rectangle = this.DisplayRectangle;
                Bitmap bitmap =
                    new Bitmap(
                        rectangle.Width,
                        rectangle.Height,
                        PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(bitmap);
                Point point = new Point();
                graphics.CopyFromScreen(
                    this.PointToScreen(point),
                    point,
                    new Size(rectangle.Width, rectangle.Height));
                graphics.Dispose();
                return bitmap;
            }
            catch (Exception)
            {
                return new Bitmap(0, 0);
            }
        }

        /// <summary>
        /// Kills all running geplugin processes on the system
        /// </summary>
        public void KillAllPluginProcesses()
        {
            System.Diagnostics.Process[] gep =
                System.Diagnostics.Process.GetProcessesByName("geplugin");

            while (gep.Length > 0)
            {
                gep[gep.Length - 1].Kill();
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method for raising the PluginReady event
        /// </summary>
        /// <param name="plugin">The plugin object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnPluginReady(object plugin, GEEventArgs e)
        {
            if (this.PluginReady != null)
            {
                this.PluginReady(plugin, e);
            }
        }

        /// <summary>
        /// Protected method for raising the KmlEvent event
        /// </summary>
        /// <param name="kmlEvent">the kml event</param>
        /// <param name="e">The eventid</param>
        protected virtual void OnKmlEvent(object kmlEvent, GEEventArgs e)
        {
            if (this.KmlEvent != null)
            {
                this.KmlEvent(kmlEvent, e);
            }
        }

        /// <summary>
        /// Protected method for raising the KmlLoaded event
        /// </summary>
        /// <param name="kmlObject">The kmlObject object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnKmlLoaded(object kmlObject, GEEventArgs e)
        {
            if (this.KmlLoaded != null)
            {
                this.KmlLoaded(kmlObject, e);
            }
        }

        /// <summary>
        /// Protected method for raising the ScriptError event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnScriptError(object sender, GEEventArgs e)
        {
            if (this.ScriptError != null)
            {
                this.ScriptError(sender, e);
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when the document has a ScriptError
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        private void External_ScriptError(object sender, GEEventArgs e)
        {
            this.OnScriptError(sender, e);
        }

        /// <summary>
        /// Called when the Plugin is Ready 
        /// </summary>
        /// <param name="plugin">The plugin instance</param>
        /// <param name="e">Event arguments</param>
        private void External_PluginReady(object plugin, GEEventArgs e)
        {
            this.geplugin = (IGEPlugin)plugin;

            // A label for the data
            e.Message = "ApiVersion";

            // The data is just the version info
            e.Data = this.geplugin.getApiVersion();

            // Raise the ready event
            this.OnPluginReady(this.geplugin, e);
        }

        /// <summary>
        /// Called when there is a Kml event 
        /// </summary>
        /// <param name="kmlEvent">the kml event</param>
        /// <param name="e">The eventId</param>
        private void External_KmlEvent(object kmlEvent, GEEventArgs e)
        {
            this.OnKmlEvent(kmlEvent, e);
        }

        /// <summary>
        /// Called when a Kml/Kmz file has loaded
        /// </summary>
        /// <param name="kmlFeature">The kml feature</param>
        /// <param name="e">Event arguments</param>
        private void External_KmlLoaded(object kmlFeature, GEEventArgs e)
        {
            this.OnKmlLoaded(kmlFeature, e);
        }

        /// <summary>
        /// Called when the Html document has finished loading
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        private void GEWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Set up the error handler for a loaded Document
            this.Document.Window.Error += new HtmlElementErrorEventHandler(this.Window_Error);
        }

        /// <summary>
        /// Called when there is a script error in the window
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            // Handle the original error
            e.Handled = true;

            // Copy the error data
            GEEventArgs ea = new GEEventArgs();
            ea.Message = e.Description;
            ea.Data = e.LineNumber.ToString();

            // Bubble the error
            this.OnScriptError(this, ea);
        }

        #endregion
    }
}
