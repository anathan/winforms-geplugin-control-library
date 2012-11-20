// <copyright file="GETourPlayer.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-02</date>
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
    /// Wrapper for the GETourPlayer com object.
    /// Maps all its getter and setter methods to managed properties
    /// </summary>
    public sealed class GETourPlayer
    {
        #region Private Fields

        /// <summary>
        /// The plugin object 
        /// </summary>
        private dynamic ge = null;

        /// <summary>
        /// The options object 
        /// </summary>
        private readonly dynamic tourPlayer = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GETourPlayer class.
        /// </summary>
        /// <param name="ge">the plugin object</param>
        public GETourPlayer(dynamic ge)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.ge = ge;
            this.tourPlayer = ge.getTourPlayer();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating the current elapsed playing time of the active tour, in seconds.
        /// </summary>
        public float CurrentTime
        {
            get { return this.tourPlayer.getCurrentTime(); }
            set { this.tourPlayer.setCurrentTime(value); }
        }

        /// <summary>
        /// Gets the total duration of the active tour, in seconds.
        /// If no tour is loaded, the behavior of this method is undefined.
        /// </summary>
        public float Duration
        {
            get { return this.tourPlayer.getDuration(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enters the given tour object, exiting any other currently active tour.
        /// This method does not automatically begin playing the tour.
        /// If the argument is null, then any currently active tour is exited and normal globe navigation is enabled.
        /// </summary>
        /// <param name="tour">A tour object (KmlTour)</param>
        public void SetTour(dynamic tour)
        {
            this.tourPlayer.setTour(tour);
        }

        /// <summary>
        /// Plays the currently active tour.
        /// </summary>
        public void Play()
        {
            this.tourPlayer.play();
        }

        /// <summary>
        /// Pauses the currently active tour.
        /// </summary>
        public void Pause()
        {
            this.tourPlayer.pause();
        }

        /// <summary>
        /// Resets the currently active tour, stopping playback and rewinding to the start of the tour.
        /// </summary>
        public void Reset()
        {
            this.tourPlayer.reset();
        }

        #endregion
    }
}