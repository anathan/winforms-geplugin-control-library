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
    /// adasdasd dsf sd fsd f sd fs
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

        /// <summary>
        /// Gets or sets a value indicating whether the server should keep listening
        /// </summary>
        public bool Listen
        {
            get
            {
                return this.keepListening;
            }

            set
            {
                this.keepListening = value;
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
            Debug.WriteLine("Running...", "Server");

            // start the listen thread
            Thread listenThread = new Thread(new ThreadStart(this.StartListen));
            listenThread.Start();

            this.keepListening = true;

            Debug.WriteLine("Start", "Server");
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            Debug.WriteLine("Stop!", "Server");
            this.keepListening = false;
        }

        #endregion

        #region Private methods

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
        /// This function send an Error Header to the client (Browser/GoogleEarth)
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
        /// This function send the Header Information to the client (Browser/GoogleEarth)
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
        /// Overloaded Function, takes string, convert to bytes and calls 
        /// overloaded sendToBrowserFunction.
        /// </summary>
        /// <param name="data">The data to be sent to google earth (client)</param>
        private void SendToBrowser(string data)
        {
            this.SendToBrowser(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sends data to the browser (client)
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
            string localDir;
            string physicalFilePath = string.Empty;
            string formattedMessage = string.Empty;
            string response = string.Empty;

            while (this.keepListening)
            {
                // Stops the call to AcceptSocket from blockin allowing the listenThread to teminate cleanly
                if (!this.tcpListener.Pending())
                {
                    Thread.Sleep(100);
                    continue;
                }
                else
                {
                    this.socket = this.tcpListener.AcceptSocket();
                }
                
                // Handle the connection
                if (this.socket.Connected)
                {
                    Debug.WriteLine("Connected: " + this.socket.RemoteEndPoint, "Server");

                    byte[] buffer = new byte[1024];
                    string data = string.Empty;

                    do
                    {
                        int read = this.socket.Receive(buffer, this.socket.Available, SocketFlags.None);
                        data += Encoding.UTF8.GetString(buffer, 0, read);
                    }
                    while (!data.EndsWith(Environment.NewLine + Environment.NewLine));

                    // Example request header from the plug-in
                    // GET /kml/wht-blank.png HTTP/1.1
                    // Accept: application/vnd.google-earth.kml+xml, application/vnd.google-earth.kmz, image/*, */*
                    // User-Agent: GoogleEarth/5.1.3509.4636(Windows;Microsoft Windows Vista (Service Pack 2);en-US;kml:2.2;client:Plus;type:plugin)
                    // Host: localhost:8080
                    // Connection: Keep-Alive
                    string requestHeader = string.Empty;
                    string acceptHeader = string.Empty;
                    string userAgentHeader = string.Empty;
                    string hostHeader = string.Empty;
                    string[] headers = data.Split('\n');

                    foreach (string line in headers)
                    {
                        if (line.StartsWith("GET"))
                        {
                            requestHeader = line;
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

                    // GET /kml/wht-blank.png HTTP/1.1
                    // RMethod [SP] Request-URI [SP] HTTP-Version [CRLF]
                    // retun if the request is invalid 
                    string[] requestTokens = requestHeader.Split(new char[] { ' ' });
                    if (requestTokens.Length != 3)
                    {
                        continue;
                    }

                    string requestMethod = requestTokens[0];
                    string requestUri = requestTokens[1];
                    string httpVersion = requestTokens[2]; // HTTP-Version HTTP/1.?
                    string requestedFile = Path.GetFileName(requestUri);
                    string requestedDirectory = Path.GetDirectoryName(requestUri);

                    if (requestedDirectory != null)
                    {
                        // remove any leading slash from the path
                        if (requestedDirectory.StartsWith("\\"))
                        {
                            requestedDirectory = requestedDirectory.Substring(1);
                        }

                        // add a trailing slash to the path if required
                        if (!requestedDirectory.EndsWith("\\"))
                        {
                            // add a trailing slash if needed
                            requestedDirectory += "\\";
                        }
                    }             

                    Debug.WriteLine(requestedDirectory, "requestedDirectory");
                    Debug.WriteLine(requestedFile, "requestedFile");

                    // GET only
                    if (requestMethod != "GET")
                    {
                        // 405 MethodNotAllowed
                        this.SendError(httpVersion, HttpStatusCode.MethodNotAllowed);
                        continue;
                    }

                    // GoogleEarth only
                    if (!userAgentHeader.StartsWith("User-Agent: GoogleEarth"))
                    {
                        Debug.WriteLine("403 Forbidden", "Server");
                        Debug.WriteLine(userAgentHeader, "Server");

                        this.SendError(httpVersion, HttpStatusCode.Forbidden);
                        continue;
                    }                

                    // Find the virtual directory
                    if (requestedDirectory == "\\")
                    {
                        localDir = this.rootDirectory;
                    }
                    else
                    {
                        localDir = this.RootDirectory + requestedDirectory;
                    }

                    if (localDir == string.Empty)
                    {
                        this.SendError(httpVersion, HttpStatusCode.NotFound);
                    }

                    if (requestedFile == string.Empty)
                    {
                        // Get the default filename
                        requestedFile = this.DefaultFileName;

                        if (requestedFile == string.Empty)
                        {
                            Debug.WriteLine("404 NotFound", "Server");
                            Debug.WriteLine("No defualt file specified", "Server");

                            this.SendError(httpVersion, HttpStatusCode.NotFound);
                        }
                    }

                    string mimeType = this.GetMimeType(requestedFile);
                    physicalFilePath = localDir + requestedFile;

                    if (File.Exists(physicalFilePath) == false)
                    {
                        Debug.WriteLine("404 NotFound", "Server");
                        Debug.WriteLine("File not found: " + physicalFilePath, "Server");
                        this.SendError(httpVersion, HttpStatusCode.NotFound);
                    }
                    else
                    {
                        Debug.WriteLine("200 OK " + physicalFilePath, "Server");

                        byte[] bytes = File.ReadAllBytes(physicalFilePath);
                        response += Encoding.ASCII.GetString(bytes);
                        this.SendHeader(httpVersion, mimeType, bytes.Length, HttpStatusCode.OK);
                        this.SendToBrowser(bytes);
                        this.socket.Close();
                    }
                }
            }
        }

        #endregion
    }
}