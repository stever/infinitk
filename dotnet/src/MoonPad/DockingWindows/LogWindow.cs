using System;
using System.Drawing;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Core;
using Timer = System.Timers.Timer;

namespace MoonPad.DockingWindows
{
    public partial class LogWindow : UserControl
    {
        private const int LogTimerPeriodMillis = 250;

        private readonly MemoryAppender memoryAppender;
        private readonly ListBoxLogger listBoxLogger;
        private readonly Timer logUpdateTimer;

        public LogWindow()
        {
            InitializeComponent();

            var listBox1 = new FlickerFreeListBox
            {
                ItemHeight = 12,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawFixed,
                Location = new Point(0, 0),
                Name = "listBox1",
                Size = new Size(597, 96),
                TabIndex = 0
            };

            Controls.Add(listBox1);

            listBoxLogger = new ListBoxLogger(listBox1);

            logUpdateTimer = new Timer(LogTimerPeriodMillis) {AutoReset = false};
            logUpdateTimer.Elapsed += logUpdateTimer_Elapsed;

            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var appender in appenders)
            {
                if (appender.Name != "MemoryAppender") continue;
                memoryAppender = (MemoryAppender) appender;
                break;
            }
        }

        public void Clear()
        {
            listBoxLogger.Clear();
        }

        private void LogWindow_Load(object sender, EventArgs e)
        {
            logUpdateTimer.Start();
        }

        private void logUpdateTimer_Elapsed(object sender, EventArgs e)
        {
            var events = memoryAppender.GetEvents();

            if (events.Length <= 0)
            {
                logUpdateTimer.Start();
                return;
            }

            listBoxLogger.BeginUpdate();

            foreach (var logEvent in events)
            {
                LogBoxLevel level;
                if (logEvent.Level == Level.Debug) level = LogBoxLevel.Debug;
                else if (logEvent.Level == Level.Info) level = LogBoxLevel.Info;
                else if (logEvent.Level == Level.Warn) level = LogBoxLevel.Warning;
                else if (logEvent.Level == Level.Error) level = LogBoxLevel.Error;
                else level = LogBoxLevel.Error;
                listBoxLogger.WriteLog(level, logEvent.RenderedMessage);
            }

            listBoxLogger.EndUpdate();

            // Clear appender until next update.
            memoryAppender?.Clear();

            logUpdateTimer.Enabled = true;
        }
    }
}
