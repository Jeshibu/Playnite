﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Playnite;
using Playnite.Common;
using Playnite.Settings;

namespace Playnite.Windows
{
    public class WindowPositionHandler
    {
        private Window window;
        private readonly string windowName;
        private readonly WindowPositions configuration;
        private bool ignoreChanges = false;
        private readonly bool saveSize;

        public WindowPositionHandler(Window window, string windowName, WindowPositions settings, bool saveSize = true)
        {
            this.window = window;
            this.windowName = windowName;
            this.saveSize = saveSize;
            configuration = settings;
            window.SizeChanged += Window_SizeChanged;
            window.LocationChanged += Window_LocationChanged;
            window.StateChanged += Window_StateChanged;
            window.Loaded += Window_Loaded;
            window.Closed += Window_Closed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            window.SizeChanged -= Window_SizeChanged;
            window.LocationChanged -= Window_LocationChanged;
            window.StateChanged -= Window_StateChanged;
            window.Loaded -= Window_Loaded;
            window.Closed -= Window_Closed;
            window = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreSizeAndLocation();
            window.Loaded -= Window_Loaded;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (window.IsLoaded)
            {
                SaveState();
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (window.IsLoaded)
            {
                SavePosition();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (window.IsLoaded)
            {
                SaveSize();
            }
        }

        private void MakeSureConfigEntryExists()
        {
            if (!configuration.Positions.ContainsKey(windowName))
            {
                configuration.Positions[windowName] = new WindowPosition();
            }
        }

        private void SaveState()
        {
            if (configuration == null || ignoreChanges)
            {
                return;
            }

            // Don't save minimized state. It would not be very user friendly if user exit Playnite while minimized
            // and it would then open minimized on next startup.
            if (window.WindowState == WindowState.Minimized)
            {
                return;
            }

            MakeSureConfigEntryExists();
            configuration.Positions[windowName].State = window.WindowState;
        }

        private void SaveSize()
        {
            if (!saveSize)
            {
                return;
            }

            if (configuration == null || ignoreChanges)
            {
                return;
            }

            // Don't save size if windows is maximized, it would be too large when it would restore back to normal state.
            // Don't save size if windows is minimized becuase it has no size :)
            if (window.WindowState != WindowState.Normal)
            {
                return;
            }

            MakeSureConfigEntryExists();
            configuration.Positions[windowName].Size = new WindowPosition.Point()
            {
                X = window.Width,
                Y = window.Height
            };
        }

        private void SavePosition()
        {
            if (configuration == null || ignoreChanges)
            {
                return;
            }

            MakeSureConfigEntryExists();
            configuration.Positions[windowName].Position = new WindowPosition.Point()
            {
                X = window.Left,
                Y = window.Top
            };
        }

        // TODO: this needs complete rework when per display DPI support is enabled in P11.
        // Stored position must be saved in DPI independent (in 100% scaling) coordinates and relative to active display,
        // not relative to the whole desktop. Otherwise it gets messy in mixed DPI multi-monitor environments.
        private void ConstrainWindow(int x, int y)
        {
            var dpi = VisualTreeHelper.GetDpi(window);
            var positioned = false;
            // Make sure that position is part of at least one connected screen
            foreach (var monitor in Computer.GetScreens())
            {
                var xTest = (int)(x * dpi.DpiScaleX);
                var yTest = (int)(y * dpi.DpiScaleY);
                if (monitor.WorkingArea.Contains(xTest, yTest))
                {
                    window.Left = x;
                    window.Top = y;
                    positioned = true;
                    break;
                }
                else if (monitor.WorkingArea.Contains(xTest + 8, yTest + 8))
                {
                    // 8 pixel offset is there for cases where a window is maximized using drag to top of the screen.
                    // Window's position is then, for some reason, set with -8,-8 pixel offset which would make constrain check to fail.
                    window.Left = x + 8;
                    window.Top = y + 8;
                    positioned = true;
                    break;
                }
            }

            if (!positioned)
            {
                window.Left = 0;
                window.Top = 0;
            }
        }

        private void RestoreSizeAndLocation()
        {
            if (!configuration.Positions.ContainsKey(windowName))
            {
                ConstrainWindow((int)window.Left, (int)window.Top);
                return;
            }

            ignoreChanges = true;

            try
            {
                var data = configuration.Positions[windowName];
                if (data.Position != null)
                {
                    ConstrainWindow((int)data.Position.X, (int)data.Position.Y);
                }

                if (saveSize)
                {
                    if (data.Size != null)
                    {
                        // If a window has some constrains set (like min size), then size change event is called when settings new value
                        // which overrides saved data because of Window_SizeChanged callback.
                        var width = data.Size.X;
                        var height = data.Size.Y;

                        if (width >= window.MinWidth)
                        {
                            window.Width = width;
                        }

                        if (height >= window.MinHeight)
                        {
                            window.Height = height;
                        }
                    }
                }

                window.WindowState = data.State;
            }
            finally
            {
                ignoreChanges = false;
            }
        }

        public bool HasSavedData()
        {
            return configuration.Positions.ContainsKey(windowName);
        }
    }
}
