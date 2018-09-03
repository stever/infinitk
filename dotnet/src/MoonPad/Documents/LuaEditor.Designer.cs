namespace MoonPad.Documents
{
    partial class LuaEditor
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
            ActiproSoftware.SyntaxEditor.Document document1 = new ActiproSoftware.SyntaxEditor.Document();
            this.syntaxEditor1 = new ActiproSoftware.SyntaxEditor.SyntaxEditor();
            this.SuspendLayout();
            //
            // syntaxEditor1
            //
            this.syntaxEditor1.BracketHighlightingInclusive = true;
            this.syntaxEditor1.BracketHighlightingVisible = true;
            this.syntaxEditor1.ContentDividersVisible = true;
            this.syntaxEditor1.CurrentLineHighlightingVisible = true;
            this.syntaxEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syntaxEditor1.Document = document1;
            this.syntaxEditor1.IndentType = ActiproSoftware.SyntaxEditor.IndentType.Block;
            this.syntaxEditor1.IndicatorMarginVisible = false;
            this.syntaxEditor1.LineNumberMarginVisible = true;
            this.syntaxEditor1.Location = new System.Drawing.Point(0, 0);
            this.syntaxEditor1.Name = "syntaxEditor1";
            this.syntaxEditor1.ScrollBarType = ActiproSoftware.SyntaxEditor.ScrollBarType.Both;
            this.syntaxEditor1.Size = new System.Drawing.Size(658, 436);
            this.syntaxEditor1.SplitType = ActiproSoftware.SyntaxEditor.SyntaxEditorSplitType.None;
            this.syntaxEditor1.TabIndex = 0;
            this.syntaxEditor1.WordWrapGlyphVisible = true;
            this.syntaxEditor1.TextChanged += new System.EventHandler(this.syntaxEditor1_TextChanged);
            //
            // LuaEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.syntaxEditor1);
            this.Name = "LuaEditor";
            this.Size = new System.Drawing.Size(658, 436);
            this.ResumeLayout(false);

        }

        #endregion

        private ActiproSoftware.SyntaxEditor.SyntaxEditor syntaxEditor1;
    }
}
