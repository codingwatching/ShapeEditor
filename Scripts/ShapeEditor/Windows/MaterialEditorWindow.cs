#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using GLUtilities3D = AeternumGames.ShapeEditor.GLUtilities.GLUtilities3D;

namespace AeternumGames.ShapeEditor
{
    /// <summary>The 3D material editor window.</summary>
    public partial class MaterialEditorWindow : GuiResizableWindow
    {
        private static readonly Color32[] materialIndexToColor = {
            new Color32(255, 255, 255, 255),
            new Color32(0, 0, 255, 255),
            new Color32(0, 255, 0, 255),
            new Color32(255, 0, 0, 255),
            new Color32(0, 255, 255, 255),
            new Color32(255, 255, 0, 255),
            new Color32(255, 0, 255, 255),
            new Color32(255, 165, 0, 255),
        };

        private static readonly float2 windowSize = new float2(360, 290);
        private GuiMaterialEditorViewport viewport;

        public MaterialEditorWindow() : base(float2.zero, windowSize) { }

        public override void OnActivate()
        {
            base.OnActivate();

            var resources = ShapeEditorResources.Instance;
            colorWindowBackground = new Color(0.192f, 0.192f, 0.192f);
            position = GetCenterPosition();

            ValidateTools();

            Add(new GuiWindowTitle("Material Editor"));
            Add(viewport = new GuiMaterialEditorViewport(new float2(1, 63), new float2(windowSize.x - 2, windowSize.y - 64)));
            OpenChildWindow(new GuiMaterialEditorTopToolbarWindow(new float2(1, 21), float2.zero));
            OpenChildWindow(new GuiMaterialEditorToolbarWindow(new float2(21, 85)));
        }

        private float2 GetCenterPosition()
        {
            return new float2(
                Mathf.RoundToInt((editor.width / 2f) - (windowSize.x / 2f)),
                Mathf.RoundToInt((editor.height / 2f) - (windowSize.y / 2f))
            );
        }

        protected override void OnResize()
        {
            viewport.size = new float2(size.x - 2, size.y - 64);
        }

        [Instructions(title: "Reset all surfaces to material index number one.", description: "Resets all materials assignments in the project to the default material slot which appears as white.")]
        private void UserResetMaterials()
        {
            // for every shape in the project:
            var project = editor.project;
            var shapesCount = project.shapes.Count;
            for (int i = 0; i < shapesCount; i++)
            {
                var shape = project.shapes[i];

                // reset the material index.
                shape.frontMaterial = 0;
                shape.backMaterial = 0;

                // for every edge in the shape:
                var segmentCount = shape.segments.Count;
                for (int j = 0; j < segmentCount; j++)
                {
                    // reset the material index.
                    var segment = shape.segments[j];
                    segment.material = 0;
                }
            }
        }

        private class GuiMaterialEditorTopToolbarWindow : GuiWindow
        {
            private MaterialEditorWindow materialEditor => parent as MaterialEditorWindow;

            private GuiMaterialIndexButton buttonSetBrushMaterial1;
            private GuiMaterialIndexButton buttonSetBrushMaterial2;
            private GuiMaterialIndexButton buttonSetBrushMaterial3;
            private GuiMaterialIndexButton buttonSetBrushMaterial4;
            private GuiMaterialIndexButton buttonSetBrushMaterial5;
            private GuiMaterialIndexButton buttonSetBrushMaterial6;
            private GuiMaterialIndexButton buttonSetBrushMaterial7;
            private GuiMaterialIndexButton buttonSetBrushMaterial8;

            public GuiMaterialEditorTopToolbarWindow(float2 position, float2 size) : base(position, size) { }

            private GuiHorizontalLayout horizontalLayout;

            public override void OnActivate()
            {
                base.OnActivate();

                var resources = ShapeEditorResources.Instance;
                colorWindowBackground = new Color(0.192f, 0.192f, 0.192f);

                var menu = new GuiMenuStrip();
                Add(menu);

                var viewMenu = menu.Add("View");
                viewMenu.Add("Reset Camera", materialEditor.viewport.camera.UserResetCamera);

                horizontalLayout = new GuiHorizontalLayout(this, 1, 21);
                horizontalLayout.Add(new GuiButton(resources.shapeEditorNew, 20, materialEditor.UserResetMaterials));
                horizontalLayout.Space(5);
                horizontalLayout.Add(buttonSetBrushMaterial1 = new GuiMaterialIndexButton("1", 20, materialIndexToColor[0], materialEditor.viewport.UserSetBrushMaterial1));
                horizontalLayout.Add(buttonSetBrushMaterial2 = new GuiMaterialIndexButton("2", 20, materialIndexToColor[1], materialEditor.viewport.UserSetBrushMaterial2));
                horizontalLayout.Add(buttonSetBrushMaterial3 = new GuiMaterialIndexButton("3", 20, materialIndexToColor[2], materialEditor.viewport.UserSetBrushMaterial3));
                horizontalLayout.Add(buttonSetBrushMaterial4 = new GuiMaterialIndexButton("4", 20, materialIndexToColor[3], materialEditor.viewport.UserSetBrushMaterial4));
                horizontalLayout.Add(buttonSetBrushMaterial5 = new GuiMaterialIndexButton("5", 20, materialIndexToColor[4], materialEditor.viewport.UserSetBrushMaterial5));
                horizontalLayout.Add(buttonSetBrushMaterial6 = new GuiMaterialIndexButton("6", 20, materialIndexToColor[5], materialEditor.viewport.UserSetBrushMaterial6));
                horizontalLayout.Add(buttonSetBrushMaterial7 = new GuiMaterialIndexButton("7", 20, materialIndexToColor[6], materialEditor.viewport.UserSetBrushMaterial7));
                horizontalLayout.Add(buttonSetBrushMaterial8 = new GuiMaterialIndexButton("8", 20, materialIndexToColor[7], materialEditor.viewport.UserSetBrushMaterial8));
            }

