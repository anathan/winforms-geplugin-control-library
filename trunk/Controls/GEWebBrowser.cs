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
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This browser control holds the Google Earth Plug-in,
    /// it also provides wrapper methods to work with the Google.Earth namespace
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed partial class GEWebBrowser : WebBrowser
    {
        #region Private Fields

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
        private External external = new External();

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
        /// Indicates whether the plug-in is ready to use.
        /// </summary>
        private volatile bool pluginIsReady = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWebBrowser class.
        /// </summary>
        public GEWebBrowser()
            : base()
        {
            this.InitializeComponent();

            // External - COM visible class
            this.ObjectForScripting = this.external;

            // when the plugin has finished loading
            // if we have a reference to the plugin object
            // set the various fields and raise the PluginReady event
            this.external.PluginReady += (o, e) =>
            {
                if (null != e.ApiObject)
                {
                    this.geplugin = e.ApiObject;
                    this.pluginIsReady = true;
                    this.OnPluginReady(this, e);

                    Form parent = this.FindForm();

                    if (parent != null)
                    {
                        parent.FormClosing += (f, x) =>
                        {
                            // prevents certain script errors on exit...
                            this.DocumentText = string.Empty;
                        };
                    }
                }
            };

            // wireup the other external events to their handlers
            this.external.KmlLoaded += this.OnKmlLoaded;
            this.external.ScriptError += this.OnScriptError;
            this.external.KmlEvent += this.OnKmlEvent;
            this.external.PluginEvent += this.OnPluginEvent;
            this.external.ViewEvent += this.OnViewEvent;

            // when a document has finished loading
            // listen for any errors in the window
            // handle any errors and raise a custom script error event
            this.DocumentCompleted += (o, e) =>
            {
                this.Document.Window.Error += (w, we) =>
                {
                    we.Handled = true;
                    this.OnScriptError(this, new GEEventArgs("line:" + we.LineNumber, "Description: " + we.Description));
                };

                this.Navigating += (b, ne) =>
                {
                    if (RedirectLinksToSystemBrowser)
                    {
                        // prevent WebBrowser navigation
                        ne.Cancel = true;
                        
                        if (ne.Url.Host.Length > 0)
                        {
                            this.pluginIsReady = false;

                            // then open the URL in the system browser
                            Process process = new Process();
                            process.StartInfo.FileName = ne.Url.ToString();
                            process.Start();
                        }
                    }
                };
            };
        }

        #region Public Events

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

        #region Public Properties

        /// <summary>
        /// Gets the plugin instance associated with the control
        /// </summary>
        public dynamic Plugin
        {
            get { return this.geplugin; }
        }

        #region Control Properties

        /// <summary>
        /// Gets or sets the current imagery base for the plug-in
        /// </summary>
        [Browsable(false)]
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
        [Browsable(false)]
        public bool PluginIsReady
        {
            get
            {
                return this.pluginIsReady;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to redirect html links to the system browser
        /// Default is true, setting this false opens links inside the GEWebBrowser control.
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether to redirect html links to the system browser."),
        DefaultValueAttribute(true)]
        public bool RedirectLinksToSystemBrowser { get; set; }

        #endregion

        #region Hidden Properties

        /// <summary>
        /// Gets or sets a value indicating whether the  control navigates to documents that are dropped onto it.
        /// </summary>
        [Browsable(false)]
        public new bool AllowWebBrowserDrop
        {
            get { return base.AllowWebBrowserDrop; }
            set { base.AllowWebBrowserDrop = value; }
        }

        /// <summary>
        /// Gets or sets the System.Windows.Forms.ContextMenuStrip associated with this control
        /// </summary>
        [Browsable(false)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether scroll bars are displayed in the control.
        /// </summary>
        [Browsable(false)]
        public new bool ScrollBarsEnabled
        {
            get { return base.ScrollBarsEnabled; }
            set { base.ScrollBarsEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the shortcut menu of the control is enabled.
        /// </summary>
        [Browsable(false)]
        public new bool IsWebBrowserContextMenuEnabled
        {
            get { return base.IsWebBrowserContextMenuEnabled; }
            set { base.IsWebBrowserContextMenuEnabled = value; }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether keyboard shortcuts are enabled within the control.
        /// </summary>
        [Browsable(false)]
        public new bool WebBrowserShortcutsEnabled
        {
            get { return base.WebBrowserShortcutsEnabled; }
            set { base.WebBrowserShortcutsEnabled = value; }
        }

        #endregion

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets thread event handles for IKmlObjects collected.
        /// </summary>
        internal static Dictionary<string, AutoResetEvent> KmlObjectCacheSyncEvents
        {
            get
            {
                return GEWebBrowser.kmlObjectCacheSyncEvents;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Wrapper for the the google.earth.addEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <param name="javascript">The name of javascript callback function to use, or an anonymous function</param>
        /// <param name="useCapture">Optionally use event capture</param>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "someFunction");</example>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "function(event){alert(event.getType);}");</example>
        public void AddEventListener(object feature, EventId action, string javascript = null, bool useCapture = false)
        {
            if (javascript != null)
            {
                javascript = "_x=" + javascript;
            }

            this.InvokeJavascript(
                JSFunction.AddEventListener,
                new object[] { feature, feature.GetHashCode(), action.ToString().ToLower(), javascript, useCapture });
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
                string name = database.ToString();
                this.InvokeJavascript(JSFunction.CreateInstance, new string[] { name });
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
        /// <param name="method">The javascript method to execute</param>
        /// <param name="context">An optional parmeter to pass to the method</param>
        public void ExecuteBatch(string method, object context = null)
        {
            if (this.Document != null)
            {
                this.Document.InvokeScript(JSFunction.ExecuteBatch, new object[] { method, context });
            }
        }

        /// <summary>
        /// Load a remote kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">string path to a kml reasource</param>
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
        /// <param name="url">Uri to a kml reasource</param>
        /// <example>GEWebBrowser.FetchKml("http://www.site.com/file.kml");</example>
        public void FetchKml(Uri url)
        {
            this.FetchKml(url.ToString(), "createCallback_('OnKmlLoaded')");
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
                this.Document.InvokeScript(JSFunction.FetchKml, new string[] { url, completionCallback });
            }
        }

        /// <summary>
        /// Same as FetchKml but returns the IKmlObject
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <param name="timeout">time to wait for return in ms</param>
        /// <returns>The kml as a kmlObject</returns>
        /// <example>GEWebBrowser.FetchKmlSynchronous("http://www.site.com/file.kml");</example>
        public object FetchKmlSynchronous(string url, int timeout = 1200)
        {
            try
            {
                string completionCallback = string.Format("createCallback_('OnKmlFetched', '{0}')", url);
                string[] paramters = new string[] { url, completionCallback };

                KmlObjectCacheSyncEvents[url] = new AutoResetEvent(false);

                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.Document.InvokeScript(JSFunction.FetchKml, paramters);
                    });
                }
                else
                {
                    this.Document.InvokeScript(JSFunction.FetchKml, paramters);
                }

                WaitHandle.WaitAll(new WaitHandle[] { KmlObjectCacheSyncEvents[url] }, timeout);

                if (External.KmlObjectCache.ContainsKey(url))
                {
                    return External.KmlObjectCache[url];
                }
            }
            catch (NullReferenceException)
            {
                /* in mscorlib.dll if method exited whilst InvokeScript */
            }

            return new object();
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
                    Debug.WriteLine("FetchKmlLocal: ", fnfex.ToString(), "GEWebBrowser");
                }
                catch (UnauthorizedAccessException uaex)
                {
                    Debug.WriteLine("FetchKmlLocal: ", uaex.ToString(), "GEWebBrowser");
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("FetchKmlLocal: ", rbex.ToString(), "GEWebBrowser");
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
        /// GEPlugin.parseKml() wrapper
        /// Parses a kml string and loads it into the plugin
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
        /// Parses a KmlObject  and loads it into the plugin.
        /// </summary>
        /// <param name="kml">kml object to process</param>
        public void ParseKmlObject(dynamic kml)
        {
            GEHelpers.AddFeaturesToPlugin(this.geplugin, kml);
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
                return (dynamic)this.InvokeJavascript(JSFunction.DoGeocode, new object[] { input });
            }
            else
            {
                return new object();
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
                    Debug.WriteLine("InjectJavascript: " + ioex.ToString(), "GEWebBrowser");
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
                Debug.WriteLine(string.Format("> InvokeJavascript: {0}({1})", function, string.Join(", ", args)));

                // see http://msdn.microsoft.com/en-us/library/4b1a88bz.aspx
                return this.Document.InvokeScript(function, args);
            }
            else
            {
                return new object();
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
                Process[] gep = Process.GetProcessesByName("geplugin");

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
                        Debug.WriteLine("KillAllPluginProcesses: ", ioex.ToString(), "GEWebBrowser");
                    }
                }
            }
            catch (InvalidOperationException ioex)
            {
                Debug.WriteLine("KillAllPluginProcesses: ", ioex.ToString(), "GEWebBrowser");
                ////throw;
            }
        }

        /// <summary>
        /// Load the embeded html document into the browser 
        /// </summary>
        public void LoadEmbededPlugin()
        {
            try
            {
                // Create a temp file and get the full path
                string path = Path.GetTempFileName();

                // Write the html to the temp file
                TextWriter tw = new StreamWriter(path);
                tw.Write(Properties.Resources.Plugin);

                // Close the temp file
                tw.Close();

                // Navigate to the temp file
                // Windows deletes the temp file automatially when the current session quits.
                this.Navigate(path);
            }
            catch (IOException ioex)
            {
                Debug.WriteLine("LoadEmbededPlugin: ", ioex.ToString(), "GEWebBrowser");
                throw;
            }
        }

        /// <summary>
        /// Wrapper for the the google.earth.removeEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <param name="useCapture">Optional, use event capture</param>
        public void RemoveEventListener(object feature, EventId action, bool useCapture = false)
        {
            this.InvokeJavascript(
                JSFunction.RemoveEventListener,
                new object[] { feature, feature.GetHashCode(), action.ToString().ToLower(), useCapture });
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
                Debug.WriteLine("ScreenGrab: ", anex.ToString(), "GEWebBrowser");
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
            this.InvokeJavascript(JSFunction.SetLanguage, new object[] { code });
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

        #region Private methods

        /// <summary>
        /// Method for raising the PluginReady event
        /// </summary>
        /// <param name="sender">The browser instance holding the plugin</param>
        /// <param name="e">Event arguments</param>
        private void OnPluginReady(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handlers = this.PluginReady;

            if (handlers != null)
            {
                handlers(this, e);
            }
        }

        /// <summary>
        /// Method for raising the KmlEvent event
        /// </summary>
        /// <param name="sender">the kml event</param>
        /// <param name="e">The eventid</param>
        private void OnKmlEvent(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handlers = this.KmlEvent;

            if (handlers != null)
            {
                handlers(this, e);
            }
        }

        /// <summary>
        /// Method for raising the KmlLoaded event
        /// </summary>
        /// <param name="sender">The kmlObject object</param>
        /// <param name="e">Event arguments</param>
        private void OnKmlLoaded(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handlers = this.KmlLoaded;

            if (handlers != null)
            {
                handlers(this, e);
            }
        }

        /// <summary>
        /// Method for raising the ScriptError event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        private void OnScriptError(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handlers = this.ScriptError;

            if (handlers != null)
            {
                handlers(this, e);
            }
        }

        /// <summary>
        /// Method for raising the PluginEvent event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        private void OnPluginEvent(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handlers = this.PluginEvent;

            if (handlers != null)
            {
                handlers(this, e);
            }
        }

        /// <summary>
        /// Method for raising the viewchange events
        /// </summary>
        /// <param name="sender">The GEView object</param>
        /// <param name="e">Event arguments</param>
        private void OnViewEvent(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handlers = this.ViewEvent;

            if (handlers != null)
            {
                handlers(this, e);
            }
        }

        #endregion
    }
}