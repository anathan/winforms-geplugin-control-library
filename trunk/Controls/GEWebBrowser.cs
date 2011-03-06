// <copyright file="GEWebBrowser.cs" company="FC">
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
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This browser control holds the Google Earth Plug-in,
    /// it also provides wrapper methods to work with the Google.Earth namespace
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class GEWebBrowser : WebBrowser
    {
        #region Private fields

        /// <summary>
        /// Cache of kml event objects
        /// </summary>
        private static Dictionary<string, AutoResetEvent> kmlObjectCacheSyncEvents =
            new Dictionary<string, AutoResetEvent>();

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
        private dynamic geplugin = null;

        /// <summary>
        /// Current plug-in imagery database
        /// </summary>
        private ImageryBase imageryBase = ImageryBase.Earth;

        /// <summary>
        /// Indicates whether the plug-in is ready to use
        /// </summary>
        private bool pluginIsReady = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWebBrowser class.
        /// </summary>
        public GEWebBrowser()
            : base()
        {
            // External - COM visible class
            this.external = new External();

            this.external.KmlLoaded += (o, e) => this.OnKmlLoaded(o, e);
            this.external.PluginReady += (o, e) => this.External_PluginReady(o, e);
            this.external.ScriptError += (o, e) => this.OnScriptError(o, e);
            this.external.KmlEvent += (o, e) => this.OnKmlEvent(o, e);
            this.external.PluginEvent += (o, e) => this.OnPluginEvent(o, e);
            this.external.ViewEvent += (o, e) => this.OnViewEvent(o, e);

            // Setup the desired control defaults
            this.AllowNavigation = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScrollBarsEnabled = false;
            this.ScriptErrorsSuppressed = false;
            this.WebBrowserShortcutsEnabled = false;

            this.DocumentCompleted += (o, e) => this.GEWebBrowser_DocumentCompleted(o, e);

            try
            {
                this.ObjectForScripting = this.external;
            }
            catch (ArgumentException aex)
            {
                Debug.WriteLine(aex.ToString(), "GEWebBrowser");
            }
        }

        #region Public events

        /// <summary>
        /// Raised when the plugin is ready
        /// </summary>
        public event EventHandler<GEEventArgs> PluginReady;

        /// <summary>
        /// Raised when there is a kmlEvent
        /// </summary>
        public event EventHandler<GEEventArgs> KmlEvent;

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        public event EventHandler<GEEventArgs> KmlLoaded;

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event EventHandler<GEEventArgs> ScriptError;

        /// <summary>
        /// Rasied when there is a GEPlugin event
        /// </summary>
        public event EventHandler<GEEventArgs> PluginEvent;

        /// <summary>
        /// Rasied when there is a viewchangebegin, viewchange or viewchangeend event 
        /// </summary>
        public event EventHandler<GEEventArgs> ViewEvent;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets thread event handles for IKmlObjects collected.
        /// </summary>
        public static Dictionary<string, AutoResetEvent> KmlObjectCacheSyncEvents
        {
            get
            {
                return GEWebBrowser.kmlObjectCacheSyncEvents;
            }
        }

        /// <summary>
        /// Gets or sets the current imagery base for the plug-in
        /// </summary>
        public ImageryBase ImageyBase
        {
            get
            {
                return this.imageryBase;
            }

            set
            {
                this.CreateInstance(this.imageryBase);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the plug-in is ready
        /// </summary>
        public bool PluginIsReady
        {
            get
            {
                return this.pluginIsReady;
            }
        }

        /// <summary>
        /// Gets or sets a COM visible object to bind to the base ObjectForScripting property.
        /// This method hides the default WebBrowswer.ObjectForScripting property
        /// </summary>
        public new object ObjectForScripting
        {
            get
            {
                return this.external;
            }

            set
            {
                try
                {
                    this.external = (External)value;
                    base.ObjectForScripting = value;
                }
                catch (InvalidCastException icex)
                {
                    Debug.WriteLine(icex.ToString(), "ObjectForScripting");
                    ////throw;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Wrapper for the the google.earth.addEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <example>GEWebBrowser.AddEventListener(object, "click");</example>
        public void AddEventListener(object feature, string action)
        {
            this.InvokeJavascript(
                "jsAddEventListener",
                new object[] { feature, action });
        }

        /// <summary>
        /// Wrapper for the the google.earth.addEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <param name="callBackFunction">The name of javascript callback function to use</param>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "function(event){alert(event.getType);}");</example>
        public void AddEventListener(object feature, string action, string callBackFunction)
        {
            this.InvokeJavascript(
                "jsAddEventListener",
                new object[] { feature, action, "_x=" + callBackFunction });
        }

        /// <summary>
        /// Wrapper for the  google.earth.createInstance method
        /// See: http://code.google.com/apis/earth/documentation/reference/google_earth_namespace.html#70288485024d8129dd1c290fb2e5553b
        /// </summary>
        /// <param name="database">The database name</param>
        /// <example>GEWebBrowser.CreateInstance(ImageryBase.Moon);</example>
        public void CreateInstance(ImageryBase database)
        {
            if (this.Document != null)
            {
                this.pluginIsReady = false;
                string name = Enum.GetName(typeof(ImageryBase), database);
                this.InvokeJavascript(
                    "jsCreateInstance",
                    new string[] { name });
                this.imageryBase = database;
            }
        }

        /// <summary>
        /// Wrapper for the google.earth.executeBatch method
        /// See: http://code.google.com/apis/earth/documentation/reference/google_earth_namespace.html#b26414915202d39cad12bcd5bd99e739
        /// Efficiently executes an arbitrary, user-defined function (the batch function),minimizing
        /// the amount of overhead incurred during cross-process communication between the browser
        /// and Google Earth Plugin. 
        /// </summary>
        public void ExecuteBatch()
        {
            throw new NotImplementedException("ExecuteBatch method is not yet implemented");
        }  

        /// <summary>
        /// Load a remote kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <example>GEWebBrowser.FetchKml("http://www.site.com/file.kml");</example>
        public void FetchKml(string url)
        {
            this.FetchKml(url, "createCallback_('OnKmlLoaded')");
        }

        /// <summary>
        /// Load a remote kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <param name="completionCallback">name of javascript callback function to call after fetching completes</param>
        /// <example>GEWebBrowser.FetchKml("http://www.site.com/file.kml", "createCallback_(OnKmlLoaded)");</example>
        public void FetchKml(string url, string completionCallback)
        {
            if (this.Document != null)
            {
                this.Document.InvokeScript(
                    "jsFetchKml",
                    new string[] { url, completionCallback });
            }
        }

        /// <summary>
        /// Same as FetchKml but returns the IKmlObject
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <returns>The kml as a kmlObject</returns>
        /// <example>GEWebBrowser.FetchKmlSynchronous("http://www.site.com/file.kml");</example>
        public object FetchKmlSynchronous(string url)
        {
            return this.FetchKmlSynchronous(url, 2000);
        }

        /// <summary>
        /// Same as FetchKml but returns the IKmlObject
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <param name="timeout">time to wait for return in ms</param>
        /// <returns>The kml as a kmlObject</returns>
        /// <example>GEWebBrowser.FetchKmlSynchronous("http://www.site.com/file.kml");</example>
        public object FetchKmlSynchronous(string url, int timeout)
        {
            string completionCallback = String.Format("createCallback_('OnKmlFetched', '{0}')", url);

            try
            {
                if (this.Document != null)
                {
                    KmlObjectCacheSyncEvents[url] = new AutoResetEvent(false);

                    this.Document.InvokeScript(
                        "jsFetchKml",
                        new string[] { url, completionCallback });

                    WaitHandle.WaitAll(
                        new WaitHandle[] { KmlObjectCacheSyncEvents[url] },
                        timeout);

                    if (External.KmlObjectCache.ContainsKey(url))
                    {
                        return External.KmlObjectCache[url];
                    }
                }
            }
            catch (InvalidCastException)
            { 
            }

            return null;
        }
        
        /// <summary>
        /// Loads a local kml file 
        /// </summary>
        /// <param name="path">path to a local kml file</param>
        /// <example>GWEebBrower.FetchKml("C:\file.kml");</example>
        public void FetchKmlLocal(string path)
        {
            if (File.Exists(path) && path.EndsWith("kml", true, System.Globalization.CultureInfo.CurrentCulture))
            {
                FileStream stream = null;
                StreamReader reader = null;

                try
                {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);
                    reader = new StreamReader(stream);
                    dynamic kml = this.geplugin.parseKml(reader.ReadToEnd());

                    this.external.InvokeCallBack(
                        "OnKmlLoaded",
                        new object[] { kml });
                }
                catch (FileNotFoundException fnfex)
                {
                    Debug.WriteLine(fnfex.ToString(), "GEWebBrowser");
                    ////throw;
                }
                catch (UnauthorizedAccessException uaex)
                {
                    Debug.WriteLine(uaex.ToString(), "GEWebBrowser");
                    ////throw;
                }
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine(ex.ToString(), "GEWebBrowser");
                    ////throw;
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }

                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Parse  a kml string and loads in plugin
        /// </summary>
        /// <param name="kml">kml string to process</param>
        public void ParseKml(string kml)
        {
            dynamic kmlObj = null;

            try
            {
                kmlObj = this.geplugin.parseKml(kml);

                if (null != kmlObj)
                {
                    this.external.InvokeCallBack(
                        "OnKmlLoaded",
                        new object[] { kmlObj });
                }
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("ParseKml: " + rbex.Message, "GEHelpers");
            }
            catch (COMException cex)
            {
                Debug.WriteLine("ParseKml: " + cex.Message, "GEHelpers");
            }
        }

        /// <summary>
        /// Parse a kml object and loads in plugin
        /// </summary>
        /// <param name="kml">kml object to process</param>
        public void ParseKmlObject(dynamic kml)
        {
            GEHelpers.AddFeaturesToPlugin(this.geplugin, kml);
        }

        /// <summary>
        /// Get the plugin instance associated with the control
        /// </summary>
        /// <returns>The plugin instance</returns>
        public dynamic GetPlugin()
        {
            return this.geplugin;
        }

        /// <summary>
        /// Invokes the javascript function 'doGeocode'
        /// Automatically flys to the location if one is found
        /// </summary>
        /// <param name="input">the location to geocode</param>
        /// <returns>the point object (if any)</returns>
        /// <example>GEWebBrowser.InvokeDoGeocode("London");</example>
        public object InvokeDoGeocode(string input)
        {
            if (null != this.Document)
            {
                return (dynamic)this.InvokeJavascript("jsDoGeocode", new object[] { input });
            }
            else
            {
                return new object { };
            }
        }

        /// <summary>
        /// Inject a javascript element into the document head
        /// </summary>
        /// <param name="javascript">the script code</param>
        /// <example>GEWebBrowser.InjectJavascript("var say=function(msg){alert(msg);}");</example>
        public void InjectJavascript(string javascript)
        {
            if (null != this.Document)
            {
                try
                {
                    HtmlElement headElement = this.Document.GetElementsByTagName("head")[0];
                    HtmlElement scriptElement = this.Document.CreateElement("script");
                    scriptElement.SetAttribute("type", "text/javascript");

                    // use the custom mshtml interface to append the script to the element
                    IHTMLScriptElement element = (IHTMLScriptElement)scriptElement.DomElement;
                    element.Text = "/* <![CDATA[ */ " + javascript + " /* ]]> */";
                    headElement.AppendChild(scriptElement);
                }
                catch (InvalidOperationException ioex)
                {
                    Debug.WriteLine(ioex.ToString(), "GEWebBrowser");
                    this.OnScriptError(this, new GEEventArgs(ioex.Message, ioex.ToString()));
                    ////throw;
                }
            }
        }

        /// <summary>
        /// Executes a script function defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <returns>The result of the evaluated function</returns>
        /// <example>GEWebBrowser.InvokeJavascript("say");</example>
        public object InvokeJavascript(string function)
        {
            return this.InvokeJavascript(function, new object[] { });
        }

        /// <summary>
        /// Executes a script function that is defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <param name="args">any arguments</param>
        /// <returns>The result of the evaluated function</returns>
        /// <example>GEWebBrowser.InvokeJavascript("say", new object[] { "hello" });</example>
        public object InvokeJavascript(string function, object[] args)
        {
            if (null != this.Document)
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
        /// Kills all running geplugin processes on the system
        /// </summary>
        public void KillAllPluginProcesses()
        {
            try
            {
                // find all the running 'geplugin' processes 
                System.Diagnostics.Process[] gep =
                    System.Diagnostics.Process.GetProcessesByName("geplugin");

                // whilst there are matching processes
                while (gep.Length > 0)
                {
                    try
                    {
                        // terminate them
                        gep[gep.Length - 1].Kill();
                    }
                    catch (InvalidOperationException ioex)
                    {
                        Debug.WriteLine(ioex.ToString(), "GEWebBrowser");
                        ////throw;
                    }
                }
            }
            catch (InvalidOperationException ioex)
            {
                Debug.WriteLine(ioex.ToString(), "GEWebBrowser");
                ////throw;
            }
        }

        /// <summary>
        /// Load the embeded html document into the browser 
        /// </summary>
        public void LoadEmbededPlugin()
        {
            string html = string.Empty;

            try
            {
                html = Properties.Resources.Plugin;

                // Create a temp file and get the full path
                string path = Path.GetTempFileName();

                // Write the html to the temp file
                TextWriter tw = new StreamWriter(path);
                tw.Write(html);

                // Close the temp file
                tw.Close();

                // Navigate to the temp file
                // Windows deletes the temp file automatially when the current session quits.
                this.Navigate(path);
            }
            catch (IOException ioex)
            {
                Debug.WriteLine(ioex.ToString(), "GEWebBrowser");
                throw;
            }
        }

        /// <summary>
        /// Wrapper for the the google.earth.removeEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        public void RemoveEventListener(object feature, string action)
        {
            this.InvokeJavascript(
                "jsRemoveEventListener",
                new object[] { feature, action });
        }

        /// <summary>
        /// Take a 'screen grab' of the current GEWebBrowser view
        /// </summary>
        /// <returns>bitmap image</returns>
        public Bitmap ScreenGrab()
        {
            // create a drawing object based on the webbrowser control
            Rectangle rectangle = this.DisplayRectangle;
            Bitmap bitmap =
                new Bitmap(
                    rectangle.Width,
                    rectangle.Height,
                    PixelFormat.Format32bppArgb);
            try
            {
                Graphics graphics = Graphics.FromImage(bitmap);
                System.Drawing.Point point =
                    new System.Drawing.Point();

                // copy the current display as a bitmap
                graphics.CopyFromScreen(
                    this.PointToScreen(point),
                    point,
                    new Size(rectangle.Width, rectangle.Height));
                graphics.Dispose();
            }
            catch (ArgumentNullException anex)
            {
                Debug.WriteLine(anex.ToString(), "GEWebBrowser");
                throw;
            }

            return bitmap;
        }

        /// <summary>
        /// Set the plugin langauge
        /// </summary>
        /// <param name="code">The language code to use</param>
        public void SetLanguage(string code)
        {
            this.pluginIsReady = false;
            this.InvokeJavascript("jsSetLanguage", new object[] { code });
        }

        /// <summary>
        /// Reloads the document currently displayed in the control
        /// Overides the default WebBrowser Refresh method
        /// </summary>
        public override void Refresh()
        {
            this.pluginIsReady = false;
            base.Refresh();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method for raising the PluginReady event
        /// </summary>
        /// <param name="sender">The browser instance holding the plugin</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnPluginReady(object sender, GEEventArgs e)
        {
            if (this.PluginReady != null)
            {
                this.PluginReady(sender, e);
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

        /// <summary>
        /// Protected method for raising the PluginEvent event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnPluginEvent(object sender, GEEventArgs e)
        {
            if (this.PluginEvent != null)
            {
                this.PluginEvent(sender, e);
            }
        }

        /// <summary>
        /// Protected method for raising the viewchange events
        /// </summary>
        /// <param name="sender">The GEView object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnViewEvent(object sender, GEEventArgs e)
        {
            if (this.ViewEvent != null)
            {
                this.ViewEvent(sender, e);
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when the Plugin is Ready, rasies OnPluginReady 
        /// </summary>
        /// <param name="plugin">The plugin instance</param>
        /// <param name="e">Event arguments</param>
        private void External_PluginReady(dynamic plugin, GEEventArgs e)
        {
            // plugin is the 'ge' object passed from javascript
            this.geplugin = plugin;

            if (null != this.geplugin)
            {
                // The data is just the version info
                e.Message = "PluginVersion";
                try
                {
                    e.Data = this.geplugin.getPluginVersion();
                }
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine(ex.ToString(), "GEWebBrowser");
                    throw;
                }
            }

            // set the ready property
            this.pluginIsReady = true;

            // Raise the ready event
            this.OnPluginReady(this, e);
        }

        /// <summary>
        /// Called when the Html document has finished loading
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        private void GEWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Set up the error handler for a loaded Document
            ////this.Document.Window.Error += new HtmlElementErrorEventHandler(this.Window_Error);

            this.Document.Window.Error += (o, ev) =>
            {
                ev.Handled = true;
                GEEventArgs ea = new GEEventArgs();
                ea.Message = "Document Error";
                ea.Data = "line " + ev.LineNumber.ToString() + " - " + ev.Description;
                this.OnScriptError(e.ToString(), ea);
            };
        }

        #endregion
    }
}