            public override void OnRender()
            {
                size = new float2(parent.size.x - 2, 42f);

                buttonSetBrushMaterial1.isChecked = materialEditor.viewport.materialIndex == 0;
                buttonSetBrushMaterial2.isChecked = materialEditor.viewport.materialIndex == 1;
                buttonSetBrushMaterial3.isChecked = materialEditor.viewport.materialIndex == 2;
                buttonSetBrushMaterial4.isChecked = materialEditor.viewport.materialIndex == 3;
                buttonSetBrushMaterial5.isChecked = materialEditor.viewport.materialIndex == 4;
                buttonSetBrushMaterial6.isChecked = materialEditor.viewport.materialIndex == 5;
                buttonSetBrushMaterial7.isChecked = materialEditor.viewport.materialIndex == 6;
                buttonSetBrushMaterial8.isChecked = materialEditor.viewport.materialIndex == 7;

                base.OnRender();
            }
        }

        /// <summary>Represents the 3D viewport of the material editor.</summary>
        private class GuiMaterialEditorViewport : GuiViewport
        {
            private MaterialEditorWindow materialEditor => parent as MaterialEditorWindow;

            private CameraFirstPerson firstPersonCamera;

            private Mesh mesh;
            public MeshRaycast meshRaycast;
            private MeshColors meshColors;
            public MeshTriangleLookupTable lookupTable;
            public byte materialIndex { get; private set; }
            public byte materialIndexUnderMouse = 255;

            public GuiMaterialEditorViewport(float2 size) : base(size)
            {
            }

            public GuiMaterialEditorViewport(float2 position, float2 size) : base(position, size)
            {
            }

            // use a first-person camera.
            public override Camera camera => firstPersonCamera ??= new CameraFirstPerson(editor);

            public override bool IsBusy() => materialEditor.activeTool.IsBusy();

            public override void OnMouseDown(int button) => materialEditor.activeTool.OnMouseDown(button);

            public override void OnMouseUp(int button) => materialEditor.activeTool.OnMouseUp(button);

            public override void OnGlobalMouseUp(int button) => materialEditor.activeTool.OnGlobalMouseUp(button);

            public override void OnMouseDrag(int button, float2 screenDelta, float2 gridDelta) => materialEditor.activeTool.OnMouseDrag(button, screenDelta, gridDelta);

            public override void OnGlobalMouseDrag(int button, float2 screenDelta, float2 gridDelta)
            {
                materialEditor.activeTool.OnGlobalMouseDrag(button, screenDelta, gridDelta);
                base.OnGlobalMouseDrag(button, screenDelta, gridDelta);
            }

            public override void OnMouseMove(float2 screenDelta, float2 gridDelta) => materialEditor.activeTool.OnMouseMove(screenDelta, gridDelta);

            public override bool OnMouseScroll(float delta) => materialEditor.activeTool.OnMouseScroll(delta);

            public override bool OnKeyDown(KeyCode keyCode)
            {
                switch (keyCode)
                {
                    case KeyCode.Alpha1: UserSetBrushMaterial1(); return true;
                    case KeyCode.Alpha2: UserSetBrushMaterial2(); return true;
                    case KeyCode.Alpha3: UserSetBrushMaterial3(); return true;
                    case KeyCode.Alpha4: UserSetBrushMaterial4(); return true;
                    case KeyCode.Alpha5: UserSetBrushMaterial5(); return true;
                    case KeyCode.Alpha6: UserSetBrushMaterial6(); return true;
                    case KeyCode.Alpha7: UserSetBrushMaterial7(); return true;
                    case KeyCode.Alpha8: UserSetBrushMaterial8(); return true;
                }
                if (materialEditor.activeTool.OnKeyDown(keyCode))
                    return true;
                return base.OnKeyDown(keyCode);
            }

            public override bool OnKeyUp(KeyCode keyCode)
            {
                if (materialEditor.activeTool.OnKeyUp(keyCode))
                    return true;
                return base.OnKeyUp(keyCode);
            }

            public override void OnFocus()
            {
                if (isMouseOver)
                {
                    RebuildMesh();
                }

                materialEditor.activeTool.OnFocus();
            }

            public override void OnFocusLost() => materialEditor.activeTool.OnFocusLost();

            protected override void OnPreRender()
            {
                if (mesh == null)
                    RebuildMesh();

                UpdateMeshColors();

                materialEditor.activeTool.OnPreRender();
            }

            protected override void OnPreRender2D()
            {
                materialEditor.activeTool.OnPreRender2D();
            }

            protected override void OnRender3D()
            {
                GLUtilities3D.DrawGuiTextured(ShapeEditorResources.Instance.shapeEditorDefaultMaterial.mainTexture, camera.transform.position, () =>
                {
                    Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
                });

                materialEditor.activeTool.OnRender3D();
            }

