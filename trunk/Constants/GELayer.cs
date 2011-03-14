// <copyright file="GELayer.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-06</date>
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
    /// A collection the names of the internal hashs used by the plugin to control inbuilt plugin layers
    /// </summary>
    public struct GELayer
    {
        /// <summary>
        /// The Terrain layer
        /// </summary>
        public const string Terrain = "terrainUUID";

        /// <summary>
        /// The Buildings layer
        /// </summary>
        public const string Buildings = "2a412484-7181-11de-8092-17a790575c91";

        /// <summary>
        /// The Low Resolution (grey) Buildings layer
        /// </summary>
        public const string BuildingsLowRes = "3a5bb88e-7181-11de-88da-17a790575c91";

        /// <summary>
        /// The Borders layer
        /// </summary>
        public const string Borders = "53004770-c7b3-11dc-92c2-dd553d8c9902";

        /// <summary>
        /// The Trees layer
        /// </summary>
        public const string Trees = "8d540610-9429-11df-ad05-451522926098";

        /// <summary>
        /// The Roads layer
        /// </summary>
        public const string Roads = "4ddec456-c7b3-11dc-aaa5-dd553d8c9902";
    }
}
