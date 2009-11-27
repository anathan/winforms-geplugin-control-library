// <copyright file="Server.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-25-11</date>
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
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// A simple, synchronous, TCP server class desinged to work with the plug-in.
    /// </summary>
    public class Server
    {
        #region Private fields

        /// <summary>
        /// Server tcp listner
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// Server IP address
        /// By defualt the server is only available to programs running on the same machine
        /// </summary>
        private IPAddress localHost = IPAddress.Loopback;

        /// <summary>
        /// Server socket 
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Server port number
        /// </summary>
        private ushort port = 8080;

        /// <summary>
        /// Server root directory
        /// </summary>
        private string rootDirectory = Path.DirectorySeparatorChar.ToString();

        /// <summary>
        /// Default file name
        /// </summary>
        private string defaultFileName = "default.kml";

        /// <summary>
        /// Keep listening
        /// </summary>
        private volatile bool keepListening = true;

        /// <summary>
        /// The HTTP version used by the server (1.1)
        /// </summary>
        private readonly string httpVersion = "HTTP/1.1";

        #endregion

        /// <summary>
        /// Initializes a new instance of the Server class
        /// </summary>
        public Server() :
            this(@"\")
        {
        }

        /// <summary>
        /// Initializes a new instance of the Server class
        /// </summary>
        /// <param name="rootDirectory">The server root directory</param>
        public Server(string rootDirectory)
        {
            this.RootDirectory = rootDirectory;
        }

        /// <summary>
        /// Initializes a new instance of the Server class
        /// </summary>
        /// <param name="rootDirectory">The server root directory</param>
        /// <param name="defaultFileName">Default file name to use</param>
        public Server(string rootDirectory, string defaultFileName)
        {
            this.RootDirectory = rootDirectory;
            this.DefaultFileName = defaultFileName;
        }

        #region Public properites

        /// <summary>
        /// Gets or sets the default file name 
        /// The default value is "default.kml"
        /// </summary>
        public string DefaultFileName
        {
            get
            {
                return this.defaultFileName;
            }

            set
            {
                if (value.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    throw new ArgumentException("Invalid Characters in default filename");
                }

                this.defaultFileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the IP Address for the server to use 
        /// The default is 127.0.0.1 (localhost/loopback)
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                return this.localHost;
            }

            set
            {
                this.localHost = value;
            }
        }

        /// <summary>
        /// A port number to use between 0–65536
        /// Defualt value is 8080
        /// </summary>
        public ushort Port
        {
            get
            {
                return this.port;
            }

            set
            {
                this.port = value;
            }
        }

        /// <summary>
        /// Gets or sets the root server directory (webroot) 
        /// http://localhost:port/ will point to this directory
        /// </summary>
        public string RootDirectory
        {
            get
            {
                return this.rootDirectory;
            }

            set
            {
                if (Directory.Exists(value))
                {
                    this.rootDirectory = value;
                }
                else
                {
                    throw new DirectoryNotFoundException("The root directory specified can not be found.");
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            // start listing on the given ip address and port
            this.tcpListener = new TcpListener(this.localHost, this.port);
            this.tcpListener.Start();

            // start the listen thread
            Thread listenThread = new Thread(new ThreadStart(this.StartListen));
            listenThread.Start();

            this.keepListening = true;
            Debug.WriteLine("Start...", "Server-Info");
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            Debug.WriteLine("Stop...", "Server-Info");
            this.keepListening = false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get the local directory from a request uri
        /// </summary>
        /// <param name="requestUri">header Request-Uri</param>
        /// <returns></returns>
        private string GetLocalDirectory(string requestUri)
        {
            string localFilePath = Path.GetDirectoryName(requestUri);

            if (localFilePath != null)
            {
                // remove any leading slash from the path
                if (localFilePath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    localFilePath.TrimStart(Path.DirectorySeparatorChar);
                }

                // add a trailing slash to the path if required
                if (!localFilePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    // add a trailing slash if needed
                    localFilePath += Path.DirectorySeparatorChar;
                }
            }

            if (localFilePath == Path.DirectorySeparatorChar.ToString())
            {
                localFilePath = this.rootDirectory;
            }
            else
            {
                localFilePath = this.RootDirectory + requestUri;
            }

            return localFilePath;
        }

        /// <summary>
        /// Attempt to get the file name from a request uri
        /// </summary>
        /// <param name="requestUri">header Request-Uri</param>
        /// <returns>the filename, the default filename or and empty string</returns>
        private string GetLocalFile(string requestUri)
        {
            string fileName = Path.GetFileName(requestUri);

            // If a file is not specified but a default filename is 
            if (fileName == string.Empty &&
                this.defaultFileName != string.Empty)
            {
                // then try the default filename
                fileName = this.defaultFileName;
            }

            return fileName;
        }

        /// <summary>
        /// Gets the mime type for the given file type
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <returns>the mime type of the file or "application/unknown"</returns>
        private string GetMimeType(string path)
        {
            string extension = string.Empty;

            try
            {
                extension = Path.GetExtension(path);
            }
            catch (ArgumentException aex)
            {
                Debug.WriteLine("GetMimeType: " + aex.ToString(), "Server");
                return "application/unknown";
            }

            switch (extension)
            {
                case ".gif":
                    return "image/gif";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".kml":
                    return "application/vnd.google-earth.kml+xml";
                case ".kmz":
                    return "application/vnd.google-earth.kmz";
                case ".png":
                    return "image/png";
                case ".txt":
                    return "text/plain";
                default:
                    return "application/unknown";
            }
        }

        /// <summary>
        /// Sends an Error Header to the client (GoogleEarth)
        /// </summary>
        /// <param name="status">The Http Status Code</param>
        private void SendError(HttpStatusCode status)
        {
            string code = string.Format("{0} {1}", (int)status, status);
            this.SendHeader(string.Empty, code.Length, status);
            this.SendToBrowser(code);
            this.socket.Close();
        }

        /// <summary>
        /// Sends Header Information to the client (GoogleEarth)
        /// </summary>
        /// <param name="mime">Mime Type of the responce</param>
        /// <param name="bytes">Total Bytes to be sent in the body</param>
        /// <param name="code">The HTTP status code</param>
        private void SendHeader(string mime, int bytes, HttpStatusCode code)
        {
            StringBuilder data = new StringBuilder();
            data.AppendFormat("{0} {1} {2}{3}", this.httpVersion, (int)code, code.ToString(), Environment.NewLine);
            data.AppendFormat("{0}: {1}{2}", HttpResponseHeader.Server, "GEPluginCtrls-a", Environment.NewLine);
            data.AppendFormat("{0}: {1}{2}", HttpResponseHeader.ContentType, mime, Environment.NewLine);
            data.AppendFormat("{0}: {1}{2}", HttpResponseHeader.AcceptRanges, "bytes", Environment.NewLine);
            data.AppendFormat("{0}: {1}{2}", HttpResponseHeader.ContentLength, bytes, Environment.NewLine);
            data.AppendFormat("{0}{1}", Environment.NewLine, Environment.NewLine);
            this.SendToBrowser(data.ToString());
        }

        /// <summary>
        /// Sends body data to the client (GoogleEarth)
        /// </summary>
        /// <param name="data">The data to be sent to google earth (client)</param>
        private void SendToBrowser(string data)
        {
            this.SendToBrowser(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sends data to the client (Browser/GoogleEarth)
        /// </summary>
        /// <param name="data">The data to be sent to google earth (client)</param>
        private void SendToBrowser(byte[] data)
        {
            int numBytes = 0;

            try
            {
                if (this.socket.Connected)
                {
                    if ((numBytes = this.socket.Send(data, data.Length, 0)) == -1)
                    {
                        Debug.WriteLine("Send Packet Error", "Server");
                    }
                }
                else
                {
                    Debug.WriteLine("Connection Dropped", "Server");
                }
            }
            catch (SocketException sex)
            {
                Debug.WriteLine("sendToBrowser: " + sex.ToString(), "Server");
            }
        }

        /// <summary>
        /// Listen for incoming connections
        /// </summary>
        private void StartListen()
        {
            while (this.keepListening)
            {
                // Stops the call to AcceptSocket from blocking
                // this allows the listenThread to teminate cleanly
                if (!this.tcpListener.Pending())
                {
                    Thread.Sleep(100); // a tenth of a second
                    continue;
                }
                else
                {
                    this.socket = this.tcpListener.AcceptSocket();
                }

                // Handle any incoming connection
                if (this.socket.Connected)
                {
                    Debug.WriteLine("Connected: " + this.socket.RemoteEndPoint, "Server-Info");

                    // set up a buffer/container for the incoming data
                    byte[] buffer = new byte[1024]; // 1KB
                    string data = string.Empty;
                    
                    do
                    {
                        int read = this.socket.Receive(buffer, this.socket.Available, SocketFlags.None);
                        data += Encoding.UTF8.GetString(buffer, 0, read);
                    }
                    // wait for the end of the request header: CRLF CRLF 
                    // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec5.html
                    while (!data.EndsWith(Environment.NewLine + Environment.NewLine));

                    // Parse the request header
                    string[] headerLines = data.Split(Environment.NewLine.ToCharArray());
                    string requestHeader = string.Empty;
                    string requestMethod = string.Empty;
                    string userAgentHeader = HttpRequestHeader.UserAgent.ToString();
                    string hostHeader = HttpRequestHeader.Host.ToString();

                    foreach (string line in headerLines)
                    {
                        // The server only supports GET and HEAD requests
                        if (line.StartsWith("GET"))
                        {
                            Debug.WriteLine(line, "Server-Request");
                            requestMethod = "GET";
                            requestHeader = line;
                        }
                        else if (line.StartsWith("HEAD"))
                        {
                            Debug.WriteLine(line, "Server-Request");
                            requestMethod = "HEAD";
                            requestHeader = line;
                        }
                        else if (line.StartsWith(userAgentHeader))
                        {
                            userAgentHeader = line;
                        }
                        else if (line.StartsWith(hostHeader))
                        {
                            hostHeader = line;
                        }
                        else
                        {
                            // if the line does not match any 
                            // of the headers we are interested in...
                            continue;
                        }
                    }

                    // Request-Line
                    // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec5.html#sec5.1
                    // Method [SP] Request-URI [SP] HTTP-Version [CRLF]
                    // If there are missing parts then skip the request
                    string[] requestTokens = requestHeader.Split(' ');
                    if (requestTokens.Length != 3)
                    {
                        continue;
                    }

                    // Method
                    // Handle GET and HEAD requests otherwise send 501 NotImplemented
                    // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.3
                    // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.4
                    if (requestMethod != "GET" && requestMethod != "HEAD")
                    {
                        Debug.WriteLine("501 NotImplemented", "Server-Send");
                        this.SendError(HttpStatusCode.NotImplemented);
                        continue;
                    }

                    // HTTP-Version 
                    // Handle HTTP/1.1 otherwise send 505 HttpVersionNotSupported
                    if ((requestTokens[2] != this.httpVersion))
                    {
                        Debug.WriteLine("505 HttpVersionNotSupported", "Server");
                        this.SendError(HttpStatusCode.HttpVersionNotSupported);
                        continue;
                    }

                    // User-Agent
                    // Handle GoogleEarth otherwise send 403 Forbidden
                    if (!userAgentHeader.StartsWith("User-Agent: GoogleEarth"))
                    {
                        Debug.WriteLine("403 Forbidden", "Server-Send");
                        this.SendError(HttpStatusCode.Forbidden);
                        continue;
                    }

                    // Get the requested/default file name from the Request-Uri
                    string localFile = this.GetLocalFile(requestTokens[1]);

                    // If no file exists then send 404 NotFound
                    if (localFile == string.Empty)
                    {
                        Debug.WriteLine("404 NotFound (File)", "Server-Send");
                        this.SendError(HttpStatusCode.NotFound);
                        continue;
                    }

                    // Get the local directory from the Request-Uri
                    string localDirectory = GetLocalDirectory(requestTokens[1]);

                    // If no directory exists then send 404 NotFound
                    if (localDirectory == string.Empty)
                    {
                        Debug.WriteLine("404 NotFound (Directory)", "Server-Send");
                        this.SendError(HttpStatusCode.NotFound);
                        continue;
                    }

                    // The physical path to the local file
                    string localPath = localDirectory + localFile;

                    if (File.Exists(localPath))
                    {
                        // Send then file 200 OK
                        Debug.WriteLine("200 OK", "Server-Send");
                        byte[] fileBuffer = File.ReadAllBytes(localPath);
                        string response = Encoding.UTF8.GetString(fileBuffer);
                        this.SendHeader(this.GetMimeType(localFile), fileBuffer.Length, HttpStatusCode.OK);

                        // Only send the file data if requested
                        if (requestMethod == "GET")
                        {
                            this.SendToBrowser(response);
                        }
                    }
                    else
                    {
                        // Anything else send a 404 NotFound
                        Debug.WriteLine("404 NotFound", "Server");
                        Debug.WriteLine("Requested: " + requestTokens[1], "Server");
                        Debug.WriteLine("Translated: " + localPath, "Server");
                        this.SendError(HttpStatusCode.NotFound);
                    }

                    this.socket.Close();
                }
            }
        }

        #endregion
    }
}