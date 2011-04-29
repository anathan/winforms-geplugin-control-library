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
    public class GEEventArgs : EventArgs
    {
        /// <summary>
        /// Event message
        /// </summary>
        private string message = string.Empty;

        /// <summary>
        /// Event data
        /// </summary>
        private string data = string.Empty;

        /// <summary>
        /// Event data object
        /// </summary>
        private object tag = null;

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
        /// <param name="tag">Tag to initialise with.</param>
        public GEEventArgs(object tag)
            : base()
        {
            this.Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="message">Event message</param>
        public GEEventArgs(string message)
            : base()
        {
            this.message = message;
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="message">Event message</param>
        /// <param name="data">Event data</param>
        public GEEventArgs(string message, string data)
            : base()
        {
            this.message = message;
            this.data = data;
        }

        /// <summary>
        /// Initializes a new instance of the GEEventArgs class
        /// </summary>
        /// <param name="message">Event message</param>
        /// <param name="data">Event data</param>
        /// <param name="tag">Event data object</param>
        public GEEventArgs(string message, string data, object tag)
            : base()
        {
            this.message = message;
            this.data = data;
            this.tag = tag;
        }

        /// <summary>
        /// Gets or sets the event message
        /// </summary>
        public string Message
        {
            get 
            {
                return this.message;
            }
            
            set
            {
                this.message = value; 
            }
        }

        /// <summary>
        /// Gets or sets the event data
        /// </summary>
        public string Data
        {
            get 
            {
                return this.data;
            }

            set
            {
                this.data = value;
            }
        }

        /// <summary>
        /// Gets or sets the event data tag
        /// </summary>
        public object Tag
        {
            get
            { 
                return this.tag;
            }

            set
            {
                this.tag = value;
            }
        }
    }
}

