// <copyright file="KMLTreeView.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
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
namespace FC.GEPluginCtrls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// The KmlTree view provides a quick way to display kml content
    /// </summary>
    public partial class KmlTreeView : TreeView, IGEControls
    {
        #region Private fields

        /// <summary>
        /// The plugin
        /// </summary>
        private dynamic geplugin = null;

        /// <summary>
        /// The current document
        /// </summary>
        private HtmlDocument htmlDocument = null;

        /// <summary>
        /// The current browser
        /// </summary>
        private GEWebBrowser gewb = null;

        /// <summary>
        /// The minimum width of any balloons triggered from the treeview
        /// </summary>
        private int balloonMinimumWidth = 200;

        /// <summary>
        /// The minimum height of any balloons triggered from the treeview
        /// </summary>
        private int balloonMinimumHeight = 200;

        /// <summary>
        /// Indicates if the tree view should expand all visible feature nodes
        /// when parsing kml
        /// </summary>
        private bool expandVisibleFeatures = false;

        /// <summary>
        /// Indicates if the plugin should 'fly to' the location
        /// when a tree node is double clicked
        /// </summary>
        private bool flyToOnDoubleClickNode = true;

        /// <summary>
        /// Indicates if the plugin should open the balloon
        /// when a tree node is double clicked
        /// </summary>
        private bool openBalloonOnDoubleClickNode = true;

        /// <summary>
        /// Indicates if the treeview should check all child nodes
        /// when a parent tree node is checked
        /// </summary>
        private bool checkAllChildren = true;

        #endregion

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        public KmlTreeView()
            : base()
        {
            this.InitializeComponent();
            this.ShowNodeToolTips = true;
            this.ShowLines = false;
        }

        #region Public properties
        
        /// <summary>
        /// Gets or sets a value indicating whether the treeview should check all child nodes
        /// when a parent tree node is checked
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the treeview should check all child nodes. Default True"),
        DefaultValueAttribute(true)]
        public bool CheckAllChildrenOnParentChecked
        {
            get 
            {
                return this.checkAllChildren; 
            }

            set
            { 
                this.checkAllChildren = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum width of any balloons triggered from the control
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum width of any balloons triggered from the control. Default 250"),
        DefaultValueAttribute(250)]
        public int BalloonMinimumWidth
        {
            get { return this.balloonMinimumWidth; }
            set { this.balloonMinimumWidth = value; }
        }

        /// <summary>
        /// Gets or sets the minimum height of any balloons triggered from the control
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum height of any balloons triggered from the control. Default 100"),
        DefaultValueAttribute(100)]
        public int BalloonMinimumHeight
        {
            get { return this.balloonMinimumHeight; }
            set { this.balloonMinimumHeight = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to expand the tree node
        /// of features with the visible element set to true
        /// The default setting is false
        /// </summary>
        [Category("Control Options"),
        Description("Specifies if the treeview should expand visible feature nodes when they are loaded. Default false"),
        DefaultValueAttribute(false)]
        public bool ExpandVisibleFeatures
        {
            get { return this.expandVisibleFeatures; }
            set { this.expandVisibleFeatures = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin should 'fly to' the location (if any) of the
        /// feature represented by the treenode
        /// The default setting is true
        /// </summary>
        [Category("Control Options"),
        Description("Specifies if the plugin should 'fly to' the location of the feature on double click. Default true"),
        DefaultValueAttribute(true)]
        public bool FlyToOnDoubleClickNode
        {
            get { return this.flyToOnDoubleClickNode; }
            set { this.flyToOnDoubleClickNode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to open the balloon (if any) when
        /// feature represented by the treenode is double clicked
        /// The default setting is true
        /// </summary>
        [Category("Control Options"),
        Description("Specifies if the plugin should open the feature balloon on double click. Default true"),
        DefaultValueAttribute(true)]
        public bool OpenBalloonOnDoubleClickNode
        {
            get { return this.openBalloonOnDoubleClickNode; }
            set { this.openBalloonOnDoubleClickNode = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance</param>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewb = browser;
            this.geplugin = browser.GetPlugin();
            this.htmlDocument = browser.Document;
            this.Nodes.Clear();
            this.Enabled = true;
        }

        /// <summary>
        /// Recursively parses a kml object into the tree
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        public void ParseKmlObject(dynamic kmlObject)
        {
            if (null != kmlObject)
            {
                string type = string.Empty;

                try
                {
                    type = kmlObject.getType();

                    switch (type)
                    {
                        case "KmlDocument":
                        case "KmlFolder":
                            this.Nodes.Add(
                                this.CreateTreeNodeFromKmlFolder(kmlObject));
                            break;
                        case "KmlNetworkLink":
                            this.Nodes.Add(
                                this.CreateTreeNodeFromKmlNetworkLink(kmlObject));
                            break;
                        case "KmlGroundOverlay":
                        case "KmlScreenOverlay":
                        case "KmlPlacemark":
                        case "KmlTour":
                        case "KmlPhotoOverlay":
                            this.Nodes.Add(
                                this.CreateTreeNodeFromKmlFeature(kmlObject));
                            break;
                        default:
                            break;
                    }
                }
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine("ParsekmlObject: " + ex.ToString(), "KmlTreeView");
                    ////throw;
                }
            }
        }

        /// <summary>
        /// Recursively parses a collection of kml object into the tree
        /// </summary>
        /// <param name="kmlObjects">The kml objects to parse</param>
        public void ParseKmlObject(object[] kmlObjects)
        {
            foreach (object kmlObject in kmlObjects)
            {
                this.ParseKmlObject(kmlObject);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Recursively iterates through a Kml Container adding any child features to the tree
        /// </summary>
        /// <param name="kmlContainer">The object to parse</param>
        /// <returns>The current tree node</returns>
        private TreeNode ParseKmlContainer(dynamic kmlContainer)
        {
            TreeNode parentNode = this.CreateTreeNodeFromKmlFeature(kmlContainer);

            try
            {
                if (Convert.ToBoolean(kmlContainer.getFeatures().hasChildNodes()))
                {
                    TreeNode childNode = new TreeNode();

                    dynamic subNodes = kmlContainer.getFeatures().getChildNodes();

                    for (int i = 0; i < subNodes.getLength(); i++)
                    {
                        dynamic subNode = subNodes.item(i);
                        string type = subNode.getType();

                        switch (type)
                        {
                            // features that implement the IkmlContainer interface
                            case "KmlDocument":
                            case "KmlFolder":
                                childNode = this.CreateTreeNodeFromKmlFolder(subNode);
                                break;

                            // network links
                            case "KmlNetworkLink":
                                childNode = this.CreateTreeNodeFromKmlNetworkLink(subNode);
                                break;

                            // all other features
                            default:
                                childNode = this.CreateTreeNodeFromKmlFeature(subNode);
                                break;
                        }

                        parentNode.Nodes.Add(childNode);
                    }
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("ParsekmlContainer: " + ex.ToString(), "KmlTreeView");
                ////throw;
            }

            return parentNode;
        }

        /// <summary>
        /// Creates a tree node from a Kml Feature
        /// </summary>
        /// <param name="kmlFeature">The kml feature to add</param>
        /// <returns>The tree node for the feature</returns>
        private TreeNode CreateTreeNodeFromKmlFeature(dynamic kmlFeature)
        {
            TreeNode treenode = new TreeNode();

            string type = string.Empty;

            try
            {
                type = kmlFeature.getType();

                treenode.Text = kmlFeature.getName();
                treenode.Tag = kmlFeature;
                treenode.Name = type;

                // TODO: and length as property
                treenode.ToolTipText = this.ShortenToolTip(kmlFeature.getDescription(), 200);

                if (Convert.ToBoolean(kmlFeature.getOpen()))
                {
                    treenode.Expand();
                }

                if (Convert.ToBoolean(kmlFeature.getVisibility()))
                {
                    treenode.Checked = true;

                    if (this.expandVisibleFeatures)
                    {
                        treenode.Expand();
                    }
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine("CreateTreeNodeFromKmlFeature:" + ex.ToString(), "KmlTreeView");
                ////throw;
            }

            switch (type)
            {
                case "KmlDocument":
                case "KmlFolder":
                    treenode.ImageKey = "folderClosed";
                    treenode.SelectedImageKey = "folderClosed";
                    break;
                case "KmlPlacemark":
                    treenode.ImageKey = "flag";
                    treenode.SelectedImageKey = "flag";
                    break;
                case "KmlGroundOverlay":
                case "KmlScreenOverlay":
                    treenode.ImageKey = "overlay";
                    treenode.SelectedImageKey = "overlay";
                    break;
                case "KmlPhotoOverlay":
                    treenode.ImageKey = "photo";
                    treenode.SelectedImageKey = "photo";
                    break;
                case "KmlTour":
                    treenode.ImageKey = "tour";
                    treenode.SelectedImageKey = "tour";
                    break;
                default:
                    break;
            }

            return treenode;
        }

        /// <summary>
        /// Adds parent features accept ListStyle checkHideChildren property
        /// </summary>
        /// <param name="kmlObject">The kml folder object</param>
        /// <returns>The tree node for the folder</returns>
        private TreeNode CreateTreeNodeFromKmlFolder(dynamic kmlObject)
        {
            try
            {
                // getComputedStyle is not part of the current Api and has issues.
                // if the call fails we default to parsing the kml container
                if (kmlObject.getOwnerDocument() != null &&
                    kmlObject.getOwnerDocument().getComputedStyle().getListStyle().getListItemType() ==
                    this.geplugin.LIST_ITEM_CHECK_HIDE_CHILDREN)
                {
                    return this.CreateTreeNodeFromKmlFeature(kmlObject);
                }
                else
                {
                    return this.ParseKmlContainer(kmlObject);
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine(ex.ToString(), "KmlTreeView");
            }

            return this.ParseKmlContainer(kmlObject);
        }

        /// <summary>
        /// Download KmlObject from Networklink then expand.
        /// </summary>
        /// <param name="kmlObject">The network link object</param>
        /// <returns>The tree node for the networklink</returns>
        private TreeNode CreateTreeNodeFromKmlNetworkLink(dynamic kmlObject)
        {    
            string url = string.Empty;

            // Kml documents using the pre 2.1 spec may contain the <Url> element 
            // in these cases the getHref call will return null
            try
            {
                url = kmlObject.getLink().getHref();
            }
            catch (NullReferenceException)
            {
                url = kmlObject.GetUrl();
            }

            // getComputedStyle is not part of the current Api and has issues.
            // if the call fails we manualy create a tree node for the link
            try
            {
                dynamic obj = this.gewb.FetchKmlSynchronous(url);

                if (obj.getOwnerDocument() != null &&
                    obj.getOwnerDocument().getComputedStyle().getListStyle().getListItemType() ==
                    this.geplugin.LIST_ITEM_CHECK_HIDE_CHILDREN)
                {
                    return this.CreateTreeNodeFromKmlFeature(obj);
                }
                else
                {
                    return this.ParseKmlContainer(obj);
                }
            }
            catch (NullReferenceException)
            {
            }

            // return a simple treenode...
            ////TreeNode node = new TreeNode(kmlObject.getType());
            TreeNode node = new TreeNode(kmlObject.getName());
            node.Tag = kmlObject;
            return node;
        }

        /// <summary>
        /// Sets the checked state of any child nodes to true
        /// </summary>
        /// <param name="treeNode">The starting node to check from</param>
        private void CheckAllChildNodes(TreeNode treeNode)
        {
            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode child in treeNode.Nodes)
                {
                    child.Checked = true;
                    this.CheckAllChildNodes(child);
                }
            }
        }

        /// <summary>
        /// Sets the checked state of any parent nodes to true
        /// </summary>
        /// <param name="treeNode">The starting node to check from</param>
        private void CheckAllParentNodes(TreeNode treeNode)
        {
            if (treeNode.Parent != null)
            {
                treeNode.Parent.Checked = true;
                this.CheckAllParentNodes(treeNode.Parent);
            }
        }

        /// <summary>
        /// Sets the checked state of any child nodes to false
        /// </summary>
        /// <param name="treeNode">The starting node to check from</param>
        private void UncheckAllChildNodes(TreeNode treeNode)
        {
            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode child in treeNode.Nodes)
                {
                    child.Checked = false;
                    this.UncheckAllChildNodes(child);
                }
            }
        }

        /// <summary>
        /// Trucates a any string over the given number of chars
        /// Appends an ellipsis (...)
        /// </summary>
        /// <param name="text">The text to truncated</param>
        /// <param name="length">The maximum string length </param>
        /// <returns>The truncated text</returns>
        private string ShortenToolTip(string text, int length)
        {
            if (text.Length > length)
            {
                return text.Substring(0, length) + "...";
            }
            else
            {
                return text;
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when a tree node is checked
        /// </summary>
        /// <param name="sender">A tree node</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            dynamic feature = e.Node.Tag;
            string type = string.Empty;

            try
            {
                type = GEHelpers.GetTypeFromRcw(feature);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine(ex.ToString(), "KmlTree_AfterCheck");
                ////throw;
            }

            if (feature != null && type != string.Empty)
            {
                if (e.Node.Checked)
                {
                    feature.setVisibility(1);

                    if (e.Action != TreeViewAction.Unknown)
                    {
                        if (this.checkAllChildren)
                        {
                            this.CheckAllChildNodes(e.Node);
                        }

                        this.CheckAllParentNodes(e.Node);
                    }

                    if ("KmlTour" == type)
                    {
                       this.geplugin.getTourPlayer().setTour(feature);
                    }
                }
                else
                {
                    feature.setVisibility(0);
                    this.UncheckAllChildNodes(e.Node);

                    if ("KmlTour" == type)
                    {
                        this.geplugin.getTourPlayer().setTour(null);
                    }
                    else if ("KmlPhotoOverlay" == type)
                    {
                        this.geplugin.getPhotoOverlayViewer().setPhotoOverlay(null);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a tree node is double clicked
        /// </summary>
        /// <param name="sender">The TreeView</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTree_DoubleClick(object sender, EventArgs e)
        {
            if (this.SelectedNode != null)
            {
                dynamic feature = SelectedNode.Tag;
                string type = string.Empty;

                this.SelectedNode.Checked = true;

                if (null != feature)
                {
                    try
                    {
                        type = feature.getType();
                    }
                    catch (RuntimeBinderException ex)
                    {
                        Debug.WriteLine(ex.ToString(), "KmlTree_DoubleClick");
                        ////throw;
                    }

                    switch (type)
                    {
                        case "KmlPlacemark":
                            if (this.openBalloonOnDoubleClickNode)
                            {
                                dynamic balloon = this.geplugin.createFeatureBalloon(String.Empty);
                                balloon.setMinHeight(this.balloonMinimumHeight);
                                balloon.setMinWidth(this.balloonMinimumWidth);
                                balloon.setFeature(feature);
                                this.geplugin.setBalloon(balloon);
                            }

                            break;
                        case "KmlTour":
                            this.geplugin.getTourPlayer().setTour(feature);
                            this.geplugin.getTourPlayer().play();
                            break;
                        case "KmlPhotoOverlay":
                            this.geplugin.getPhotoOverlayViewer().setPhotoOverlay(feature);
                            break;
                        default:
                            break;
                    }

                    if (this.flyToOnDoubleClickNode)
                    {
                        GEHelpers.LookAt(this.geplugin, feature, this.gewb);
                    }
                }
            }
        }

        /// <summary>
        /// Called after a tree node has expanded
        /// </summary>
        /// <param name="sender">The TreeView</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            dynamic feature = e.Node.Tag;
            string type = string.Empty;

            if (null != feature)
            {
                try
                {
                    type = feature.getType();
                }
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine(ex.ToString(), "KmlTreeView_AfterExpand");
                    ////throw;
                }

                switch (type)
                {
                    case "KmlDocument":
                    case "KmlFolder":
                        e.Node.ImageKey = "folderOpen";
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Called after a tree node has collapsed
        /// </summary>
        /// <param name="sender">The TreeView</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            try
            {
                dynamic feature = e.Node.Tag;
                if (feature != null)
                {
                    string type = GEHelpers.GetTypeFromRcw(feature);

                    switch (type)
                    {
                        case "KmlDocument":
                        case "KmlFolder":
                            e.Node.ImageKey = "folderClosed";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (InvalidCastException icex)
            {
                Debug.WriteLine(icex.ToString(), "KmlTreeView");
                ////throw;
            }
        }

        /// <summary>
        /// Called when the user clicks on the control (TODO)
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The Event arguments</param>
        private void KmlTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = this.GetNodeAt(p);

                if (node != null)
                {
                    // holder for the cuurent node
                    ////currentNode = this.SelectedNode;

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    this.SelectedNode = node;

                    // Highlight the selected node.
                    ////this.SelectedNode = currentNode;
                    ////currentNode = null;
                }
            }
        }

        #endregion
    }
}