            private GuiMaterialIndexButton guiMaterialIndexTooltipButton = new GuiMaterialIndexButton("1", 20, materialIndexToColor[0], null);

            private void RenderMaterialIndexTooltip(byte materialIndexUnderMouse)
            {
                guiMaterialIndexTooltipButton.editor = editor;
                guiMaterialIndexTooltipButton.color = materialIndexToColor[materialIndexUnderMouse];
                guiMaterialIndexTooltipButton.text = (materialIndexUnderMouse + 1).ToString();
                guiMaterialIndexTooltipButton.position = mousePosition + new float2(10, 10);
                guiMaterialIndexTooltipButton.OnRender();
            }

            protected override void OnPostRender2D()
            {
                materialEditor.activeTool.OnPostRender2D();

                if (materialIndexUnderMouse != 255)
                    RenderMaterialIndexTooltip(materialIndexUnderMouse);
            }

            protected override void OnPostRender()
            {
                materialEditor.activeTool.OnPostRender();
            }

            private void RebuildMesh()
            {
                // ensure the project data is ready.
                editor.project.Validate();
                var convexPolygons2D = editor.project.GenerateConvexPolygons();
                convexPolygons2D.CalculateBounds2D();
                mesh = MeshGenerator.CreateExtrudedPolygonMesh(convexPolygons2D, 0.5f);
                meshRaycast = new MeshRaycast(mesh);
                lookupTable = new MeshTriangleLookupTable(meshRaycast.Triangles, meshRaycast.Vertices, editor.project);
                meshColors = new MeshColors(mesh);
            }

            private void UpdateMeshColors()
            {
                var vertices = meshRaycast.Vertices;
                var triangles = meshRaycast.Triangles;

                // find all triangles that are part of the edge.
                for (int k = 0; k < triangles.Length; k += 3)
                {
                    Color32 color = new Color32(255, 255, 255, 255);

                    if (lookupTable.TryGetSegmentsForTriangleIndex(k, out var segments))
                    {
                        color = materialIndexToColor[segments[0].material];
                    }
                    else if (lookupTable.TryGetShapesForTriangleIndex(k, out var shapes))
                    {
                        var v1 = vertices[triangles[k]];
                        var v2 = vertices[triangles[k + 1]];
                        var v3 = vertices[triangles[k + 2]];
                        var plane = new Plane(v1, v2, v3);

                        if (plane.normal.z < 0.5f)
                        {
                            color = materialIndexToColor[shapes[0].frontMaterial];
                        }
                        else if (plane.normal.z > 0.5f)
                        {
                            color = materialIndexToColor[shapes[0].backMaterial];
                        }
                    }

                    meshColors[triangles[k]] = color;
                    meshColors[triangles[k + 1]] = color;
                    meshColors[triangles[k + 2]] = color;
                }
                meshColors.UpdateMesh();
            }

            [Instructions(title: "Draw with material index number one.", shortcut: "1 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).\n\nThis is the default material slot and appears as white.")]
            public void UserSetBrushMaterial1() => UserSetBrushMaterial(0);

