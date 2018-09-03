namespace MoonPad
{
    partial class FindToolBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindToolBar));
            this.findToolStrip = new System.Windows.Forms.ToolStrip();
            this.FindTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.findPreviousButton = new System.Windows.Forms.ToolStripButton();
            this.findNextButton = new System.Windows.Forms.ToolStripButton();
            this.findCloseButton = new System.Windows.Forms.ToolStripButton();
            this.findToolStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // findToolStrip
            //
            this.findToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.findToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.findToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindTextBox,
            this.findPreviousButton,
            this.findNextButton,
            this.findCloseButton});
            this.findToolStrip.Location = new System.Drawing.Point(0, 0);
            this.findToolStrip.Name = "findToolStrip";
            this.findToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.findToolStrip.Size = new System.Drawing.Size(635, 25);
            this.findToolStrip.TabIndex = 1;
            this.findToolStrip.Text = "toolStrip1";
            //
            // findTextBox
            //
            this.FindTextBox.Name = "FindTextBox";
            this.FindTextBox.Size = new System.Drawing.Size(100, 25);
            this.FindTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.findTextBox_KeyDown);
            //
            // findPreviousButton
            //
            this.findPreviousButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findPreviousButton.Image = ((System.Drawing.Image)(resources.GetObject("findPreviousButton.Image")));
            this.findPreviousButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findPreviousButton.Name = "findPreviousButton";
            this.findPreviousButton.Size = new System.Drawing.Size(23, 22);
            this.findPreviousButton.Text = "Find Previous";
            this.findPreviousButton.Click += new System.EventHandler(this.findPreviousButton_Click);
            //
            // findNextButton
            //
            this.findNextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findNextButton.Image = ((System.Drawing.Image)(resources.GetObject("findNextButton.Image")));
            this.findNextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(23, 22);
            this.findNextButton.Text = "Find Next";
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
            //
            // findCloseButton
            //
            this.findCloseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.findCloseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findCloseButton.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.findCloseButton.ForeColor = System.Drawing.Color.Black;
            this.findCloseButton.Image = ((System.Drawing.Image)(resources.GetObject("findCloseButton.Image")));
            this.findCloseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findCloseButton.Name = "findCloseButton";
            this.findCloseButton.Size = new System.Drawing.Size(23, 22);
            this.findCloseButton.Text = "X";
            this.findCloseButton.Click += new System.EventHandler(this.findCloseButton_Click);
            //
            // FindToolBar
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.findToolStrip);
            this.Name = "FindToolBar";
            this.Size = new System.Drawing.Size(635, 25);
            this.findToolStrip.ResumeLayout(false);
            this.findToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip findToolStrip;
        private System.Windows.Forms.ToolStripButton findPreviousButton;
        private System.Windows.Forms.ToolStripButton findNextButton;
        private System.Windows.Forms.ToolStripButton findCloseButton;
    }
}
