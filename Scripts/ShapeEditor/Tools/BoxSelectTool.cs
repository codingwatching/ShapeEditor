﻿#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    public class BoxSelectTool : Tool
    {
        private bool isMarqueeActive;
        private bool isMarqueeSubtractive;
        private static readonly Color marqueeColor = new Color(1.0f, 0.5f, 0.0f);

        public override void OnActivate()
        {
            isMarqueeActive = false;
        }

        public override void OnRender()
        {
            GLUtilities.DrawGui(() =>
            {
                if (isMarqueeActive)
                {
                    // draw the marquee selection if active.
                    var marqueeBegin = editor.GridPointToScreen(editor.mouseGridInitialPosition);
                    var marqueeEnd = editor.GridPointToScreen(editor.mouseGridPosition);
                    var marqueeRect = MathEx.RectXYXY(marqueeBegin, marqueeEnd);

                    GLUtilities.DrawRectangleOutline(marqueeRect.x, marqueeRect.y, marqueeRect.width, marqueeRect.height, marqueeColor);
                }
            });
        }

        public override void OnGlobalMouseUp(int button)
        {
            if (button == 0)
            {
                // unless the shift key is held down we clear the selection.
                if (!editor.isShiftPressed && !isMarqueeSubtractive)
                    editor.project.ClearSelection();

                if (isMarqueeActive)
                {
                    // iterate over all segments within the marquee.
                    var marqueeRect = MathEx.RectXYXY(editor.mouseGridInitialPosition, editor.mouseGridPosition);
                    foreach (var segment in editor.ForEachSelectableInGridRect(marqueeRect))
                        segment.selected = !isMarqueeSubtractive;

                    // todo, check for edges in edge mode.
                    // todo, check for shapes in shape mode.
                }
                else
                {
                    switch (editor.shapeSelectMode)
                    {
                        case ShapeSelectMode.Vertex:
                            // find the closest segment to the click position.
                            var segment = editor.FindSegmentAtScreenPosition(editor.mousePosition, 60.0f);
                            if (segment != null)
                                segment.selected = !segment.selected;
                            break;

                        case ShapeSelectMode.Edge:
                            // find the closest edge to the click position.
                            var lineResult = new ShapeEditorWindow.FindSegmentLineResult();
                            if (editor.FindSegmentLineAtScreenPosition(editor.mousePosition, 60.0f, ref lineResult))
                            {
                                if (lineResult.segment1.selected && lineResult.segment2.selected)
                                {
                                    lineResult.segment1.selected = false;
                                    lineResult.segment2.selected = false;
                                }
                                else
                                {
                                    lineResult.segment1.selected = true;
                                    lineResult.segment2.selected = true;
                                }
                            }
                            break;

                        case ShapeSelectMode.Face:
                            // find what shape the click position is inside of.
                            var shape = editor.FindShapeAtScreenPosition(editor.mousePosition);
                            if (shape != null)
                            {
                                if (shape.IsSelected())
                                {
                                    shape.ClearSelection();
                                }
                                else
                                {
                                    shape.SelectAll();
                                }
                            }
                            break;
                    }
                }

                isMarqueeActive = false;
            }
        }

        public override void OnMouseDrag(int button, float2 screenDelta, float2 gridDelta)
        {
            if (button == 0)
            {
                // once the mouse moves a bit from the initial position the marquee activates.
                if (!isMarqueeActive)
                {
                    isMarqueeActive = (math.distance(editor.mouseInitialPosition, editor.mousePosition) > 3.0f);
                    isMarqueeSubtractive = editor.isCtrlPressed;
                }
            }
        }

        public override bool OnKeyDown(KeyCode keyCode)
        {
            // these single-use tools are not available while in single-use mode.
            if (isSingleUse) return false;

            switch (keyCode)
            {
                case KeyCode.G:
                    if (editor.selectedSegmentsCount > 0)
                    {
                        editor.UseTool(new TranslateTool());
                        return true;
                    }
                    return false;

                case KeyCode.S:
                    if (editor.selectedSegmentsCount > 1)
                    {
                        editor.UseTool(new ScaleTool());
                        return true;
                    }
                    return false;

                case KeyCode.R:
                    if (editor.selectedSegmentsCount > 1)
                    {
                        editor.UseTool(new RotateTool());
                        return true;
                    }
                    return false;

                case KeyCode.C:
                    editor.UseTool(new CutTool());
                    return false;

                case KeyCode.B:
                    if (editor.selectedSegmentsCount > 0)
                    {
                        editor.ToggleBezierTest();
                        return true;
                    }
                    return false;

                case KeyCode.N:
                    if (editor.selectedSegmentsCount > 0)
                    {
                        editor.ToggleSineTest();
                        return true;
                    }
                    return false;

                case KeyCode.V:
                    if (editor.selectedSegmentsCount > 0)
                    {
                        editor.ApplyGeneratorTest();
                        return true;
                    }
                    return false;
            }
            return false;
        }
    }
}

#endif