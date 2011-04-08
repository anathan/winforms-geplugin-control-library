// <copyright file="AltitudeMode.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-01</date>
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
    /// Enumeration of the available altitude modes for the plug-in.
    /// </summary>
    public enum AltitudeMode
    {
        /// <summary>
        /// Specifies that altitudes are at ground level.
        /// For Ground overlays, this means that the image will be draped over the terrain.
        /// </summary>
        ClampToGround = 0,

        /// <summary>
        /// Specifies that altitudes are to be interpreted as meters above or below ground level.
        /// (i.e. the elevation of the terrain at the location).
        /// </summary>
        RelativeToGround = 1,

        /// <summary>
        /// Specifies that altitudes are to be interpreted as meters above or below sea level,
        /// regardless of the actual elevation of the terrain beneath the object.
        /// For example, if you set the altitude of an object to 10 meters with an absolute altitude mode,
        /// the object will appear to be at ground level if the terrain beneath is also 10 meters above sea level.
        /// If the terrain is 3 meters above sea level, the object will appear elevated above the terrain by 7 meters.
        /// If, on the other hand, the terrain is 15 meters above sea level, the object may be completely invisible.
        /// </summary>
        Absolute = 2,

        /// <summary>
        /// Specifies that altitudes are at sea floor level.
        /// </summary>
        ClampToSeaFloor = 4,

        /// <summary>
        /// Specifies that altitudes are to be interpreted as meters above sea floor.
        /// (i.e. the elevation of the sea floor at the location).
        /// </summary>
        RelativeToSeaFloor = 5,

        /// <summary>
        /// Specifies that no altitude is to be interpreted
        /// </summary>
        None = 99
    }
}