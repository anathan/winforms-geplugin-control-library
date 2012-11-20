// <copyright file="IExternal.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-10-04</date>
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

#region

using System;

#endregion

namespace FC.GEPluginCtrls
{
    /// <summary>
    /// This interface should be implemented by any object
    /// that is designed to act as the interface between javascript and managed code
    /// </summary>
    public interface IExternal
    {
        /// <summary>
        /// Raised when the plugin is ready
        /// </summary>
        event EventHandler<GEEventArgs> PluginReady;

        /// <summary>
        /// Raised when there is a kml event
        /// </summary>
        event EventHandler<GEEventArgs> KmlEvent;

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        event EventHandler<GEEventArgs> KmlLoaded;

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        event EventHandler<GEEventArgs> ScriptError;

        /// <summary>
        /// Should be called from javascript to invoke a method in managed code
        /// </summary>
        /// <param name="name">the name of method to be called</param>
        /// <param name="parameters">array of parameter objects</param>
        void InvokeCallback(string name, object parameters);

        /// <summary>
        /// Should be called from javascript when the plugin is ready
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        void Ready(object ge);

        /// <summary>
        /// Should be called from javascript when there is an error
        /// </summary>
        /// <param name="type">the error type</param>
        /// <param name="message">the error message</param>
        void SendError(string type, string message);

        /// <summary>
        /// Should be called from javascript when there is a kml event
        /// </summary>
        /// <param name="kmlEvent">the kml event</param>
        /// <param name="action">the event id</param>
        void KmlEventCallback(object kmlEvent, string action);

        /// <summary>
        /// Should be called from javascript when there is a GEPlugin event
        /// </summary>
        /// <param name="sender">The plugin object</param>
        /// <param name="action">The event action</param>
        void PluginEventCallback(object sender, string action);
    }
}