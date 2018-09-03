using System;
using System.Windows.Forms;

namespace MoonPad.Persistence
{
    internal class WindowsFormGeometryPersistence
    {
        private readonly string id;
        private readonly int? defaultHeight;
        private readonly int? defaultWidth;

        public WindowsFormGeometryPersistence(string id)
        {
            this.id = id;
            defaultWidth = 800;
            defaultHeight = 600;
        }

        public WindowsFormGeometryPersistence(string id, int defaultWidth, int defaultHeight)
        {
            this.id = id;
            this.defaultWidth = defaultWidth;
            this.defaultHeight = defaultHeight;
        }

        public void Persist(Form window)
        {
            var state = window.WindowState;
            if (state != FormWindowState.Normal)
            {
                window.WindowState = FormWindowState.Normal;
            }

            Update(new WindowsFormGeometry
            {
                Top = window.Top,
                Left = window.Left,
                Width = window.Width,
                Height = window.Height,
                State = state
            });
        }

        public void Restore(Form window)
        {
            var geometry = Get();

            // Size to fit.
            if (geometry.Height > Screen.PrimaryScreen.WorkingArea.Height)
                geometry.Height = Screen.PrimaryScreen.WorkingArea.Height;
            if (geometry.Width > Screen.PrimaryScreen.WorkingArea.Width)
                geometry.Width = Screen.PrimaryScreen.WorkingArea.Width;

            // Move into view.
            if (geometry.Top + geometry.Height / 2 > Screen.PrimaryScreen.WorkingArea.Height)
                geometry.Top = Screen.PrimaryScreen.WorkingArea.Height - geometry.Height;
            if (geometry.Top < 0) geometry.Top = 0;
            if (geometry.Left + geometry.Width / 2 > Screen.PrimaryScreen.WorkingArea.Width)
                geometry.Left = Screen.PrimaryScreen.WorkingArea.Width - geometry.Width;
            if (geometry.Left < 0) geometry.Left = 0;

            // Apply geometry to window.
            if (geometry.Top != null) window.Top = geometry.Top.Value;
            if (geometry.Left != null) window.Left = geometry.Left.Value;
            if (geometry.Width != null) window.Width = geometry.Width.Value;
            if (geometry.Height != null) window.Height = geometry.Height.Value;
            window.WindowState = geometry.State;
            window.StartPosition = geometry.Top != null
                ? FormStartPosition.Manual // Manually position or center.
                : FormStartPosition.CenterScreen;
        }

        private void Update(WindowsFormGeometry geometry)
        {
            var x = geometry.Left;
            var y = geometry.Top;
            var w = geometry.Width;
            var h = geometry.Height;

            string state;
            switch (geometry.State)
            {
                case FormWindowState.Normal:
                case FormWindowState.Minimized:
                    state = "Normal";
                    break;
                case FormWindowState.Maximized:
                    state = "Maximized";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Settings.Default[$"Geometry_{id}"] = $"{x}|{y}|{w}|{h}|{state}";
            Settings.Default.Save();
        }

        private WindowsFormGeometry Get()
        {
            var key = $"Geometry_{id}";
            if (!Settings.Default.ContainsKey(key) || string.IsNullOrEmpty(Settings.Default[key]))
            {
                return new WindowsFormGeometry
                {
                    Width = defaultWidth,
                    Height = defaultHeight
                };
            }

            var element = Settings.Default[key].Split('|');

            FormWindowState state;
            switch (element[4])
            {
                case "Normal":
                case "Minimised":
                    state = FormWindowState.Normal;
                    break;
                case "Maximized":
                    state = FormWindowState.Maximized;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new WindowsFormGeometry
            {
                Left = int.Parse(element[0]),
                Top = int.Parse(element[1]),
                Width = int.Parse(element[2]),
                Height = int.Parse(element[3]),
                State = state
            };
        }
    }
}
