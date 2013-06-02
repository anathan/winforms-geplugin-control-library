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
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    using FC.GEPluginCtrls.Properties;

    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This browser control holds the Google Earth Plug-in,
    /// it also provides wrapper methods to work with the Google.Earth namespace
    /// </summary>
    [SecurityCritical]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed partial class GEWebBrowser : WebBrowser, INotifyPropertyChanged
    {
        #region Private Fields

        /// <summary>
        /// External is A COM Visible class that holds all the public methods
        /// to be called from JavaScript. An instance of this is set
        /// to the base object's ObjectForScripting property in the constructor.
        /// </summary>
        private readonly External external = new External();

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private dynamic plugin;

        /// <summary>
        /// Current plug-in Imagery database - uses INotifyPropertyChanged
        /// </summary>
        private ImageryBase imageryBase = ImageryBase.Earth;

        /// <summary>
        /// Current plug-in state - uses INotifyPropertyChanged
        /// </summary>
        private bool pluginIsReady;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWebBrowser class.
        /// </summary>
        public GEWebBrowser()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;
            this.ObjectForScripting = this.external;
            this.external.PluginReady += this.OnExternal_PluginReady;
            this.external.KmlLoaded += (o, e) => this.KmlLoaded(this, e);
            this.external.ScriptError += (o, e) => this.ScriptError(this, e);
            this.external.KmlEvent += (o, e) => this.KmlEvent(this, e);
            this.external.PluginEvent += (o, e) => this.PluginEvent(this, e);
            this.external.ViewEvent += (o, e) => this.ViewEvent(this, e);
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
        /// Raised when a KML/KMZ file has loaded
        /// </summary>
        public event EventHandler<GEEventArgs> KmlLoaded = delegate { };

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event EventHandler<GEEventArgs> ScriptError = delegate { };

        /// <summary>
        /// Raised when there is a GEPlugin event
        /// </summary>
        public event EventHandler<GEEventArgs> PluginEvent = delegate { };

        /// <summary>
        /// Raised when there is a viewchangebegin, viewchange or viewchangeend event 
        /// </summary>
        public event EventHandler<GEEventArgs> ViewEvent = delegate { };

        /// <summary>
        /// INotifyPropertyChanged implementation 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the plugin instance associated with the control
        /// </summary>
        public dynamic Plugin
        {
            get
            {
                return this.plugin;
            }
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
                this.OnPropertyChanged("ImageryBase");
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

            set
            {
                this.pluginIsReady = value;
                this.OnPropertyChanged("PluginIsReady");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to redirect html links to the system browser
        /// Default is true, setting this false opens links inside the GEWebBrowser control.
        /// </summary>
        [Category("Control Options")]
        [Description("Gets or sets a value indicating whether to redirect html links to the system browser.")]
        [DefaultValue(true)]
        public bool RedirectLinksToSystemBrowser { get; set; }

        #endregion

        #region Hidden Properties

        /// <summary>
        /// Gets or sets a value indicating whether the  control navigates to documents that are dropped onto it.
        /// </summary>
        [Browsable(false)]
        public new bool AllowWebBrowserDrop
        {
            get
            {
                return base.AllowWebBrowserDrop;
            }
            set
            {
                base.AllowWebBrowserDrop = value;
            }
        }

        /// <summary>
        /// Gets or sets the System.Windows.Forms.ContextMenuStrip associated with this control
        /// </summary>
        [Browsable(false)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether scroll bars are displayed in the control.
        /// </summary>
        [Browsable(false)]
        public new bool ScrollBarsEnabled
        {
            get
            {
                return base.ScrollBarsEnabled;
            }
            set
            {
                base.ScrollBarsEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the shortcut menu of the control is enabled.
        /// </summary>
        [Browsable(false)]
        public new bool IsWebBrowserContextMenuEnabled
        {
            get
            {
                return base.IsWebBrowserContextMenuEnabled;
            }
            set
            {
                base.IsWebBrowserContextMenuEnabled = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether keyboard shortcuts are enabled within the control.
        /// </summary>
        [Browsable(false)]
        public new bool WebBrowserShortcutsEnabled
        {
            get
            {
                return base.WebBrowserShortcutsEnabled;
            }
            set
            {
                base.WebBrowserShortcutsEnabled = value;
            }
        }

        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Kills the current plug-in processes on the system.
        /// </summary>
        /// <param name="all">Optionally kill all Google Earth plug-in processes on the system. Default is False</param>
        public void KillPlugin(bool all)
        {
            this.PluginIsReady = false;

            if (all)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "geplugin")
                    {
                        Debug.WriteLine(process.Id.ToString(CultureInfo.InvariantCulture), "KillPlugin");
                        process.Kill();
                    }
                }

                return;
            }

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

        /// <summary>
        /// Wrapper for the the google.earth.addEventListener method
        /// </summary>
        /// <param name="feature">The target feature</param>
        /// <param name="action">The event Id</param>
        /// <param name="callback">Optional, the name of JavaScript callback function to use, or an anonymous function</param>
        /// <param name="useCapture">Optionally use event capture</param>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "someFunction");</example>
        /// <example>GEWebBrowser.AddEventListener(object, "click", "function(event){alert(event.getType);}");</example>
        public void AddEventListener(object feature, EventId action, string callback = null, bool useCapture = false)
        {
            if (!string.IsNullOrEmpty(callback))
            {
                callback = "_x=" + callback;
            }

            this.InvokeJavaScript(
                JSFunction.AddEventListener,
                feature,
                feature.GetHashCode(),
                action.ToString().ToUpperInvariant(),
                callback,
                useCapture);
        }

        /// <summary>
        /// Wrapper for the  google.earth.createInstance method
        /// See: http://code.google.com/apis/earth/documentation/reference/google_earth_namespace.html#70288485024d8129dd1c290fb2e5553b
        /// </summary>
        /// <param name="database">The database name</param>
        /// <example>Example: GEWebBrowser.CreateInstance(ImageryBase.Moon);</example>
        public void CreateInstance(ImageryBase database)
        {
            if (null == this.Document)
            {
                return;
            }

            this.PluginIsReady = false;
            string name = database.ToString();
            this.InvokeJavaScript(JSFunction.CreateInstance, name);
            this.imageryBase = database;
        }

        /// <summary>
        /// Wrapper for the google.earth.executeBatch method
        /// Efficiently executes an arbitrary, user-defined function (the batch function),minimizing
        /// the amount of overhead incurred during cross-process communication between the browser
        /// and Google Earth Plug-in. 
        /// </summary>
        /// <param name="method">The JavaScript method to execute</param>
        /// <param name="context">An optional parameter to pass to the method</param>
        public void ExecuteBatch(string method, object context = null)
        {
            // see: http://code.google.com/apis/earth/documentation/reference/google_earth_namespace.html#b26414915202d39cad12bcd5bd99e739
            if (this.Document != null)
            {
                this.InvokeJavaScript(JSFunction.ExecuteBatch, method, context);
            }
        }

        /// <summary>
        /// Asynchronously load a remote KML/KMZ file 
        /// This function invokes "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a KML/KMZ file</param>
        public void FetchKml(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("url is null or empty");
            }

            this.FetchKml(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Synchronously load a remote KML/KMZ file.
        /// The result of the synchronous request is parsed by the plug-in and the resultant object is returned. 
        /// This function invokes "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a KML/KMZ file</param>
        public void FetchKml(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            this.InvokeJavaScript(JSFunction.FetchKml, url.ToString());
        }

        /// <summary>
        /// Synchronously load a remote KML/KMZ file.
        /// The result of the synchronous request is parsed by the plug-in and the resultant object is returned. 
        /// </summary>
        /// <param name="url">path to a KML/KMZ file</param>
        /// <param name="timeout">time to wait for return in milliseconds</param>
        /// <returns>A KmlObject or null.</returns>
        public object FetchKmlSynchronous(string url, int timeout = 5000)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("url is null or empty");
            }

            return this.FetchKmlSynchronous(new Uri(url, UriKind.RelativeOrAbsolute), timeout);
        }

        /// <summary>
        /// Synchronously load a remote KML/KMZ file.
        /// The result of the synchronous request is parsed by the plug-in and the resultant object is returned. 
        /// </summary>
        /// <param name="url">Uri of a KML/KMZ file</param>
        /// <param name="timeout">time to wait for return in milliseconds</param>
        /// <returns>The KML as a KmlObject</returns>
        public object FetchKmlSynchronous(Uri url, int timeout = 5000)
        {
            string path = url.ToString();
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)(() => this.InvokeJavaScript(JSFunction.FetchKmlSynchronous, path)));
            }
            else
            {
                this.InvokeJavaScript(JSFunction.FetchKmlSynchronous, path);
            }

            External.ResetDictionary[path] = new AutoResetEvent(false);
            WaitHandle.WaitAll(new WaitHandle[] { External.ResetDictionary[path] }, timeout);
            return External.KmlDictionary.ContainsKey(path) ? External.KmlDictionary[path] : null;
        }

        /// <summary>
        /// As FetchKmlSynchronous - but uses a native HttpWebRequest rather than google.earth.fetchKml.
        /// The result of the synchronous request is parsed by the plug-in and the resultant object is returned.    
        /// </summary>
        /// <param name="url">path to a KML file</param>
        /// <param name="timeout">time to wait for return in milliseconds</param>
        /// <returns>A KmlObject or null</returns>
        public object FetchAndParse(string url, int timeout = 5000)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            return this.FetchAndParse(new Uri(url), timeout);
        }

        /// <summary>
        /// Same as FetchKmlSynchronous but uses a native HttpWebRequest rather than google.earth.fetchKml.
        /// The result of the synchronous request is parsed by the plug-in and the resultant object is returned.    
        /// </summary>
        /// <param name="url">Uri of a KML file</param>
        /// <param name="timeout">time to wait for return in milliseconds</param>
        /// <returns>A KmlObject or null</returns>
        public object FetchAndParse(Uri url, int timeout = 5000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            request.Timeout = timeout;

            WebResponse response = null;
            Stream responseStream = null;
            XmlDocument kml = new XmlDocument();

            try
            {
                response = request.GetResponse();
                responseStream = response.GetResponseStream();
                kml.Load(response.GetResponseStream());
                return this.ParseKml(kml.InnerXml);
            }
            catch (WebException wex)
            {
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1}", wex.Status, url), "GEWebBrowser");
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }
                }

                Debug.WriteLine("FetchAndParse: " + url, "GEWebBrowser");
            }

            return null;
        }

        /// <summary>
        /// Loads KML file from the local file system and attempts to parse the data into the plug-in.
        /// </summary>
        /// <param name="path">path to a KML file</param>
        /// <returns>A KmlObject or null</returns>
        public object FetchKmlLocal(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            dynamic result = null;

            if (File.Exists(path) && path.EndsWith("kml", StringComparison.OrdinalIgnoreCase))
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
                    Debug.WriteLine("FetchKmlLocal: " + uaex, "GEWebBrowser");
                }
                catch (RuntimeBinderException rbex)
                {
                    Debug.WriteLine("FetchKmlLocal: " + rbex, "GEWebBrowser");
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
        /// Invokes the JavaScript function 'jsDoGeocode'
        /// Automatically flies to the location if one is found for the input
        /// </summary>
        /// <param name="input">the location to geocode</param>
        /// <returns>the KmlPoint object for the geocode, or an empty object</returns>
        public object InvokeDoGeocode(string input)
        {
            if (null != this.Document)
            {
                return this.InvokeJavaScript(JSFunction.DoGeocode, input);
            }

            return null;
        }

        /// <summary>
        /// Inject a JavaScript element into the document head
        /// </summary>
        /// <param name="javaScript">the script code</param>
        public void InjectJavaScript(string javaScript)
        {
            if (null != this.Document)
            {
                try
                {
                    HtmlElement headElement = this.Document.GetElementsByTagName("head")[0];
                    HtmlElement scriptElement = this.Document.CreateElement("script");
                    if (scriptElement == null)
                    {
                        return;
                    }

                    scriptElement.SetAttribute("type", "text/javascript");

                    // use the custom mshtml interface to append the script to the element
                    IHtmlScriptElement element = (IHtmlScriptElement)scriptElement.DomElement;
                    element.Text = "/* <![CDATA[ */ " + javaScript + " /* ]]> */";
                    headElement.AppendChild(scriptElement);
                }
                catch (InvalidOperationException ioex)
                {
                    Debug.WriteLine("InjectJavascript: " + ioex, "GEWebBrowser");
                }
            }
        }

        /// <summary>
        /// Executes a script function that is defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <param name="args">any arguments</param>
        /// <returns>The result of the evaluated function</returns>
        public object InvokeJavaScript(string function, params object[] args)
        {
            if (null == this.Document)
            {
                return null;
            }

            Debug.WriteLine(
                string.Format(
                    CultureInfo.InvariantCulture, "InvokeJavascript: {0}( {1} )", function, string.Join(", ", args)),
                "GEWebBrowser");

            // see http://msdn.microsoft.com/en-us/library/4b1a88bz.aspx
            return this.Document.InvokeScript(function, args);
        }

        /// <summary>
        /// Load the embedded html document into the browser 
        /// </summary>
        public void LoadEmbeddedPlugin()
        {
            ////this.DocumentStream = new MemoryStream(new UTF8Encoding(false).GetBytes(Properties.Resources.Plugin));

            string path = Path.GetTempFileName();
            using (TextWriter tw = new StreamWriter(path))
            {
                tw.Write(Resources.Plugin);
                tw.Close();
            }

            this.Navigate(new Uri(path));
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
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            // feature, hash, action, useCapture
            this.InvokeJavaScript(JSFunction.RemoveEventListener, feature, feature.GetHashCode(), action, useCapture);
        }

        /// <summary>
        /// Take a 'screen grab' of the current GEWebBrowser view
        /// </summary>
        /// <returns>bitmap image</returns>
        public Bitmap ScreenGrab()
        {
            // create a drawing object based on the web browser control
            Rectangle rectangle = this.DisplayRectangle;
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);

            try
            {
                Graphics graphics = Graphics.FromImage(bitmap);
                Point point = new Point();

                // copy the current display as a bitmap
                graphics.CopyFromScreen(this.PointToScreen(point), point, new Size(rectangle.Width, rectangle.Height));
                graphics.Dispose();
            }
            catch (ArgumentNullException anex)
            {
                Debug.WriteLine("ScreenGrab: " + anex, "GEWebBrowser");
            }

            return bitmap;
        }

        /// <summary>
        /// Set the plug-in language
        /// </summary>
        /// <param name="code">The language code to use</param>
        public void SetLanguage(string code)
        {
            this.PluginIsReady = false;
            this.InvokeJavaScript(JSFunction.SetLanguage, code);
        }

        /// <summary>
        /// Reloads the document currently displayed in the control
        /// Overrides the default WebBrowser Refresh method
        /// </summary>
        public override void Refresh()
        {
            if (null == this.Document)
            {
                return;
            }

            this.KillPlugin(false);
            base.Refresh();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles any navigation events in the browser based on the <see cref="RedirectLinksToSystemBrowser"/> property
        /// </summary>
        /// <param name="e">The navigation event arguments</param>
        /// <summary>
        /// Handles any navigation events in the browser based on the <see cref="RedirectLinksToSystemBrowser"/> property
        /// </summary>
        /// <param name="e">The navigation event arguments</param>
        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "res" || e.Url.Host.Length == 0)
            {
                base.OnNavigating(e);
                return;
            }

            if (e.Url.Fragment.StartsWith("#error=ERR"))
            {
                string error = Uri.EscapeDataString(e.Url.Fragment.Remove(0, 7));
                Debug.WriteLine("OnNavigating: " + error, "GEWebBrowser");
                this.PluginIsReady = false;
                base.OnNavigating(e);
                return;
            }

            if (!this.PluginIsReady || !this.RedirectLinksToSystemBrowser)
            {
                base.OnNavigating(e);
                return;
            }

            e.Cancel = true;
            using (Process process = new Process { StartInfo = { FileName = e.Url.ToString() } })
            {
                process.Start();
            }
        }

        /// <summary>
        /// Handles any document completed events in the browser.
        /// Wires up the custom error handling.
        /// </summary>
        /// <param name="e">The document completed event arguments</param>
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(e);

            if (this.Document != null && this.Document.Window != null)
            {
                this.Document.Window.Error += this.OnWindow_Error;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles the external plug-in ready event.
        /// Sets the various fields and raise the PluginReady event.
        /// Wires up the form closing event
        /// </summary>
        /// <param name="sender">The external class</param>
        /// <param name="e">The event arguments</param>
        private void OnExternal_PluginReady(object sender, GEEventArgs e)
        {
            if (null == e.ApiObject)
            {
                throw new Exception("GEPlugin is null or not an object");
            }

            this.plugin = e.ApiObject;
            this.PluginIsReady = true;
            this.PluginReady(this, e);

            Form parent = this.FindForm();
            if (parent != null)
            {
                parent.FormClosing += this.OnForm_Closing;
            }
        }

        /// <summary>
        /// Handles the parent form closing.
        /// Kill the plug-in and clears the HTML document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (this.plugin == null)
            {
                return;
            }

            this.KillPlugin(false);
            this.DocumentText = string.Empty;
        }

        /// <summary>
        /// Handles any native errors in the window and raises a custom script error in their place.
        /// </summary>
        /// <param name="w">the window object</param>
        /// <param name="we">the error arguments</param>
        private void OnWindow_Error(object w, HtmlElementErrorEventArgs we)
        {
            we.Handled = true;
            this.ScriptError(this, new GEEventArgs("line:" + we.LineNumber, "Description: " + we.Description));
        }

        /// <summary>
        /// Used to raise the property changed event.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}