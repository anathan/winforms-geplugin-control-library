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
    using System.Security.Permissions;
    using System.Windows.Forms;
    using GEPlugin;

    /// <summary>
    /// Custom event handler
    /// </summary>
    /// <param name="sender">The sending object</param>
    /// <param name="e">Any event arguments</param>
    public delegate void GEWebBorwserEventHandeler(object sender, GEEventArgs e);

    /// <summary>
    /// This control simplifies working with the Google Earth Plugin
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class GEWebBrowser : WebBrowser
    {
        //// Private fields

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private IGEPlugin geplugin = null;

        //// Constructors 

        /// <summary>
        /// Initializes a new instance of the GEWebBrowser class.
        /// </summary>
        public GEWebBrowser()
            : base()
        {
            this.AllowNavigation = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScrollBarsEnabled = false;
            this.WebBrowserShortcutsEnabled = false;
            this.ObjectForScripting = this;
            this.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.GEWebBrowser_DocumentCompleted);
        }

        //// Public events

        /// <summary>
        /// Raised when the plugin is ready
        /// </summary>
        public event GEWebBorwserEventHandeler PluginReady;

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        public event GEWebBorwserEventHandeler KmlLoaded;

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event GEWebBorwserEventHandeler ScriptError;

        //// Public methods

        /// <summary>
        /// This method should be called when the plugin is ready
        /// Ideally this would be done in the javascript "google.earth.createInstance" callback function.
        /// i.e. window.external.Ready(ge);
        /// notice we can pass the ge object as a managed type directly from javascipt
        /// </summary>
        /// <param name="plugin">The GEPlugin instance</param>
        public void Ready(IGEPlugin plugin)
        {
            this.geplugin = plugin;

            GEEventArgs e = new GEEventArgs();

            // the message holds the data format
            e.Message = "ApiVersion:EarthVersion:PluginVersion";

            // the data is just the version info
            e.Data = this.geplugin.getApiVersion() +
                ":" + this.geplugin.getEarthVersion() +
                ":" + this.geplugin.getPluginVersion();

            // Raise the ready event
            this.OnPluginReady(this.geplugin, e);
        }

        /// <summary>
        /// Load a kml/kmz file 
        /// This function requires a 'twin' LoadKml function in javascript
        /// this twin function will call "google.earth.fetchKml"
        /// </summary>
        /// <param name="url">path to a kml/kmz file</param>
        public void LoadKml(string url)
        {
            if (this.Document != null)
            {
                this.Document.InvokeScript("LoadKml", new object[] { url });
            }
        }

        /// <summary>
        /// This method should be called when a kml or kmz file has been sucessfully loaded.
        /// Ideally this is done in the javascript "google.earth.fetchKml" callback function.
        /// i.e. window.external.LoadKmlCallBack(kmlFeature);
        /// </summary>
        /// <param name="kmlFeature">IKmlFeature COM interface</param>
        public void LoadKmlCallBack(IKmlFeature kmlFeature)
        {
            this.OnKmlLoaded(kmlFeature, new GEEventArgs());
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
                    GEEventArgs ea = new GEEventArgs();
                    ea.Message = e.Message;
                    ea.Data = e.InnerException.ToString();
                    this.OnScriptError(this, ea);
                }
            }
        }

        /// <summary>
        /// Ammend or overwirte the css style of a DOM element
        /// </summary>
        /// <param name="elementId">The ID of the element</param>
        /// <param name="css">The Style to apply</param>
        /// <param name="overwrite">Overwrite any existing style if true</param>
        public void ModifyCss(string elementId, string css, bool overwrite)
        {
            HtmlElement element = this.Document.GetElementById(elementId);

            if (element != null && element.TagName == "style")
            {
                if (overwrite)
                {
                    element.Style = css;
                }
                else
                {
                    element.Style += css;
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
                return null;
            }  
        }

        /// <summary>
        /// Executes a script function that is defined in the currently loaded document. 
        /// </summary>
        /// <param name="function">The name of the function to invoke</param>
        /// <param name="args">
        /// The arguments to pass to the function
        /// i.e.
        /// new object[] { "arg1", "arg2" };
        /// </param>
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
                return null;
            }
        }

        //// Protected methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

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

        //// Private Methods

        /// <summary>
        /// Called when the document has finished loading
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void GEWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Set up the error handler for a loaded Document
            this.Document.Window.Error += new HtmlElementErrorEventHandler(this.Window_Error);
        }

        /// <summary>
        /// Called when there is a script error in the window
        /// </summary>
        /// <param name="sender">Event object</param>
        /// <param name="e">Event arguments</param>
        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            // Handle the error
            e.Handled = true;

            // Copy the error data
            GEEventArgs ea = new GEEventArgs();
            ea.Message = e.Description;
            ea.Data = e.LineNumber.ToString();

            // Bubble the error
            this.OnScriptError(this, ea);
        }
    }
}
