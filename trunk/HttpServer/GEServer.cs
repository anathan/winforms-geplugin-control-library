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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple HTTP server class to allow the use of local files in the Google Earth Plugin
    /// </summary>
    public class GEServer
    {
        #region Private fields

        /// <summary>
        /// Thread signal
        /// </summary>
        private static readonly ManualResetEvent ResetEvent = new ManualResetEvent(false);

        #endregion

        /// <summary>
        /// Initializes a new instance of the GEServer class.
        /// </summary>
        public GEServer() :
            this(IPAddress.Loopback, 8080, Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the GEServer class.
        /// </summary>
        /// <param name="ip">the server ip address</param>
        /// <param name="port">the server port</param>
        /// <param name="rootDirectory">the server root directory</param>
        public GEServer(IPAddress ip, int port, string rootDirectory)
        {
            this.IPAddress = ip;
            this.Port = port;
            RootDirectory = rootDirectory;
        }

        #region Public properites

        /// <summary>
        /// Gets or sets the root server directory (webroot) 
        /// http://localhost:port/ will point to this directory
        /// </summary>
        public static string RootDirectory { get; set; }

        /// <summary>
        /// Gets the current base URL (protocol, IP, port) of the server, e.g. "http://127.0.0.1:8080/"
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
        /// The default value is "default. "
        /// </summary>
        public string DefaultFileName { get; set; }

        /// <summary>
        /// Gets or sets the IP Address for the server to use 
        /// The default is 127.0.0.1 (localhost/loopback address)
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the port for the server to use
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Private properites

        /// <summary>
        /// Gets or sets a value indicating whether the server should accept new clients
        /// </summary>
        private bool AcceptClients { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            this.AcceptClients = true;
            Task.Factory.StartNew(this.Accept);
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            this.AcceptClients = false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Asynchronously accepts an incoming connection attempt and on a socket.
        /// Begins to asynchronously receive data from a connected socket.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation..</param>
        private static void AcceptCallback(IAsyncResult result)
        {
            // Signal the main thread to continue.
            ResetEvent.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);

            // Create the state object.
            StateObject state = new StateObject { Socket = handler };
            handler.BeginReceive(
                state.Buffer,
                0,
                StateObject.BufferSize,
                0,
                ReadCallback,
                state);
        }

        /// <summary>
        /// Builds the HTTP response headers
        /// </summary>
        /// <param name="mime">mime type for the response</param>
        /// <param name="bytes">size of the response in bytes</param>
        /// <param name="code">the status code for the response (100, 200, 404, etc)</param>
        /// <returns>A formatted HTTP response header</returns>
        private static string BuildResponseHeader(string mime, int bytes, HttpStatusCode code)
        {
            StringBuilder data = new StringBuilder();

            string httpDate = DateTime.Now.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture);
            string status = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}",
                (int)code,
                code.ToString());

            // Status-Line
            // HTTP-Version SP Status-Code SP Reason-Phrase CRLF
            // See http://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html#sec6.1
            data.AppendFormat(
                CultureInfo.InvariantCulture,
                "{0} {1}{2}",
                "HTTP/1.1",
                status,
                Environment.NewLine);
            data.AppendFormat("Date: {0}{1}", httpDate, Environment.NewLine);
            data.AppendLine("Accept-Ranges: bytes");
            data.AppendLine("Connection: Keep-Alive");
            data.AppendLine("Expires: Thu, 01 Jan 1970 00:00:00 GMT");
            data.AppendLine("Cache-Control: no-cache");
            data.AppendLine("Pragma: no-cache");
            data.AppendFormat("Content-Length: {0}{1}", bytes, Environment.NewLine);
            data.AppendFormat("Content-Type: {0}{1}", mime, Environment.NewLine);
            ////data.AppendFormat("Host: {0}{1}", BaseUrl , Environment.NewLine);
            data.AppendLine();
            return data.ToString();
        }

        /// <summary>
        /// Ends a pending asynchronous read and checks for a HTTP request header.
        /// If it is a file request the file (if any) is served.
        /// </summary>
        /// <param name="result">Represents the status of an asynchronous operation.</param>
        private static void ReadCallback(IAsyncResult result)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)result.AsyncState;
            Socket handler = state.Socket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(result);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.Data.Append(new UTF8Encoding(false).GetString(state.Buffer, 0, bytesRead));
                string rawData = state.Data.ToString();

                // check for end-of-header (CRLFCRLF)
                if (rawData.EndsWith(
                    string.Format(CultureInfo.CurrentCulture, "{0}{0}", Environment.NewLine),
                    StringComparison.OrdinalIgnoreCase))
                {
                    HttpRequest request = new HttpRequest(rawData); // TODO: handle errors here
                    string filePath = TranslatePath(request.Uri);
                    SendFile(filePath, handler);
                }
                else
                {
                    // Send interim 100 Continue
                    Send(
                        handler,
                        BuildResponseHeader("text/plain; charset=UTF-8", 0, HttpStatusCode.Continue));

                    // carry on reading...
                    handler.BeginReceive(
                        state.Buffer,
                        0,
                        StateObject.BufferSize,
                        0,
                        ReadCallback,
                        state);
                }
            }
        }

        /// <summary>
        /// Encodes the specified string data in to bytes and sends it asynchronously to the specified socket.
        /// </summary>
        /// <param name="handler">the specified socket</param>
        /// <param name="data">string data to send</param>
        private static void Send(Socket handler, string data)
        {
            Send(handler, Encoding.ASCII.GetBytes(data));
        }

        /// <summary>
        /// Sends the specified byte data asynchronously to the specified socket.
        /// </summary>
        /// <param name="handler">the specified socket</param>
        /// <param name="data">byte data to send</param>
        private static void Send(Socket handler, byte[] data)
        {
            handler.BeginSend(data, 0, data.Length, 0, SendCallback, handler);
        }

        /// <summary>
        /// Ends a pending asynchronous data send.
        /// </summary>
        /// <param name="result">The status of the operation</param>
        private static void SendCallback(IAsyncResult result)
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)result.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                handler.EndSend(result);
                ////Debug.WriteLine("Sent {0} bytes to client.", bytesSent); 
            }
            catch (SocketException sex)
            {
                Debug.WriteLine("SendCallback: " + sex, "Server-Error");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

/*
        /// <summary>
        /// Encodes an HTTP Error and sends the data asynchronously to the specified socket.
        /// The plain-text error is also sent, e.g: 404 NotFound
        /// </summary>
        /// <param name="handler">The specified socket</param>
        /// <param name="status">The Http Error Code</param>
        private static void SendError(Socket handler, HttpStatusCode status)
        {
            string code = string.Format(CultureInfo.InvariantCulture, "{0} {1}", (int)status, status);
            Send(handler, BuildResponseHeader("text/plain; charset=UTF-8", code.Length, status));
            Send(handler, code);
        }
*/

        /// <summary>
        /// Attempts to asynchronously serve a file on the specified socket.
        /// Sends a HTTP 404 error on the socket if the file is not found.
        /// </summary>
        /// <param name="filePath">The local path of a file</param>
        /// <param name="handler">The socket to send the file data on</param>
        private static void SendFile(string filePath, Socket handler)
        {
            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);

                // 200 OK
                string header = BuildResponseHeader(GetMimeType(filePath), data.Length, HttpStatusCode.OK);
                Send(handler, header);

                StateObject state = new StateObject { Socket = handler };
                state.Data.Append(filePath);
                handler.BeginSendFile(filePath, SendFileCallback, state);
            }
            else
            {
                // 404 NotFound
                string code = (int)HttpStatusCode.NotFound + " " + HttpStatusCode.NotFound;
                Send(handler, BuildResponseHeader("text/plain; charset=UTF-8", code.Length, HttpStatusCode.NotFound));
                Send(handler, code);
            }
        }

        /// <summary>
        /// Ends a pending asynchronous file send.
        /// </summary>
        /// <param name="result">The status of the operation</param>
        private static void SendFileCallback(IAsyncResult result)
        {
            // Retrieve the socket from the state object.
            StateObject state = (StateObject)result.AsyncState;
            Socket handler = state.Socket;

            try
            {
                // Complete sending the file to the socket.
                handler.EndSendFile(result);
                ResetEvent.Set();

                Debug.WriteLine(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Sent {0} to {1}",
                    state.Data,
                    handler.RemoteEndPoint),
                    "GEServer");
            }
            catch (SocketException)
            {
                ////Debug.WriteLine("SendFileCallback: Forcibly closed...", "Server-Error");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        /// <summary>
        /// Gets the mime type for the given file type
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <returns>the mime type of the file or "application/unknown"</returns>
        private static string GetMimeType(string path)
        {
            string extension = string.Empty;

            try
            {
                extension = Path.GetExtension(path);
            }
            catch (ArgumentException aex)
            {
                Debug.WriteLine("GetMimeType: " + aex, "Server-Error");
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
                    // see http://www.iana.org/assignments/media-types/application/vnd.google-earth.kml+xml
                    return "application/vnd.google-earth.kml+xml";
                case ".kmz":
                    // see http://www.iana.org/assignments/media-types/application/vnd.google-earth.kmz
                    return "application/vnd.google-earth.kmz";
                case ".png":
                    return "image/png";
                case ".txt":
                    return "text/plain";
                case ".dae":
                    // see http://www.iana.org/assignments/media-types/model/vnd.collada+xml
                    return "model/vnd.collada+xml";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Translates a virtual path (uri) into a local file path
        /// </summary>
        /// <param name="requestUri">the uri for the file</param>
        /// <returns>the local file path for the uri</returns>
        private static string TranslatePath(string requestUri)
        {
            string directory = Path.GetDirectoryName(requestUri.Replace("/", Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)));
            string fileName = Path.GetFileName(requestUri);

            if (!string.IsNullOrEmpty(directory))
            {
                // remove any leading slash from the path
                if (directory.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                {
                    directory = directory.TrimStart(Path.DirectorySeparatorChar);
                }

                // add a trailing slash to the path if required
                if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                {
                    directory += Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "default.kml";
            }

            // builds the local file path and removes any double slashes e.g. 
            // \root\dir\file.kml
            string path = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}{3}",
                RootDirectory,
                Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
                directory,
                fileName).Replace(
                Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture) +
                Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
                Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));

            return path;
        }

        /// <summary>
        /// Sets up a socket bound to the IPAddress and Port then places it in the listening state. 
        /// Finally it begins an asynchronous operation to accept an incoming connection attempt on the socket.
        /// </summary>
        private void Accept()
        {
            // Data buffer for incoming data.
            ////byte[] bytes = new byte[StateObject.BufferSize];

            // Establish the local endpoint for the socket.
            IPEndPoint localEndPoint = new IPEndPoint(this.IPAddress, this.Port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (this.AcceptClients)
                {
                    // Set the event to nonsignaled state.
                    ResetEvent.Reset();

                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(AcceptCallback, listener);

                    // Wait until a connection is made before continuing.
                    ResetEvent.WaitOne();
                }
            }
            catch (SocketException sex)
            {
                Debug.WriteLine("Accept: " + sex, "GEServer");
            }
            finally
            {
                listener.Dispose();
            }
        }

        #endregion
    }
}