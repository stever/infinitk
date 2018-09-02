using System.Drawing;
using System.Windows.Forms;
using TD.SandDock;
using TD.SandDock.Rendering;

namespace MoonPad.SandDockRendering
{
    internal class Office2003RendererBase : Office2003Renderer
    {
        public BoxModel TabStripMetrics2 => TabStripMetrics;
        public BoxModel TabMetrics2 => TabMetrics;
        public BoxModel TitleBarMetrics2 => TitleBarMetrics;
        public TabTextDisplayMode TabTextDisplay2 => TabTextDisplay;
        public int DocumentTabExtra2 => DocumentTabExtra;
        public int DocumentTabSize2 => DocumentTabSize;
        public int DocumentTabStripSize2 => DocumentTabStripSize;

        public void DrawDocumentStripBackground2(Graphics graphics, Rectangle bounds)
        {
            DrawDocumentStripBackground(graphics, bounds);
        }

        public void DrawControlClientBackground2(Graphics graphics, Rectangle bounds, Color backColor)
        {
            DrawControlClientBackground(graphics, bounds, backColor);
        }

        public void DrawDocumentClientBackground2(Graphics graphics, Rectangle bounds, Color backColor)
        {
            DrawDocumentClientBackground(graphics, bounds, backColor);
        }

        public void DrawDocumentStripTab2(Graphics graphics, Rectangle bounds, Rectangle contentBounds, Image image, string text, Font font, Color backColor, Color foreColor, DrawItemState state, bool drawSeparator)
        {
            DrawDocumentStripTab(graphics, bounds, contentBounds, image, text, font, backColor, foreColor, state, drawSeparator);
        }

        public void DrawDockContainerBackground2(Graphics graphics, DockContainer container, Rectangle bounds)
        {
            DrawDockContainerBackground(graphics, container, bounds);
        }

        public void DrawDocumentStripButton2(Graphics graphics, Rectangle bounds, SandDockButtonType buttonType, DrawItemState state)
        {
            DrawDocumentStripButton(graphics, bounds, buttonType, state);
        }

        public void DrawTabStripBackground2(Control container, Control control, Graphics graphics, Rectangle bounds, int selectedTabOffset)
        {
            DrawTabStripBackground(container, control, graphics, bounds, selectedTabOffset);
        }

        public void DrawSplitter2(Control container, Control control, Graphics graphics, Rectangle bounds, Orientation orientation)
        {
            DrawSplitter(container, control, graphics, bounds, orientation);
        }

        public void DrawTabStripTab2(Graphics graphics, Rectangle bounds, Image image, string text, Font font, Color backColor, Color foreColor, DrawItemState state, bool drawSeparator)
        {
            DrawTabStripTab(graphics, bounds, image, text, font, backColor, foreColor, state, drawSeparator);
        }

        public void DrawAutoHideBarBackground2(Control container, Control control, Graphics graphics, Rectangle bounds)
        {
            DrawAutoHideBarBackground(container, control, graphics, bounds);
        }

        public void DrawCollapsedTab2(Graphics graphics, Rectangle bounds, DockSide dockSide, Image image, string text, Font font, Color backColor, Color foreColor, DrawItemState state, bool vertical)
        {
            DrawCollapsedTab(graphics, bounds, dockSide, image, text, font, backColor, foreColor, state, vertical);
        }

        public void DrawTitleBarBackground2(Graphics graphics, Rectangle bounds, bool focused)
        {
            DrawTitleBarBackground(graphics, bounds, focused);
        }

        public void DrawTitleBarText2(Graphics graphics, Rectangle bounds, bool focused, string text, Font font)
        {
            DrawTitleBarText(graphics, bounds, focused, text, font);
        }

        public void DrawTitleBarButton2(Graphics graphics, Rectangle bounds, SandDockButtonType buttonType, DrawItemState state, bool focused, bool toggled)
        {
            DrawTitleBarButton(graphics, bounds, buttonType, state, focused, toggled);
        }
    }
}
