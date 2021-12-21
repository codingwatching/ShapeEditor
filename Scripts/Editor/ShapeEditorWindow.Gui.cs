﻿#if UNITY_EDITOR

using System.Collections.Generic;
using Unity.Mathematics;

namespace AeternumGames.ShapeEditor
{
    public partial class ShapeEditorWindow
    {
        private List<GuiWindow> windows;

        /// <summary>Do not access directly, use <see cref="activeWindow"/>!</summary>
        private GuiWindow _activeWindow;

        /// <summary>The window that currently has input focus or null.</summary>
        internal GuiWindow activeWindow
        {
            get => _activeWindow;
            set
            {
                if (_activeWindow == value) return;

                if (_activeWindow != null)
                {
                    var lastActiveWindow = _activeWindow;
                    _activeWindow = value;
                    lastActiveWindow.OnFocusLost();
                }

                _activeWindow = value;
                _activeWindow?.OnFocus();
            }
        }

        public void DrawWindows()
        {
            if (windows == null)
            {
                windows = new List<GuiWindow>()
                {
                    new TopToolbarGuiWindow(this, float2.zero, float2.zero),
                    new BottomToolbarGuiWindow(this, float2.zero, float2.zero),
                    new ToolbarGuiWindow(this, new float2(20, 40), new float2(30, 400)),
                    new TextboxTestWindow(this, new float2(300, 100), new float2(220, 80)),
                };
            }

            var windowsCount = windows.Count;
            for (int i = 0; i < windowsCount; i++)
                windows[i].OnRender();
        }

        /// <summary>Attempts to find the top window at the given position.</summary>
        /// <param name="position">The position to find the window at.</param>
        /// <returns>The window instance if found else null.</returns>
        private GuiWindow FindWindowAtPosition(float2 position)
        {
            if (windows == null) return null;
            var windowsCount = windows.Count;
            for (int i = 0; i < windowsCount; i++)
                if (windows[i].rect.Contains(position))
                    return windows[i];
            return null;
        }
    }
}

#endif