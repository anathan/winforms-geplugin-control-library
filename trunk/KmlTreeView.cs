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
    using System.Drawing;
    using System.Windows.Forms;
    using GEPlugin;

    /// <summary>
    /// The KmlTree view provides a quick way to display kml content
    /// </summary>
    public partial class KmlTreeView : TreeView, IGEControls
    {
        #region Private fields

        /// <summary>
        /// The plugin
        /// </summary>
        private IGEPlugin geplugin = null;

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
        }

        /// <summary>
        /// Recursively parses a kml object into the tree
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        public void ParsekmlObject(object kmlObject)
        {
            IKmlObject obj = kmlObject as IKmlObject;

            if (null != obj)
            {
                string type = obj.getType();

                switch (type)
                {
                    case "KmlDocument":
                    case "KmlFolder":
                        this.Nodes.Add(
                            this.ParsekmlContainer((IKmlContainer)obj));
                        break;
                    case "KmlNetworkLink":
                    case "KmlGroundOverlay":
                    case "KmlScreenOverlay":
                    case "KmlPlacemark":
                    case "KmlTour":
                    case "KmlPhotoOverlay":
                        this.Nodes.Add(
                            this.CreateTreeNodeFromKmlFeature((IKmlFeature)obj));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Recursively parses a collection of kml object into the tree
        /// </summary>
        /// <param name="kmlObjects">The kml objects to parse</param>
        public void ParsekmlObject(object[] kmlObjects)
        {
            foreach (object kmlObject in kmlObjects)
            {
                this.ParsekmlObject(kmlObject);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Recursivly iterates through a Kml Container
        /// Adding any child features to the tree
        /// </summary>
        /// <param name="kmlContainer">The object to parse</param>
        /// <returns>The current tree node</returns>
        private TreeNode ParsekmlContainer(IKmlContainer kmlContainer)
        {
            TreeNode tn = this.CreateTreeNodeFromKmlFeature((IKmlFeature)kmlContainer);
            TreeNode node;

            if (Convert.ToBoolean(kmlContainer.getFeatures().hasChildNodes()))
            {
                IKmlObjectList subNodes = kmlContainer.getFeatures().getChildNodes();
                for (int i = 0; i < subNodes.getLength(); i++)
                {
                    IKmlObject subNode = subNodes.item(i);
                    switch (subNode.getType())
                    {
                        case "KmlDocument":
                        case "KmlFolder":
                            node = this.ParsekmlContainer((IKmlContainer)subNode);
                            break;
                        default:
                            node = this.CreateTreeNodeFromKmlFeature((IKmlFeature)subNode);
                            break;
                    }

                    tn.Nodes.Add(node);
                }
            }

            return tn;
        }

        /// <summary>
        /// Creates a tree node from a Kml Feature
        /// </summary>
        /// <param name="kmlFeature">The feature to add</param>
        /// <returns>The created tree node of the feature</returns>
        private TreeNode CreateTreeNodeFromKmlFeature(IKmlFeature kmlFeature)
        {
            string type = kmlFeature.getType();

            TreeNode tn = new TreeNode();
            tn.Text = kmlFeature.getName();
            tn.Tag = kmlFeature;
            tn.Name = type;
            tn.ToolTipText = this.ShortenToolTip(kmlFeature.getDescription());

            if (Convert.ToBoolean(kmlFeature.getOpen()))
            {
                tn.Expand();
            }

            if (Convert.ToBoolean(kmlFeature.getVisibility()))
            {
                tn.Checked = true;

                if (this.expandVisibleFeatures)
                {
                    tn.Expand();
                }
            }

            switch (type)
            {
                case "KmlDocument":
                case "KmlFolder":
                    tn.ImageKey = "folderClosed";
                    tn.SelectedImageKey = "folderClosed";
                    break;
                case "KmlPlacemark":
                    tn.ImageKey = "flag";
                    tn.SelectedImageKey = "flag";
                    break;
                case "KmlGroundOverlay":
                case "KmlScreenOverlay":
                    tn.ImageKey = "overlay";
                    tn.SelectedImageKey = "overlay";
                    break;
                case "KmlPhotoOverlay":
                    tn.ImageKey = "photo";
                    tn.SelectedImageKey = "photo";
                    break;
                case "KmlTour":
                    tn.ImageKey = "tour";
                    tn.SelectedImageKey = "tour";
                    break;
                default:
                    break;
            }

            return tn;
        }

        /// <summary>
        /// Sets the checked state of any parent nodes to true
        /// </summary>
        /// <param name="tn">The tree node to check from</param>
        private void CheckAllParentNodes(TreeNode tn)
        {
            if (tn.Parent != null)
            {
                tn.Parent.Checked = true;
                this.CheckAllParentNodes(tn.Parent);
            }
        }

        /// <summary>
        /// Sets the checked state of any child nodes to false
        /// </summary>
        /// <param name="tn">The tree node to check from</param>
        private void UncheckAllChildNodes(TreeNode tn)
        {
            if (tn.Nodes.Count > 0)
            {
                foreach (TreeNode child in tn.Nodes)
                {
                    child.Checked = false;
                    this.UncheckAllChildNodes(child);
                }
            }
        }

        /// <summary>
        /// Trucates a any string over 200 chars
        /// Appends an ellipsis (...)
        /// </summary>
        /// <param name="text">The text to truncated</param>
        /// <returns>The truncated text</returns>
        private string ShortenToolTip(string text)
        {
            if (text.Length > 200)
            {
                return text.Substring(0, 200) + "...";
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
            IKmlFeature feature = e.Node.Tag as IKmlFeature;

            string type = feature.getType();

            if (feature != null)
            {
                if (e.Node.Checked)
                {
                    feature.setVisibility(1);
                    this.CheckAllParentNodes(e.Node);

                    if ("KmlTour" == type)
                    {
                        geplugin.getTourPlayer().setTour(feature);
                    }
                }
                else
                {
                    feature.setVisibility(0);
                    this.UncheckAllChildNodes(e.Node);

                    if ("KmlTour" == type)
                    {
                        geplugin.getTourPlayer().setTour(null);
                    }
                    else if ("KmlPhotoOverlay" == type)
                    {
                        geplugin.getPhotoOverlayViewer().setPhotoOverlay(null);
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
                IKmlFeature feature = SelectedNode.Tag as IKmlFeature;

                this.SelectedNode.Checked = true;

                if (null != feature)
                {
                    string type = feature.getType();

                    switch (type)
                    {
                        case "KmlPlacemark":
                            if (this.openBalloonOnDoubleClickNode)
                            {
                                IGEFeatureBalloon balloon = this.geplugin.createFeatureBalloon(String.Empty);
                                balloon.setMinHeight(this.balloonMinimumHeight);
                                balloon.setMinWidth(this.balloonMinimumWidth);
                                balloon.setFeature(feature);
                                this.geplugin.setBalloon(balloon);           
                            }
                            break;
                        case "KmlTour":
                            geplugin.getTourPlayer().setTour(feature);
                            geplugin.getTourPlayer().play();
                            break;
                        case "KmlPhotoOverlay":
                            geplugin.getPhotoOverlayViewer().setPhotoOverlay(feature);
                            break;
                        default:
                            break;
                    }

                    if (this.flyToOnDoubleClickNode)
                    {
                        GEHelpers.LookAt(this.geplugin, feature);
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
            IKmlFeature feature = e.Node.Tag as IKmlFeature;

            if (null != feature)
            {
                string type = feature.getType();

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
                IKmlFeature feature = (IKmlFeature)e.Node.Tag;
                if (feature != null)
                {
                    switch (feature.getType())
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
                System.Diagnostics.Debug.WriteLine(icex.ToString());
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