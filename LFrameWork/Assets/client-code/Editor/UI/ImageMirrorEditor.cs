
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The editor for <see cref="ImageMirror"/>
    /// </summary>
    [CustomEditor(typeof(ImageMirror))]
    internal sealed class ImageMirrorEditor : Editor
    {
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();
            if (GUILayout.Button("Set Native Size"))
            {
                var mirror = (ImageMirror)this.target;
                Undo.RecordObject(mirror.transform, "Set Native Size");
                mirror.SetNativeSize();
            }
        }
    }

