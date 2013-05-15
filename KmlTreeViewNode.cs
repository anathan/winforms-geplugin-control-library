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
    using System.Windows.Forms;

    /// <summary>
    /// Custom node class for the <see cref="KmlTreeView"/>
    /// </summary>
    [Serializable]
    public sealed class KmlTreeViewNode : TreeNode
    {
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
                if (this.ApiObject != null)
                {
                    return KmlHelpers.GetUrl(this.ApiObject).ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the list style of the underlying kml object. Default is ListItemStyle.Check
        /// </summary>
        internal ListItemStyle KmlListStyle
        {
            get
            {
                if (this.ApiObject != null)
                {
                    return KmlHelpers.GetListItemType(this.ApiObject);
                }

                return ListItemStyle.Check;
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
                case ApiType.KmlDocument:
                case ApiType.KmlFolder:
                    {
                        if (this.IsExpanded)
                        {
                            this.ImageKey = "folderOpen";
                            this.SelectedImageKey = "folderOpen";
                        }
                        else
                        {
                            this.ImageKey = "folderClosed";
                            this.SelectedImageKey = "folderClosed";
                        }
                    }

                    return;

                case ApiType.KmlNetworkLink:
                    {
                        if (this.IsExpanded)
                        {
                            this.ImageKey = "linkFolderOpen";
                            this.SelectedImageKey = "linkFolderOpen";
                        }
                        else
                        {
                            this.ImageKey = "linkFolderClosed";
                            this.SelectedImageKey = "linkFolderClosed";
                        }
                    }

                    return;

                case ApiType.KmlPlacemark:
                    {
                        this.ImageKey = "flag";
                        this.SelectedImageKey = "flag";
                    }

                    return;

                case ApiType.KmlGroundOverlay:
                case ApiType.KmlScreenOverlay:
                    {
                        this.ImageKey = "overlay";
                        this.SelectedImageKey = "overlay";
                    }

                    return;

                case ApiType.KmlPhotoOverlay:
                    {
                        this.ImageKey = "photo";
                        this.SelectedImageKey = "photo";
                    }

                    return;

                case ApiType.KmlTour:
                    {
                        this.ImageKey = "tour";
                        this.SelectedImageKey = "tour";
                    }

                    return;

                case ApiType.KmlLayer:
                    {
                        this.ImageKey = "tour";
                        this.SelectedImageKey = "tour";
                    }

                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Animates the icon style for the node.
        /// </summary>
        internal void Animate()
        {
            this.IsLoading = true;

            Timer t = new Timer { Interval = 500, Enabled = true };
            int i = 2;

            t.Tick += (o, e) =>
            {
                if (i >= 0)
                {
                    this.ImageKey = "linkFolderClosed_" + i;
                    this.SelectedImageKey = "linkFolderClosed_" + i;
                    i -= 1;
                }
                else
                {
                    i = 2;
                    this.ImageKey = "linkFolderClosed_" + i;
                    this.SelectedImageKey = "linkFolderClosed_" + i;
                }
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
                this.Text = this.ApiObject.getName();
                this.ApiObjectVisible = Convert.ToBoolean(this.ApiObject.getVisibility());
                this.StateImageIndex = this.ApiObjectVisible ? 1 : 0;
                this.Checked = this.ApiObjectVisible;
                this.SetStyle();
            }
            catch (COMException cex)
            {
                Debug.WriteLine(cex, "Refresh");
            }
        }

        #endregion

        #region Private Methods

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