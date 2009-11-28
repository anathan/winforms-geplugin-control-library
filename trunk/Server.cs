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
    /// A simple HTTP server class to allow the use of local files
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
        /// </summary>
        private IPAddress localHost = IPAddress.Loopback;

        /// <summary>
        /// Server socket 
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Server port number
        /// </summary>
        private int port = 8080;

        /// <summary>
        /// Server root directory
        /// </summary>
        private string rootDirectory = @"\";

        /// <summary>
        /// Default file name
        /// </summary>
        private string defaultFileName = "default.kml";

        /// <summary>
        /// Keep listening
        /// </summary>
        private volatile bool keepListening = true;

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
            string directory = Path.GetDirectoryName(requestUri);

            if (null != directory)
            {
                // remove any leading slash from the path
                if (directory.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    directory.TrimStart(Path.DirectorySeparatorChar);
                }

                // add a trailing slash to the path if required
                if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    // add a trailing slash if needed
                    directory += Path.DirectorySeparatorChar;
                }
            }
             
            if (directory == Path.DirectorySeparatorChar.ToString())
            {
                directory = this.rootDirectory;
            }
            else
            {
                directory = this.RootDirectory + requestUri;
            }

            return directory;
        }

        /// <summary>
        /// Attempt to get the file name from a request uri
        /// </summary>
        /// <param name="requestUri">header Request-Uri</param>
        /// <returns>the filename, the default filename or and empty string</returns>
        private string GetLocalFile(string requestUri)
        {
            string fileName = Path.GetFileName(requestUri);

            // If a file is not specified 
            if (fileName == string.Empty)
            {
                // try the default filename
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
        /// <param name="version">The HTTP version</param>
        /// <param name="status">The Http Error Code</param>
        private void SendError(string version, HttpStatusCode status)
        {
            string code = string.Format("{0} {1}", (int)status, status);
            this.SendHeader(version, string.Empty, code.Length, status);
            this.SendToBrowser(code);
            this.socket.Close();
        }

        /// <summary>
        /// Sends Header Information to the client (GoogleEarth)
        /// </summary>
        /// <param name="version">The HTTP Version number</param>
        /// <param name="mime">Mime Type of the responce</param>
        /// <param name="bytes">Total Bytes to be sent in the body</param>
        /// <param name="code">The HTTP status code</param>
        private void SendHeader(string version, string mime, int bytes, HttpStatusCode code)
        {
            StringBuilder data = new StringBuilder();
            data.AppendFormat("{0} {1} {2}{3}", version, (int)code, code.ToString(), Environment.NewLine);
            data.AppendLine("Server: GEPluginCtrls-a");
            data.AppendFormat("Content-Type: {0}{1}", mime, Environment.NewLine);
            data.AppendLine("Accept-Ranges: bytes");
            data.AppendFormat("Content-Length: {0}{1}{2}", bytes, Environment.NewLine, Environment.NewLine);

            byte[] header = Encoding.UTF8.GetBytes(data.ToString());
            this.SendToBrowser(header);
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
                    Thread.Sleep(100);
                    continue;
                }
                else
                {
                    this.socket = this.tcpListener.AcceptSocket();
                }

                // Handle the incoming connection
                if (this.socket.Connected)
                {
                    Debug.WriteLine("Connected: " + this.socket.RemoteEndPoint, "Server-Info");
                    byte[] buffer = new byte[1024]; // 1KB
                    string data = string.Empty;

                    // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec5.html
                    // wait for the end of the header: CRLF CRLF 
                    do
                    {
                        int read = this.socket.Receive(buffer, this.socket.Available, SocketFlags.None);
                        data += Encoding.UTF8.GetString(buffer, 0, read);
                    }
                    while (!data.EndsWith(Environment.NewLine + Environment.NewLine));

                    // parse the header
                    string[] headerLines = data.Split(Environment.NewLine.ToCharArray());
                    string requestHeader = string.Empty;
                    string requestMethod = string.Empty;
                    string acceptHeader = string.Empty;
                    string userAgentHeader = string.Empty;
                    string hostHeader = string.Empty;
                    

                    foreach (string line in headerLines)
                    {
                        // Get and head only...
                        if (line.StartsWith("GET"))
                        {
                            Debug.WriteLine(line, "Server-Request");
                            requestHeader = line;
                            requestMethod = "GET";
                        }
                        else if (line.StartsWith("HEAD"))
                        {
                            Debug.WriteLine(line, "Server-Request");
                            requestHeader = line;
                            requestMethod = "HEAD";
                        }
                        else if (line.StartsWith("Accept:"))
                        {
                            acceptHeader = line;
                        }
                        else if (line.StartsWith("User-Agent"))
                        {
                            userAgentHeader = line;
                        }
                        else if (line.StartsWith("Host:"))
                        {
                            hostHeader = line;
                        }
                    }

                    // Request-Line
                    // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec5.html#sec5.1
                    // Method [SP] Request-URI [SP] HTTP-Version [CRLF]
                    // if there are missing parts then skip the request
                    string[] requestTokens = requestHeader.Split(' ');
                    if (requestTokens.Length != 3)
                    {
                        continue;
                    }

                    // Method
                    // Handle GET and HEAD requests otherwise send 501 NotImplemented
                    // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.3
                    // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.4
                    if (requestTokens[0] != "GET" && requestTokens[0] != "HEAD")
                    {
                        Debug.WriteLine("501 NotImplemented", "Server-Send");
                        this.SendError(requestTokens[2], HttpStatusCode.NotImplemented);
                        continue;
                    }

                    // HTTP-Version 
                    // Handle HTTP/1.1 otherwise send 505 HttpVersionNotSupported
                    if (requestTokens[2] != "HTTP/1.1")
                    {
                        Debug.WriteLine("505 HttpVersionNotSupported", "Server");
                        this.SendError(requestTokens[2], HttpStatusCode.HttpVersionNotSupported);
                        continue;
                    }

                    // User-Agent
                    // Handle GoogleEarth otherwise send 403 Forbidden
                    if (!userAgentHeader.StartsWith("User-Agent: GoogleEarth"))
                    {
                        Debug.WriteLine("403 Forbidden", "Server-Send");
                        this.SendError(requestTokens[2], HttpStatusCode.Forbidden);
                        continue;
                    }

                    // Get the requested/default file from the Request-Uri
                    string localFile = this.GetLocalFile(requestTokens[1]);

                    // If no file exists then send 404 NotFound
                    if (localFile == string.Empty)
                    {
                        Debug.WriteLine("404 NotFound (File)", "Server-Send");
                        this.SendError(requestTokens[2], HttpStatusCode.NotFound);
                        continue;
                    }

                    // Get the local directory file from the Request-Uri
                    string localDirectory = GetLocalDirectory(requestTokens[1]);

                    // If no directory exists then send 404 NotFound
                    if (localDirectory == string.Empty)
                    {
                        Debug.WriteLine("404 NotFound (Directory)", "Server-Send");
                        this.SendError("HTTP/1.1", HttpStatusCode.NotFound);
                        continue;
                    }

                    // The physical path to the local file
                    string localPath = localDirectory + localFile;

                    if (File.Exists(localPath))
                    {
                        // Send then file 200 OK
                        Debug.WriteLine("200 OK", "Server-Send");
                        Debug.WriteLine("Requested: " + requestTokens[1], "Server");
                        Debug.WriteLine("Translated: " + localPath, "Server");
                        byte[] bytes = File.ReadAllBytes(localPath);
                        string response = Encoding.UTF8.GetString(bytes);
                        this.SendHeader(requestTokens[2], this.GetMimeType(localFile), bytes.Length, HttpStatusCode.OK);
                        if (requestMethod == "GET")
                        {
                            this.SendToBrowser(bytes);
                        }

                        this.socket.Close();
                    }
                    else
                    {
                        // Send a 404 NotFound
                        Debug.WriteLine("404 NotFound", "Server");
                        Debug.WriteLine("Requested: " + requestTokens[1], "Server");
                        Debug.WriteLine("Translated: " + localPath, "Server");
                        this.SendError(requestTokens[2], HttpStatusCode.NotFound);
                    }
                }
            }
        }

        #endregion
    }
}