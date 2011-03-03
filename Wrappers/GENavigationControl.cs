// <copyright file="GENavigationControl.cs" company="FC">
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
// </summary>
namespace FC.GEPluginCtrls
{
    using System;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// Wrapper used to used to manipulate the navigation controls in Google Earth.
    /// </summary>
    /// <remarks>http://code.google.com/apis/earth/documentation/reference/interface_g_e_navigation_control.html</remarks>
    public class GENavigationControl
    {
        #region Private fields

        private dynamic ge = null;

        private dynamic navigation = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEOptions class.
        /// </summary>
        /// <param name="ge">GEPlugin COM object</param>
        public GENavigationControl(dynamic ge)
        {
            if (!GEHelpers.IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            this.ge = ge;
            this.navigation = ge.getNavigationControl();
        }

        /// <summary>
        /// Initializes a new instance of the GEOptions class.
        /// </summary>
        /// <param name="ge">GEPlugin COM object</param>
        /// <param name="controlType">The control type. Defualt is NavigationControl.Large</param>
        /// <param name="visiblity">The visiblity of the control. Defualt is Visiblity.Show</param>
        /// <param name="streetViewEnabled">Optionally enables the streetview features. Default is true (on)</param>
        public GENavigationControl(
            dynamic ge,
            NavigationControl controlType = NavigationControl.Large,
            Visiblity visiblity = Visiblity.Show,
            bool streetViewEnabled = false)
        {
            if (!GEHelpers.IsGe(ge))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            this.ge = ge;
            this.navigation = ge.getNavigationControl();

            this.Type = controlType;
            this.StreetViewEnabled = streetViewEnabled;
            this.Visiblity = visiblity;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the navigaton control type
        /// </summary>
        public NavigationControl Type
        {
            get
            {
                return (NavigationControl)this.navigation.getControlType();
            }
            set
            {
                this.navigation.setControlType(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the street view functionality is enabled
        /// </summary>
        public bool StreetViewEnabled
        {
            get
            {
                return Convert.ToBoolean(this.navigation.getStreetViewEnabled());
            }
            set
            {
                this.navigation.setStreetViewEnabled(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is:
        /// always visible, always hidden, or visible only when the user intends to use the control.
        /// </summary>
        public Visiblity Visiblity
        {
            get
            {
                return (Visiblity)this.navigation.getVisibility();
            }
            set
            {
                this.navigation.setVisibility(value);
            }
        }

        #endregion
    }
}