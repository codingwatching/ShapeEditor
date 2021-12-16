using UnityEditor;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    [CreateAssetMenu(fileName = "ShapeEditorResources", menuName = "ScriptableObjects/ShapeEditorResources", order = 1)]
    public class ShapeEditorResources : ScriptableObject
    {
        private static ShapeEditorResources s_Instance;

        /// <summary>Gets the singleton shape editor resources instance or creates it.</summary>
        public static ShapeEditorResources Instance
        {
            get
            {
                // if known, immediately return the instance.
                if (s_Instance) return s_Instance;

                // load the shape editor resources from the resources directory.
                LoadResources();

                return s_Instance;
            }
        }

        /// <summary>
        /// Before the first scene loads, we access the instance property to load all resources.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeSceneLoad()
        {
            // load the shape editor resources from the resources directory.
            LoadResources();
        }

        /// <summary>Loads the shape editor resources from the resources directory.</summary>
        private static void LoadResources()
        {
            s_Instance = (ShapeEditorResources)Resources.Load("ShapeEditorResources");
        }

        public Texture2D shapeEditorCreatePolygon;
        public Texture2D shapeEditorDelete;
        public Texture2D shapeEditorExtrudeBevel;
        public Texture2D shapeEditorExtrudePoint;
        public Texture2D shapeEditorExtrudeRevolve;
        public Texture2D shapeEditorExtrudeShape;
        public Texture2D shapeEditorFlipHorizontally;
        public Texture2D shapeEditorFlipVertically;
        public Texture2D shapeEditorHome;
        public Texture2D shapeEditorIcon;
        public Texture2D shapeEditorNew;
        public Texture2D shapeEditorOpen;
        public Texture2D shapeEditorRestore;
        public Texture2D shapeEditorRotate90Left;
        public Texture2D shapeEditorRotate90Right;
        public Texture2D shapeEditorSave;
        public Texture2D shapeEditorSegmentBezier;
        public Texture2D shapeEditorSegmentBezierDetail;
        public Texture2D shapeEditorSegmentExtrude;
        public Texture2D shapeEditorSegmentInsert;
        public Texture2D shapeEditorSegmentLinear;
        public Texture2D shapeEditorShapeCreate;
        public Texture2D shapeEditorShapeDuplicate;
        public Texture2D shapeEditorZoomIn;
        public Texture2D shapeEditorZoomOut;

        public Material shapeEditorGridMaterial;
        public Material shapeEditorLineMaterial;
        public Material shapeEditorDrawTextureMaterial;

        private static Material _temporaryGridMaterial;

        public static Material temporaryGridMaterial
        {
            get
            {
                if (!_temporaryGridMaterial)
                    _temporaryGridMaterial = new Material(Instance.shapeEditorGridMaterial);
                return _temporaryGridMaterial;
            }
        }

        private static Material _temporaryLineMaterial;

        public static Material temporaryLineMaterial
        {
            get
            {
                if (!_temporaryLineMaterial)
                    _temporaryLineMaterial = new Material(Instance.shapeEditorLineMaterial);
                return _temporaryLineMaterial;
            }
        }

        private static Material _temporaryDrawTextureMaterial;

        public static Material temporaryDrawTextureMaterial
        {
            get
            {
                if (!_temporaryDrawTextureMaterial)
                    _temporaryDrawTextureMaterial = new Material(Instance.shapeEditorDrawTextureMaterial);
                return _temporaryDrawTextureMaterial;
            }
        }

        private static GUIStyle _toolbarStyle;

        public static GUIStyle toolbarStyle
        {
            get
            {
                if (_toolbarStyle == null)
                    _toolbarStyle = new GUIStyle(EditorStyles.toolbar);
                return _toolbarStyle;
            }
        }

        private static GUIStyle _toolbarButtonStyle;

        public static GUIStyle toolbarButtonStyle
        {
            get
            {
                if (_toolbarButtonStyle == null)
                    _toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
                return _toolbarButtonStyle;
            }
        }
    }
}