﻿// <copyright file="GEStatusStrip.Designer.cs" company="FC">
// Copyright (c) 2008 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2009-09-19</date>
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
    /// <summary>
    /// Designer file
    /// </summary>
    public partial class GEStatusStrip
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// The API version status label
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel apiVersionStatusLabel;

        /// <summary>
        /// The browser version status label
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel browserVersionStatusLabel;

        /// <summary>
        /// The plug-in version status label
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel pluginVersionStatusLabel;

        /// <summary>
        /// The streaming progress bar
        /// </summary>
        private System.Windows.Forms.ToolStripProgressBar streamingProgressBar;

        /// <summary>
        /// The streaming status label
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel streamingStatusLabel;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.apiVersionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.browserVersionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.streamingProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.streamingStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.pluginVersionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // apiVersionStatusLabel
            // 
            this.apiVersionStatusLabel.ForeColor = System.Drawing.Color.Gray;
            this.apiVersionStatusLabel.Name = "apiVersionStatusLabel";
            this.apiVersionStatusLabel.Size = new System.Drawing.Size(23, 17);
            this.apiVersionStatusLabel.Text = "api ";
            this.apiVersionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // browserVersionStatusLabel
            // 
            this.browserVersionStatusLabel.ForeColor = System.Drawing.Color.Gray;
            this.browserVersionStatusLabel.Name = "browserVersionStatusLabel";
            this.browserVersionStatusLabel.Size = new System.Drawing.Size(23, 17);
            this.browserVersionStatusLabel.Spring = true;
            this.browserVersionStatusLabel.Text = "ie ";
            this.browserVersionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // streamingProgressBar
            // 
            this.streamingProgressBar.Name = "streamingProgressBar";
            this.streamingProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // streamingStatusLabel
            // 
            this.streamingStatusLabel.Name = "streamingStatusLabel";
            this.streamingStatusLabel.Size = new System.Drawing.Size(23, 17);
            this.streamingStatusLabel.Text = "idle";
            // 
            // pluginVersionStatusLabel
            // 
            this.pluginVersionStatusLabel.ForeColor = System.Drawing.Color.Gray;
            this.pluginVersionStatusLabel.Name = "pluginVersionStatusLabel";
            this.pluginVersionStatusLabel.Size = new System.Drawing.Size(23, 17);
            this.pluginVersionStatusLabel.Text = "plugin ";
            this.pluginVersionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GEStatusStrip
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.streamingProgressBar,
            this.streamingStatusLabel,
            this.browserVersionStatusLabel,
            this.apiVersionStatusLabel,
            this.pluginVersionStatusLabel});
            this.Location = new System.Drawing.Point(0, 347);
            this.Name = "geStatusStrip1";
            this.Size = new System.Drawing.Size(693, 22);
            this.Text = "geStatusStrip1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
    }
}
