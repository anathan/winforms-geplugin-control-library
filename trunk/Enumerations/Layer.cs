// <copyright file="Layer.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-08-11</date>
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

using System;

namespace FC.GEPluginCtrls
{
    using System.ComponentModel;

    /// <summary>
    /// Enumeration of the available layers for the plug-in.
    /// </summary>
    [Flags]
    public enum Layer
    {
        /// <summary>
        /// No layer (none)
        /// </summary>
        [Description("")] None = 0x0,

        /// <summary>
        /// The Terrain layer - 3D terrain
        /// </summary>
        [Description("terrainUUID")] Terrain = 0x1,

        /// <summary>
        /// The Buildings layer - 3D buildings
        /// </summary>
        [Description("2a412484-7181-11de-8092-17a790575c91")] Buildings = 0x2,

        /// <summary>
        /// The Low Resolution (grey) Buildings layer <para>Note that this does not contain copies of photorealistic buildings;
        /// rather, it includes buildings that do not yet exist in the photorealistic buildings layer.
        /// As photorealistic versions of grey buildings are created, the buildings will be added to
        /// <see cref="Buildings" />
        /// and removed from
        /// <see cref="BuildingsLowRes" />
        /// </para>
        /// </summary>
        [Description("3a5bb88e-7181-11de-88da-17a790575c91")] BuildingsLowRes = 0x4,

        /// <summary>
        /// The Borders layer <para>Shows country and area borders, and place labels for cities, states, countries, oceans, etc.</para>
        /// </summary>
        [Description("53004770-c7b3-11dc-92c2-dd553d8c9902")] Borders = 0x8,

        /// <summary>
        /// The Roads layer - roads and road names
        /// </summary>
        [Description("4ddec456-c7b3-11dc-aaa5-dd553d8c9902")] Roads = 0x10,

        /// <summary>
        /// The Tree layer - 3D tree models
        /// </summary>
        [Description("8d540610-9429-11df-ad05-451522926098")] Trees = 0x20,

        /// <summary>
        /// All the available layers 
        /// </summary>
        All = Terrain | Buildings | BuildingsLowRes | Borders | Roads | Trees
    }
}