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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.VisualStyles;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// The KmlTreeView provides a quick way to display kml content.
    /// </summary>
    /// <remarks>
    /// It supports virtual loading of networklink content using the 
    /// <see cref="KmlTreeViewNode"/> class. The KmlTreeView makes hevay use of stacks
    /// and background workers to keep the control fast and responsive when dealing with
    /// large numbers of nodes. The control supports many user control options including 
    /// node specifc context menus, kml tool tips and 'computed views' for objects 
    /// without abstract views defined. 
    /// </remarks>
    [Designer(typeof(ControlDesigner))]
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

        /// <summary>
        /// The custom image list for tri-state check-box support
        /// </summary>
        private ImageList triStateImageList;

        /// <summary>
        /// Vaule indicating whether check-boxes are visible.
        /// </summary>
        private bool checkBoxesVisible = true;

        /// <summary>
        /// Vaule indicating whether to prevent the check-boxes check event.
        /// </summary>
        private bool preventChecking;

        #endregion

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        public KmlTreeView()
            : base()
        {
            this.InitializeComponent();

            this.BalloonMinimumHeight = 10;
            this.BalloonMinimumWidth = 10;
            this.CheckAllChildren = true;
            this.ExpandNetworkLinkContentOnLoad = true;
            this.FlyToOnDoubleClickNode = true;
            this.OpenBalloonsOnDoubleClick = true;
            this.UseUnsafeHtmlBalloons = false;
            this.ShowNodeToolTips = true;
            this.checkBoxesVisible = true;
            base.CheckBoxes = true;
            this.BuildTriStateImageList();
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
        /// then calling <see cref="ParseKmlObject"/> 
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
        /// Gets or sets the minimum height of any balloons triggered from the control.
        /// Default is 10
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum height of any balloons triggered from the control. Default 10"),
        DefaultValueAttribute(10)]
        public int BalloonMinimumHeight { get; set; }

        /// <summary>
        /// Gets or sets the minimum width of any balloons triggered from the control.
        /// Default is 10
        /// </summary>
        [Category("Control Options"),
        Description("Gets or sets the minimum width of any balloons triggered from the control. Default 10"),
        DefaultValueAttribute(10)]
        public int BalloonMinimumWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the treeview should display checkboxes or not.
        /// Default True
        /// </summary>
        [Category("Control Options")]
        [Description("Gets or sets a value indicating whether the treeview should display checkboxes or not. Default True")]
        [DefaultValue(true)]
        public new bool CheckBoxes
        {
            get 
            {
                return this.checkBoxesVisible; 
            }

            set
            {
                this.checkBoxesVisible = value;
                base.CheckBoxes = this.checkBoxesVisible;
                this.StateImageList = this.checkBoxesVisible ? this.triStateImageList : null;
            }
        }

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
        [Browsable(false)]
        public new ImageList StateImageList
        {
            get { return base.StateImageList; }
            set { base.StateImageList = value; }
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
                    e.Result = this.CreateKmlTreeNode(e.Argument);
                }
                catch (COMException)
                {
                    return;
                }
                catch (InvalidComObjectException)
                {
                    return;
                }
            };

            worker.RunWorkerCompleted += (o, e) =>
            {
                if (e.Result != null)
                {
                    this.Nodes.Add(e.Result as KmlTreeViewNode);
                    this.Refresh();
                }
            };

            worker.RunWorkerAsync(kmlObject);
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
        /// Refreshes and updates the layout of the control
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            if (!this.CheckBoxes)
            {
                return;
            }

            base.CheckBoxes = false;

            Stack<KmlTreeViewNode> stack =
                new Stack<KmlTreeViewNode>(this.Nodes.Count);

            foreach (KmlTreeViewNode node in this.Nodes)
            {
                stack.Push(node);
            }

            while (stack.Count > 0)
            {
                KmlTreeViewNode node = stack.Pop();

                if (node.StateImageIndex == -1)
                {
                    node.StateImageIndex = node.Checked ? 1 : 0;
                }

                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    stack.Push(node.Nodes[i] as KmlTreeViewNode);
                }
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
            this.CheckBoxes = this.checkBoxesVisible;

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
                return new TreeNode() as KmlTreeViewNode;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raised when a tree node is double clicked
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseDoubleClick(e);
            this.OnNodeMouseClick(e);

            KmlTreeViewNode node = e.Node as KmlTreeViewNode;

            if (node != null && !node.IsLoading)
            {
                bool state = !node.Checked;
                node.ApiObjectVisible = state;
                node.Checked = state;
                this.CheckParentNodes(node);

                switch (node.ApiObjectType)
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
                                    setBalloon: state);
                            }
                        }

                        break;

                    case ApiType.KmlTour:
                    case ApiType.KmlPhotoOverlay:
                        {
                            GEHelpers.ToggleMediaPlayer(this.geplugin, node.ApiObject, state);
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
            base.OnAfterCollapse(e);

            KmlTreeViewNode treeNode = e.Node as KmlTreeViewNode;
            treeNode.SetStyle();
        }

        /// <summary>
        /// Raised after a tree node has expanded
        /// </summary>
        /// <param name="e">The TreeViewEventArgs arugments</param>
        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);

            foreach (TreeNode child in e.Node.Nodes)
            {
                if (child.StateImageIndex == -1)
                {
                    child.StateImageIndex = child.Checked ? 1 : 0;
                }
            }

            KmlTreeViewNode node = e.Node as KmlTreeViewNode;
            node.SetStyle();

            // if the user directly caused the action...
            if (e.Action != TreeViewAction.Unknown)
            {
                // and it is an unopend network link
                if (node.ApiObjectType == ApiType.KmlNetworkLink && !node.Fetched)
                {
                    // set-up a temp-node and background-worker for each link to be fetched
                    // set the fetched flag to stop node reloading if it colapsed/expanded during loading.
                    KmlTreeViewNode tempNode = node.Nodes[0] as KmlTreeViewNode;
                    tempNode.Animate();
                    node.Fetched = true;

                    BackgroundWorker worker = new BackgroundWorker();

                    // fetch the content in the background.
                    worker.DoWork += (o, dwea) =>
                    {
                        dwea.Result =
                            this.CreateKmlTreeNode(((KmlTreeViewNode)dwea.Argument).ApiObject);
                    };

                    // when the worker is finished
                    worker.RunWorkerCompleted += (o, rwcea) =>
                    {
                        // Get the result, add it to the temp-node parent and set the fetched flag. 
                        KmlTreeViewNode newnode = rwcea.Result as KmlTreeViewNode;

                        if (tempNode.Parent != null && newnode != null)
                        {
                            ((KmlTreeViewNode)tempNode.Parent).Fetched = true;
                            tempNode.Parent.Nodes.Add(newnode);
                            tempNode.Remove();

                            if (this.ExpandNetworkLinkContentOnLoad)
                            {
                                newnode.Expand();
                            }
                        }
                        else
                        {
                            // set the fetched flag to allow an attempt to reload...
                            node.Fetched = false;
                        }
                    };

                    worker.RunWorkerAsync(tempNode);
                }
            }
        }

        /// <summary>
        /// Raised when a tree node is checked
        /// </summary>
        /// <param name="e">The TreeViewEventArgs arugments</param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);

            if (this.preventChecking)
            {
                return;
            }

            this.OnNodeMouseClick(new TreeNodeMouseClickEventArgs(e.Node, MouseButtons.Left, 0, 0, 0));
        }

        /// <summary>
        /// Raised when the user clicks on the control
        /// </summary>
        /// <param name="e">The MouseEventArgs arguments</param>
        /// <remarks>handles the context menu system in the treeview</remarks>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                KmlTreeViewNode node = this.GetNodeAt(p) as KmlTreeViewNode;

                if (node != null)
                {
                    this.SelectedNode = node;

                    // Find the appropriate ContextMenu depending on the selected node type
                    switch (node.ApiObjectType)
                    {
                        case ApiType.KmlNetworkLink:
                            contextMenuStripNetworkLinks.Show(this, p);
                            break;
                        case ApiType.KmlTour:
                        case ApiType.KmlPhotoOverlay:
                            //TODO contextMenuStripRichMedia
                            contextMenuStripNodes.Show(this, p);
                            break;
                        default:
                            contextMenuStripNodes.Show(this, p);
                            break;
                    }
                }
                else
                {
                    // context menu for the base tree
                    contextMenuStripKmlTreeView.Show(this, p);
                }
            }
        }

        /// <summary>
        /// Raised when the user clicks on a node in the control
        /// </summary>
        /// <param name="e">The TreeNodeMouseClickEventArgs arguments</param>
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);
            this.preventChecking = true;

            int space = this.ImageList == null ? 0 : 18;

            if (e.X > e.Node.Bounds.Left - space ||
                e.X < e.Node.Bounds.Left - (space + 16))
            {
                return;
            }

            KmlTreeViewNode node = e.Node as KmlTreeViewNode;
            node.Checked = !node.Checked;
            node.ApiObjectVisible = !node.Checked;

            GEHelpers.ToggleMediaPlayer(this.geplugin, node.ApiObject, node.Checked);

            Stack<KmlTreeViewNode> stack =
                new Stack<KmlTreeViewNode>(node.Nodes.Count);
            stack.Push(node);

            do
            {
                node = stack.Pop();
                node.Checked = e.Node.Checked;
                node.ApiObjectVisible = e.Node.Checked;

                ////GEHelpers.ToggleMediaPlayer(this.geplugin, node.ApiObject, e.Node.Checked);

                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    stack.Push(node.Nodes[i] as KmlTreeViewNode);
                }
            }
            while (stack.Count > 0);

            CheckParentNodes(e.Node as KmlTreeViewNode);

            this.preventChecking = false;
        }

        /// <summary>
        /// Raised when there is Layout event in the the control
        /// </summary>
        /// <param name="e">The LayoutEventArgs arguments</param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            this.Refresh();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Builds the tri-state image list for use with the control
        /// </summary>
        private void BuildTriStateImageList()
        {
            this.triStateImageList = new ImageList();

            CheckBoxState state = CheckBoxState.UncheckedNormal;

            for (int i = 0; i < 3; i++)
            {
                Bitmap bitmap = new Bitmap(16, 16);
                Graphics graphics = Graphics.FromImage(bitmap);

                switch (i)
                {
                    case 0:
                        state = CheckBoxState.UncheckedNormal;
                        break;

                    case 1:
                        state = CheckBoxState.CheckedNormal;
                        break;

                    case 2:
                        state = CheckBoxState.MixedNormal;
                        break;
                }

                CheckBoxRenderer.DrawCheckBox(graphics, new Point(2, 2), state);
                graphics.Save();

                this.triStateImageList.Images.Add(bitmap);
            }
        }

        /// <summary>
        /// Sets the checkstate on a given nodes partents based on that node's checkstate
        /// </summary>
        /// <param name="node">the node to check from</param>
        private void CheckParentNodes(KmlTreeViewNode node)
        {
            bool mixed = false;

            while (node.Parent != null)
            {
                foreach (KmlTreeViewNode child in node.Parent.Nodes)
                {
                    mixed |= child.Checked != node.Checked | node.StateImageIndex == 2;
                    child.ApiObjectVisible = child.Checked;
                }

                int index = (int)Convert.ToUInt16(node.Checked);
                bool state = mixed || (index > 0);

                node.Parent.Checked = state;
                ((KmlTreeViewNode)node.Parent).ApiObjectVisible = state;

                if (mixed)
                {
                    node.Parent.StateImageIndex = 2;
                }
                else
                {
                    node.Parent.StateImageIndex = index;
                }

                node = node.Parent as KmlTreeViewNode;
            }
        }

        /// <summary>
        /// Recursively iterates through a Kml object adding any child features to the tree
        /// </summary>
        /// <param name="feature">The object to parse</param>
        /// <returns>The current tree node</returns>
        private KmlTreeViewNode CreateKmlTreeNode(dynamic feature)
        {
            KmlTreeViewNode treeNode = new KmlTreeViewNode(feature);

            // basic node no children
            if ((treeNode.ApiObjectType != ApiType.KmlDocument &&
                treeNode.ApiObjectType != ApiType.KmlFolder &&
                treeNode.ApiObjectType != ApiType.KmlNetworkLink))
            {
                return treeNode;
            }

            // container node with no children
            if (!Convert.ToBoolean(feature.getFeatures().hasChildNodes()))
            {
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
                    KmlTreeViewNode node = null;

                    switch (type)
                    {
                        // GEFeatureContainers 
                        case ApiType.KmlDocument:
                        case ApiType.KmlFolder:
                            {
                                // check to see if we should open it...
                                if (KmlHelpers.GetListItemType(kmlNode) != ListItemStyle.CheckHideChildren)
                                {
                                    node = this.CreateKmlTreeNode(kmlNode);
                                }
                            }

                            break;

                        case ApiType.KmlNetworkLink:
                            {
                                node = this.CreateKmlTreeLinkNode(kmlNode);
                            }

                            break;

                        default:
                            {
                                node = new KmlTreeViewNode(kmlNode);
                            }

                            break;
                    }

                    if (node != null)
                    {
                        treeNode.Nodes.Add(node);
                        CheckParentNodes(node);
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
                    if (treeNode.ApiObjectType != ApiType.KmlNetworkLink)
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
            catch (InvalidComObjectException)
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
                        linkNode.Nodes.Add(child);
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

        #endregion
    }
}