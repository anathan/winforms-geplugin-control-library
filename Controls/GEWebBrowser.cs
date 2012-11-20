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
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
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
        private dynamic plugin = null;

        /// <summary>
        /// Current plug-in Imagery database
        /// </summary>
        private ImageryBase imageryBase = ImageryBase.Earth;

        /// <summary>
        /// Indicates whether the plug-in is ready to use.
        /// </summary>
        private bool pluginIsReady = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWebBrowser class.
        /// </summary>
        public GEWebBrowser()
            : base()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;

            // External - COM visible class
            this.ObjectForScripting = this.external;

            // when the plugin has finished loading
            // if we have a reference to the plugin object
            // set the various fields and raise the PluginReady event
            this.external.PluginReady += (o, e) =>
            {
                if (null != e.ApiObject)
                {
                    this.plugin = e.ApiObject;
                    this.pluginIsReady = true;
                    this.PluginReady(this, e);

                    Form parent = this.FindForm();

                    if (parent != null)
                    {
                        parent.FormClosing += (f, x) =>
                        {
                            // prevents script errors on exit...
                            this.KillPlugin();
                            this.DocumentText = string.Empty;
                        };
                    }
                }
            };

            // wireup the other external events to their handlers
            this.external.KmlLoaded += (o, e) => this.KmlLoaded(this, e);
            this.external.ScriptError += (o, e) => this.ScriptError(this, e);
            this.external.KmlEvent += (o, e) => this.KmlEvent(this, e);
            this.external.PluginEvent += (o, e) => this.PluginEvent(this, e);
            this.external.ViewEvent += (o, e) => this.ViewEvent(this, e);

            // when a document has finished loading
            // listen for any errors in the window
            // handle any errors and raise a custom script error event
            this.DocumentCompleted += (o, e) =>
            {
                this.Document.Window.Error += (w, we) =>
                {
                    we.Handled = true;
                    this.ScriptError(this, new GEEventArgs("line:" + we.LineNumber, "Description: " + we.Description));
                };

                this.Navigating += (b, ne) =>
                {
                    if (RedirectLinksToSystemBrowser)
                    {
                        // prevent WebBrowser navigation
                        ne.Cancel = true;
                        
                        if (ne.Url.Host.Length > 0)
                        {
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
        public event EventHandler<GEEventArgs> PluginReady = delegate { };

        /// <summary>
        /// Raised when there is a kmlEvent
        /// </summary>
        public event EventHandler<GEEventArgs> KmlEvent = delegate { };

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        public event EventHandler<GEEventArgs> KmlLoaded = delegate { };

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event EventHandler<GEEventArgs> ScriptError = delegate { };

        /// <summary>
        /// Rasied when there is a GEPlugin event
        /// </summary>
        public event EventHandler<GEEventArgs> PluginEvent = delegate { };

        /// <summary>
        /// Rasied when there is a viewchangebegin, viewchange or viewchangeend event 
        /// </summary>
        public event EventHandler<GEEventArgs> ViewEvent = delegate { };

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the plugin instance associated with the control
        /// </summary>
        public dynamic Plugin
        {
            get { return this.plugin; }
        }

        #region Control Properties

        /// <summary>
        /// Gets or sets the current imagery base for the plug-in
        /// </summary>
        [Browsable(false)]
        public ImageryBase ImageryBase
        {
            get
            {
                return this.imageryBase;
            }

            set
            {
                this.imageryBase = value;
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
        /// Kills all running geplugin processes on the system
        /// </summary>
        public void KillPlugin()
        {
            this.pluginIsReady = false;

            if (this.plugin != null)
            {
                try
                {
                    this.plugin.Kill_();
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("Refresh: " + rbex.Message, "GEWebBrowser");
                }
                catch (COMException cex)
                {
                    Debug.WriteLine("Refresh: " + cex.Message, "GEWebBrowser");
                }
            }
        }

        /// <summary>
        /// Wrapper for the the google.earth.addEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <param name="callback">Optional, the name of javascript callback function to use, or an anonymous function</param>
        /// <param name="useCapture">Optionally use event capture</param>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "someFunction");</example>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "function(event){alert(event.getType);}");</example>
        public void AddEventListener(object feature, EventId action, string callback = null, bool useCapture = false)
        {
            if (!string.IsNullOrEmpty(callback))
            {
                callback = "_x=" + callback;
            }

            object[] args = new object[]
            { 
                feature, 
                feature.GetHashCode(), 
                action.ToString().ToUpperInvariant(), 
                callback,
                useCapture 
            };

            this.InvokeJavaScript(JSFunction.AddEventListener, args);
        }

        /// <summary>
        /// Wrapper for the  google.earth.createInstance method
        /// See: http://code.google.com/apis/earth/documentation/reference/google_earth_namespace.html#70288485024d8129dd1c290fb2e5553b
        /// </summary>
        /// <param name="database">The database name</param>
        /// <example>Example: GEWebBrowser.CreateInstance(ImageryBase.Moon);</example>
        public void CreateInstance(ImageryBase database)
        {
            if (this.Document != null)
            {
                this.pluginIsReady = false;
                string name = database.ToString();
                this.InvokeJavaScript(JSFunction.CreateInstance, new string[] { name });
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
                this.InvokeJavaScript(JSFunction.ExecuteBatch, new object[] { method, context });
            }
        }

        /// <summary>
        /// Load a remote kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">string path to a kml reasource</param>
        /// <example>Example: GEWebBrowser.FetchKml("http://www.site.com/file.kml");</example>
        public void FetchKml(string url)
        {
            this.FetchKml(new Uri(url), "createCallback_('OnKmlLoaded')");
        }

        /// <summary>
        /// Load a remote kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">Uri to a kml reasource</param>
        /// <example>Example: GEWebBrowser.FetchKml("http://www.site.com/file.kml");</example>
        public void FetchKml(Uri url)
        {
            this.FetchKml(url, "createCallback_('OnKmlLoaded')");
        }

        /// <summary>
        /// Asynchronously load a remote kml/kmz file 
        /// This function invokes "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <param name="completionCallback">name of javascript callback function to call after fetching completes</param>
        /// <example>Example: GEWebBrowser.FetchKml("http://www.site.com/file.kml", "createCallback_(OnKmlLoaded)");</example>
        public void FetchKml(string url, string completionCallback)
        {
            FetchKml(new Uri(url), completionCallback);
        }

        /// <summary>
        /// Synchronously load a remote kml/kmz file.
        /// The result of the synchronous request is parsed by the plugin and the resultant object is returned. 
        /// This function invokes "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <param name="completionCallback">name of javascript callback function to call after fetching completes</param>
        /// <example>Example: GEWebBrowser.FetchKml("http://www.site.com/file.kml", "createCallback_(OnKmlLoaded)");</example>
        public void FetchKml(Uri url, string completionCallback)
        {
            if (this.Document != null)
            {
                this.InvokeJavaScript(
                    JSFunction.FetchKml,
                    new string[] { url.ToString(), completionCallback });
            }
        }

        /// <summary>
        /// Synchronously load a remote kml/kmz file.
        /// The result of the synchronous request is parsed by the plugin and the resultant object is returned. 
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        /// <param name="timeout">time to wait for return in ms</param>
        /// <returns>A KmlObject or null.</returns>
        /// <example>Example: GEWebBrowser.FetchKmlSynchronous("http://www.site.com/file.kml");</example>
        public object FetchKmlSynchronous(string url, int timeout = 5000)
        {
            return this.FetchKmlSynchronous(new Uri(url), timeout);
        }

        /// <summary>
        /// Synchronously load a remote kml/kmz file.
        /// The result of the synchronous request is parsed by the plugin and the resultant object is returned. 
        /// </summary>
        /// <param name="url">Uri of a kml/kmz file</param>
        /// <param name="timeout">time to wait for return in ms</param>
        /// <returns>The kml as a kmlObject</returns>
        /// <example>Example: GEWebBrowser.FetchKmlSynchronous("http://www.site.com/file.kml");</example>
        public object FetchKmlSynchronous(Uri url, int timeout = 5000)
        {
            try
            {
                string completionCallback = string.Format(
                    CultureInfo.InvariantCulture,
                    "createCallback_('OnKmlFetched', '{0}')",
                    url);

                string[] paramters = new string[] { url.ToString(), completionCallback };

                KmlObjectCacheSyncEvents[url.ToString()] = new AutoResetEvent(false);

                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.InvokeJavaScript(JSFunction.FetchKml, paramters);
                    });
                }
                else
                {
                    this.InvokeJavaScript(JSFunction.FetchKml, paramters);
                }

                WaitHandle.WaitAll(new WaitHandle[] { KmlObjectCacheSyncEvents[url.ToString()] }, timeout);

                if (External.KmlObjectCache.ContainsKey(url.ToString()))
                {
                    return External.KmlObjectCache[url.ToString()];
                }
            }
            catch (NullReferenceException)
            {
                /* in mscorlib.dll if method exited whilst InvokeScript */
            }

            return null;
        }

        /// <summary>
        /// As FetchKmlSynchronous - but uses a native HttpWebRequest rather than google.earth.fetchKml.
        /// The result of the synchronous request is parsed by the plugin and the resultant object is returned.    
        /// </summary>
        /// <param name="url">path to a kml file</param>
        /// <param name="timeout">time to wait for return in ms</param>
        /// <returns>A KmlObject or null</returns>
        public object FetchAndParse(string url, int timeout = 5000)
        {
            return this.FetchAndParse(new Uri(url), timeout);
        }

        /// <summary>
        /// Same as FetchKmlSynchronous but uses a native HttpWebRequest rather than google.earth.fetchKml.
        /// The result of the synchronous request is parsed by the plugin and the resultant object is returned.    
        /// </summary>
        /// <param name="url">Uri of a kml file</param>
        /// <param name="timeout">time to wait for return in ms</param>
        /// <returns>A KmlObject or null</returns>
        public object FetchAndParse(Uri url, int timeout = 5000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = timeout;

            WebResponse response = null;
            Stream responseStream = null;
            XmlDocument kml = new XmlDocument();
            string name = string.Empty;

            try
            {
                response = request.GetResponse();
                responseStream = response.GetResponseStream();
                kml.Load(response.GetResponseStream());
                return this.ParseKml(kml.InnerXml);
            }
            catch (WebException wex)
            {
                Debug.WriteLine(
                    string.Format(CultureInfo.InvariantCulture, "{0} {1}", wex.Status, url),
                    "GEWebBrowser");
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    responseStream.Close();
                }

                Debug.WriteLine("FetchAndParse: " + url, "GEWebBrowser");
            }

            return null;
        }

        /// <summary>
        /// Loads kml file from the local file system and attempts to parse the data into the plugin.
        /// </summary>
        /// <param name="path">path to a kml file</param>
        /// <returns>A KmlObject or null</returns>
        /// <example>Example: GWEebBrower.FetchKml("C:\file.kml");</example>
        public object FetchKmlLocal(string path)
        {
            dynamic result = null;

            if (File.Exists(path) && 
                path.EndsWith("kml", StringComparison.OrdinalIgnoreCase))
            {
                FileStream stream = null;
                StreamReader reader = null;

                try
                {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);
                    reader = new StreamReader(stream);
                    result = this.ParseKml(reader.ReadToEnd());
                    ////this.external.InvokeCallback("OnKmlLoaded", new object[] { kml });
                }
                catch (UnauthorizedAccessException uaex)
                {
                    Debug.WriteLine("FetchKmlLocal: ", uaex.ToString(), "GEWebBrowser");
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("FetchKmlLocal: ", rbex.Message, "GEWebBrowser");
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

            return result;
        }

        /// <summary>
        /// GEPlugin.parseKml() wrapper
        /// Parse a string of KML and return a handle to the root of the KML object structure that was created
        /// </summary>
        /// <param name="kml">a string of KML to process</param>
        /// <returns>The KML object structure that was created</returns>
        public object ParseKml(string kml)
        {
            dynamic result = null;

            try
            {
                result = this.plugin.parseKml(kml);
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("ParseKml: " + rbex.Message, "GEWebBrowser");
            }
            catch (COMException cex)
            {
                Debug.WriteLine("ParseKml: " + cex.Message, "GEWebBrowser");
            }

            return result;
        }

        /// <summary>
        /// Parses a KmlObject and loads it into the plugin.
        /// </summary>
        /// <param name="kml">kml object to parse</param>
        public void ParseKmlObject(dynamic kml)
        {
            GEHelpers.AddFeaturesToPlugin(this.plugin, kml);
        }

        /// <summary>
        /// Invokes the javascript function 'jsDoGeocode'
        /// Automatically flys to the location if one is found for the input
        /// </summary>
        /// <param name="input">the location to geocode</param>
        /// <returns>the KmlPoint object for the geocode, or an empty object</returns>
        /// <example>Example: GEWebBrowser.InvokeDoGeocode("London");</example>
        public object InvokeDoGeocode(string input)
        {
            if (null != this.Document)
            {
                return this.InvokeJavaScript(JSFunction.DoGeocode, new object[] { input });
            }

            return null;
        }

        /// <summary>
        /// Inject a javascript element into the document head
        /// </summary>
        /// <param name="javaScript">the script code</param>
        /// <example>GEWebBrowser.InjectJavascript("var say=function(msg){alert(msg);}");</example>
        public void InjectJavaScript(string javaScript)
        {
            if (null != this.Document)
            {
                try
                {
                    HtmlElement headElement = this.Document.GetElementsByTagName("head")[0];
                    HtmlElement scriptElement = this.Document.CreateElement("script");
                    scriptElement.SetAttribute("type", "text/javascript");

                    // use the custom mshtml interface to append the script to the element
                    IHtmlScriptElement element = (IHtmlScriptElement)scriptElement.DomElement;
                    element.Text = "/* <![CDATA[ */ " + javaScript + " /* ]]> */";
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
        /// <example>Example: GEWebBrowser.InvokeJavascript("say");</example>
        public object InvokeJavaScript(string function)
        {
            return this.InvokeJavaScript(function, new object[] { });
        }

        /// <summary>
        /// Executes a script function that is defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <param name="args">any arguments</param>
        /// <returns>The result of the evaluated function</returns>
        /// <example>Example: GEWebBrowser.InvokeJavascript("say", new object[] { "hello" });</example>
        public object InvokeJavaScript(string function, object[] args)
        {
            if (null != this.Document)
            {
                Debug.WriteLine(
                    string.Format(
                    CultureInfo.InvariantCulture, 
                    "InvokeJavascript: {0}( {1} )",
                    function,
                    string.Join(", ", args)),
                    "GEWebBrowser");

                // see http://msdn.microsoft.com/en-us/library/4b1a88bz.aspx
                return this.Document.InvokeScript(function, args);
            }

            return null;
        }

        /// <summary>
        /// Load the embedded html document into the browser 
        /// </summary>
        public void LoadEmbeddedPlugin()
        {
            ////this.DocumentStream = new MemoryStream(new UTF8Encoding(false).GetBytes(Properties.Resources.Plugin));

            string path = Path.GetTempFileName();
            TextWriter tw = new StreamWriter(path);
            tw.Write(Properties.Resources.Plugin);
            tw.Close();

            this.Navigate(new Uri(path), "_self", null, "User-Agent: GEWebBrowser");
        }

        /// <summary>
        /// Wrapper for the the google.earth.removeEventListener method
        /// See: https://developers.google.com/earth/documentation/reference/google_earth_namespace#a4367d554eb492adcafa52925ddbf0c71
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <param name="useCapture">Optional, use event capture</param>
        public void RemoveEventListener(object feature, EventId action, bool useCapture = false)
        {
            //feature, hash, action, useCapture
            object[] args = new object[] 
            { 
                feature, 
                feature.GetHashCode(),
                action.ToString().ToUpperInvariant(),
                useCapture 
            };

            this.InvokeJavaScript(JSFunction.RemoveEventListener, args);
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
            this.InvokeJavaScript(JSFunction.SetLanguage, new object[] { code });
        }

        /// <summary>
        /// Reloads the document currently displayed in the control
        /// Overides the default WebBrowser Refresh method
        /// </summary>
        public override void Refresh()
        {
            this.KillPlugin();
            base.Refresh();
        }

        #endregion
    }
}