namespace MoonPad
{
    partial class FormWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.luaScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.formBorderPanel = new System.Windows.Forms.Panel();
            this.sandDockPanel = new System.Windows.Forms.Panel();
            this.dockContainer1 = new TD.SandDock.DockContainer();
            this.luaScriptsWindow = new TD.SandDock.DockableWindow();
            this.sandDockManager1 = new TD.SandDock.SandDockManager();
            this.dockContainer2 = new TD.SandDock.DockContainer();
            this.logDockWindow = new TD.SandDock.DockableWindow();
            this.tabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllDocumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllButThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.formBorderPanel.SuspendLayout();
            this.sandDockPanel.SuspendLayout();
            this.dockContainer1.SuspendLayout();
            this.dockContainer2.SuspendLayout();
            this.tabContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(120, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGLToolStripMenuItem,
            this.toolStripSeparator4,
            this.logToolStripMenuItem,
            this.luaScriptsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // openGLToolStripMenuItem
            // 
            this.openGLToolStripMenuItem.Name = "openGLToolStripMenuItem";
            this.openGLToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.openGLToolStripMenuItem.Text = "OpenGL";
            this.openGLToolStripMenuItem.Click += new System.EventHandler(this.openGLToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(128, 6);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // luaScriptsToolStripMenuItem
            // 
            this.luaScriptsToolStripMenuItem.Name = "luaScriptsToolStripMenuItem";
            this.luaScriptsToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.luaScriptsToolStripMenuItem.Text = "Lua Scripts";
            this.luaScriptsToolStripMenuItem.Click += new System.EventHandler(this.luaScriptsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.toolStripSeparator3,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // viewHelpToolStripMenuItem
            // 
            this.viewHelpToolStripMenuItem.Enabled = false;
            this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
            this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.viewHelpToolStripMenuItem.Text = "View Help";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(159, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.aboutToolStripMenuItem.Text = "About MoonPad";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(40, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // formBorderPanel
            // 
            this.formBorderPanel.BackColor = System.Drawing.SystemColors.Control;
            this.formBorderPanel.Controls.Add(this.sandDockPanel);
            this.formBorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formBorderPanel.Location = new System.Drawing.Point(0, 24);
            this.formBorderPanel.Name = "formBorderPanel";
            this.formBorderPanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.formBorderPanel.Size = new System.Drawing.Size(800, 404);
            this.formBorderPanel.TabIndex = 2;
            // 
            // sandDockPanel
            // 
            this.sandDockPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.sandDockPanel.Controls.Add(this.dockContainer1);
            this.sandDockPanel.Controls.Add(this.dockContainer2);
            this.sandDockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sandDockPanel.Location = new System.Drawing.Point(4, 0);
            this.sandDockPanel.Name = "sandDockPanel";
            this.sandDockPanel.Size = new System.Drawing.Size(792, 402);
            this.sandDockPanel.TabIndex = 0;
            // 
            // dockContainer1
            // 
            this.dockContainer1.ContentSize = 200;
            this.dockContainer1.Controls.Add(this.luaScriptsWindow);
            this.dockContainer1.Dock = System.Windows.Forms.DockStyle.Right;
            this.dockContainer1.LayoutSystem = new TD.SandDock.SplitLayoutSystem(new System.Drawing.SizeF(250F, 400F), System.Windows.Forms.Orientation.Horizontal, new TD.SandDock.LayoutSystemBase[] {
            ((TD.SandDock.LayoutSystemBase)(new TD.SandDock.ControlLayoutSystem(new System.Drawing.SizeF(250F, 400F), new TD.SandDock.DockControl[] {
                        ((TD.SandDock.DockControl)(this.luaScriptsWindow))}, this.luaScriptsWindow)))});
            this.dockContainer1.Location = new System.Drawing.Point(588, 0);
            this.dockContainer1.Manager = this.sandDockManager1;
            this.dockContainer1.Name = "dockContainer1";
            this.dockContainer1.Size = new System.Drawing.Size(204, 250);
            this.dockContainer1.TabIndex = 6;
            // 
            // luaScriptsWindow
            // 
            this.luaScriptsWindow.Guid = new System.Guid("78571c2f-c0e3-4a1f-a4a5-5affab7100b3");
            this.luaScriptsWindow.Location = new System.Drawing.Point(4, 23);
            this.luaScriptsWindow.Name = "luaScriptsWindow";
            this.luaScriptsWindow.ShowOptions = false;
            this.luaScriptsWindow.Size = new System.Drawing.Size(200, 203);
            this.luaScriptsWindow.TabIndex = 0;
            this.luaScriptsWindow.Text = "Lua Scripts";
            // 
            // sandDockManager1
            // 
            this.sandDockManager1.BorderStyle = TD.SandDock.Rendering.BorderStyle.None;
            this.sandDockManager1.DockSystemContainer = this.sandDockPanel;
            this.sandDockManager1.OwnerForm = this;
            this.sandDockManager1.ShowControlContextMenu += new TD.SandDock.ShowControlContextMenuEventHandler(this.sandDockManager1_ShowControlContextMenu);
            // 
            // dockContainer2
            // 
            this.dockContainer2.ContentSize = 148;
            this.dockContainer2.Controls.Add(this.logDockWindow);
            this.dockContainer2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dockContainer2.LayoutSystem = new TD.SandDock.SplitLayoutSystem(new System.Drawing.SizeF(250F, 400F), System.Windows.Forms.Orientation.Vertical, new TD.SandDock.LayoutSystemBase[] {
            ((TD.SandDock.LayoutSystemBase)(new TD.SandDock.ControlLayoutSystem(new System.Drawing.SizeF(250F, 400F), new TD.SandDock.DockControl[] {
                        ((TD.SandDock.DockControl)(this.logDockWindow))}, this.logDockWindow)))});
            this.dockContainer2.Location = new System.Drawing.Point(0, 250);
            this.dockContainer2.Manager = this.sandDockManager1;
            this.dockContainer2.Name = "dockContainer2";
            this.dockContainer2.Size = new System.Drawing.Size(792, 152);
            this.dockContainer2.TabIndex = 7;
            // 
            // logDockWindow
            // 
            this.logDockWindow.Guid = new System.Guid("d90b7314-2d06-4edb-842b-20af56d4788e");
            this.logDockWindow.Location = new System.Drawing.Point(0, 27);
            this.logDockWindow.Name = "logDockWindow";
            this.logDockWindow.ShowOptions = false;
            this.logDockWindow.Size = new System.Drawing.Size(792, 101);
            this.logDockWindow.TabIndex = 1;
            this.logDockWindow.Text = "Log";
            // 
            // tabContextMenu
            // 
            this.tabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem1,
            this.closeAllDocumentsToolStripMenuItem,
            this.closeAllButThisToolStripMenuItem});
            this.tabContextMenu.Name = "tabContextMenu";
            this.tabContextMenu.Size = new System.Drawing.Size(185, 70);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
            this.closeToolStripMenuItem1.Text = "Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // closeAllDocumentsToolStripMenuItem
            // 
            this.closeAllDocumentsToolStripMenuItem.Name = "closeAllDocumentsToolStripMenuItem";
            this.closeAllDocumentsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeAllDocumentsToolStripMenuItem.Text = "Close All Documents";
            this.closeAllDocumentsToolStripMenuItem.Click += new System.EventHandler(this.closeAllDocumentsToolStripMenuItem_Click);
            // 
            // closeAllButThisToolStripMenuItem
            // 
            this.closeAllButThisToolStripMenuItem.Name = "closeAllButThisToolStripMenuItem";
            this.closeAllButThisToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeAllButThisToolStripMenuItem.Text = "Close All But This";
            this.closeAllButThisToolStripMenuItem.Click += new System.EventHandler(this.closeAllButThisToolStripMenuItem_Click);
            // 
            // FormWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.formBorderPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormWindow";
            this.Text = "MoonPad";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWindow_FormClosing);
            this.Load += new System.EventHandler(this.FormWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.formBorderPanel.ResumeLayout(false);
            this.sandDockPanel.ResumeLayout(false);
            this.dockContainer1.ResumeLayout(false);
            this.dockContainer2.ResumeLayout(false);
            this.tabContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel formBorderPanel;
        private System.Windows.Forms.Panel sandDockPanel;
        private TD.SandDock.DockContainer dockContainer1;
        private TD.SandDock.DockableWindow luaScriptsWindow;
        private TD.SandDock.SandDockManager sandDockManager1;
        private TD.SandDock.DockContainer dockContainer2;
        private TD.SandDock.DockableWindow logDockWindow;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem luaScriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip tabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeAllDocumentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllButThisToolStripMenuItem;
    }
}