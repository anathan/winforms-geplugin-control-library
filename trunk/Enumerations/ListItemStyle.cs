// <copyright file="ListItemStyle.cs" company="FC">
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
    /// Enumeration of the available imagery bases for the plug-in.
    /// </summary>
    public enum ListItemStyle
    {
        /// <summary>
        /// No style (none)
        /// </summary>
        None = 0, 

        /// <summary>
        /// The feature's visibility is tied to its list item's checkbox state.
        /// </summary>
        Check = 1,

        /// <summary>
        /// When specified for a folder, document or network link, prevents all items from being made visible at once.
        /// That is, the user can turn all children off but cannot turn them all on at the same time.
        /// This setting is useful for containers or network links containing large amounts of data.
        /// </summary>
        CheckOffOnly = 2,

        /// <summary>
        /// Use a normal checkbox for visibility but do not display children in a list view.
        /// The item's checkbox should allows the user to toggle visibility of the child objects in the viewport.
        /// </summary>
        CheckHideChildren = 3,

        /// <summary>
        /// When specified for a container (a folder or a document), only one of the container's items should be visible at a time.
        /// *not yet supported in KmlTreeView*
        /// </summary>
        RadioFolder = 5
    }
}