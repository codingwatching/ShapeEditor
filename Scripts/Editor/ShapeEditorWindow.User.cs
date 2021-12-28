﻿#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    public partial class ShapeEditorWindow
    {
        // general purpose user functions, typically invoked through the gui or keyboard.

        /// <summary>Deletes the selected objects from the project.</summary>
        internal void UserDeleteSelection()
        {
            RegisterUndo("Delete Selection");

            // for every shape in the project:
            var shapesCount = project.shapes.Count;
            for (int i = shapesCount; i-- > 0;)
            {
                var shape = project.shapes[i];

                // for every segment in the project:
                var segments = shape.segments;
                var segmentsCount = segments.Count;
                for (int j = segmentsCount; j-- > 0;)
                    if (segments[j].selected)
                        shape.RemoveSegment(segments[j]);

                // remove the shape if it's empty.
                if (segments.Count == 0)
                    project.shapes.RemoveAt(i);
            }
        }

        internal void UserToggleBezierTest()
        {
            // for every shape in the project:
            var shapesCount = project.shapes.Count;
            for (int i = shapesCount; i-- > 0;)
            {
                var shape = project.shapes[i];

                // for every segment in the project:
                var segments = shape.segments;
                var segmentsCount = segments.Count;
                for (int j = segmentsCount; j-- > 0;)
                {
                    if (segments[j].selected)
                    {
                        // get the current segment and the next segment (wrapping around).
                        var segment = shape.segments[j];

                        if (segment.generator.type != SegmentGeneratorType.Bezier)
                        {
                            segment.generator = new SegmentGenerator(segment, SegmentGeneratorType.Bezier);
                        }
                        else
                        {
                            segment.generator = new SegmentGenerator(segment);
                        }
                    }
                }
            }
        }

        internal void UserToggleSineTest()
        {
            // for every shape in the project:
            var shapesCount = project.shapes.Count;
            for (int i = shapesCount; i-- > 0;)
            {
                var shape = project.shapes[i];

                // for every segment in the project:
                var segments = shape.segments;
                var segmentsCount = segments.Count;
                for (int j = segmentsCount; j-- > 0;)
                {
                    if (segments[j].selected)
                    {
                        // get the current segment and the next segment (wrapping around).
                        var segment = shape.segments[j];

                        if (segment.generator.type != SegmentGeneratorType.Sine)
                        {
                            segment.generator = new SegmentGenerator(segment, SegmentGeneratorType.Sine);
                        }
                        else
                        {
                            segment.generator = new SegmentGenerator(segment);
                        }
                    }
                }
            }
        }

        internal void UserToggleRepeatTest()
        {
            // for every shape in the project:
            var shapesCount = project.shapes.Count;
            for (int i = shapesCount; i-- > 0;)
            {
                var shape = project.shapes[i];

                // for every segment in the project:
                var segments = shape.segments;
                var segmentsCount = segments.Count;
                for (int j = segmentsCount; j-- > 0;)
                {
                    if (segments[j].selected)
                    {
                        // get the current segment and the next segment (wrapping around).
                        var segment = shape.segments[j];

                        if (segment.generator.type != SegmentGeneratorType.Repeat)
                        {
                            segment.generator = new SegmentGenerator(segment, SegmentGeneratorType.Repeat);
                        }
                        else
                        {
                            segment.generator = new SegmentGenerator(segment);
                        }
                    }
                }
            }
        }

        internal void UserApplyGeneratorTest()
        {
            // for every shape in the project:
            var shapesCount = project.shapes.Count;
            for (int i = shapesCount; i-- > 0;)
            {
                var shape = project.shapes[i];

                // for every segment in the project:
                var segments = shape.segments;
                var segmentsCount = segments.Count;
                for (int j = segmentsCount; j-- > 0;)
                {
                    if (segments[j].selected)
                    {
                        // get the current segment and the next segment (wrapping around).
                        var segment = shape.segments[j];

                        segment.generator.ApplyGenerator();
                        segment.generator = new SegmentGenerator(segment);
                    }
                }
            }
        }

        internal void UserAssignProjectToTargets()
        {
            var transform = Selection.activeTransform;
            if (transform)
            {
                var target = transform.GetComponent<ShapeEditorTarget>();
                if (target)
                {
                    target.OnShapeEditorUpdateProject(project);
                }
            }
        }

        internal void UserAddShapeToProject()
        {
            project.shapes.Add(new Shape());
        }

        /// <summary>Switches to the box select tool unless already active.</summary>
        internal void UserSwitchToBoxSelectTool() => SwitchTool(boxSelectTool);

        /// <summary>Switches to the translate tool unless already active.</summary>
        internal void UserSwitchToTranslateTool() => SwitchTool(translateTool);

        /// <summary>Switches to the rotate tool unless already active.</summary>
        internal void UserSwitchToRotateTool() => SwitchTool(rotateTool);

        /// <summary>Switches to the scale tool unless already active.</summary>
        internal void UserSwitchToScaleTool() => SwitchTool(scaleTool);

        /// <summary>Switches to the cut tool unless already active.</summary>
        internal void UserSwitchToCutTool() => SwitchTool(cutTool);

        /// <summary>Switches to the vertex select mode.</summary>
        internal void UserSwitchToVertexSelectMode() => shapeSelectMode = ShapeSelectMode.Vertex;

        /// <summary>Switches to the edge select mode.</summary>
        internal void UserSwitchToEdgeSelectMode() => shapeSelectMode = ShapeSelectMode.Edge;

        /// <summary>Switches to the face select mode.</summary>
        internal void UserSwitchToFaceSelectMode() => shapeSelectMode = ShapeSelectMode.Face;

        internal void UserNewProject()
        {
            NewProject();
        }

        /// <summary>Displays a file open dialog to load a project file.</summary>
        internal void UserOpenProject()
        {
            string path = EditorUtility.OpenFilePanel("Load 2D Shape Editor Project", "", "s2d,sabre2d");
            if (path.Length != 0)
            {
                OpenProject(path);
            }
        }

        /// <summary>Displays a file save dialog to save a project file.</summary>
        internal void UserSaveProjectAs()
        {
            try
            {
                string path = EditorUtility.SaveFilePanel("Save 2D Shape Editor Project", "", "Project", "s2d");
                if (path.Length != 0)
                {
                    SaveProject(path);
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("2D Shape Editor", "An exception occured while saving the project:\r\n" + ex.Message, "Ohno!");
            }
        }

        /// <summary>Closes the shape editor window.</summary>
        internal void UserExitShapeEditor()
        {
            Close();
        }

        internal void UserShowTextboxTestWindow()
        {
            OpenWindow(new TextboxTestWindow(new float2(300, 100), new float2(220, 80)), false);
        }

        /// <summary>Displays the about window.</summary>
        internal void UserShowAboutWindow()
        {
            OpenWindow(new AboutGuiWindow());
        }

        /// <summary>Opens the GitHub repository in a browser window.</summary>
        internal void UserOpenGitHubRepository()
        {
            Application.OpenURL("https://github.com/Henry00IS/ShapeEditor");
        }

        /// <summary>Opens the GitHub repository wiki in a browser window.</summary>
        internal void UserOpenOnlineManual()
        {
            Application.OpenURL("https://github.com/Henry00IS/ShapeEditor/wiki");
        }

        /// <summary>Called on CTRL+Z when the editor window has focus.</summary>
        internal void UserUndo()
        {
            OnUndo();
        }

        /// <summary>Called on CTRL+Y when the editor window has focus.</summary>
        internal void UserRedo()
        {
            OnRedo();
        }

        /// <summary>Snaps the selected objects to the grid.</summary>
        internal void UserSnapSelectionToGrid()
        {
            RegisterUndo("Snap Selection To Grid");
            foreach (var selectable in ForEachSelectedObject())
                selectable.position = selectable.position.Snap(gridSnap);
        }

        /// <summary>Centers the camera by resetting the grid offset and zoom.</summary>
        internal void UserResetCamera()
        {
            GridResetOffset();
            GridResetZoom();
        }
    }
}

#endif