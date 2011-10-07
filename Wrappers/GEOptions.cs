// <copyright file="GEOptions.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
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
// </summary>namespace FC.GEPluginCtrls.Enumerations
namespace FC.GEPluginCtrls
{
    using System;

    /// <summary>
    /// Wrapper for the GEOptions com object.
    /// Maps all its getter and setter methods to managed properties
    /// </summary>
    public sealed class GEOptions
    {
        #region Private Fields

        /// <summary>
        /// The plugin object 
        /// </summary>
        private dynamic ge = null;

        /// <summary>
        /// The options object 
        /// </summary>
        private dynamic options = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEOptions class.
        /// </summary>
        /// <param name="ge">the plugin object</param>
        public GEOptions(dynamic ge)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.ge = ge;
            this.options = ge.getOptions();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to show the blue atmosphere that appears around the perimeter of the Earth
        /// On by default.
        /// </summary>
        public bool AtmosphereVisibility
        {
            get
            {
                return this.options.getAtmosphereVisibility();
            }

            set
            {
                this.options.setAtmosphereVisibility(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic ground level view is enabled.
        /// When enabled, the view will change to 'ground level view' when the camera reaches ground level.
        /// This view provides pan and lookAt controls, but no zoom slider. 
        /// The tilt will be set to 90, or parallel with level ground.
        /// </summary>
        public bool AutoGroundLevelViewEnabled
        {
            get
            {
                return this.options.getAutoGroundLevelViewEnabled();
            }

            set
            {
                this.options.setAutoGroundLevelViewEnabled(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether building highlighting is enabled. 
        /// When enabled, buildings will be highlighted when they are moused over.
        /// Disabled by default
        /// </summary>
        public bool BuildingHighlightingEnabled
        {
            get
            {
                return this.options.getBuildingHighlightingEnabled();
            }

            set
            {
                this.options.setBuildingHighlightingEnabled(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether building selection is enabled. 
        /// If enabled, clicking a building will pop a feature balloon containing information from the Google 3D Warehouse database.
        /// </summary>
        public bool BuildingSelectionEnabled
        {
            get
            {
                return this.options.getBuildingSelectionEnabled();
            }

            set
            {
                this.options.setBuildingSelectionEnabled(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether animation of a features when added/removed from the plugin is enabled
        /// The animation consists of a slight change of scale.
        /// Default is true.
        /// </summary>
        public bool FadeInOutEnabled
        {
            get
            {
                return this.options.getFadeInOutEnabled();
            }

            set
            {
                this.options.setFadeInOutEnabled(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the speed at which the camera moves (0 to 5.0).
        /// Set to SPEED_TELEPORT to immediately appear at selected destination.
        /// </summary>
        public double FlyToSpeed
        {
            get
            {
                return this.options.getFlyToSpeed();
            }

            set
            {
                this.options.setFlyToSpeed(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid is visible. Disabled by default.
        /// </summary>
        public bool GridVisibility
        {
            get
            {
                return this.options.getGridVisibility();
            }

            set
            {
                this.options.setGridVisibility(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user mouse control is enabled. Enabled by default.
        /// Note: This also enables and disables keyboard navigation (arrow keys, page-up/page-down, etc).
        /// </summary>
        public bool MouseNavigationEnabled
        {
            get
            {
                return this.options.getMouseNavigationEnabled();
            }

            set
            {
                this.options.setMouseNavigationEnabled(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the overview map is visible. Disabled by default.
        /// </summary>
        public bool OverviewMapVisibility
        {
            get
            {
                return this.options.getOverviewMapVisibility();
            }

            set
            {
                this.options.setOverviewMapVisibility(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the scale legend is visible. Disabled by default.
        /// </summary>
        public bool ScaleLegendVisibility
        {
            get
            {
                return this.options.getScaleLegendVisibility();
            }

            set
            {
                this.options.setScaleLegendVisibility(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the speed of zoom when user rolls the mouse wheel. 
        /// Default is 1. Set to a negative number to reverse the zoom direction.
        /// </summary>
        public double ScrollWheelZoomSpeed
        {
            get
            {
                return this.options.getScrollWheelZoomSpeed();
            }

            set
            {
                this.options.setScrollWheelZoomSpeed(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the status bar is visible. Disabled by default.
        /// </summary>
        public bool StatusBarVisibility
        {
            get
            {
                return this.options.getStatusBarVisibility();
            }

            set
            {
                this.options.setStatusBarVisibility(Convert.ToUInt16(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the terrain exaggeration value.
        /// Valid values are in the range of 1.0 through 3.0. 
        /// Attempting to set outside of this range will result in the value being clamped.
        /// </summary>
        public double TerrainExaggeration
        {
            get
            {
                return this.options.getTerrainExaggeration();
            }

            set
            {
                this.options.setTerrainExaggeration(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display units in imperial (feet and miles)
        /// or metric (meters and kilometres). A value of true uses imperial, false metric.
        /// This setting affects only the values displayed in the status bar and the scale bar.
        /// The values passed and returned with an object's properties are always metric.
        /// Default value is false (metric)
        /// </summary>
        public bool UnitsFeetMiles
        {
            get
            {
                return this.options.getUnitsFeetMiles();
            }

            set
            {
                this.options.setUnitsFeetMiles(Convert.ToUInt16(value));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the map type to Earth or sky mode.
        /// </summary>
        /// <param name="mapType">The maptype to use in the plugin</param>
        public void SetMapType(MapType mapType)
        {
            this.options.setMapType(mapType);
        }

        #endregion
    }
}