using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ActiproSoftware.Drawing;
using ActiproSoftware.SyntaxEditor;
using log4net;

namespace MoonPad.Documents
{
    internal partial class LuaEditor : UserControl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int FontLineHeight = 16;

        public string ScriptName { get; }

        private readonly bool isConstructing;
        private readonly FormWindow formWindow;

        private Database Database => formWindow?.Database;

        public LuaEditor(FormWindow formWindow, string scriptName)
        {
            InitializeComponent();

            isConstructing = true;
            this.formWindow = formWindow;
            ScriptName = scriptName;

            // Font
            syntaxEditor1.Font = new Font(ResourceFontLibrary.GetPragmataPro(), FontLineHeight, FontStyle.Regular, GraphicsUnit.Pixel);
            syntaxEditor1.LineNumberMarginFont = syntaxEditor1.Font;
            syntaxEditor1.UserMarginFont = syntaxEditor1.Font;

            // Load the script from XML Part.
            var luaScript = Database.GetLuaScript(scriptName);
            syntaxEditor1.Text = luaScript ?? throw new DocumentNotFoundException();

            // Load the Lua syntax XML definition file for SyntaxEditor from assembly resource.
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.ActiproSoftware_Lua));
            syntaxEditor1.Document.LoadLanguageFromXml(stream, 0);

            // We want automatic outlining for our syntax code file. NOTE: This isn't enabled by default.
            syntaxEditor1.Document.Outlining.Mode = OutliningMode.Automatic;

            // Remove border from control.
            syntaxEditor1.Renderer = new VisualStudio2005SyntaxEditorRenderer
            {
                Border = new SimpleBorder(SimpleBorderStyle.None, Color.Transparent),
                LineNumberMarginBorderDashStyle = DashStyle.Solid,
                LineNumberMarginBorderColor = Color.FromArgb(0xDD, 0xDD, 0xDD),
                LineNumberMarginBackgroundFill = new SolidColorBackgroundFill(Color.FromArgb(0xF7, 0xF7, 0xF7)),
                LineNumberMarginForeColor = Color.FromArgb(0x99, 0x99, 0x99),
                SelectionFocusedForeColor = Color.Black,
                SelectionUnfocusedForeColor = Color.Black,
                SelectionFocusedBackColor = Color.FromArgb(0xD7, 0xD4, 0xF0),
                SelectionUnfocusedBackColor = Color.FromArgb(0xD9, 0xD9, 0xD9),
                //SelectionFocusedBackColor = Color.FromArgb(0xD4, 0xFB, 0xA9),
                //SelectionUnfocusedBackColor = Color.FromArgb(0xE1, 0xE7, 0xEC),
            };

            syntaxEditor1.LineNumberMarginWidth = 36;

            // Current line highlight colour.
            // Customise the appearance using the global renderer.
            var renderer = syntaxEditor1.RendererResolved;
            //renderer.CurrentLineHighlightBackColor = Color.White;
            //renderer.CurrentLineHighlightBorderColor = Color.FromArgb(234, 234, 234);
            renderer.CurrentLineHighlightBackColor = Color.Transparent;
            renderer.CurrentLineHighlightBorderColor = Color.Transparent;

            isConstructing = false;
        }

        private void syntaxEditor1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (isConstructing) return;
                Database.SaveLuaScript(ScriptName, syntaxEditor1.Text);
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }
    }
}
