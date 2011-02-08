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
    /// Event handler for methods to be called from javascript
    /// </summary>
    /// <param name="sender">the sending object</param>
    /// <param name="e">the event arguments</param>
    public delegate void ExternalEventHandler(object sender, GEEventArgs e);

    /// <summary>
    /// This COM Visible class contains all the methods to be called from Javascript
    /// </summary>
    [ComVisibleAttribute(true)]
    public partial class External : IExternal
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
        public event ExternalEventHandler PluginReady;

        /// <summary>
        /// Raised when there is a kml event
        /// </summary>
        public event ExternalEventHandler KmlEvent;

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        public event ExternalEventHandler KmlLoaded;

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event ExternalEventHandler ScriptError;

        /// <summary>
        /// Raised when there is a GEPlugin event (frameend, balloonclose) 
        /// </summary>
        public event ExternalEventHandler PluginEvent;

        /// <summary>
        /// Rasied when there is a viewchangebegin, viewchange or viewchangeend event 
        /// </summary>
        public event ExternalEventHandler ViewEvent;

        #endregion

        #region Public properites

        /// <summary>
        /// Gets the store of fetched IKmlObjects
        /// </summary>
        public static Dictionary<string, object> KmlObjectCache
        {
            get
            {
                return External.kmlObjectCache;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Can be called from javascript to invoke method
        /// </summary>
        /// <param name="name">the name of method to be called</param>
        /// <param name="parameters">array of parameter objects</param>
        public void InvokeCallBack(string name, object parameters)
        {
            try
            {
                object[] objArr;

                if (parameters.GetType().Name == "__ComObject")
                {
                    objArr = DispatchHelpers.GetObjectArrayFrom__COMObjectArray(parameters);
                }
                else
                {
                    objArr = (object[])parameters;
                }

                GEEventArgs ea = new GEEventArgs(objArr);
                MethodInfo info = this.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
                info.Invoke(this, new object[] { ea });
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("InvokeCallBack: " + ex.ToString(), "External");
                ////throw;
            }
        }

        /// <summary>
        /// Called from javascript when the plugin is ready
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        public void Ready(object ge)
        {
            dynamic pluginObject = ge;

            try
            {
                this.OnPluginReady(
                    ge,
                    new GEEventArgs(pluginObject.getApiVersion(), pluginObject.getPluginVersion()));
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("Ready: " + ex.ToString(), "External");
                ////throw;
            }
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
                    sender,
                    new GEEventArgs(kmlEvent.getType(), action, kmlEvent.getTarget()));
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("KmlEventCallBack: " + ex.ToString(), "External");
                ////throw;
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
                    pluginEvent,
                    new GEEventArgs(pluginEvent.getType(), action, string.Empty));
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("ViewEventCallBack: " + ex.ToString(), "External");
                ////throw;
            }
        }

        /// <summary>
        /// Called from javascript when there is a viewchange event
        /// </summary>
        /// <param name="sender">The GEView object</param>
        /// <param name="action">The event action (viewchangebegin, viewchange or viewchangeend)</param>
        public void ViewEventCallBack(object sender, string action)
        {
            dynamic viewEvent  = sender;

            try
            {
                this.OnViewEvent(
                    sender,
                    new GEEventArgs(viewEvent.getType(), action, string.Empty));
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine("ViewEventCallBack: " + ex.ToString(), "External");
                ////throw;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method for raising the PluginReady event
        /// </summary>
        /// <param name="ge">The plugin object</param>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnPluginReady(object ge, GEEventArgs e)
        {
            if (this.PluginReady != null)
            {
                this.PluginReady(ge, e);
            }
        }

        /// <summary>
        /// Protected method for raising the KmlEvent event
        /// </summary>
        /// <param name="kmlEvent">The kmlEvent object</param>
        /// <param name="e">The Event arguments</param>
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
        /// <param name="e">The Event arguments</param>
        protected virtual void OnKmlLoaded(GEEventArgs e)
        {
            object kmlObject = ((object[])e.Tag)[0];

            if (this.KmlLoaded != null)
            {
                this.KmlLoaded(kmlObject, e);
            }
        }

        /// <summary>
        /// Protected method for capturing fetched IKmlObjects
        /// </summary>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnKmlFetched(GEEventArgs e)
        {
            object kmlObject = ((object[])e.Tag)[0];
            string url = (string)((object[])e.Tag)[1];
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
        /// Protected method for raising the ViewEvent event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnViewEvent(object sender, GEEventArgs e)
        {
            if (this.ViewEvent != null)
            {
                this.ViewEvent(sender, e);
            }
        }

        #endregion
    }
}
