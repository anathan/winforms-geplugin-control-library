// <copyright file="ViewRefreshMode.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-12-13</date>
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
    /// <summary>
    /// Enumeration of the available view-based refresh modes for the plug-in.
    /// </summary>
    public enum ViewRefreshMode
    {
        /// <summary>
        /// Ignore changes in the view. Also ignore viewFormat parameters, if any.
        /// This view refresh mode is the default.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Refresh the file only when the user explicitly requests it.
        /// </summary>
        Request = 1,

        /// <summary>
        /// Refresh n seconds after movement stops, where n is specified in viewRefreshTime.
        /// </summary>
        Stop = 2,

        /// <summary>
        /// Refresh only when the feature's Region becomes active.
        /// </summary>
        Region = 3
    }
}