// <copyright file="RefreshMode.cs" company="FC">
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
    /// Enumeration of the available time-based refresh modes for the plug-in.
    /// </summary>
    public enum RefreshMode
    {
        /// <summary>
        /// Refresh when the file is loaded and whenever the Link parameters change.
        /// This refresh mode is the default.
        /// </summary>
        Change = 0,

        /// <summary>
        /// Refresh every n seconds (specified in refreshInterval).
        /// </summary>
        Interval = 1,

        /// <summary>
        /// Refresh when the expiration time is reached. If a fetched file has a NetworkLinkControl,
        /// the expires time takes precedence over expiration times specified in HTTP headers.
        /// If no expires time is specified, the HTTP max-age header is used (if present).
        /// If max-age is not present, the Expires HTTP header is used (if present).
        /// </summary>
        Expire = 2
    }
}