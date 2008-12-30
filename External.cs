// <copyright file="External.cs" company="FC">
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
    using System.Runtime.InteropServices;
    using GEPlugin;

    public delegate void ExternalEventHandeler(object sender, GEEventArgs e);

    [ComVisibleAttribute(true)]
    public class External
    {
        /// <summary>
        /// Initializes a new instance of the COM Visible External class.
        /// </summary>
        public External()
        {
        }

        #region Public events

        /// <summary>
        /// Raised when the plugin is ready
        /// </summary>
        public event ExternalEventHandeler PluginReady;

        /// <summary>
        /// Raised when a kml/kmz file has loaded
        /// </summary>
        public event ExternalEventHandeler KmlLoaded;

        /// <summary>
        /// Raised when there is a script error in the document 
        /// </summary>
        public event ExternalEventHandeler ScriptError;
        
        #endregion

        #region Public methods

        /// <summary>
        /// Called from javascript when the plugin is ready
        /// </summary>
        /// <param name="ge">the plugin instance</param>
        public void Ready(IGEPlugin ge)
        {
            this.OnPluginReady(ge, new GEEventArgs());
        }

        /// <summary>
        /// Called from javascripy when a kml/kmz file has been loaded
        /// </summary>
        /// <param name="kmlFeature">the loaded kml feature</param>
        public void LoadKmlCallBack(IKmlFeature kmlFeature)
        {
            this.OnKmlLoaded(kmlFeature, new GEEventArgs());
        }
        
        #endregion

        #region Protected methods

        /// <summary>
        /// Protected method for raising the PluginReady event
        /// </summary>
        /// <param name="plugin">The plugin object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnPluginReady(IGEPlugin ge, GEEventArgs e)
        {
            if (this.PluginReady != null)
            {
                this.PluginReady(ge, e);
            }
        }

        /// <summary>
        /// Protected method for raising the KmlLoaded event
        /// </summary>
        /// <param name="kmlObject">The kmlObject object</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnKmlLoaded(IKmlFeature kmlFeature, GEEventArgs e)
        {
            if (this.KmlLoaded != null)
            {
                this.KmlLoaded(kmlFeature, e);
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
