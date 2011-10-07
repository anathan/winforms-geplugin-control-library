// <copyright file="StringValueAttribute.cs" company="FC">
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
namespace FC.GEPluginCtrls
{
    using System;

    /// <summary>
    /// Helper class for working with 'extended' enums using StringValueAttribute attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the StringValueAttribute class.
        /// </summary>
        /// <param name="value">the string value to set as the attribute</param>
        public StringValueAttribute(string value) 
        { 
            this.Value = value; 
        }

        /// <summary>
        /// Gets the Attribute value
        /// </summary>
        public string Value { get; private set; }
    }
}