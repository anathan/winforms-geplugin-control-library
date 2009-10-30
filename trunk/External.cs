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
    using GEPlugin;

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
    public class External : IExternal
    {
        #region Private fields

        /// <summary>
        /// Stores fetched Kml Objects
        /// </summary>
        private static Dictionary<string, IKmlObject> kmlObjectCache =
            new Dictionary<string, IKmlObject>();

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

        #endregion

        #region Public properites

        /// <summary>
        /// Gets the store of fetched IKmlObjects
        /// </summary>
        public static Dictionary<string, IKmlObject> KmlObjectCache
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
            catch (COMException cex)
            {
                Debug.WriteLine("InvokeCallBack: " + cex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Called from javascript when the plugin is ready
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        public void Ready(IGEPlugin ge)
        {
            try
            {
                this.OnPluginReady(
                    ge,
                    new GEEventArgs(ge.getApiVersion(), ge.getPluginVersion()));
            }
            catch (COMException cex)
            {
                Debug.WriteLine("Ready: " + cex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Called from javascript when there is an error
        /// </summary>
        /// <param name="message">the error message</param>
        public void Error(string message)
        {
            this.OnScriptError(
                this,
                new GEEventArgs(message));
        }

        /// <summary>
        /// Called from javascript when there is a kml event
        /// </summary>
        /// <param name="kmlEvent">the kml event</param>
        /// <param name="action">the event id</param>
        public void KmlEventCallBack(IKmlEvent kmlEvent, string action)
        {
            try
            {
                this.OnKmlEvent(
                    kmlEvent,
                    new GEEventArgs(kmlEvent.getType(), action));
            }
            catch (COMException cex)
            {
                Debug.WriteLine("KmlEventCallBack: " + cex.ToString());
                throw;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method for raising the PluginReady event
        /// </summary>
        /// <param name="ge">The plugin object</param>
        /// <param name="e">The Event arguments</param>
        protected virtual void OnPluginReady(IGEPlugin ge, GEEventArgs e)
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
        protected virtual void OnKmlEvent(IKmlEvent kmlEvent, GEEventArgs e)
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
            IKmlObject kmlObject = (IKmlObject)((object[])e.Tag)[0];

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
            IKmlObject kmlObject = (IKmlObject)((object[])e.Tag)[0];
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

        #endregion
    }
}
