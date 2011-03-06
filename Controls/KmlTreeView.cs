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
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
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
        private int balloonMinimumWidth = 0;

        /// <summary>
        /// The minimum height of any balloons triggered from the treeview
        /// </summary>
        private int balloonMinimumHeight = 0;

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

        /// <summary>
        /// Indicates if the treeview should use unsafe html balloons when node is clicked
        /// </summary>
        private bool useUnsafeHtmlBalloons = false;

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
        /// Gets or sets the minimum height of any balloons triggered from the control
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum height of any balloons triggered from the control. Default 0"),
        DefaultValueAttribute(0)]
        public int BalloonMinimumHeight
        {
            get { return this.balloonMinimumHeight; }
            set { this.balloonMinimumHeight = value; }
        }

        /// <summary>
        /// Gets or sets the minimum width of any balloons triggered from the control
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum width of any balloons triggered from the control. Default 0"),
        DefaultValueAttribute(0)]
        public int BalloonMinimumWidth
        {
            get { return this.balloonMinimumWidth; }
            set { this.balloonMinimumWidth = value; }
        }

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

        /// <summary>
        /// Gets or sets a value indicating whether to use unsafe html balloons (if any) when
        /// feature represented by the treenode is double clicked
        /// The default setting is false
        /// </summary>
        [Category("Control Options"),
        Description("Specifies if the plugin should use unsafe html balloons when opening balloons. Default false"),
        DefaultValueAttribute(false)]
        public bool UseUnsafeHtmlBalloons
        {
            get { return this.useUnsafeHtmlBalloons; }
            set { this.useUnsafeHtmlBalloons = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Recursively parses a kml object into the tree
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        public void ParseKmlObject(dynamic kmlObject)
        {
            object kml = kmlObject;
            Task.Factory.StartNew(() => this.ObjectParser(kml), TaskCreationOptions.None);
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

        /// <summary>
        /// Set the browser instance for the control to work with
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance</param>
        public void SetBrowserInstance(GEWebBrowser browser)
        {
            this.gewb = browser;
            this.geplugin = browser.GetPlugin();

            if (!GEHelpers.IsGe(this.geplugin))
            {
                throw new ApplicationException("ge is not of the type GEPlugin");
            }

            this.htmlDocument = browser.Document;
            this.Nodes.Clear();
            this.Enabled = true;
        }

        #endregion

        #region Private methods

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
                treenode.ToolTipText = this.StripHTML(kmlFeature.getDescription());

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
                Debug.WriteLine("CreateTreeNodeFromKmlFolder: " + ex.ToString(), "KmlTreeView");
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
            // Create a simple node 
            TreeNode node = new TreeNode();
            node.Text = kmlObject.getName();
            node.ToolTipText = this.StripHTML(kmlObject.getDescription());
            node.Tag = kmlObject;
  
            // TODO some image key for networkinks - node.ImageKey = "folderLinkClosed";

            // Kml documents using the pre 2.1 spec may contain the <Url> element 
            // in these cases the getHref call will return null 
            string url = KmlHelpers.GetUrl(kmlObject);

            // if that didn't work we can try getUrl()
            if (url == string.Empty)
            {
                try
                {
                    url = kmlObject.getUrl();
                }
                catch (RuntimeBinderException)
                {
                }

                // and if that didn't work we can try getLink().getHref()
                if (url == string.Empty)
                {
                    try
                    {
                        url = kmlObject.getLink().getHref();
                    }
                    catch (RuntimeBinderException)
                    {
                    }
                }
            }
            
            // getComputedStyle is not part of the current Api and has issues.
            // if the call fails we manualy create a tree node for the link.
            try
            {
                dynamic obj = this.gewb.FetchKmlSynchronous(url);

                if (obj != null)
                {
                    if (obj.getOwnerDocument() != null)
                    {
                        int listItemType = obj.getOwnerDocument().getComputedStyle().getListStyle().getListItemType();

                        if (listItemType == this.geplugin.LIST_ITEM_CHECK_HIDE_CHILDREN)
                        {
                            // The childnodes are hidden so we create a basic feature node.
                            return this.CreateTreeNodeFromKmlFeature(obj);
                        }
                        else
                        {
                            // Create a temporary subnode, when this is expaneded 
                            // we will call ParseKmlContainer on the kmlObject held in the 
                            // tag property 
                            TreeNode nl = new TreeNode(obj.getName());
                            nl.Tag = obj;
                            nl.ToolTipText = this.StripHTML(obj.getDescription());
                            node.Nodes.Add(nl);
                            return node;
                        }
                    }
                }
            }
            catch (RuntimeBinderException)
            {
            }

            // if all else has failed we return the simple treenode...
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
                    dynamic obj = child.Tag as dynamic;

                    if (obj != null)
                    {
                        obj.setVisibility(1);
                    }

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
                dynamic obj = treeNode.Parent.Tag as dynamic;

                if (obj != null)
                {
                    obj.setVisibility(1);
                }

                this.CheckAllParentNodes(treeNode.Parent);
            }
        }

        /// <summary>
        /// Recursively parses a kml object into the tree
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        private void ObjectParser(dynamic kmlObject)
        {
            if (null != kmlObject)
            {
                string type = string.Empty;

                try
                {
                    type = kmlObject.getType();
                }
                catch (RuntimeBinderException ex)
                {
                    Debug.WriteLine("ParsekmlObject: " + ex.ToString(), "KmlTreeView");
                    return;
                }

                switch (type)
                {
                    case "KmlDocument":
                    case "KmlFolder":
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Nodes.Add(this.CreateTreeNodeFromKmlFolder(kmlObject));
                                this.Update();
                            });
                        }
                        else
                        {
                            this.Nodes.Add(this.CreateTreeNodeFromKmlFolder(kmlObject));
                            this.Update();
                        }

                        break;
                    case "KmlNetworkLink":
                        string name = kmlObject.getName();

                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Nodes.Add(this.CreateTreeNodeFromKmlNetworkLink(kmlObject));
                                this.Update();
                            });
                        }
                        else
                        {
                            this.Nodes.Add(this.CreateTreeNodeFromKmlNetworkLink(kmlObject));
                            this.Update();
                        }

                        break;
                    case "KmlGroundOverlay":
                    case "KmlScreenOverlay":
                    case "KmlPlacemark":
                    case "KmlTour":
                    case "KmlPhotoOverlay":
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Nodes.Add(this.CreateTreeNodeFromKmlFeature(kmlObject));
                                this.Update();
                            });
                        }
                        else
                        {
                            this.Nodes.Add(this.CreateTreeNodeFromKmlFeature(kmlObject));
                            this.Update();
                        }

                        break;
                    default:
                        break;
                }
            }
        }

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
            }

            return parentNode;
        }

        /// <summary>
        /// Clean any html and add linebreaks for use with tooltips.
        /// </summary>
        /// <param name="html">a html string</param>
        /// <returns>plain text with linebreaks</returns>
        private string StripHTML(string html)
        {
            System.Text.RegularExpressions.Regex reg =
                new System.Text.RegularExpressions.Regex(
                    "<[^>]+>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            html = reg.Replace(html, Environment.NewLine);

            return html.Replace(Environment.NewLine + Environment.NewLine, string.Empty);
        }

        /// <summary>
        /// Sets the checked state of any child nodes to false
        /// </summary>
        /// <param name="treeNode">The starting node to check from</param>
        private void UncheckAllChildNodes(TreeNode treeNode)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = false;
                if (node.Nodes.Count > 0)
                {
                    this.UncheckAllChildNodes(node);
                }
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

            string type = feature.getType();

            if (feature != null && type != string.Empty)
            {
                feature.setVisibility(Convert.ToInt16(e.Node.Checked));
            
                if (e.Node.Checked)
                {
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
                    else if ("KmlNetworkLink" == type)
                    {
                        this.geplugin.getFeatures().appendChild(feature);
                    }
                }
                else
                {
                    if (e.Action != TreeViewAction.Unknown)
                    {
                        this.UncheckAllChildNodes(e.Node);
                    }

                    if ("KmlTour" == type)
                    {
                        this.geplugin.getTourPlayer().setTour(null);
                    }
                    else if ("KmlPhotoOverlay" == type)
                    {
                        this.geplugin.getPhotoOverlayViewer().setPhotoOverlay(null);
                    }
                    else if ("KmlNetworkLink" == type)
                    {
                        this.geplugin.getFeatures().removeChild(feature);
                    }
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
                    case "KmlNetworkLink":
                        // handle networklink recursion here...?
                        TreeNode subnode = e.Node.Nodes[0];
                        e.Node.Nodes.Add(this.ParseKmlContainer(subnode.Tag));
                        e.Node.Nodes.Remove(subnode);

                        break;
                    default:
                        break;
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
                                if (this.useUnsafeHtmlBalloons)
                                {
                                    GEHelpers.OpenBalloonHtmlUnsafe(
                                        this.geplugin,
                                        feature,
                                        this.balloonMinimumWidth,
                                        this.balloonMinimumHeight);
                                }
                                else
                                {
                                    GEHelpers.OpenFeatureBalloon(
                                        this.geplugin,
                                        feature,
                                        this.balloonMinimumWidth,
                                        this.balloonMinimumHeight);
                                }
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
                        GEHelpers.LookAt(feature, this.gewb);
                    }
                }
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