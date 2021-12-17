#if UNITY_EDITOR

using Unity.Mathematics;

namespace AeternumGames.ShapeEditor
{
    /// <summary>Represents a viewport tool that's used to manipulate shapes.</summary>
    public abstract class Tool
    {
        /// <summary>The shape editor window.</summary>
        public ShapeEditorWindow editor;

        /// <summary>Called when the tool is activated.</summary>
        public virtual void OnActivate()
        {
        }

        /// <summary>Called when the tool is deactivated.</summary>
        public virtual void OnDeactivate()
        {
        }

        /// <summary>Called when the tool is rendered.</summary>
        public virtual void OnRender()
        {
        }

        /// <summary>Called when the tool receives a mouse down event.</summary>
        public virtual void OnMouseDown(int button)
        {
        }

        /// <summary>Called when the tool receives a mouse up event.</summary>
        public virtual void OnMouseUp(int button)
        {
        }

        /// <summary>Called when the tool receives a global mouse up event.</summary>
        public virtual void OnGlobalMouseUp(int button)
        {
        }

        /// <summary>Called when the tool receives a mouse drag event.</summary>
        public virtual void OnMouseDrag(int button, float2 screenDelta, float2 gridDelta)
        {
        }

        /// <summary>Called when the tool receives a mouse move event.</summary>
        public virtual void OnMouseMove(float2 screenDelta)
        {
        }
    }
}

#endif