// <copyright file="Layer.cs" company="FC">
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
    /// The available layer types in the plugin
    /// </summary>
    public sealed class Layer
    {
        #region Public Fields

        /// <summary>
        /// The Terrain layer
        /// </summary>
        public static readonly Layer Terrain = new Layer(0, "terrainUUID");

        /// <summary>
        /// The Buildings layer
        /// </summary>
        public static readonly Layer Buildings = new Layer(1, "2a412484-7181-11de-8092-17a790575c91");

        /// <summary>
        /// The Low Resolution (grey) Buildings layer
        /// </summary>
        public static readonly Layer BuildingsLowRes = new Layer(2, "3a5bb88e-7181-11de-88da-17a790575c91");

        /// <summary>
        /// The Borders layer
        /// </summary>
        public static readonly Layer Borders = new Layer(3, "53004770-c7b3-11dc-92c2-dd553d8c9902");

        /// <summary>
        /// The Trees layer
        /// </summary>
        public static readonly Layer Trees = new Layer(4, "8d540610-9429-11df-ad05-451522926098");

        /// <summary>
        /// The Roads layer
        /// </summary>
        public static readonly Layer Roads = new Layer(5, "4ddec456-c7b3-11dc-aaa5-dd553d8c9902");

        #endregion

        #region Private Fields

        /// <summary>
        /// The id of the layer
        /// </summary>
        private readonly string layerId;

        /// <summary>
        /// The 'enum' value
        /// </summary>
        private readonly int value;

        #endregion

        /// <summary>
        /// Initializes a new instance of the Layer class.
        /// </summary>
        /// <param name="value">the 'enum' value</param>
        /// <param name="layerId">the layer id</param>
        private Layer(int value, string layerId)
        {
            this.layerId = layerId;
            this.value = value;
        }

        #region Public Methods

        /// <summary>
        /// Overrides the ToString method to give the layer-id to pass to the plug-in
        /// </summary>
        /// <returns>The layer id  as a string value</returns>
        public override string ToString()
        {
            return this.layerId;
        }

        #endregion
    }
}