            [Instructions(title: "Draw with material index number two.", shortcut: "2 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial2() => UserSetBrushMaterial(1);

            [Instructions(title: "Draw with material index number three.", shortcut: "3 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial3() => UserSetBrushMaterial(2);

            [Instructions(title: "Draw with material index number four.", shortcut: "4 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial4() => UserSetBrushMaterial(3);

            [Instructions(title: "Draw with material index number five.", shortcut: "5 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial5() => UserSetBrushMaterial(4);

            [Instructions(title: "Draw with material index number six.", shortcut: "6 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial6() => UserSetBrushMaterial(5);

            [Instructions(title: "Draw with material index number seven.", shortcut: "7 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial7() => UserSetBrushMaterial(6);

            [Instructions(title: "Draw with material index number eight.", shortcut: "8 key", description: "You can draw this material index with the left mouse button on the mesh in the 3D viewport. Once you have extruded your shape, you can assign materials in the scene to these slots (indicated here by colored areas).")]
            public void UserSetBrushMaterial8() => UserSetBrushMaterial(7);

            /// <summary>Sets the material index used by the brush.</summary>
            /// <param name="materialIndex">The material index to be used.</param>
            private void UserSetBrushMaterial(byte materialIndex)
            {
                this.materialIndex = materialIndex;
            }
        }

        private class MeshColors
        {
            /// <summary>The mesh that will be updated.</summary>
            private Mesh mesh;

            /// <summary>An array containing all triangles in the mesh.</summary>
            private int[] triangles;

            /// <summary>An array containing all vertices in the mesh.</summary>
            private Vector3[] vertices;

            /// <summary>An array containing all colors in the mesh.</summary>
            private Color32[] colors;

            /// <summary>Whether the color array has been updated.</summary>
            private bool dirty;

            /// <summary>Gets an array containing all triangles in the mesh.</summary>
            public int[] Triangles => triangles;

            /// <summary>Gets an array containing all vertices in the mesh.</summary>
            public Vector3[] Vertices => vertices;

            /// <summary>Gets or sets the color for the specified vertex index.</summary>
            /// <param name="i">The vertex index</param>
            /// <returns>The color of the vertex at the specified index.</returns>
            public Color32 this[int i]
            {
                get => colors[i];
                set
                {
                    // only mark dirty if the assigned color is different.
                    if (!colors[i].Equals(value))
                        dirty = true;
                    colors[i] = value;
                }
            }

            /// <summary>Creates a new instance and assigns white vertex colors to the mesh.</summary>
            /// <param name="mesh">The mesh to have editable vertex colors.</param>
            public MeshColors(Mesh mesh)
            {
                this.mesh = mesh;
                triangles = mesh.triangles;
                vertices = mesh.vertices;
                colors = new Color32[vertices.Length];

                var white = new Color32(255, 255, 255, 255);
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = white;
                mesh.colors32 = colors;
            }

            /// <summary>Updates the mesh colors if they were changed.</summary>
            public void UpdateMesh()
            {
                if (dirty)
                {
                    dirty = false;
                    mesh.colors32 = colors;
                }
            }
        }

        private class MeshTriangleLookupTable
        {
            /// <summary>The project containing all shapes and segments.</summary>
            private Project project;

            /// <summary>An array containing all triangles in the mesh.</summary>
            private int[] triangles;

            /// <summary>An array containing all vertices in the mesh.</summary>
            private Vector3[] vertices;

            /// <summary>Gets an array containing all triangles in the mesh.</summary>
            public int[] Triangles => triangles;

            /// <summary>Gets an array containing all vertices in the mesh.</summary>
            public Vector3[] Vertices => vertices;

            /// <summary>The internal dictionary of triangle indices for a segment.</summary>
            private Dictionary<Segment, List<int>> segmentTriangles = new Dictionary<Segment, List<int>>();

            /// <summary>The internal dictionary of segments for a triangle index.</summary>
            private Dictionary<int, List<Segment>> triangleSegments = new Dictionary<int, List<Segment>>();

            /// <summary>The internal dictionary of shapes for a triangle index.</summary>
            private Dictionary<int, List<Shape>> triangleShapes = new Dictionary<int, List<Shape>>();

            public MeshTriangleLookupTable(Mesh mesh, Project project)
            {
                this.project = project;
                triangles = mesh.triangles;
                vertices = mesh.vertices;

                CalculateSegmentLookupTables();
            }

            public MeshTriangleLookupTable(int[] triangles, Vector3[] vertices, Project project)
            {
                this.project = project;
                this.triangles = triangles;
                this.vertices = vertices;

                CalculateSegmentLookupTables();
                CalculateShapeLookupTables();
            }

            private void CalculateSegmentLookupTables()
            {
                // for every shape in the project:
                var shapesCount = project.shapes.Count;
                for (int i = 0; i < shapesCount; i++)
                {
                    var shape = project.shapes[i];

                    // for every edge in the shape:
                    var segmentCount = shape.segments.Count;
                    for (int j = 0; j < segmentCount; j++)
                    {
                        // get the current segment.
                        var segment = shape.segments[j];

                        // find all triangles that are part of the edge.
                        for (int k = 0; k < triangles.Length; k += 3)
                        {
                            var v1 = vertices[triangles[k]];
                            var v2 = vertices[triangles[k + 1]];
                            var v3 = vertices[triangles[k + 2]];
                            var plane = new Plane(v1, v2, v3);

                            if (plane.normal == Vector3.zero)
                                continue;

                            // the triangle must not be facing front or back.
                            if (plane.normal.z.EqualsWithEpsilon5(0.0f))
                            {
                                // flatten the triangle vertices into 2D space.

                                v1.z = 0.0f;
                                v2.z = 0.0f;
                                v3.z = 0.0f;
                                var v1to2 = math.distance(v1, v2);
                                var v1to3 = math.distance(v1, v3);
                                var v2to3 = math.distance(v2, v3);
                                float2 p1;
                                float2 p2;

                                // find the triangle edge with the most 2D X&Y movement:

                                if (v1to2 > v1to3) // v1to3 out
                                {
                                    if (v1to2 > v2to3) // v2to3 out
                                    {
                                        p1 = new float2(v1.x, -v1.y);
                                        p2 = new float2(v2.x, -v2.y);
                                    }
                                    else // v1to2 out
                                    {
                                        p1 = new float2(v2.x, -v2.y);
                                        p2 = new float2(v3.x, -v3.y);
                                    }
                                }
                                else // v1to2 out
                                {
                                    if (v1to3 > v2to3) // v2to3 out
                                    {
                                        p1 = new float2(v1.x, -v1.y);
                                        p2 = new float2(v3.x, -v3.y);
                                    }
                                    else // v1to3 out
                                    {
                                        p1 = new float2(v2.x, -v2.y);
                                        p2 = new float2(v3.x, -v3.y);
                                    }
                                }

                                // associates the current triangle index with the current segment.
                                System.Action associate = () =>
                                {
                                    if (segmentTriangles.TryGetValue(segment, out var triangles))
                                        triangles.Add(k);
                                    else
                                        segmentTriangles.Add(segment, new List<int>() { k });

                                    if (triangleSegments.TryGetValue(k, out var segments))
                                        segments.Add(segment);
                                    else
                                        triangleSegments.Add(k, new List<Segment>() { segment });
                                };

                                // given two points checks whether they both lie on the triangle edge we chose.
                                System.Func<float2, float2, bool> check = (a, b)
                                    => (MathEx.IsPointOnLine2(a, p1, p2, 0.0001403269f)
                                    && MathEx.IsPointOnLine2(b, p1, p2, 0.0001403269f))
                                    || (MathEx.IsPointOnLine2(p1, a, b, 0.0001403269f)
                                    && MathEx.IsPointOnLine2(p2, a, b, 0.0001403269f));

                                // iterate over all points of the edge (including the segment generator):
                                float2 last = segment.position;
                                foreach (var point in segment.generator.ForEachAdditionalSegmentPoint())
                                {
                                    // if this segment lies on the triangle edge:
                                    if (check(last, point))
                                    {
                                        associate();
                                        break;
                                    }
                                    last = point;
                                }
                                if (check(last, segment.next.position))
                                    associate();
                            }
                        }
                    }
                }
            }

            private void CalculateShapeLookupTables()
            {
                // iterate over all triangles:
                for (int k = 0; k < triangles.Length; k += 3)
                {
                    var v1 = vertices[triangles[k]];
                    var v2 = vertices[triangles[k + 1]];
                    var v3 = vertices[triangles[k + 2]];
                    var plane = new Plane(v1, v2, v3);

                    if (plane.normal == Vector3.zero)
                        continue;

                    // the triangle must be facing front or back.
                    if (!plane.normal.z.EqualsWithEpsilon5(0.0f))
                    {
                        // flatten the triangle vertices into 2D space.
                        v1.z = 0.0f;
                        v2.z = 0.0f;
                        v3.z = 0.0f;

                        // find the center of the triangle.
                        var center = (v1 + v2 + v3) / 3f;
                        center.y = -center.y;

                        // for every shape in the project:
                        var shapesCount = project.shapes.Count;
                        for (int i = shapesCount; i-- > 0;)
                        {
                            var shape = project.shapes[i];

                            // if the center point of the triangle is inside of the shape:
                            if (shape.ContainsPoint(center) >= 0)
                            {
                                if (triangleShapes.TryGetValue(k, out var shapes))
                                    shapes.Add(shape);
                                else
                                    triangleShapes.Add(k, new List<Shape>() { shape });

                                // respect the shape sorting.
                                break;
                            }
                        }
                    }
                }
            }

            /// <summary>Looks up all triangle indices associated with the specified segment.</summary>
            /// <param name="segment">The segment to find triangle indices for.</param>
            /// <param name="triangles">The triangle indices that lie on the edge.</param>
            /// <returns>True when the segment was found else false.</returns>
            public bool TryGetTrianglesForSegment(Segment segment, out List<int> triangles)
            {
                if (segment == null) { triangles = null; return false; }
                return segmentTriangles.TryGetValue(segment, out triangles);
            }

            /// <summary>Looks up all segments associated with the specified triangle index.</summary>
            /// <param name="triangleIndex">The triangle index to find segments for.</param>
            /// <param name="segments">The segments that lie on the triangle edge.</param>
            /// <returns>True when the triangle index was found else false.</returns>
            public bool TryGetSegmentsForTriangleIndex(int triangleIndex, out List<Segment> segments)
            {
                return triangleSegments.TryGetValue(triangleIndex, out segments);
            }

            /// <summary>Looks up all shapes associated with the specified triangle index.</summary>
            /// <param name="triangleIndex">The triangle index to find segments for.</param>
            /// <param name="shapes">The shapes that contain the the triangle center.</param>
            /// <returns>True when the triangle index was found else false.</returns>
            public bool TryGetShapesForTriangleIndex(int triangleIndex, out List<Shape> shapes)
            {
                return triangleShapes.TryGetValue(triangleIndex, out shapes);
            }
        }

        /// <summary>Displays a small color line at the bottom of the button.</summary>
        private class GuiMaterialIndexButton : GuiButton
        {
            /// <summary>Gets or sets the color of the line.</summary>
            public Color32 color { get; set; }

            public GuiMaterialIndexButton(string text, float2 size, Color32 color, System.Action onClick) : base(text, size, onClick)
            {
                this.color = color;
            }

            public override void OnRender()
            {
                base.OnRender();

                GLUtilities.DrawGui(() =>
                {
                    var rect = drawRect;

                    GL.Color(color);
                    GLUtilities.DrawRectangle(rect.x + 2, rect.yMax - 4, rect.width - 4, 2);
                });
            }
        }

        public class GuiMaterialEditorToolbarWindow : GuiWindow
        {
            private MaterialEditorWindow materialEditor => parent as MaterialEditorWindow;

            private GuiButton cameraButton;
            private GuiButton brushButton;

            public GuiMaterialEditorToolbarWindow(float2 position) : base(position, float2.zero) { }

            private GuiVerticalLayout verticalLayout;

            public override void OnActivate()
            {
                base.OnActivate();

                verticalLayout = new GuiVerticalLayout(this);

                verticalLayout.Add(cameraButton = new GuiButton(ShapeEditorResources.Instance.shapeEditorSelectBox, 28, materialEditor.UserSwitchToCameraTool));
                verticalLayout.Add(brushButton = new GuiButton(ShapeEditorResources.Instance.shapeEditorDraw, 28, materialEditor.UserSwitchToBrushTool));

                size = verticalLayout.windowSize;
            }

            public override void OnRender()
            {
                Type type = materialEditor.activeTool.GetType();
                cameraButton.isChecked = type == typeof(GuiMaterialEditorCameraTool);
                brushButton.isChecked = type == typeof(GuiMaterialEditorBrushTool);

                base.OnRender();
            }
        }

        internal void UserSwitchToCameraTool() => SwitchTool(cameraTool);

        internal void UserSwitchToBrushTool() => SwitchTool(brushTool);

        private interface GuiMaterialEditorTool : IEditorEventReceiver
        {
            /// <summary>The material editor window.</summary>
            public MaterialEditorWindow window { get; set; }

            /// <summary>
            /// The parent tool that called this tool (if any), to which the editor will return once the
            /// tool is finished. This is set when a single-use tool is instantiated with a keyboard binding.
            /// </summary>
            public GuiMaterialEditorTool parent { get; set; }

            /// <summary>
            /// Called at the beginning of the control's <see cref="OnRender"/> function. This draws
            /// on the normal screen.
            /// </summary>
            public void OnPreRender();

            /// <summary>
            /// Called before drawing the 3D world on the render texture with a 2D pixel matrix.
            /// </summary>
            public void OnPreRender2D();

            /// <summary>
            /// Called when the 3D world is to be drawn on the render texture with a 3D projection matrix.
            /// </summary>
            public void OnRender3D();

            /// <summary>
            /// Called after drawing the 3D world on the render texture with a 2D pixel matrix.
            /// </summary>
            public void OnPostRender2D();

            /// <summary>
            /// Called at the end of the control's <see cref="OnRender"/> function. This draws
            /// on the normal screen.
            /// </summary>
            public void OnPostRender();
        }

        private class GuiMaterialEditorCameraTool : Tool, GuiMaterialEditorTool
        {
            public MaterialEditorWindow window { get; set; }
            public GuiMaterialEditorViewport viewport => window.viewport;
            GuiMaterialEditorTool GuiMaterialEditorTool.parent { get; set; }

            /// <summary>
            /// Gets whether the object is busy and has to maintain the input focus, making it
            /// impossible to switch to another object.
            /// </summary>
            public override bool IsBusy()
            {
                return base.IsBusy();
            }

            /// <summary>Called when the object is activated.</summary>
            public override void OnActivate()
            {
            }

            /// <summary>Called when the object is deactivated.</summary>
            public override void OnDeactivate()
            {
            }

            public void OnPreRender()
            {
            }

            public void OnPreRender2D()
            {
            }

            public void OnRender3D()
            {
                // no need to do raycasting when the mouse isn't over the window.
                if (viewport.isMouseOver)
                {
                    MeshRaycastHit hit = null;
                    viewport.materialIndexUnderMouse = 255;

                    GLUtilities3D.DrawGuiLines(() =>
                    {
                        var ray = viewport.camera.ScreenPointToRay(viewport.mousePosition);

                        if (viewport.meshRaycast.Raycast(ray.origin, ray.direction, out hit))
                        {
                            if (hit.normal.z.EqualsWithEpsilon5(0.0f))
                            {
                                GL.Color(Color.black);

                                var pos = new float2(hit.point.x, -hit.point.y);
                                var segment = editor.project.FindSegmentLineAtPosition(pos, 1f);
                                if (viewport.lookupTable.TryGetTrianglesForSegment(segment, out var triangleIndices))
                                {
                                    foreach (var triangleIndex in triangleIndices)
                                    {
                                        var v1 = viewport.lookupTable.Vertices[viewport.lookupTable.Triangles[triangleIndex]];
                                        var v2 = viewport.lookupTable.Vertices[viewport.lookupTable.Triangles[triangleIndex + 1]];
                                        var v3 = viewport.lookupTable.Vertices[viewport.lookupTable.Triangles[triangleIndex + 2]];

                                        GLUtilities3D.DrawLine(v1, v2);
                                        GLUtilities3D.DrawLine(v2, v3);
                                        GLUtilities3D.DrawLine(v3, v1);
                                    }
                                }
                            }
                            else
                            {
                                // todo: triangle based lookup table for shapes:

                                var pos = new float2(hit.point.x, -hit.point.y);
                                var shape = editor.FindShapeAtGridPosition(pos);
                                if (shape != null)
                                {
                                    GL.Color(Color.black);
                                    GLUtilities3D.DrawLine(hit.vertex1, hit.vertex2);
                                    GLUtilities3D.DrawLine(hit.vertex2, hit.vertex3);
                                    GLUtilities3D.DrawLine(hit.vertex3, hit.vertex1);
                                }
                            }
                        }
                    });

                    if (hit != null)
                    {
                        var pos = new float2(hit.point.x, -hit.point.y);
                        if (hit.normal.z.EqualsWithEpsilon5(0.0f))
                        {
                            var segmentUnderMouse = editor.project.FindSegmentLineAtPosition(pos, 1f);
                            viewport.materialIndexUnderMouse = 0;
                            if (segmentUnderMouse != null)
                                viewport.materialIndexUnderMouse = segmentUnderMouse.material;
                        }
                        else
                        {
                            viewport.materialIndexUnderMouse = 0;

                            // todo: triangle based lookup table for shapes:
                            var shape = editor.FindShapeAtGridPosition(pos);
                            if (shape != null)
                            {
                                if (hit.normal.z < 0.5f)
                                {
                                    viewport.materialIndexUnderMouse = shape.frontMaterial;
                                }
                                else if (hit.normal.z > 0.5f)
                                {
                                    viewport.materialIndexUnderMouse = shape.backMaterial;
                                }
                            }
                        }
                    }
                }
            }

            public void OnPostRender2D()
            {
            }

            public void OnPostRender()
            {
            }

            /// <summary>Called when the object receives a mouse down event.</summary>
            public override void OnMouseDown(int button)
            {
            }

            /// <summary>Called when the object receives a mouse up event.</summary>
            public override void OnMouseUp(int button)
            {
            }

            /// <summary>Called when the object receives a global mouse up event.</summary>
            public override void OnGlobalMouseUp(int button)
            {
            }

            /// <summary>Called when the object receives a mouse drag event.</summary>
            public override void OnMouseDrag(int button, float2 screenDelta, float2 gridDelta)
            {
            }

            /// <summary>Called when the object receives a global mouse drag event.</summary>
            public override void OnGlobalMouseDrag(int button, float2 screenDelta, float2 gridDelta)
            {
            }

            /// <summary>Called when the object receives a mouse move event.</summary>
            public override void OnMouseMove(float2 screenDelta, float2 gridDelta)
            {
            }

            /// <summary>Called when the object receives a mouse scroll event.</summary>
            public override bool OnMouseScroll(float delta)
            {
                return base.OnMouseScroll(delta);
            }

            /// <summary>Called when the object receives a key down event.</summary>
            public override bool OnKeyDown(KeyCode keyCode)
            {
                return base.OnKeyDown(keyCode);
            }

            /// <summary>Called when the object receives a key up event.</summary>
            public override bool OnKeyUp(KeyCode keyCode)
            {
                return base.OnKeyUp(keyCode);
            }

            /// <summary>Called when the object receives input focus.</summary>
            public override void OnFocus()
            {
            }

            /// <summary>Called when the object loses input focus.</summary>
            public override void OnFocusLost()
            {
            }
        }

        private class GuiMaterialEditorBrushTool : Tool, GuiMaterialEditorTool
        {
            public MaterialEditorWindow window { get; set; }
            public GuiMaterialEditorViewport viewport => window.viewport;
            GuiMaterialEditorTool GuiMaterialEditorTool.parent { get; set; }

            /// <summary>
            /// Gets whether the object is busy and has to maintain the input focus, making it
            /// impossible to switch to another object.
            /// </summary>
            public override bool IsBusy()
            {
                return base.IsBusy();
            }

            /// <summary>Called when the object is activated.</summary>
            public override void OnActivate()
            {
            }

            /// <summary>Called when the object is deactivated.</summary>
            public override void OnDeactivate()
            {
            }

            public void OnPreRender()
            {
            }

            public void OnPreRender2D()
            {
            }

            public void OnRender3D()
            {
                // no need to do raycasting when the mouse isn't over the window.
                if (viewport.isMouseOver)
                {
                    MeshRaycastHit hit = null;
                    viewport.materialIndexUnderMouse = 255;

                    GLUtilities3D.DrawGuiLines(() =>
                    {
                        var ray = viewport.camera.ScreenPointToRay(viewport.mousePosition);

                        if (viewport.meshRaycast.Raycast(ray.origin, ray.direction, out hit))
                        {
                            if (hit.normal.z.EqualsWithEpsilon5(0.0f))
                            {
                                GL.Color(materialIndexToColor[viewport.materialIndex]);

                                var pos = new float2(hit.point.x, -hit.point.y);
                                var segment = editor.project.FindSegmentLineAtPosition(pos, 1f);
                                if (viewport.lookupTable.TryGetTrianglesForSegment(segment, out var triangleIndices))
                                {
                                    foreach (var triangleIndex in triangleIndices)
                                    {
                                        var v1 = viewport.lookupTable.Vertices[viewport.lookupTable.Triangles[triangleIndex]];
                                        var v2 = viewport.lookupTable.Vertices[viewport.lookupTable.Triangles[triangleIndex + 1]];
                                        var v3 = viewport.lookupTable.Vertices[viewport.lookupTable.Triangles[triangleIndex + 2]];

                                        GLUtilities3D.DrawLine(v1, v2);
                                        GLUtilities3D.DrawLine(v2, v3);
                                        GLUtilities3D.DrawLine(v3, v1);
                                    }
                                }
                            }
                            else
                            {
                                // todo: triangle based lookup table for shapes:

                                var pos = new float2(hit.point.x, -hit.point.y);
                                var shape = editor.FindShapeAtGridPosition(pos);
                                if (shape != null)
                                {
                                    GL.Color(materialIndexToColor[viewport.materialIndex]);
                                    GLUtilities3D.DrawLine(hit.vertex1, hit.vertex2);
                                    GLUtilities3D.DrawLine(hit.vertex2, hit.vertex3);
                                    GLUtilities3D.DrawLine(hit.vertex3, hit.vertex1);
                                }
                            }
                        }
                    });

                    if (hit != null)
                    {
                        var pos = new float2(hit.point.x, -hit.point.y);
                        if (hit.normal.z.EqualsWithEpsilon5(0.0f))
                        {
                            if (viewport.isActive && editor.isLeftMousePressed)
                            {
                                if (viewport.lookupTable.TryGetSegmentsForTriangleIndex(hit.triangleIndex, out var segments))
                                {
                                    foreach (var segment in segments)
                                    {
                                        segment.material = viewport.materialIndex;
                                    }
                                }
                            }

                            var segmentUnderMouse = editor.project.FindSegmentLineAtPosition(pos, 1f);
                            viewport.materialIndexUnderMouse = 0;
                            if (segmentUnderMouse != null)
                                viewport.materialIndexUnderMouse = segmentUnderMouse.material;
                        }
                        else
                        {
                            viewport.materialIndexUnderMouse = 0;

                            // todo: triangle based lookup table for shapes:
                            var shape = editor.FindShapeAtGridPosition(pos);
                            if (viewport.isActive && editor.isLeftMousePressed)
                            {
                                if (shape != null)
                                {
                                    if (hit.normal.z < 0.5f)
                                    {
                                        shape.frontMaterial = viewport.materialIndex;
                                    }
                                    else if (hit.normal.z > 0.5f)
                                    {
                                        shape.backMaterial = viewport.materialIndex;
                                    }
                                }
                            }

                            if (shape != null)
                            {
                                if (hit.normal.z < 0.5f)
                                {
                                    viewport.materialIndexUnderMouse = shape.frontMaterial;
                                }
                                else if (hit.normal.z > 0.5f)
                                {
                                    viewport.materialIndexUnderMouse = shape.backMaterial;
                                }
                            }
                        }
                    }
                }
            }

            public void OnPostRender2D()
            {
            }

            public void OnPostRender()
            {
            }

            /// <summary>Called when the object receives a mouse down event.</summary>
            public override void OnMouseDown(int button)
            {
            }

            /// <summary>Called when the object receives a mouse up event.</summary>
            public override void OnMouseUp(int button)
            {
            }

            /// <summary>Called when the object receives a global mouse up event.</summary>
            public override void OnGlobalMouseUp(int button)
            {
            }

            /// <summary>Called when the object receives a mouse drag event.</summary>
            public override void OnMouseDrag(int button, float2 screenDelta, float2 gridDelta)
            {
            }

            /// <summary>Called when the object receives a global mouse drag event.</summary>
            public override void OnGlobalMouseDrag(int button, float2 screenDelta, float2 gridDelta)
            {
            }

            /// <summary>Called when the object receives a mouse move event.</summary>
            public override void OnMouseMove(float2 screenDelta, float2 gridDelta)
            {
            }

            /// <summary>Called when the object receives a mouse scroll event.</summary>
            public override bool OnMouseScroll(float delta)
            {
                return base.OnMouseScroll(delta);
            }

            /// <summary>Called when the object receives a key down event.</summary>
            public override bool OnKeyDown(KeyCode keyCode)
            {
                return base.OnKeyDown(keyCode);
            }

            /// <summary>Called when the object receives a key up event.</summary>
            public override bool OnKeyUp(KeyCode keyCode)
            {
                return base.OnKeyUp(keyCode);
            }

            /// <summary>Called when the object receives input focus.</summary>
            public override void OnFocus()
            {
            }

            /// <summary>Called when the object loses input focus.</summary>
            public override void OnFocusLost()
            {
            }
        }
    }

    public partial class MaterialEditorWindow
    {
        /// <summary>The currently active viewport tool.</summary>
        private GuiMaterialEditorTool activeTool;

        private GuiMaterialEditorCameraTool cameraTool;
        private GuiMaterialEditorBrushTool brushTool;

        /// <summary>Ensures that a valid tools always exists, to handle C# reloads.</summary>
        private void ValidateTools()
        {
            if (cameraTool == null)
            {
                cameraTool = new GuiMaterialEditorCameraTool();
                brushTool = new GuiMaterialEditorBrushTool();
            }

            if (activeTool == null)
                SwitchTool(cameraTool);
        }

        /// <summary>Switches the from current tool to the specified tool.</summary>
        /// <param name="tool">The tool to switch to.</param>
        private void SwitchTool(GuiMaterialEditorTool tool)
        {
            // if the tool is unchanged we ignore this call.
            if (activeTool == tool) return;

            // if there was an active tool we deactivate it.
            if (activeTool != null)
                activeTool.OnDeactivate();

            // switch to the new tool and activate it.
            tool.editor = editor;
            tool.window = this;
            activeTool = tool;
            activeTool.OnActivate();

            // fixme: not tested with single-use tools:
            editor.TrySwitchActiveEventReceiver(activeTool);
        }

        /// <summary>
        /// This function switches to the specified single-use tool and returns to the current tool
        /// when it's done. This is useful for single-use tools that are instantiated with a
        /// keyboard binding.
        /// </summary>
        /// <param name="tool">The single-use tool to switch to.</param>
        private void UseTool(GuiMaterialEditorTool tool)
        {
            // prevent accidental errors.
            if (activeTool == tool) { Debug.LogWarning("Cannot UseTool() the already active tool!"); return; }

            // set the parent of the tool to be the currently active tool.
            tool.parent = activeTool;

            // switch to the new tool.
            SwitchTool(tool);
        }

        /// <summary>Draws the active tool.</summary>
        private void DrawTool()
        {
            if (activeTool != null)
                activeTool.OnRender();
        }
    }
}

#endif