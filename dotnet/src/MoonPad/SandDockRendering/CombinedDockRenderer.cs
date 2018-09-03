using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using TD.SandDock;
using TD.SandDock.Rendering;

namespace MoonPad.SandDockRendering
{
    internal class CombinedDockRenderer : RendererBase
    {
        private readonly EverettRendererBase everettRenderer = new EverettRendererBase();
        private readonly WhidbeyRendererBase whidbeyRenderer = new WhidbeyRendererBase();
        private readonly Office2003RendererBase office2003Renderer = new Office2003RendererBase();

        protected override BoxModel TabStripMetrics => office2003Renderer.TabStripMetrics2;
        protected override BoxModel TabMetrics => office2003Renderer.TabMetrics2;
        protected override BoxModel TitleBarMetrics => whidbeyRenderer.TitleBarMetrics2;
        protected override TabTextDisplayMode TabTextDisplay => office2003Renderer.TabTextDisplay2;
        protected override int DocumentTabExtra => everettRenderer.DocumentTabExtra2;
        protected override int DocumentTabSize => everettRenderer.DocumentTabSize2;
        protected override int DocumentTabStripSize => everettRenderer.DocumentTabStripSize2;
        public override Size TabControlPadding => office2003Renderer.TabControlPadding;

        public CombinedDockRenderer(Color tabColour)
        {
            var controlColour = Color.FromKnownColor(KnownColor.Control);

            everettRenderer.HighlightColor = tabColour;
            everettRenderer.ShadowColor = tabColour;

            whidbeyRenderer.InactiveTitleBarBackgroundColor = controlColour;
            whidbeyRenderer.ActiveTitleBarBackgroundColor1 = Color.FromArgb(0x00, 0x7A, 0xCC);
            whidbeyRenderer.ActiveTitleBarBackgroundColor2 = Color.FromArgb(0x00, 0x63, 0xAC);
            whidbeyRenderer.ActiveTitleBarForegroundColor = Color.White;

            whidbeyRenderer.LayoutBackgroundColor1 = controlColour;
            whidbeyRenderer.LayoutBackgroundColor2 = controlColour;
            whidbeyRenderer.DocumentStripBackgroundColor1 = controlColour;
            whidbeyRenderer.DocumentStripBackgroundColor2 = controlColour;

            office2003Renderer.LayoutBackgroundColor1 = controlColour;
            office2003Renderer.LayoutBackgroundColor2 = controlColour;
            office2003Renderer.DocumentStripBackgroundColor1 = controlColour;
            office2003Renderer.DocumentStripBackgroundColor2 = controlColour;
        }

        #region RenderSession

        public override void StartRenderSession(HotkeyPrefix hotKeys)
        {
            everettRenderer.StartRenderSession(hotKeys);
            whidbeyRenderer.StartRenderSession(hotKeys);
            office2003Renderer.StartRenderSession(hotKeys);
        }

        public override void FinishRenderSession()
        {
            everettRenderer.FinishRenderSession();
            whidbeyRenderer.FinishRenderSession();
            office2003Renderer.FinishRenderSession();
        }

        #endregion

        protected override Size MeasureDocumentStripTab(Graphics graphics, Image image, string text, Font font, DrawItemState state)
        {
            return everettRenderer.MeasureTabControlTab(graphics, image, text, font, state);
        }

        protected override Size MeasureTabStripTab(Graphics graphics, Image image, string text, Font font, DrawItemState state)
        {
            return office2003Renderer.MeasureTabControlTab(graphics, image, text, font, state);
        }

        protected override void DrawDocumentStripBackground(Graphics graphics, Rectangle bounds)
        {
            whidbeyRenderer.DrawDocumentStripBackground2(graphics, bounds);
        }

        protected override void DrawControlClientBackground(Graphics graphics, Rectangle bounds, Color backColor)
        {
            office2003Renderer.DrawControlClientBackground2(graphics, bounds, backColor);
        }

        protected override void DrawDocumentClientBackground(Graphics graphics, Rectangle bounds, Color backColor)
        {
            everettRenderer.DrawDocumentClientBackground2(graphics, bounds, backColor);
        }

        protected override void DrawDocumentStripTab(Graphics graphics, Rectangle bounds, Rectangle contentBounds, Image image, string text, Font font, Color backColor, Color foreColor, DrawItemState state, bool drawSeparator)
        {
            everettRenderer.DrawDocumentStripTab2(graphics, bounds, contentBounds, image, text, font, backColor, foreColor, state, drawSeparator);
        }

        protected override void DrawDockContainerBackground(Graphics graphics, DockContainer container, Rectangle bounds)
        {
            office2003Renderer.DrawDockContainerBackground2(graphics, container, bounds);
        }

        protected override void DrawDocumentStripButton(Graphics graphics, Rectangle bounds, SandDockButtonType buttonType, DrawItemState state)
        {
            everettRenderer.DrawDocumentStripButton2(graphics, bounds, buttonType, state);
        }

        protected override void DrawTabStripBackground(Control container, Control control, Graphics graphics, Rectangle bounds, int selectedTabOffset)
        {
            office2003Renderer.DrawTabStripBackground2(container, control, graphics, bounds, selectedTabOffset);
        }

        protected override void DrawSplitter(Control container, Control control, Graphics graphics, Rectangle bounds, Orientation orientation)
        {
            office2003Renderer.DrawSplitter2(container, control, graphics, bounds, orientation);
        }

        protected override void DrawTabStripTab(Graphics graphics, Rectangle bounds, Image image, string text, Font font, Color backColor, Color foreColor, DrawItemState state, bool drawSeparator)
        {
            office2003Renderer.DrawTabStripTab2(graphics, bounds, image, text, font, backColor, foreColor, state, drawSeparator);
        }

        protected override void DrawAutoHideBarBackground(Control container, Control control, Graphics graphics, Rectangle bounds)
        {
            office2003Renderer.DrawAutoHideBarBackground2(container, control, graphics, bounds);
        }

        protected override void DrawCollapsedTab(Graphics graphics, Rectangle bounds, DockSide dockSide, Image image, string text, Font font, Color backColor, Color foreColor, DrawItemState state, bool vertical)
        {
            everettRenderer.DrawCollapsedTab2(graphics, bounds, dockSide, image, text, font, backColor, foreColor, state, vertical);
        }

        protected override void DrawTitleBarBackground(Graphics graphics, Rectangle bounds, bool focused)
        {
            whidbeyRenderer.DrawTitleBarBackground2(graphics, bounds, focused);
        }

        protected override void DrawTitleBarText(Graphics graphics, Rectangle bounds, bool focused, string text, Font font)
        {
            whidbeyRenderer.DrawTitleBarText2(graphics, bounds, focused, text, font);
        }

        protected override void DrawTitleBarButton(Graphics graphics, Rectangle bounds, SandDockButtonType buttonType, DrawItemState state, bool focused, bool toggled)
        {
            whidbeyRenderer.DrawTitleBarButton2(graphics, bounds, buttonType, state, focused, toggled);
        }
    }
}
