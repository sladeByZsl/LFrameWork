
    using UnityEditor;
    using UnityEditor.UI;

    /// <summary>
    /// The editor for <see cref="CircleImage"/>.
    /// </summary>
    [CustomEditor(typeof(CircleImage), true)]
    [CanEditMultipleObjects]
    internal sealed class CircleImageEditor : ImageEditor
    {
        private SerializedProperty segmentCount;
        private SerializedProperty fillPercent;

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.segmentCount);
            EditorGUILayout.PropertyField(this.fillPercent);
            this.serializedObject.ApplyModifiedProperties();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            var serObj = this.serializedObject;
            this.segmentCount = serObj.FindProperty("segmentCount");
            this.fillPercent = serObj.FindProperty("fillPercent");
        }
    }

