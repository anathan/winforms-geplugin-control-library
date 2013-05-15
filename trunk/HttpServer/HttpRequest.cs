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
    using System;

    /// <summary>
    /// For http request header data in the <see cref="GEServer"/>
    /// </summary>
    internal struct HttpRequest
    {
        /// <summary>
        /// Initializes a new instance of the HttpRequest struct.
        /// </summary>
        /// <param name="data">the raw data to base the HttpRequest on</param>
        internal HttpRequest(string data) : 
            this()
        {
            // split the request by CRLF into lines
            string[] headerLines = data.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            string[] tokens = new string[] { };

            // Find the headers we are interested in from the request
            foreach (string line in headerLines)
            {
                // Get and head only...
                if (line.StartsWith("GET", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("HEAD", StringComparison.OrdinalIgnoreCase))
                {
                    tokens = line.Split(' ');
                }
                else if (line.StartsWith("User-Agent: ", StringComparison.OrdinalIgnoreCase))
                {
                    this.UserAgent = line.Remove(0, "User-Agent: ".Length);
                }
                else if (line.StartsWith("Host: ", StringComparison.OrdinalIgnoreCase))
                {
                    this.HostHeader = line.Remove(0, "Host: ".Length);
                }
            }

            // tokens: Method, Request-URI and HTTP-Version
            if (tokens.Length == 3)
            {
                this.Method = tokens[0];
                this.Uri = tokens[1];
                this.HttpVersion = tokens[2];
                this.Query = string.Empty;

                if (this.Uri.IndexOf('?') > -1)
                {
                    string[] parts = this.Uri.Split('?');
                    ////this.Uri = parts[0]; 
                    this.Query = parts[1];
                }
            }
        }

        /// <summary>
        /// Gets the HTTP request method (GET, HEAD)
        /// </summary>
        internal string Method { get; private set; }

        /// <summary>
        /// Gets the HTTP user agent (GoogleEarth)
        /// </summary>
        internal string UserAgent { get; private set; }

        /// <summary>
        /// Gets the HTTP host header
        /// </summary>
        internal string HostHeader { get; private set; }

        /// <summary>
        /// Gets the HTTP request Uri
        /// </summary>
        internal string Uri { get; private set; }

        /// <summary>
        /// Gets the HTTP version (HTTP/1.1)
        /// </summary>
        internal string HttpVersion { get; private set; }

        /// <summary>
        /// Gets the GET query string
        /// </summary>
        internal string Query { get; private set; }
    }
}
