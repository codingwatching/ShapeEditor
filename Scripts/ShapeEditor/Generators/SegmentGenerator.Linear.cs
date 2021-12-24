﻿#if UNITY_EDITOR

namespace AeternumGames.ShapeEditor
{
    public partial class SegmentGenerator
    {
        // a segment generator that generates straight lines between two segments.

        private void Linear_DrawSegments()
        {
            // we just draw manually here.
            var editor = ShapeEditorWindow.Instance;
            var p1 = editor.GridPointToScreen(segment.position);
            var p2 = editor.GridPointToScreen(segment.next.position);
            GLUtilities.DrawLine(1.0f, p1.x, p1.y, p2.x, p2.y, segment.selected ? ShapeEditorWindow.segmentPivotOutlineColor : ShapeEditorWindow.segmentColor, segment.next.selected ? ShapeEditorWindow.segmentPivotOutlineColor : ShapeEditorWindow.segmentColor);
        }
    }
}

#endif