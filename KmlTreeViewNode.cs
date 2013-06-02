// <copyright file="KmlTreeViewNode.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-06</date>
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
namespace FC.GEPluginCtrls
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// Custom node class for the <see cref="KmlTreeView"/>
    /// </summary>
    [Serializable]
    public sealed class KmlTreeViewNode : TreeNode
    {
        /// <summary>
        /// Animation timer
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The available Image Icons for KmlTreeViewNodes
        /// </summary>
        private enum Icon
        {
            /// <summary>
            /// The Earth image icon
            /// </summary>
            Ge = 0,

            /// <summary>
            /// The Kml file image icon
            /// </summary>
            Kml = 1,

            /// <summary>
            /// The placemark/flag image icon
            /// </summary>
            Flag = 2,

            /// <summary>
            /// The Image overlay image icon
            /// </summary>
            Overlay = 3,

            /// <summary>
            /// The KmlPhotoOverlay image icon
            /// </summary>
            Photo = 4,

            /// <summary>
            /// The KmlTour image icon
            /// </summary>
            Tour = 5,

            /// <summary>
            /// The folder closed image icon
            /// </summary>
            FolderClosed = 6,

            /// <summary>
            /// The folder open image icon
            /// </summary>
            FolderOpen = 7,

            /// <summary>
            /// The link folder closed image icon
            /// </summary>
            LinkFolderClosed = 8,

            /// <summary>
            /// The link folder open image icon
            /// </summary>
            LinkFolderOpen = 9,

            /// <summary>
            /// The link folder state 0 image icon
            /// </summary>
            LinkFolderLoading0 = 10,

            /// <summary>
            /// The link folder state 1 image icon
            /// </summary>
            linkFolderLoading1 = 11,

            /// <summary>
            /// The link folder state 2 image icon
            /// </summary>
            LinkFolderLoading2 = 12,

            /// <summary>
            /// The link folder disconnected image icon
            /// </summary>
            LinkFolderClosedDisconected = 13
        }

        /// <summary>
        /// Initializes a new instance of the KmlTreeViewNode class.
        /// </summary>
        internal KmlTreeViewNode()
        {
            this.ApiObject = null;
            this.ApiType = ApiType.None;
            this.ImageKey = this.SelectedImageKey = "linkFolderClosed_0";
            this.Name = ApiType.None.ToString();
            this.StateImageIndex = 0;
            this.Text = "Loading...";
            this.IsLoading = false;
        }

        /// <summary>
        /// Initializes a new instance of the KmlTreeViewNode class.
        /// </summary>
        /// <param name="kmlFeature">A <paramref name="kmlFeature">KML feature</paramref> to base the tree node on</param>
        internal KmlTreeViewNode(dynamic kmlFeature)
            : this()
        {
            if (kmlFeature == null)
            {
                return;
            }

            this.ApiObject = kmlFeature;
            this.Refresh();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the value of the node tool tip text.
        /// </summary>
        public new string ToolTipText
        {
            get { return base.ToolTipText; }
            set { base.ToolTipText = TidyToolTip(value); }
        }

        /// <summary>
        /// Gets the underlying KML object that the node represents.
        /// </summary>
        public dynamic ApiObject { get; private set; }

        /// <summary>
        /// Gets a value indicating the Google API type (e.g. KmlPlacemark, KmlDocument)
        /// of the underlying <see cref="ApiObject">feature</see> the node represents.
        /// </summary>
        public ApiType ApiType { get; private set; }

        public new KmlTreeViewNode FirstNode
        {
            get
            {
                return (KmlTreeViewNode)base.FirstNode;
            }
        }

        public new KmlTreeViewNode Parent
        {
            get
            {
                return (KmlTreeViewNode)base.Parent;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets a value indicating the url, if any, of the data the node is based on
        /// </summary>
        internal string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the node is loading data
        /// </summary>
        internal bool IsLoading { get; set; }

        /// <summary>
        /// Gets the URL of the underlying KML object.
        /// </summary>
        /// <remarks>
        /// The URL is obtained via kmlHelpers which adds support for legacy KML specifications.
        /// </remarks>
        internal string KmlUrl
        {
            get
            {
                return KmlHelpers.GetUrl(this.ApiObject);
            }
        }

        /// <summary>
        /// Gets the list style of the underlying kml object. Default is ListItemStyle.Check
        /// </summary>
        internal ListItemStyle KmlListStyle
        {
            get
            {
                return this.ApiObject == null ? ListItemStyle.Check : KmlHelpers.GetListItemType(this.ApiObject);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is checked and the KML object is visible
        /// </summary>
        internal bool ApiObjectVisible
        {
            get
            {
                if (this.ApiObject != null)
                {
                    try
                    {
                        return Convert.ToBoolean(this.ApiObject.getVisibility());
                    }
                    catch (COMException)
                    {
                    }
                }

                return false;
            }

            set
            {
                if (this.ApiObject != null)
                {
                    try
                    {
                        this.ApiObject.setVisibility(Convert.ToUInt16(value));
                    }
                    catch (COMException)
                    {
                    }
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Sets the icon style for the node
        /// </summary>
        internal void SetStyle()
        {
            switch (this.ApiType)
            {
                case ApiType.KmlFolder:
                case ApiType.KmlDocument:
                    this.SetIcon(this.IsExpanded ? Icon.FolderOpen : Icon.FolderClosed);
                    return;

                case ApiType.KmlNetworkLink:
                    this.SetIcon(
                        this.IsExpanded ? Icon.LinkFolderOpen : Icon.LinkFolderClosed);
                    return;

                case ApiType.KmlPlacemark:
                    this.SetIcon(Icon.Flag);
                    return;

                case ApiType.KmlGroundOverlay:
                case ApiType.KmlScreenOverlay:
                    this.SetIcon(Icon.Overlay);
                    return;

                case ApiType.KmlPhotoOverlay:
                    this.SetIcon(Icon.Photo);
                    return;

                case ApiType.KmlTour:
                case ApiType.KmlLayer:
                    this.SetIcon(Icon.Tour);
                    return;
            }
        }

        /// <summary>
        /// Animates the icon style for the node.
        /// </summary>
        internal void Animate()
        {
            if (this.timer != null)
            {
                this.IsLoading = false;
                this.timer.Stop();
                this.timer.Dispose();
                return;
            }

            this.timer = new Timer { Interval = 500, Enabled = true };
            this.IsLoading = true;
            var icon = Icon.LinkFolderLoading2;
            this.timer.Tick += (o, e) =>
            {
                if (icon < Icon.LinkFolderLoading0)
                {
                    icon = Icon.LinkFolderLoading2;
                }

                this.SetIcon(icon--);
            };
        }

        /// <summary>
        /// see http://code.google.com/p/winforms-geplugin-control-library/issues/detail?id=66
        /// </summary>
        internal void Refresh()
        {
            if (this.ApiObject == null)
            {
                return;
            }

            try
            {
                this.Name = this.ApiObject.getId();
                this.ApiType = GEHelpers.GetApiType(this.ApiObject);
                this.Text = RemoveScrubbingString(this.ApiObject.getName());
                this.StateImageIndex = this.ApiObjectVisible ? 1 : 0;
                this.Checked = this.ApiObjectVisible;
                this.SetStyle();
            }
            catch (COMException cex)
            {
                Debug.WriteLine("Refresh" + cex.Message, "KmlTreeViewNode");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the <paramref name="icon"/> for the base ImageIndex and SelectedImageIndex.
        /// </summary>
        /// <param name="icon">
        /// The icon. 
        /// </param>
        private void SetIcon(Icon icon)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)icon;
        }

        /// <summary>
        /// Removes the API scrubbing string if present 
        /// and converts HTML entities.
        /// </summary>
        /// <param name="name">The string to clean</param>
        /// <returns>A string with out the API scrubbing string in it</returns>
        private static string RemoveScrubbingString(string name)
        {
            string result = Regex.Replace(name, @"<!--\s*Content-type: mhtml-die-die-die\s*-->", string.Empty);
            result = result.Replace("&#040;", "(");
            result = result.Replace("&#041;", ")");
            return result;
        }

        /// <summary>
        /// Clean any html and add line-breaks for use with tooltips.
        /// TODO : make this a lot better...
        /// </summary>
        /// <param name="source">a html string</param>
        /// <returns>plain text with line-breaks</returns>
        private static string TidyToolTip(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            foreach (char @let in source)
            {
                if (@let == '<')
                {
                    inside = true;
                    continue;
                }

                if (@let == '>')
                {
                    inside = false;
                    continue;
                }

                if (!inside)
                {
                    array[arrayIndex] = @let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }

        #endregion
    }
}