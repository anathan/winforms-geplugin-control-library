// <copyright file="StateObject.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-30-12</date>
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
namespace FC.GEPluginCtrls.HttpServer
{
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// State information for the various asynchronous operations in the GEServer.
    /// </summary>
    internal sealed class StateObject
    {
        /// <summary>
        /// Buffer size (1KB)
        /// </summary>
        internal const int BufferSize = 1024;

        /// <summary>
        /// Initializes a new instance of the StateObject class.
        /// </summary>
        internal StateObject()
        {
            this.Buffer = new byte[BufferSize];
            this.Data = new StringBuilder(string.Empty);
        }

        /// <summary>
        /// Gets the current data
        /// </summary>
        internal StringBuilder Data { get; private set; }

        /// <summary>
        /// Gets the Read buffer
        /// </summary>
        internal byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets or sets the working socket
        /// </summary>
        internal Socket Socket { get; set; }
    }
}