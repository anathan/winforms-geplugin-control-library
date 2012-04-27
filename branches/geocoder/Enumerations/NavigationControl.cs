// <copyright file="NavigationControl.cs" company="FC">
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
    /// <summary>
    /// The available navigation control types in the plugin
    /// </summary>
    public enum NavigationControl
    {
        /// <summary>
        /// The large navigation control type, used with GENavigationControl.setControlType().
        /// </summary>
        Large = 0,

        /// <summary>
        /// The small navigation control type, used with GENavigationControl.setControlType().
        /// </summary>
        Small = 1
    }
}
