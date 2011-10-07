// <copyright file="GEServer.cs" company="FC">
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
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple HTTP server class to allow the use of local files in the Google Earth Plugin
    /// </summary>
    public sealed class GEServer : IDisposable
    {
        #region Private fields

        /// <summary>
        /// The Server HTTP version
        /// </summary>
        private readonly string httpVersion = "HTTP/1.1";

        /// <summary>
        /// Server tcp listner
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// Server socket 
        /// </summary>
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Default file name to look for in a directory
        /// </summary>
        private string defaultFileName = "default.kml";

        /// <summary>
        /// Keep listening switch
        /// </summary>
        private volatile bool keepListening = true;

        /// <summary>
        /// Task for listining to incomming connections
        /// </summary>
        private Task listenTask = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEServer class.
        /// </summary>
        public GEServer()
        {
            this.Port = 8080;
            this.RootDirectory = @"\";
            this.defaultFileName = "default.kml";
            this.IPAddress = IPAddress.Loopback;
        }

        /// <summary>
        /// Initializes a new instance of the GEServer class.
        /// </summary>
        /// <param name="rootDirectory">Server root directory</param>
        /// <param name="defaultFileName">Default file name to look for in a directory</param>
        /// <param name="ip">Server ip address</param>
        /// <param name="port">Server port number</param>
        public GEServer(
            string rootDirectory = @"\",
            string defaultFileName = "default.kml",
            IPAddress ip = null,
            int port = 8080)
        {
            this.Port = port;
            this.RootDirectory = rootDirectory;
            this.defaultFileName = defaultFileName;

            // setting a tcp port of zero (0) will find
            // first available port and pass it back as the assigned port.
            // see: http://code.google.com/p/winforms-geplugin-control-library/issues/detail?id=27
            if (this.Port == 0)
            {
                this.Port = ((IPEndPoint)this.tcpListener.LocalEndpoint).Port;
            }

            if (ip == null)
            {
                this.IPAddress = IPAddress.Loopback;
            }
            else
            {
                this.IPAddress = ip;
            }
        }

        #region Public properites

        /// <summary>
        /// Gets the current base url (protocol, ip, port) of the server, e.g. "http://127.0.0.1:8080/"
        /// This address will point at the <see cref="RootDirectory"/> of the sever.
        /// </summary>
        public Uri BaseUrl
        {
            get
            {
                return new Uri(string.Format(
                    CultureInfo.InvariantCulture,
                    "http://{0}:{1}/",
                    this.IPAddress,
                    this.Port));
            }
        }

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

                this.DefaultFileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the IP Address for the server to use 
        /// The default is 127.0.0.1 (localhost/loopback)
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the root server directory (webroot) 
        /// http://localhost:port/ will point to this directory
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the port for the server to use
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Public methods
        
        /// <summary>
        /// Dispose of managed resources 
        /// </summary>
        public void Dispose()
        {
            this.socket.Dispose();
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            // start listing on the given ip address and port
            this.tcpListener = new TcpListener(this.IPAddress, this.Port);
            this.tcpListener.Start();

            // start the listen thread
            this.listenTask = Task.Factory.StartNew(
                () =>
                this.Listen(),
                TaskCreationOptions.LongRunning);

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
        /// <returns>The local/root directory path</returns>
        private string GetLocalDirectory(string requestUri)
        {
            string directory = Path.GetDirectoryName(requestUri.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (null != directory)
            {
                // remove any leading slash from the path
                if (directory.StartsWith(
                    Path.DirectorySeparatorChar.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    directory = directory.TrimStart(Path.DirectorySeparatorChar);
                }

                // add a trailing slash to the path if required
                if (!directory.EndsWith(
                    Path.DirectorySeparatorChar.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    directory += Path.DirectorySeparatorChar;
                }
            }

            if (directory == Path.DirectorySeparatorChar.ToString())
            {
                directory = this.RootDirectory + Path.DirectorySeparatorChar.ToString();
            }
            else
            {
                directory = this.RootDirectory + Path.DirectorySeparatorChar.ToString() + directory;
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
            if (string.IsNullOrEmpty(fileName))
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
                Debug.WriteLine("GetMimeType: " + aex.ToString(), "Server-Error");
                this.SendError(HttpStatusCode.InternalServerError);
                return string.Empty;
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
                    this.SendError(HttpStatusCode.UnsupportedMediaType);
                    return string.Empty;
            }
        }

        /// <summary>
        /// Checks the raw http reqest header and converts it to 
        /// a <see cref="HttpRequest"/> object. 
        /// </summary>
        /// <param name="data">a raw http header string</param>
        /// <returns>a <see cref="HttpRequest"/> object based on the raw data</returns>
        private HttpRequest PraseRawRequest(string data)
        {
            // split the request by CRLF into lines
            string[] headerLines = data.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            HttpRequest request = new HttpRequest();

            // Find the headers we are interested in from the request
            foreach (string line in headerLines)
            {
                // Get and head only...
                if (line.StartsWith("GET", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine(line, "Server-Request");
                    request.ReqestTokens = line.Split(' ');
                    request.Method = "GET";
                }
                else if (line.StartsWith("HEAD", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine(line, "Server-Request");
                    request.ReqestTokens = line.Split(' ');
                    request.Method = "HEAD";
                }
                else if (line.StartsWith("User-Agent:", StringComparison.OrdinalIgnoreCase))
                {
                    request.UserAgent = line;
                }
                else if (line.StartsWith("Host:", StringComparison.OrdinalIgnoreCase))
                {
                    request.HostHeader = line;
                }
            }

            // There must be three tokens: Method, Request-URI and HTTP-Version
            if (request.ReqestTokens.Length != 3)
            {
                Debug.WriteLine("400 Bad Request", "Server-Error");
                this.SendError(HttpStatusCode.BadRequest);
            }
            else
            {
                // we use the request uri to translate the local file path later...
                request.Uri = request.ReqestTokens[1];
            }

            // Handle GET and HEAD requests otherwise send 501 NotImplemented
            // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.3
            // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.4
            if (request.Method != "GET" && request.Method != "HEAD")
            {
                Debug.WriteLine("501 Not Implemented", "Server-Error");
                this.SendError(HttpStatusCode.NotImplemented);
            }

            // HTTP-Version 
            // Handle version 1.1 otherwise send 505 HttpVersionNotSupported
            if (request.ReqestTokens[2] != this.httpVersion)
            {
                Debug.WriteLine("505 Http Version Not Supported", "Server-Error");
                this.SendError(HttpStatusCode.HttpVersionNotSupported);
            }

            // Host
            // Reject any requset that does not have a host header
            if (string.IsNullOrEmpty(request.HostHeader))
            {
                Debug.WriteLine("400 Bad Request", "Server-Error");
                this.SendError(HttpStatusCode.BadRequest);
            }

            // User-Agent
            // Handle GoogleEarth otherwise send 403 Forbidden
            if (!request.UserAgent.StartsWith("User-Agent: GoogleEarth", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("403 Forbidden", "Server-Error");
                this.SendError(HttpStatusCode.Forbidden);
            }

            return request;
        }

        /// <summary>
        /// Receive raw data from the socket until encountering CRLF CRLF
        /// </summary>
        /// <returns>The raw http reqest header as a string</returns>
        private string ReceiveHeader()
        {
            byte[] buffer = new byte[1024]; // 1KB
            string data = string.Empty;

            // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec5.html
            // wait for the end of the header: CRLF CRLF 
            try
            {
                do
                {
                    int read = this.socket.Receive(buffer, this.socket.Available, SocketFlags.None);
                    data += Encoding.UTF8.GetString(buffer, 0, read);
                }
                while (!data.EndsWith(
                    Environment.NewLine + Environment.NewLine,
                    StringComparison.OrdinalIgnoreCase));
            }
            catch (SocketException sex)
            {
                Debug.WriteLine("StartListen" + sex.ToString(), "Server-Error");
                this.SendError(HttpStatusCode.InternalServerError);
            }

            return data;
        }

        /// <summary>
        /// Sends an Error Header to the client
        /// </summary>
        /// <param name="status">The Http Error Code</param>
        private void SendError(HttpStatusCode status)
        {
            string code = string.Format(CultureInfo.InvariantCulture, "{0} {1}", (int)status, status);
            this.SendHeader("text/plain; charset=UTF-8", code.Length, status);
            this.SendToClient(code);
            this.socket.Disconnect(true);
        }

        /// <summary>
        /// Sends Header Information to the client (GoogleEarth)
        /// See http://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html
        /// </summary>
        /// <param name="mime">MMIE (type) for the responce</param>
        /// <param name="bytes">Total Bytes to be sent in the body</param>
        /// <param name="code">The HTTP status code</param>
        private void SendHeader(string mime, int bytes, HttpStatusCode code)
        {
            string httpDate = DateTime.Now.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture);
            StringBuilder data = new StringBuilder();

            // Status-Line
            // HTTP-Version SP Status-Code SP Reason-Phrase CRLF
            // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html#sec6.1
            data.AppendFormat(
                CultureInfo.InvariantCulture,
                "{0} {1} {2}{3}",
                this.httpVersion,
                (int)code,
                code.ToString(),
                Environment.NewLine);
            
            data.AppendFormat("Date: {0}{1}", httpDate, Environment.NewLine);
            data.AppendLine("Accept-Ranges: bytes");
            data.AppendFormat("Content-Length: {0}{1}", bytes, Environment.NewLine);
            data.AppendFormat("Content-Type: {0}{1}", mime, Environment.NewLine);
            data.AppendLine("Connection: close");
            data.AppendLine();

            this.SendToClient(data.ToString());
        }

        /// <summary>
        /// Sends body data to the client (GoogleEarth)
        /// </summary>
        /// <param name="data">The data to be sent to google earth (client)</param>
        private void SendToClient(string data)
        {
            this.SendToClient(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Sends data to the client (Browser/GoogleEarth)
        /// </summary>
        /// <param name="data">The data to be sent to google earth (client)</param>
        private void SendToClient(byte[] data)
        {
            int numBytes = 0;

            try
            {
                if (this.socket.Connected)
                {
                    // send the data
                    if ((numBytes = this.socket.Send(data, data.Length, SocketFlags.None)) == -1)
                    {
                        Debug.WriteLine("Send Packet Error", "Server-Error");
                    }
                }
                else
                {
                    Debug.WriteLine("Connection Dropped...", "Server-Info");
                }
            }
            catch (SocketException sex)
            {
                Debug.WriteLine("sendToBrowser: " + sex.ToString(), "Server-Error");
            }
        }

        /// <summary>
        /// Serves a file based on the given requset uri string
        /// </summary>
        /// <param name="requestUri">the file Uri as a string</param>
        private void ServeFile(string requestUri)
        {
            string localFile = this.GetLocalFile(requestUri);
            string localDirectory = this.GetLocalDirectory(requestUri);
            string localPath = localDirectory + localFile;

            // If no file or directory exists then send 404 NotFound
            if (string.IsNullOrEmpty(localDirectory))
            {
                Debug.WriteLine("404 Not Found (File/Directory missing)", "Server-Error");
                Debug.WriteLine("Translated path: " + localDirectory + localFile, "Server-Info");
                this.SendError(HttpStatusCode.NotFound);
            }

            if (File.Exists(localPath))
            {
                // Send then file 200 OK
                byte[] bytes = File.ReadAllBytes(localPath);
                string response = Encoding.UTF8.GetString(bytes);
                string mimeType = this.GetMimeType(localPath);

                Debug.WriteLine("200 OK", "Server-Response");

                this.SendHeader(mimeType, bytes.Length, HttpStatusCode.OK);
                this.SendToClient(bytes);
            }
            else
            {
                // Send a 404 NotFound
                Debug.WriteLine("404 Not Found", "Server-Error");
                Debug.WriteLine("Requested path: " + requestUri, "Server-Info");
                Debug.WriteLine("Translated path: " + localPath, "Server-Info");
                this.SendError(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Listen for incoming connections
        /// </summary>
        private void Listen()
        {
            while (this.keepListening)
            {
                // Stops the call to AcceptSocket from blocking
                // this allows the listenThread to teminate cleanly
                if (!this.tcpListener.Pending())
                {
                    try
                    {
                        this.listenTask.Wait(100);
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
                else
                {
                    this.socket = this.tcpListener.AcceptSocket();
                }

                // Handle the incoming connection
                if (this.socket.Connected)
                {
                    Debug.WriteLine("Connected: " + this.socket.RemoteEndPoint, "Server-Info");
                    HttpRequest request = this.PraseRawRequest(this.ReceiveHeader());
                    this.ServeFile(request.Uri);
                }
            }
        }

        #endregion
    }
}