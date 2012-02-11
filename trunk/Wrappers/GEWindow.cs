<<<<<<< .mine
﻿// <copyright file="GEWindow.cs" company="FC">
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
namespace FC.GEPluginCtrls
{
    using System;

    /// <summary>
    /// Wrapper for the GEWindow com object.
    /// Maps all its getter and setter methods to managed properties
    /// </summary>
    public sealed class GEWindow
    {
        #region Private Fields

        /// <summary>
        /// The plugin object 
        /// </summary>
        private dynamic ge = null;

        /// <summary>
        /// The options object 
        /// </summary>
        private dynamic window = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWindow class.
        /// </summary>
        /// <param name="ge">the plugin object</param>
        public GEWindow(dynamic ge)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.ge = ge;
            this.window = ge.getWindow();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether Google Earth is visible inside the browser
        /// </summary>
        public bool Visibility
        {
            get { return Convert.ToBoolean(this.window.getVisibility()); }
            set { this.window.setVisibility(Convert.ToUInt16(value)); }
        }
   
        #endregion

        #region Public Methods

        /// <summary>
        /// Gives the Google Earth object focus.
        /// </summary>
        public void Focus()
        {
            this.window.focus();
        }

        /// <summary>
        /// Removes focus from the Google Earth object.
        /// </summary>
        public void Blur()
        {
            this.window.blur();
        }

        #endregion
    }
=======
 // <copyright file="GEWindow.cs" company="FC">
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
namespace FC.GEPluginCtrls
{
    using System;

    /// <summary>
    /// Wrapper for the GEWindow com object.
    /// Maps all its getter and setter methods to managed properties
    /// </summary>
    public sealed class GEWindow
    {
        #region Private Fields

        /// <summary>
        /// The plugin object 
        /// </summary>
        private dynamic ge = null;

        /// <summary>
        /// The options object 
        /// </summary>
        private dynamic window = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEWindow class.
        /// </summary>
        /// <param name="ge">the plugin object</param>
        public GEWindow(dynamic ge)
        {
            if (!GEHelpers.IsGE(ge))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.ge = ge;
            this.window = ge.getWindow();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether Google Earth is visible inside the browser
        /// </summary>
        public bool Visibility
        {
            get { return Convert.ToBoolean(this.window.getVisibility()); }
            set { this.window.setVisibility(Convert.ToUInt16(value)); }
        }
   
        #endregion

        #region Public Methods

        /// <summary>
        /// Gives the Google Earth object focus.
        /// </summary>
        public void Focus()
        {
            this.window.focus();
        }

        /// <summary>
        /// Removes focus from the Google Earth object.
        /// </summary>
        public void Blur()
        {
            this.window.blur();
        }

        #endregion
    }
>>>>>>> .r469
}