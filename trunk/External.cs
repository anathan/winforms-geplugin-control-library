// <copyright file="External.cs" company="FC">
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
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// This COM Visible class contains all the public methods to be called from Javascript.
    /// The various events are used by the <see cref="GEWebBrowser"/> when dealing with the plugin
    /// </summary>
    [ComVisibleAttribute(true)]
    public class External : IExternal
    {
        #region Private fields

        /// <summary>
        /// Stores fetched Kml Objects
        /// </summary>
        private static Dictionary<string, object> kmlObjectCache =
            new Dictionary<string, object>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the External class.
        /// </summary>
        public External()
        {
        }

        #region Public events

        /// <summary>
        /// Raised when the plugin is ready
        /// </summary>
        public event EventHandler<GEEventArgs> PluginReady;

        /// <summary>
        /// Raised when there is a kml event
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
        /// Raised when there is a GEPlugin event (frameend, balloonclose) 
        /// </summary>
        public event EventHandler<GEEventArgs> PluginEvent;

        /// <summary>
        /// Rasied when there is a viewchangebegin, viewchange or viewchangeend event 
        /// </summary>
        public event EventHandler<GEEventArgs> ViewEvent;

        #endregion

        #region Internal properites

        /// <summary>
        /// Gets the store of fetched IKmlObjects.
        /// Used in the process of synchronously loading networklinks
        /// </summary>
        internal static Dictionary<string, object> KmlObjectCache
        {
            get { return External.kmlObjectCache; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Allow javascript to send debug messages
        /// </summary>
        /// <param name="message">the debug message</param>
        public void DebugMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message, "JS> ");
        }

        /// <summary>
        /// Can be called from javascript to invoke method in managed code.
        /// </summary>
        /// <param name="name">the name of the managed method to be called</param>
        /// <param name="parameters">array of parameter objects</param>
        public void InvokeCallBack(string name, dynamic parameters)
        {
            try
            {
                object[] data;

                if (parameters.GetType().Name == "__ComObject")
                {
                    data = new object[] { (dynamic)parameters.kmlObject, (string)parameters.url };
                }
                else
                {
                    data = (object[])parameters;
                }

                GEEventArgs ea = new GEEventArgs(data);
                MethodInfo info = this.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
                info.Invoke(this, new object[] { ea });
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("InvokeCallBack: " + rbex.ToString(), "External");
            }
        }

        /// <summary>
        /// Called from javascript when the plugin is ready
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        public void Ready(dynamic ge)
        {
            if (!GEHelpers.IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            this.OnPluginReady(this, new GEEventArgs("Ready", ge.getPluginVersion(), ge));
        }

        /// <summary>
        /// Called from javascript when there is an error
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="type">the error type</param>
        public void Error(string message, string type)
        {
            this.OnScriptError(
                this,
                new GEEventArgs(message, type));
        }

        /// <summary>
        /// Called from javascript when there is a kml event
        /// </summary>
        /// <param name="sender">the kml event</param>
        /// <param name="action">the event id</param>
        public void KmlEventCallBack(object sender, string action)
        {
            dynamic kmlEvent = sender;

            try
            {
                this.OnKmlEvent(
                    this,
                    new GEEventArgs(kmlEvent.getType(), action, kmlEvent));
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("KmlEventCallBack: " + rbex.ToString(), "External");
            }
        }

        /// <summary>
        /// Called from javascript when there is a GEPlugin event
        /// </summary>
        /// <param name="sender">The plugin object</param>
        /// <param name="action">The event action</param>
        public void PluginEventCallBack(object sender, string action)
        {
            dynamic pluginEvent = sender;

            try
            {
                this.OnPluginEvent(
                    this,
                    new GEEventArgs(pluginEvent.getType(), action, pluginEvent));
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("PluginEventCallBack: " + rbex.ToString(), "External");
            }
        }

        /// <summary>
        /// Called from javascript when there is a View event
        /// </summary>
        /// <param name="sender">The plugin object</param>
        /// <param name="action">The event action</param>
        public void ViewEventCallBack(object sender, string action)
        {
            dynamic viewEvent = sender;

            try
            {
                this.OnViewEvent(
                    this,
                    new GEEventArgs(viewEvent.getType(), action, viewEvent));
            }
            catch (RuntimeBinderException rbex)
            {
                Debug.WriteLine("ViewEventCallBack: " + rbex.ToString(), "External");
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method for raising the PluginReady event
        /// </summary>
        /// <param name="sender">The plugin object</param>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnPluginReady(object sender, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handler = this.PluginReady;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Protected method for raising the KmlEvent event
        /// </summary>
        /// <param name="kmlEvent">The kmlEvent object</param>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnKmlEvent(dynamic kmlEvent, GEEventArgs e)
        {
            EventHandler<GEEventArgs> handler = this.KmlEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Protected method for raising the KmlLoaded event
        /// </summary>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnKmlLoaded(GEEventArgs e)
        {
            EventHandler<GEEventArgs> handler = this.KmlLoaded;
            dynamic kmlObject = ((object[])e.ApiObject)[0];

            if (handler != null)
            {
                handler(this, new GEEventArgs(kmlObject));
            }
        }

        /// <summary>
        /// Protected method for capturing fetched IKmlObjects
        /// </summary>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnKmlFetched(GEEventArgs e)
        {
            dynamic kmlObject = ((object[])e.ApiObject)[0];
            string url = (string)((object[])e.ApiObject)[1];

            lock (KmlObjectCache)
            {
                KmlObjectCache[url] = kmlObject;
                GEWebBrowser.KmlObjectCacheSyncEvents[url].Set();
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
                this.ScriptError(this, e);
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
                this.PluginEvent(this, e);
            }
        }

        /// <summary>
        /// Protected method for raising the ViewEvent event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnViewEvent(object sender, GEEventArgs e)
        {
            if (this.ViewEvent != null)
            {
                this.ViewEvent(this, e);
            }
        }

        #endregion
    }
}
