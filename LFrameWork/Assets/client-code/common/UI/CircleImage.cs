#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.Sprites;
    using UnityEngine.UI;

    /// <summary>
    /// It used to draw a image in circle.
    /// </summary>
    public sealed class CircleImage : Image
    {
        [SerializeField]
        [Range(4, 360)]
        private int segmentCount = 36;

        [SerializeField]
        [Range(-100, 100)]
        private int fillPercent = 100;

        /// <summary>
        /// Gets or sets the segment count.
        /// </summary>
        public int SegmentCount
        {
            get => this.segmentCount;

            set
            {
                if (this.segmentCount != value)
                {
                    this.segmentCount = value;
                    this.SetVerticesDirty();
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this.transform);
#endif
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var pivot = this.rectTransform.pivot;
            var rect = this.rectTransform.rect;
            var outer = -pivot.x * rect.width;

            var tw = this.rectTransform.rect.width;
            var th = this.rectTransform.rect.height;

            var uv = this.overrideSprite != null ?
                DataUtility.GetOuterUV(this.overrideSprite) : Vector4.zero;
            var uvCenterX = (uv.x + uv.z) * 0.5f;
            var uvCenterY = (uv.y + uv.w) * 0.5f;
            var uvScaleX = (uv.z - uv.x) / tw;
            var uvScaleY = (uv.w - uv.y) / th;

            var totalAngle = this.fillPercent / 100f * (Mathf.PI * 2f);
            var anglePerSeg = totalAngle / this.segmentCount;
            var currentAngle = 0.0f;
            var prev = Vector2.zero;
            for (int i = 0; i < this.segmentCount + 1; ++i)
            {
                var c = Mathf.Cos(currentAngle);
                var s = Mathf.Sin(currentAngle);

                var pos0 = prev;
                var pos1 = new Vector2(outer * c, outer * s);
                var pos2 = Vector2.zero;
                var pos3 = Vector2.zero;

                prev = pos1;

                var uv0 = new Vector2(
                    (pos0.x * uvScaleX) + uvCenterX,
                    (pos0.y * uvScaleY) + uvCenterY);
                var uv1 = new Vector2(
                    (pos1.x * uvScaleX) + uvCenterX,
                    (pos1.y * uvScaleY) + uvCenterY);
                var uv2 = new Vector2(
                    (pos2.x * uvScaleX) + uvCenterX,
                    (pos2.y * uvScaleY) + uvCenterY);
                var uv3 = new Vector2(
                    (pos3.x * uvScaleX) + uvCenterX,
                    (pos3.y * uvScaleY) + uvCenterY);

                var verts = new UIVertex[]
                {
                    new UIVertex() { color = this.color, position = pos0, uv0 = uv0 },
                    new UIVertex() { color = this.color, position = pos1, uv0 = uv1 },
                    new UIVertex() { color = this.color, position = pos2, uv0 = uv2 },
                    new UIVertex() { color = this.color, position = pos3, uv0 = uv3 },
                };

                vh.AddUIVertexQuad(verts);

                currentAngle += anglePerSeg;
            }
        }
    }
