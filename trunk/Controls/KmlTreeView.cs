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
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// The KmlTree view provides a quick way to display kml content
    /// </summary>
    public sealed partial class KmlTreeView : TreeView, IGEControls
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

        #endregion

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        public KmlTreeView()
            : base()
        {
            this.BalloonMinimumHeight = 10;
            this.BalloonMinimumWidth = 10;
            this.CheckAllChildren = true;
            this.ExpandNetworkLinkContentOnLoad = true;
            this.FlyToOnDoubleClickNode = true;
            this.OpenBalloonsOnDoubleClick = true;
            this.UseUnsafeHtmlBalloons = false;

            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance to work with</param>
        /// <remarks>Equivalent to initializing then calling <see cref="SetBrowserInstance"/></remarks>
        public KmlTreeView(GEWebBrowser browser)
            : this()
        {
            this.SetBrowserInstance(browser);
        }

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        /// <param name="browser">The GEWebBrowser instance to work with</param>
        /// <param name="kmlObject">A kml object to parse into the tree</param>
        /// <remarks>
        /// Equivalent to initializing, calling <see cref="SetBrowserInstance"/>
        /// then calling ParseKmlObject 
        /// </remarks>
        public KmlTreeView(GEWebBrowser browser, dynamic kmlObject)
            : this(browser)
        {
            this.SetBrowserInstance(browser);

            if (kmlObject != null)
            {
                this.ParseKmlObject(kmlObject);
            }
        }

        #region Public properties

        #region Control Properties

        /// <summary>
        /// Gets or sets the minimum height of any balloons triggered from the control
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum height of any balloons triggered from the control. Default 10"),
        DefaultValueAttribute(10)]
        public int BalloonMinimumHeight { get; set; }

        /// <summary>
        /// Gets or sets the minimum width of any balloons triggered from the control
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum width of any balloons triggered from the control. Default 10"),
        DefaultValueAttribute(10)]
        public int BalloonMinimumWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the treeview should check all children when a parent is checked. Default True
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the treeview should check all children when a parent is checked. Default True"),
        DefaultValueAttribute(true)]
        public bool CheckAllChildren { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to expand the tree node based on the kml object's visiblity.
        /// If true and the kml object is visible the node expands on construction.
        /// The default setting is false.
        /// <remarks>Please note network links have a further setting to allow this behavior <see cref="ExpandVisibleNetworkLinks"/></remarks>
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the treeview should expand visible features when they are loaded. Default false"),
        DefaultValueAttribute(false)]
        public bool ExpandVisibleFeatures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to expand the tree nodes based on the networklink's visiblity.
        /// If true, and the networklink is visible (and <see cref="ExpandVisibleFeatures"/> is set to true)
        /// the node expands on construction.
        /// The default setting is false.
        /// <remarks>
        /// Setting this to true can cause the tree to take a long time to load a large chains of networklinks
        /// if they themselves are set to visible.
        /// </remarks>
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the treeview should expand visible networklinks when they are loaded. Default false"),
        DefaultValueAttribute(false)]
        public bool ExpandVisibleNetworkLinks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the treeview should expand to show the contents of networklinks 
        /// when they are loaded. Default true.
        /// <remarks>
        /// When set to true the tree gives better visual feedback when links have finished loading.
        /// </remarks>
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the treeview should expand to show the contents of networklinks when they are loaded. Default true"),
        DefaultValueAttribute(true)]
        public bool ExpandNetworkLinkContentOnLoad { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin should 'fly to' the location (if any) of the
        /// feature represented by the treenode
        /// The default setting is true
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the plugin should 'fly to' the location of the feature on double click. Default true"),
        DefaultValueAttribute(true)]
        public bool FlyToOnDoubleClickNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to open the balloon (if any) when
        /// feature represented by the treenode is double clicked
        /// The default setting is true
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the plugin should open the feature balloon on double click. Default true"),
        DefaultValueAttribute(true)]
        public bool OpenBalloonsOnDoubleClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use unsafe html balloons (if any) when
        /// feature represented by the treenode is double clicked
        /// The default setting is false
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether the plugin should use unsafe html balloons when opening balloons. Default false"),
        DefaultValueAttribute(false)]
        public bool UseUnsafeHtmlBalloons { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether nodes should use the kml descripton for their tooltip text
        /// The default setting is false and the snippet is used instead.
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether nodes should use the kml descripton for their tooltip text. By default the snippet is used"),
        DefaultValueAttribute(false)]
        public bool UseDescriptionsForToolTips { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ToolTips are show for a node. By default the snippet is used if turned on. Default is true
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets a value indicating whether ToolTips are show for a node. By default the snippet is used if turned on. Default is true"),
        DefaultValueAttribute(true)]
        public new bool ShowNodeToolTips
        {
            get { return base.ShowNodeToolTips; }
            set { base.ShowNodeToolTips = value; }
        }

        #endregion

        #region Hidden Properties

        /// <summary>
        /// Gets or sets a value indicating whether the control can accept data that
        /// the user drags onto it.
        /// </summary>
        [Browsable(false)]
        public new bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether check boxes are displayed next to
        /// the tree nodes in the tree view control.
        /// </summary>
        /// <remarks>Checkboxes are used to control the visibility of the kml feature they represent</remarks>
        [Browsable(false)]
        public new bool CheckBoxes
        {
            get { return base.CheckBoxes; }
            set { base.CheckBoxes = value; }
        }

        /// <summary>
        /// Gets or sets the context menu strip for the control
        /// </summary>
        /// <remarks>TODO: context menu for networklinks, etc</remarks>
        [Browsable(false)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }

        /// <summary>
        /// Gets or sets the image-list index value of the default image that is displayed
        /// by the tree nodes.
        /// </summary>
        [Browsable(false)]
        public new int ImageIndex
        {
            get { return base.ImageIndex; }
            set { base.ImageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the ImageList that contains the icons used by the tree nodes.
        /// </summary>
        [Browsable(false)]
        public new ImageList ImageList
        {
            get { return base.ImageList; }
            set { base.ImageList = value; }
        }

        /// <summary>
        /// Gets or sets the key of the default image shoe when a node is in an unselected state.
        /// </summary>
        [Browsable(false)]
        public new string ImageKey
        {
            get { return base.ImageKey; }
            set { base.ImageKey = value; }
        }

        /// <summary>
        /// Gets or sets the key of the default image shown when a node is in a selected state.
        /// </summary>
        [Browsable(false)]
        public new string SelectedImageKey
        {
            get { return base.SelectedImageKey; }
            set { base.SelectedImageKey = value; }
        }

        /// <summary>
        ///  Gets or sets the image list index value of the image that is displayed when
        ///  a tree node is selected.
        /// </summary>
        /// <remarks>TODO: tri-state nodes</remarks>
        [Browsable(false)]
        public new int SelectedImageIndex
        {
            get { return base.SelectedImageIndex; }
            set { base.SelectedImageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the image list used for indicating the state of the TreeView
        /// and its nodes.
        /// </summary>
        /// <remarks>TODO: tri-state nodes</remarks>
        [Browsable(false)]
        public new ImageList StateImageList
        {
            get { return base.StateImageList; }
            set { base.StateImageList = value; }
        }

        /// <summary>
        /// Gets the collection of Nodes that are assigned to the control
        /// </summary>
        [Browsable(false)]
        public new TreeNodeCollection Nodes
        {
            get { return base.Nodes; }
        }

        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Recursively parses a kml object into the tree
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        public void ParseKmlObject(dynamic kmlObject)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, e) => 
            {
                try
                {
                    e.Result = this.CreateKmlTreeNode(kmlObject);
                }
                catch (COMException)
                {
                    /* a pointer to the COM object went away */
                    return;
                }
                catch (InvalidComObjectException)
                {
                    /* a pointer to the COM object went away */
                    return;
                }
            };

            worker.RunWorkerCompleted += (o, e) => 
            {
                if (e.Result != null)
                {
                    this.Nodes.Add(e.Result as KmlTreeViewNode);
                    this.Update();
                }
            };

            worker.RunWorkerAsync(new object[] { kmlObject });
        }

        /// <summary>
        /// Recursively parses a collection of kml objects into the tree
        /// </summary>
        /// <param name="kmlObjects">The kml objects to parse</param>
        public void ParseKmlObject(dynamic[] kmlObjects)
        {
            foreach (dynamic kmlObject in kmlObjects)
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
            this.geplugin = browser.Plugin;

            if (!GEHelpers.IsGe(this.geplugin))
            {
                throw new ArgumentException("ge is not of the type GEPlugin");
            }

            this.htmlDocument = browser.Document;
            this.Nodes.Clear();
            this.Enabled = true;

            this.gewb.PluginReady += (o, e) => 
            {
                this.Enabled = true;
            };
        }

        /// <summary>
        ///  Returns the index of the first occurrence of a tree node with the specified key.
        ///  As the key is automatically set from the kmlObject id these should be unique.
        /// </summary>
        /// <param name="id">The object id</param>
        /// <returns>The treenode that represents the object</returns>
        public KmlTreeViewNode GetNodeByApiId(string id)
        {
            if (this.Nodes.ContainsKey(id))
            {
                int i = this.Nodes.IndexOfKey(id);
                return this.Nodes[i] as KmlTreeViewNode;
            }
            else
            {
                return new TreeNode("not-found") as KmlTreeViewNode;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raised when a tree node is double clicked
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            KmlTreeViewNode node = this.SelectedNode as KmlTreeViewNode;

            if (node != null && !node.IsLoading)
            {
                node.ApiObjectVisible = true;
                node.Checked = true;

                this.OnAfterCheck(new TreeViewEventArgs(node, TreeViewAction.ByMouse));

                switch (node.KmlType)
                {
                    case ApiType.KmlPlacemark:
                    case ApiType.KmlFolder:
                    case ApiType.KmlDocument:
                        {
                            if (this.OpenBalloonsOnDoubleClick)
                            {
                                GEHelpers.OpenFeatureBalloon(
                                    this.geplugin,
                                    node.ApiObject,
                                    this.UseUnsafeHtmlBalloons,
                                    this.BalloonMinimumWidth,
                                    this.BalloonMinimumHeight,
                                    setBalloon: true);
                            }
                        }

                        break;

                    case ApiType.KmlTour:
                        {
                            GETourPlayer tourPlayer = new GETourPlayer(this.geplugin);
                            tourPlayer.SetTour(node.ApiObject);
                            tourPlayer.Play();
                        }

                        return;

                    case ApiType.KmlPhotoOverlay:
                        {
                            this.geplugin.getPhotoOverlayViewer().setPhotoOverlay(node.ApiObject);
                        }

                        return;

                    default:
                        break;
                }

                if (this.FlyToOnDoubleClickNode)
                {
                    GEHelpers.FlyToObject(this.geplugin, node.ApiObject);
                }
            }
        }

        /// <summary>
        /// Raised after a tree node has collapsed
        /// </summary>
        /// <param name="e">Event Arugments</param>
        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            KmlTreeViewNode treeNode = e.Node as KmlTreeViewNode;
            treeNode.SetStyle();
        }

        /// <summary>
        /// Raised after a tree node has expanded
        /// </summary>
        /// <param name="e">Event Arugments</param>
        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            KmlTreeViewNode node = e.Node as KmlTreeViewNode;
            node.SetStyle();

            // if the user directly caused the action...
            if (e.Action != TreeViewAction.Unknown)
            {
                // and it is an unopend network link
                if (node.KmlType == ApiType.KmlNetworkLink && !node.Fetched)
                {
                    // set-up a temp-node and background-worker for each link to be fetched
                    KmlTreeViewNode tempNode = node.Nodes[0] as KmlTreeViewNode;
                    BackgroundWorker worker = new BackgroundWorker();
                    tempNode.Animate();

                    // stop open/close node forcing reload!
                    node.Fetched = true;

                    // fetch the content in the background.
                    worker.DoWork += (o, we) => { we.Result = this.CreateKmlTreeNode(tempNode.ApiObject); };

                    // when it is finished
                    worker.RunWorkerCompleted += (o, wc) =>
                    {
                        // Get the result, add it to the temp-node parent and set the fetched flag. 
                        KmlTreeViewNode newnode = wc.Result as KmlTreeViewNode;

                        if (tempNode.Parent != null && newnode != null)
                        {
                            tempNode.Parent.Nodes.Add(newnode);
                            ((KmlTreeViewNode)tempNode.Parent).Fetched = true;
                            tempNode.Remove();

                            if (this.ExpandNetworkLinkContentOnLoad)
                            {
                                newnode.Expand();
                            }
                        }
                        else
                        {
                            // allow an to attempt reload...
                            node.Fetched = false;
                        }
                    };

                    worker.RunWorkerAsync(new object[] { tempNode });
                }
            }
        }

        /// <summary>
        /// Called when a tree node is checked
        /// </summary>
        /// <param name="e">Event Arugments</param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            KmlTreeViewNode node = e.Node as KmlTreeViewNode;

            if (node.ApiObject != null && !node.IsLoading)
            {
                node.ApiObjectVisible = node.Checked;

                if (node.Checked)
                {
                    if (e.Action != TreeViewAction.Unknown)
                    {
                        if (node.KmlType == ApiType.KmlTour)
                        {
                            this.geplugin.getTourPlayer().setTour(node.ApiObject);
                        }
                        else if (node.KmlType == ApiType.KmlPhotoOverlay)
                        {
                            this.geplugin.getPhotoOverlayViewer().setPhotoOverlay(node.ApiObject);
                        }

                        this.CheckAllParentNodes(e.Node as KmlTreeViewNode);

                        if (this.CheckAllChildren && node.KmlListStyle != ListItemStyle.CheckOffOnly)
                        {
                            this.CheckAllChildNodes(e.Node as KmlTreeViewNode);
                        }
                    }
                }
                else
                {
                    if (node.KmlType == ApiType.KmlTour)
                    {
                        this.geplugin.getTourPlayer().setTour(null);
                    }
                    else if (node.KmlType == ApiType.KmlPhotoOverlay)
                    {
                        this.geplugin.getPhotoOverlayViewer().setPhotoOverlay(null);
                    }

                    if (e.Action != TreeViewAction.Unknown)
                    {
                        this.CheckAllChildNodes(node, false);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the user clicks on the control (TODO)
        /// </summary>
        /// <param name="e">The Event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
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

        #region Private methods

        /// <summary>
        /// Recursively iterates through a Kml object adding any child features to the tree
        /// </summary>
        /// <param name="feature">The object to parse</param>
        /// <returns>The current tree node</returns>
        private KmlTreeViewNode CreateKmlTreeNode(dynamic feature)
        {
            KmlTreeViewNode treeNode = new KmlTreeViewNode(feature);

            if (treeNode.KmlType != ApiType.KmlDocument &&
                treeNode.KmlType != ApiType.KmlFolder &&
                treeNode.KmlType != ApiType.KmlNetworkLink)
            {
                // basic node
                return treeNode;
            }

            if (!Convert.ToBoolean(feature.getFeatures().hasChildNodes()))
            {
                // container with no children
                return treeNode;
            }

            // this whole try/catch block is to prevent com exceptions if
            // the application closes whilst accessing an api object
            try
            {
                dynamic kmlChildNodes = feature.getFeatures().getChildNodes();
                int count = kmlChildNodes.getLength();

                for (int i = 0; i < count; i++)
                {
                    dynamic kmlNode = kmlChildNodes.item(i);
                    string type = kmlNode.getType();

                    switch (type)
                    {
                        // GEFeatureContainers 
                        case ApiType.KmlDocument:
                        case ApiType.KmlFolder:
                            {
                                // check to see if we should open it...
                                if (KmlHelpers.GetListItemType(kmlNode) != ListItemStyle.CheckHideChildren)
                                {
                                    // ...if so add it and recurse
                                    treeNode.Nodes.Add(this.CreateKmlTreeNode(kmlNode));
                                }
                            }

                            break;

                        case ApiType.KmlNetworkLink:
                            {
                                treeNode.Nodes.Add(this.CreateKmlTreeLinkNode(kmlNode));
                            }

                            break;

                        default:
                            treeNode.Nodes.Add(new KmlTreeViewNode(kmlNode));
                            break;
                    }
                }

                if (this.UseDescriptionsForToolTips)
                {
                    treeNode.ToolTipText = treeNode.ApiObject.getDescription();
                }
                else
                {
                    treeNode.ToolTipText = treeNode.ApiObject.getSnippet();
                }

                if (this.ExpandVisibleFeatures && Convert.ToBoolean(treeNode.ApiObject.getOpen()))
                {
                    if (treeNode.KmlType != ApiType.KmlNetworkLink)
                    {
                        treeNode.Expand();
                    }
                    else
                    {
                        if (this.ExpandVisibleNetworkLinks)
                        {
                            treeNode.Expand();
                        }
                    }
                }
            }
            catch (RuntimeBinderException)
            {
                /* a pointer to the COM object went away */
            }
            catch (COMException)
            {
                /* a pointer to the COM object went away */
            }

            // return the treenode with children added as applicable 
            return treeNode;
        }

        /// <summary>
        /// Attempts to fetch networklink content to build a tree node
        /// </summary>
        /// <param name="networkLink">The networklink to parse</param>
        /// <returns>The network link node</returns>
        private KmlTreeViewNode CreateKmlTreeLinkNode(dynamic networkLink)
        {
            KmlTreeViewNode linkNode = new KmlTreeViewNode(networkLink);

            dynamic kmlObject = this.gewb.FetchKmlSynchronous(linkNode.KmlUrl);

            if (kmlObject != null)
            {
                if (kmlObject.getOwnerDocument() != null)
                {
                    KmlTreeViewNode child = new KmlTreeViewNode(kmlObject);

                    if (child.KmlListStyle != ListItemStyle.CheckHideChildren)
                    {
                        linkNode.Nodes.Add(new KmlTreeViewNode(kmlObject));
                    }
                }
            }
            else
            {
                // TODO : Icon for failed links - option to retry?
                linkNode.BackColor = Color.Gray;
            }

            return linkNode;
        }

        /// <summary>
        /// Sets the checked state of any child nodes to the given checkState
        /// </summary>
        /// <param name="node">The starting node to check from</param>
        /// <param name="check">The desired check state of the node, true is checked, false unchecked.
        /// Default is true</param>
        private void CheckAllChildNodes(KmlTreeViewNode node, bool check = true)
        {
            foreach (KmlTreeViewNode child in node.Nodes)
            {
                child.ApiObjectVisible = check;
                child.Checked = check;

                if (check)
                {
                    // checking - so only recurse if CheckOffOnly isn't set
                    if (child.KmlListStyle != ListItemStyle.CheckOffOnly)
                    {
                        this.CheckAllChildNodes(child, check);
                    }
                }
                else
                {
                    // unchecking so recurse whatever
                    this.CheckAllChildNodes(child, check);     
                }
            }
        }

        /// <summary>
        /// Sets the checked state of any parent nodes to true
        /// </summary>
        /// <param name="node">The starting node to check from</param>
        private void CheckAllParentNodes(KmlTreeViewNode node)
        {
            KmlTreeViewNode parent = node.Parent as KmlTreeViewNode;

            if (parent != null)
            {
                parent.ApiObjectVisible = true;
                parent.Checked = true;
                this.CheckAllParentNodes(node.Parent as KmlTreeViewNode);
            }
        }

        #endregion
    }
}