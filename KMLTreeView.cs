// <copyright file="KMLTreeView.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2008-12-22</date>
// <summary>This program is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
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
    public partial class KmlTreeView : TreeView
    {
        //// Private fields

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// The image list for the tree nodes
        /// </summary>
        private ImageList imageList1;

        /// <summary>
        /// Use the IGEPlugin COM interface. 
        /// Equivalent to QueryInterface for COM objects
        /// </summary>
        private IGEPlugin geplugin;

        /// <summary>
        /// Indicates whether the plugin should 'fly to' the location
        /// When a tree node is double clicked
        /// </summary>
        private bool flyToOnDoubleClickNode = true;

        /// <summary>
        /// Indicates whether the plugin should open the balloon
        /// When a tree node is double clicked
        /// </summary>
        private bool openBalloonOnDoubleClickNode = true;

        //// Constructors

        /// <summary>
        /// Initializes a new instance of the KmlTreeView class.
        /// </summary>
        public KmlTreeView()
            : base()
        {
            this.InitializeComponent();
            this.ShowNodeToolTips = true;
        }

        //// Public events    

        //// Public properties

        /// <summary>
        /// Gets or sets a value indicating whether the plugin should 'fly to' the location (if any) of the
        /// feature represented by the treenode
        /// the default setting is true
        /// </summary>
        [Category("Control Options"),
        Description("Specifies if the plugin should 'fly to' the location of the feature on double click."),
        DefaultValueAttribute(true)]
        public bool FlyToOnDoubleClickNode
        {
            get { return this.flyToOnDoubleClickNode; }
            set { this.flyToOnDoubleClickNode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to open the balloon (if any) when
        /// feature represented by the treenode is double clicked
        /// the default setting is true
        /// </summary>
        [Category("Control Options"),
        Description("Specifies if the plugin should open the feature balloon on double click."),
        DefaultValueAttribute(true)]
        public bool OpenBalloonOnDoubleClickNode
        {
            get { return this.openBalloonOnDoubleClickNode; }
            set { this.openBalloonOnDoubleClickNode = value; }
        }

        //// Public methods

        /// <summary>
        /// Set an instance of the plugin to work with
        /// this should be the same object returned by GEWebBrowser.PluginReady
        /// </summary>
        /// <param name="ge">the plugin object</param>
        public void SetPluginInstance(object ge)
        {
            try
            {
                this.geplugin = (IGEPlugin)ge;
            }
            catch (InvalidCastException)
            {
            }
        }

        /// <summary>
        /// Recursivly parses a kml object into the tree
        /// </summary>
        /// <param name="kmlObject">The kml object to parse</param>
        public void ParsekmlObject(object kmlObject)
        {
            try
            {
                IKmlObject obj = (IKmlObject)kmlObject;
                switch (obj.getType())
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
                        this.Nodes.Add(
                            this.CreateTreeNodeFromKmlFeature((IKmlFeature)obj));
                        break;
                    default:
                        break;
                }
            }
            catch (InvalidCastException)
            {
            }
        }

        //// Protected methods

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        //// Private methods

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
            TreeNode tn = new TreeNode();
            tn.Text = kmlFeature.getName();
            tn.Tag = kmlFeature;
            tn.Name = kmlFeature.getType();
            tn.ToolTipText = this.ShortenToolTip(kmlFeature.getDescription());

            if (Convert.ToBoolean(kmlFeature.getOpen())) 
            { 
                tn.Expand(); 
            }

            if (Convert.ToBoolean(kmlFeature.getVisibility()))
            { 
                tn.Checked = true;

                // if feature is visible open the node by default??
                ////tn.Expand();
            }

            switch (kmlFeature.getType())
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
                case"KmlGroundOverlay":
                case "KmlScreenOverlay":
                    tn.ImageKey = "overlay";
                    tn.SelectedImageKey = "overlay";
                    break;
                default:
                    break;
            }

            return tn;
        }

        /// <summary>
        /// Fires when a tree node is checked
        /// </summary>
        /// <param name="sender">A tree node</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTree_AfterCheck(object sender, TreeViewEventArgs e)
        {          
            IKmlFeature feature = (IKmlFeature)e.Node.Tag;

            if (feature != null)
            {
                if (e.Node.Checked)
                {
                    feature.setVisibility(1);
                    this.CheckAllParentNodes(e.Node);
                }
                else
                {
                    feature.setVisibility(0);
                    this.UncheckAllChildNodes(e.Node);
                }
            }
        }

        /// <summary>
        /// Fires when a tree node is double clicked
        /// </summary>
        /// <param name="sender">The TreeView</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTree_DoubleClick(object sender, EventArgs e)
        {
            if (this.SelectedNode != null && this.geplugin != null)
            {
                this.SelectedNode.Checked = true;

                IKmlFeature feature = (IKmlFeature)SelectedNode.Tag;

                if (feature != null && feature.getType() == "KmlPlacemark")
                {
                    IKmlPlacemark placemark = (IKmlPlacemark)feature;
                    IKmlAbstractView view = placemark.getAbstractView();
                    IKmlGeometry geometry = placemark.getGeometry();
                    IGEFeatureBalloon balloon = this.geplugin.createFeatureBalloon(String.Empty);

                    if (this.openBalloonOnDoubleClickNode)
                    {
                        ////ge.setBalloon(null);
                        balloon.setMinHeight(100);
                        balloon.setMinWidth(100);
                        balloon.setFeature(placemark);
                        this.geplugin.setBalloon(balloon);
                    }

                    if (this.flyToOnDoubleClickNode)
                    {
                        if (view != null)
                        {
                            this.geplugin.getView().setAbstractView(view);
                            return;
                        }
                        else if (geometry != null)
                        {
                            IKmlLookAt lookat;
                            ////System.Diagnostics.Debug.WriteLine(geometry.getType());
                            switch (geometry.getType())
                            {
                                case "KmlPoint":
                                    IKmlPoint point = (IKmlPoint)geometry;
                                    lookat = this.geplugin.createLookAt(String.Empty);
                                    lookat.set(point.getLatitude(), point.getLongitude(), 100, this.geplugin.ALTITUDE_RELATIVE_TO_GROUND, 0, 0, 1000);
                                    this.geplugin.getView().setAbstractView(lookat);
                                    break;
                                case "KmlPolygon":
                                    IKmlPolygon polygon = (IKmlPolygon)geometry;
                                    IKmlCoord coord = polygon.getOuterBoundary().getCoordinates().get(0);
                                    lookat = this.geplugin.createLookAt(String.Empty);
                                    lookat.set(coord.getLatitude(), coord.getLongitude(), 100, this.geplugin.ALTITUDE_RELATIVE_TO_GROUND, 0, 0, 1000);
                                    this.geplugin.getView().setAbstractView(lookat);
                                    break;
                                case "KmlLineString":
                                ////IKmlLineString lineString = (IKmlLineString)geometry;
                                case "KmlMultiGeometry":
                                ////IKmlMultiGeometry multiGeometry = (IKmlMultiGeometry)geometry;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fires after a tree node has expanded
        /// </summary>
        /// <param name="sender">The TreeView</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            IKmlFeature feature = (IKmlFeature)e.Node.Tag;
            if (feature != null)
            {
                switch (feature.getType())
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
        /// Fires after a tree node has collapsed
        /// </summary>
        /// <param name="sender">The TreeView</param>
        /// <param name="e">Event Arugments</param>
        private void KmlTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
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

        /// <summary>
        /// Required method for Designer support
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(KmlTreeView));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();

            // imageList1
            this.imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ge");
            this.imageList1.Images.SetKeyName(1, "kml");
            this.imageList1.Images.SetKeyName(2, "folderClosed");
            this.imageList1.Images.SetKeyName(3, "folderOpen");
            this.imageList1.Images.SetKeyName(4, "flag");
            this.imageList1.Images.SetKeyName(5, "overlay");

            // KmlTreeView
            this.CheckBoxes = true;
            this.ImageIndex = 0;
            this.ImageList = this.imageList1;
            this.LineColor = System.Drawing.Color.Black;
            this.SelectedImageIndex = 0;
            this.ShowNodeToolTips = true;
            this.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.KmlTree_AfterCheck);
            this.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.KmlTreeView_AfterCollapse);
            this.DoubleClick += new System.EventHandler(this.KmlTree_DoubleClick);
            this.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.KmlTreeView_AfterExpand);
            this.ResumeLayout(false);
        }
    }
}