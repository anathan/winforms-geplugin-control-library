// <copyright file="HttpRequest.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
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
namespace FC.GEPluginCtrls.HttpServer
{
    /// <summary>
    /// For http request header data in the <see cref="GEServer"/>
    /// </summary>
    internal struct HttpRequest
    {
        /// <summary>
        /// Gets or sets the ReqestTokens (Method, Request-URI, HTTP-Version)
        /// </summary>
        internal string[] ReqestTokens { get; set; }

        /// <summary>
        /// Gets or sets the HTTP request method 
        /// </summary>
        internal string Method { get; set; }

        /// <summary>
        /// Gets or sets the HTTP user agent 
        /// </summary>
        internal string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the HTTP host header
        /// </summary>
        internal string HostHeader { get; set; }

        /// <summary>
        /// Gets or sets the HTTP request Uri
        /// </summary>
        internal string Uri { get; set; }
    }
}
