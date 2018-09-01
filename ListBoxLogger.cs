using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MoonPad
{
    internal class ListBoxLogger : IDisposable
    {
        // Based on example from StackOverflow.
        // https://stackoverflow.com/a/6587172

        private const int DefaultMaxLines = 2000;
        private const string DefaultMessageFormat = "{0} [{6}] {8}";
        private const int FontLineHeight = 12;

        private class LogEvent
        {
            public readonly DateTime EventTime;

            public readonly LogBoxLevel Level;
            public readonly string Message;

            public LogEvent(LogBoxLevel level, string message)
            {
                EventTime = DateTime.Now;
                Level = level;
                Message = message;
            }
        }

        private readonly int maxLines;
        private readonly string messageFormat;
        private readonly Font font;

        private ListBox listBox;
        private bool canAdd;
        private bool disposed;

        public ListBoxLogger(ListBox listBox,
            string messageFormat = DefaultMessageFormat,
            int maxLines = DefaultMaxLines)
        {
            disposed = false;

            this.listBox = listBox;
            this.messageFormat = messageFormat;
            this.maxLines = maxLines;

            canAdd = listBox.IsHandleCreated;

            listBox.SelectionMode = SelectionMode.MultiExtended;

            listBox.HandleCreated += ListBox_HandleCreated;
            listBox.HandleDestroyed += ListBox_HandleDestroyed;
            listBox.DrawItem += ListBox_DrawItem;
            listBox.KeyDown += ListBox_KeyDown;

            MenuItem[] menuItems =
            {
                new MenuItem("Copy", (sender, e) => CopyToClipboard()),
                new MenuItem("Clear", (sender, e) => Clear()),
            };

            listBox.ContextMenu = new ContextMenu(menuItems);
            listBox.ContextMenu.Popup += CopyMenuPopupHandler;

            listBox.DrawMode = DrawMode.OwnerDrawFixed;

            font = new Font(ResourceFontLibrary.GetProFontWindows(),
                FontLineHeight, FontStyle.Regular, GraphicsUnit.Pixel);

            listBox.ItemHeight = FontLineHeight;
        }

        ~ListBoxLogger()
        {
            if (disposed) return;
            Dispose(false);
            disposed = true;
        }

        public void Dispose()
        {
            if (disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
            disposed = true;
        }

        public void Clear()
        {
            listBox.Items.Clear();
        }

        // ReSharper disable once UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            if (listBox == null) return;

            canAdd = false;

            listBox.HandleCreated -= ListBox_HandleCreated;
            listBox.HandleCreated -= ListBox_HandleDestroyed;
            listBox.DrawItem -= ListBox_DrawItem;
            listBox.KeyDown -= ListBox_KeyDown;

            listBox.ContextMenu.MenuItems.Clear();
            listBox.ContextMenu.Popup -= CopyMenuPopupHandler;
            listBox.ContextMenu = null;

            listBox.Items.Clear();
            listBox.DrawMode = DrawMode.Normal;
            listBox = null;
        }

        private static string LevelName(LogBoxLevel level)
        {
            switch (level)
            {
                case LogBoxLevel.Critical: return "Critical";
                case LogBoxLevel.Error: return "Error";
                case LogBoxLevel.Warning: return "Warning";
                case LogBoxLevel.Info: return "Info";
                case LogBoxLevel.Verbose: return "Verbose";
                case LogBoxLevel.Debug: return "Debug";
                default: return $"<value={(int) level}>";
            }
        }

        private static string FormatMessage(LogEvent logEvent, string format)
        {
            var message = logEvent.Message ?? "<NULL>";
            return string.Format(format,
                /* {0} */ logEvent.EventTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                /* {1} */ logEvent.EventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                /* {2} */ logEvent.EventTime.ToString("yyyy-MM-dd"),
                /* {3} */ logEvent.EventTime.ToString("HH:mm:ss.fff"),
                /* {4} */ logEvent.EventTime.ToString("HH:mm:ss"),
                /* {5} */ LevelName(logEvent.Level)[0],
                /* {6} */ LevelName(logEvent.Level),
                /* {7} */ (int) logEvent.Level,
                /* {8} */ message);
        }

        #region Log entry adding

        private delegate void AddLogEntryDelegate(object item);

        public void WriteLog(LogBoxLevel level, string message)
        {
            if (!canAdd) return;
            var logEvent = new LogEvent(level, message);
            listBox.BeginInvoke(new AddLogEntryDelegate(AddLogEntry), logEvent);
        }

        private void AddLogEntry(object item)
        {
            listBox.Items.Add(item);

            listBox.BeginUpdate();
            listBox.SelectedIndex = listBox.Items.Count - 1;
            if (listBox.Items.Count > maxLines)
                listBox.Items.RemoveAt(0);
            listBox.EndUpdate();

            listBox.TopIndex = listBox.Items.Count - 1;
        }

        #endregion

        #region List box drawing event handling

        private void ListBox_HandleCreated(object sender, EventArgs e)
        {
            canAdd = true;
        }

        private void ListBox_HandleDestroyed(object sender, EventArgs e)
        {
            canAdd = false;
        }

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var logEvent = ((ListBox) sender).Items[e.Index] as LogEvent
                           ?? new LogEvent(LogBoxLevel.Critical, ((ListBox) sender).Items[e.Index].ToString());

            var msg = FormatMessage(logEvent, "{8}");

            var backColor = Color.FromKnownColor(KnownColor.Control);
            var foreColor = Color.Black;
            switch (logEvent.Level)
            {
                case LogBoxLevel.Critical:
                    foreColor = Color.White;
                    break;
                case LogBoxLevel.Error:
                    backColor = Color.Red;
                    foreColor = Color.Black;
                    break;
                case LogBoxLevel.Warning:
                    backColor = Color.Yellow;
                    break;
                case LogBoxLevel.Info:
                    // Defaults
                    break;
                case LogBoxLevel.Verbose:
                    foreColor = Color.Blue;
                    break;
            }

            var backBrush = new SolidBrush(backColor);
            var foreBrush = new SolidBrush(foreColor);

            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            e.Graphics.DrawString(msg, font, foreBrush, e.Bounds);
        }

        #endregion

        #region Copy event handling

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
                CopyToClipboard();
        }

        private void CopyToClipboard()
        {
            if (listBox.SelectedItems.Count <= 0) return;

            // Plain text
            var txt = new StringBuilder();
            foreach (LogEvent logEvent in listBox.SelectedItems)
            {
                txt.AppendLine(FormatMessage(logEvent, messageFormat));
            }

            // Rich-formatted text
            var rtf = new StringBuilder();
            rtf.AppendLine(@"{\rtf1\ansi\deff0{\fonttbl{\f0\fcharset0 Courier;}}");
            rtf.AppendLine(@"{\colortbl;\red255\green255\blue255;\red255\green0\blue0;\red218\green165\blue32;\red0\green128\blue0;\red0\green0\blue255;\red0\green0\blue0}");
            foreach (LogEvent logEvent in listBox.SelectedItems)
            {
                rtf.AppendFormat(@"{{\f0\fs16\chshdng0\chcbpat{0}\cb{0}\cf{1} ",
                    logEvent.Level == LogBoxLevel.Critical ? 2 : 1,
                    logEvent.Level == LogBoxLevel.Critical ? 1 : (int) logEvent.Level > 5 ? 6 : (int) logEvent.Level + 1);
                rtf.Append(FormatMessage(logEvent, messageFormat));
                rtf.AppendLine(@"\par}");
            }
            rtf.AppendLine(@"}");

            // Add both formats to the clipboard.
            Clipboard.Clear();
            var clips = new DataObject();
            clips.SetData(DataFormats.Text, txt);
            clips.SetData(DataFormats.Rtf, rtf);
            Clipboard.SetDataObject(clips, true);
        }

        private void CopyMenuPopupHandler(object sender, EventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.MenuItems[0].Enabled = listBox.SelectedItems.Count > 0;
        }

        #endregion

        public void BeginUpdate()
        {
            if (listBox.InvokeRequired)
                listBox.Invoke((Action)(() => listBox.BeginUpdate()));
        }

        public void EndUpdate()
        {
            if (listBox.InvokeRequired)
                listBox.Invoke((Action)(() => listBox.EndUpdate()));
        }
    }
}
