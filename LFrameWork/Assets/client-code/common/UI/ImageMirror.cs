
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Sprites;
    using UnityEngine.UI;

    /// <summary>
    /// The UI image mirror used to mirror image.
    /// </summary>
    [AddComponentMenu("Oasis/UI/Image Mirror")]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public sealed class ImageMirror : BaseMeshEffect, ILayoutElement
    {
        [SerializeField]
        [Tooltip("The mirror type.")]
        private MirrorModeType mirrorMode = MirrorModeType.Horizontal;

        /// <summary>
        /// The mirror type.
        /// </summary>
        public enum MirrorModeType
        {
            /// <summary>
            /// Mirror horizontal mode.
            /// </summary>
            Horizontal,

            /// <summary>
            /// Mirror vertical mode.
            /// </summary>
            Vertical,

            /// <summary>
            /// Quarter mode.
            /// </summary>
            Quarter,

            /// <summary>
            /// Mirror horizontal and inverse vertical mode.
            /// </summary>
            HorizontalInverse,

            /// <summary>
            /// Mirror vertical and inverse horizontal mode.
            /// </summary>
            VerticalInverse,
        }

        /// <summary>
        /// Gets or sets the mirror mode.
        /// </summary>
        public MirrorModeType MirrorMode
        {
            get => this.mirrorMode;

            set
            {
                if (this.mirrorMode != value)
                {
                    this.mirrorMode = value;
                    this.graphic?.SetVerticesDirty();
                }
            }
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public float flexibleHeight => -1;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public float flexibleWidth => -1;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public int layoutPriority => 0;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public float minHeight => 0;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public float minWidth => 0;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public float preferredHeight
        {
            get
            {
                var image = this.graphic as Image;
                if (image == null)
                {
                    return 0;
                }

                switch (this.mirrorMode)
                {
                case MirrorModeType.Vertical:
                case MirrorModeType.Quarter:
                    return 2 * image.preferredHeight;
                default:
                    return image.preferredHeight;
                }
            }
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public float preferredWidth
        {
            get
            {
                var image = this.graphic as Image;
                if (image == null)
                {
                    return 0;
                }

                switch (this.mirrorMode)
                {
                case MirrorModeType.Horizontal:
                case MirrorModeType.Quarter:
                    return 2 * image.preferredWidth;
                default:
                    return image.preferredWidth;
                }
            }
        }

        /// <summary>
        /// Set the native size for this mirror.
        /// </summary>
        public void SetNativeSize()
        {
            var image = this.graphic as Image;
            if (image == null)
            {
                return;
            }

            var overrideSprite = image.overrideSprite;
            if (overrideSprite == null)
            {
                return;
            }

            var w = overrideSprite.rect.width / image.pixelsPerUnit;
            var h = overrideSprite.rect.height / image.pixelsPerUnit;
            var rectTransform = this.transform as RectTransform;
            rectTransform.anchorMax = rectTransform.anchorMin;

            switch (this.mirrorMode)
            {
            case MirrorModeType.Horizontal:
                rectTransform.sizeDelta = new Vector2(w * 2, h);
                break;
            case MirrorModeType.Vertical:
                rectTransform.sizeDelta = new Vector2(w, h * 2);
                break;
            case MirrorModeType.Quarter:
                rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                break;
            }

            image.SetVerticesDirty();
        }

        /// <inheritdoc/>
        public void CalculateLayoutInputHorizontal()
        {
        }

        /// <inheritdoc/>
        public void CalculateLayoutInputVertical()
        {
        }

        /// <inheritdoc/>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive() || vh.currentVertCount <= 0)
            {
                return;
            }

            var verts = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(verts);
            var newVerts = this.Modify(verts);
            if (newVerts != null)
            {
                vh.Clear();
                vh.AddUIVertexTriangleStream(newVerts);
            }

            ListPool<UIVertex>.Release(verts);
        }

#if UNITY_EDITOR
        /// <inheritdoc/>
        protected override void OnValidate()
        {
            base.OnValidate();
            this.graphic?.SetVerticesDirty();
        }
#endif

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.graphic?.SetVerticesDirty();
        }

        private static float GetCenter(float p1, float p2, float p3)
        {
            var max = Mathf.Max(Mathf.Max(p1, p2), p3);
            var min = Mathf.Min(Mathf.Min(p1, p2), p3);
            return (max + min) / 2;
        }

        private static Vector2 GetOverturnUV(
            Vector2 uv, float start, float end, bool isHorizontal = true)
        {
            if (isHorizontal)
            {
                uv.x = end - uv.x + start;
            }
            else
            {
                uv.y = end - uv.y + start;
            }

            return uv;
        }

        private List<UIVertex> Modify(List<UIVertex> verts)
        {
            var image = this.graphic as Image;
            if (image == null)
            {
                return verts;
            }

            switch (image.type)
            {
            case Image.Type.Simple:
                return this.ModifySimple(verts);
            case Image.Type.Sliced:
                return this.ModifySliced(image, verts);
            case Image.Type.Tiled:
                return this.ModifyTiled(image, verts);
            }

            return null;
        }

        private List<UIVertex> ModifySimple(List<UIVertex> verts)
        {
            switch (this.mirrorMode)
            {
            case MirrorModeType.Horizontal:
                return this.ModifySimpleHorizontal(verts, false);
            case MirrorModeType.HorizontalInverse:
                return this.ModifySimpleHorizontal(verts, true);
            case MirrorModeType.Vertical:
                return this.ModifySimpleVertical(verts, false);
            case MirrorModeType.VerticalInverse:
                return this.ModifySimpleVertical(verts, true);
            case MirrorModeType.Quarter:
                return this.ModifySimpleQuarter(verts);
            }

            return null;
        }

        private List<UIVertex> ModifySimpleHorizontal(
            List<UIVertex> verts, bool inverse)
        {
            var rect = this.graphic.GetPixelAdjustedRect();
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.x = (position.x + rect.x) * 0.5f;
                vertex.position = position;

                verts[i] = vertex;
            }

            verts.Reserve(count * 2);
            if (inverse)
            {
                this.MirrorVertsHorizontalInverse(rect, verts);
            }
            else
            {
                this.MirrorVertsHorizontal(rect, verts);
            }

            return verts;
        }

        private List<UIVertex> ModifySimpleVertical(
            List<UIVertex> verts, bool inverse)
        {
            var rect = this.graphic.GetPixelAdjustedRect();
            var halfHeight = rect.height * 0.5f;
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.y = ((position.y + rect.y) * 0.5f) + halfHeight;
                vertex.position = position;

                verts[i] = vertex;
            }

            verts.Reserve(count * 2);
            if (inverse)
            {
                this.MirrorVertsVecticalInverse(rect, verts);
            }
            else
            {
                this.MirrorVertsVectical(rect, verts);
            }

            return verts;
        }

        private List<UIVertex> ModifySimpleQuarter(List<UIVertex> verts)
        {
            var rect = this.graphic.GetPixelAdjustedRect();
            var halfHeight = rect.height * 0.5f;
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.x = (position.x + rect.x) * 0.5f;
                position.y = ((position.y + rect.y) * 0.5f) + halfHeight;
                vertex.position = position;

                verts[i] = vertex;
            }

            verts.Reserve(count * 4);
            this.MirrorVertsHorizontal(rect, verts);
            this.MirrorVertsVectical(rect, verts);
            return verts;
        }

        private List<UIVertex> ModifySliced(Image image, List<UIVertex> verts)
        {
            switch (this.mirrorMode)
            {
            case MirrorModeType.Horizontal:
                return this.ModifySlicedHorizontal(image, verts, false);
            case MirrorModeType.HorizontalInverse:
                return this.ModifySlicedHorizontal(image, verts, true);
            case MirrorModeType.Vertical:
                return this.ModifySlicedVertical(image, verts, false);
            case MirrorModeType.VerticalInverse:
                return this.ModifySlicedVertical(image, verts, true);
            case MirrorModeType.Quarter:
                return this.ModifySlicedQuarter(image, verts);
            }

            return null;
        }

        private List<UIVertex> ModifySlicedHorizontal(
            Image image, List<UIVertex> verts, bool inverse)
        {
            var rect = this.graphic.GetPixelAdjustedRect();
            var halfWidth = rect.width * 0.5f;
            var midX = rect.x + halfWidth;
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];
                var position = vertex.position;

                if (position.x >= midX)
                {
                    position.x = midX;
                }

                vertex.position = position;
                verts[i] = vertex;
            }

            this.SliceExcludeVerts(verts);
            verts.Reserve(count * 2);

            if (inverse)
            {
                this.MirrorVertsHorizontalInverse(rect, verts);
            }
            else
            {
                this.MirrorVertsHorizontal(rect, verts);
            }

            return verts;
        }

        private List<UIVertex> ModifySlicedVertical(
            Image image, List<UIVertex> verts, bool inverse)
        {
            var rect = this.graphic.GetPixelAdjustedRect();
            var halfHeight = rect.height * 0.5f;
            var midY = rect.y + halfHeight;
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];
                var position = vertex.position;

                if (position.y <= midY)
                {
                    position.y = midY;
                }

                vertex.position = position;
                verts[i] = vertex;
            }

            this.SliceExcludeVerts(verts);
            verts.Reserve(count * 2);
            if (inverse)
            {
                this.MirrorVertsVecticalInverse(rect, verts);
            }
            else
            {
                this.MirrorVertsVectical(rect, verts);
            }

            return verts;
        }

        private List<UIVertex> ModifySlicedQuarter(
            Image image, List<UIVertex> verts)
        {
            var rect = this.graphic.GetPixelAdjustedRect();
            var halfWidth = rect.width * 0.5f;
            var halfHeight = rect.height * 0.5f;
            var midX = rect.x + halfWidth;
            var midY = rect.y + halfHeight;
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];
                var position = vertex.position;

                if (position.x >= midX)
                {
                    position.x = midX;
                }

                if (position.y <= midY)
                {
                    position.y = midY;
                }

                vertex.position = position;
                verts[i] = vertex;
            }

            this.SliceExcludeVerts(verts);
            verts.Reserve(count * 2);
            this.MirrorVertsHorizontal(rect, verts);
            this.MirrorVertsVectical(rect, verts);

            return verts;
        }

        private List<UIVertex> ModifyTiled(Image image, List<UIVertex> verts)
        {
            switch (this.mirrorMode)
            {
            case MirrorModeType.Horizontal:
                return this.ModifyTiledHorizontal(image, verts);
            case MirrorModeType.Vertical:
                return this.ModifyTiledVertical(image, verts);
            case MirrorModeType.Quarter:
                return this.ModifyTiledQuarter(image, verts);
            }

            return null;
        }

        private List<UIVertex> ModifyTiledHorizontal(
            Image image, List<UIVertex> verts)
        {
            var overrideSprite = image.overrideSprite;
            if (overrideSprite == null)
            {
                return null;
            }

            var rect = this.graphic.GetPixelAdjustedRect();
            var inner = DataUtility.GetInnerUV(overrideSprite);

            var w = overrideSprite.rect.width / image.pixelsPerUnit;

            var count = verts.Count;
            var len = count / 3;
            for (var i = 0; i < len; ++i)
            {
                var baseIndex = i * 3;
                var v1 = verts[baseIndex + 0];
                var v2 = verts[baseIndex + 1];
                var v3 = verts[baseIndex + 2];

                var centerX = GetCenter(
                    v1.position.x, v2.position.x, v3.position.x);
                if (Mathf.FloorToInt((centerX - rect.xMin) / w) % 2 == 1)
                {
                    v1.uv0.x = inner.z - v1.uv0.x + inner.x;
                    v2.uv0.x = inner.z - v2.uv0.x + inner.x;
                    v3.uv0.x = inner.z - v3.uv0.x + inner.x;
                }

                verts[baseIndex + 0] = v1;
                verts[baseIndex + 1] = v2;
                verts[baseIndex + 2] = v3;
            }

            return verts;
        }

        private List<UIVertex> ModifyTiledVertical(
            Image image, List<UIVertex> verts)
        {
            var overrideSprite = image.overrideSprite;
            if (overrideSprite == null)
            {
                return null;
            }

            var rect = this.graphic.GetPixelAdjustedRect();
            var inner = DataUtility.GetInnerUV(overrideSprite);

            var h = overrideSprite.rect.height / image.pixelsPerUnit;

            var count = verts.Count;
            var len = count / 3;
            for (var i = 0; i < len; ++i)
            {
                var baseIndex = i * 3;
                var v1 = verts[baseIndex + 0];
                var v2 = verts[baseIndex + 1];
                var v3 = verts[baseIndex + 2];

                var centerY = GetCenter(v1.position.y, v2.position.y, v3.position.y);
                if (Mathf.FloorToInt((centerY - rect.yMin) / h) % 2 == 1)
                {
                    v1.uv0.y = inner.w - v1.uv0.y + inner.y;
                    v2.uv0.y = inner.w - v2.uv0.y + inner.y;
                    v3.uv0.y = inner.w - v3.uv0.y + inner.y;
                }

                verts[baseIndex + 0] = v1;
                verts[baseIndex + 1] = v2;
                verts[baseIndex + 2] = v3;
            }

            return verts;
        }

        private List<UIVertex> ModifyTiledQuarter(
            Image image, List<UIVertex> verts)
        {
            var overrideSprite = image.overrideSprite;
            if (overrideSprite == null)
            {
                return null;
            }

            var rect = this.graphic.GetPixelAdjustedRect();
            var inner = DataUtility.GetInnerUV(overrideSprite);

            var w = overrideSprite.rect.width / image.pixelsPerUnit;
            var h = overrideSprite.rect.height / image.pixelsPerUnit;

            var count = verts.Count;
            var len = count / 3;
            for (var i = 0; i < len; ++i)
            {
                var baseIndex = i * 3;
                var v1 = verts[baseIndex + 0];
                var v2 = verts[baseIndex + 1];
                var v3 = verts[baseIndex + 2];

                var centerX = GetCenter(
                    v1.position.x, v2.position.x, v3.position.x);
                var centerY = GetCenter(
                    v1.position.y, v2.position.y, v3.position.y);

                if (Mathf.FloorToInt((centerX - rect.xMin) / w) % 2 == 1)
                {
                    v1.uv0.x = inner.z - v1.uv0.x + inner.x;
                    v2.uv0.x = inner.z - v2.uv0.x + inner.x;
                    v3.uv0.x = inner.z - v3.uv0.x + inner.x;
                }

                if (Mathf.FloorToInt((centerY - rect.yMin) / h) % 2 == 1)
                {
                    v1.uv0.y = inner.w - v1.uv0.y + inner.y;
                    v2.uv0.y = inner.w - v2.uv0.y + inner.y;
                    v3.uv0.y = inner.w - v3.uv0.y + inner.y;
                }

                verts[baseIndex + 0] = v1;
                verts[baseIndex + 1] = v2;
                verts[baseIndex + 2] = v3;
            }

            return verts;
        }

        private void MirrorVertsHorizontal(Rect rect, List<UIVertex> verts)
        {
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.x = (rect.center.x * 2) - position.x;
                vertex.position = position;

                verts.Add(vertex);
            }
        }

        private void MirrorVertsHorizontalInverse(
            Rect rect, List<UIVertex> verts)
        {
            var count = verts.Count;
            var center = rect.center;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.x = (center.x * 2) - position.x;
                position.y = (center.y * 2) - position.y;
                vertex.position = position;

                verts.Add(vertex);
            }
        }

        private void MirrorVertsVectical(Rect rect, List<UIVertex> verts)
        {
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.y = (rect.center.y * 2) - position.y;
                vertex.position = position;

                verts.Add(vertex);
            }
        }

        private void MirrorVertsVecticalInverse(
            Rect rect, List<UIVertex> verts)
        {
            var count = verts.Count;
            for (int i = 0; i < count; ++i)
            {
                var vertex = verts[i];

                var position = vertex.position;
                position.x = (rect.center.x * 2) - position.x;
                position.y = (rect.center.y * 2) - position.y;
                vertex.position = position;

                verts.Add(vertex);
            }
        }

        private Vector4 GetAdjustedBorders(Image image, Rect rect)
        {
            var overrideSprite = image.overrideSprite;
            var border = overrideSprite.border;
            border = border / image.pixelsPerUnit;

            var combinedWidth = border.x + border.z;
            if (rect.size.x < combinedWidth && combinedWidth != 0)
            {
                var scaleRatio = rect.size.x / combinedWidth;
                border.x *= scaleRatio;
                border.z *= scaleRatio;
            }

            var combinedHeight = border.y + border.w;
            if (rect.size.y < combinedHeight && combinedHeight != 0)
            {
                var scaleRatio = rect.size.y / combinedHeight;
                border.y *= scaleRatio;
                border.w *= scaleRatio;
            }

            return border;
        }

        private void SliceExcludeVerts(List<UIVertex> verts)
        {
            var realCount = verts.Count;
            var i = 0;
            while (i < realCount)
            {
                var v1 = verts[i];
                var v2 = verts[i + 1];
                var v3 = verts[i + 2];

                if (v1.position == v2.position ||
                    v2.position == v3.position ||
                    v3.position == v1.position)
                {
                    verts[i] = verts[realCount - 3];
                    verts[i + 1] = verts[realCount - 2];
                    verts[i + 2] = verts[realCount - 1];

                    realCount -= 3;
                    continue;
                }

                i += 3;
            }

            if (realCount < verts.Count)
            {
                verts.RemoveRange(realCount, verts.Count - realCount);
            }
        }
    }

