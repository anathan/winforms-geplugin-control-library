// <copyright file="GEEventArgs.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
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
    /// Custom event arguments 
    /// </summary>
    public sealed class GEEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        public GEEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="apiObject">Plugin API object to initialise with.</param>
        public GEEventArgs(object apiObject)
            : base()
        {
            this.ApiObject = apiObject;
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="message">Event message</param>
        public GEEventArgs(string message)
            : base()
        {
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="message">Event message</param>
        /// <param name="data">Event data</param>
        public GEEventArgs(string message, string data)
            : this(message)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="message">Event message</param>
        /// <param name="data">Event data</param>
        /// <param name="apiObject">Event data object</param>
        public GEEventArgs(string message, string data, dynamic apiObject)
            : this(message, data)
        {
            this.ApiObject = apiObject;
        }

        /// <summary>
        /// Gets or sets the event message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the event data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the event data ApiObject 
        /// </summary>
        public dynamic ApiObject { get; set; }
    }
}