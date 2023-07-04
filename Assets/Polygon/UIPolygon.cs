/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/uipolygon
using System.Collections.Generic;
namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
    public class UIPolygon : MaskableGraphic
    {
        [SerializeField]
        Texture m_Texture;
        public bool fill = true;
        public float thickness = 5;
        [Range(3, 360)]
        public int sides = 3;
        [Range(0, 360)]
        public float rotation = 0;
        [Range(0, 1)]
        public float[] VerticesDistances = new float[3];
        private float size = 0;

        [SerializeField]
        Sprite lineSprite;
        [SerializeField]
        List<Image> lines;
        [SerializeField]
        Sprite pointSprite;
        [SerializeField]
        List<RectTransform> points;

        private bool Isuplins = false;
        bool IsUpUi = false;
        #region lines
        //更新位置
        void UpDateLinesPos()
        {
            if (Isuplins == false)
                return;
            for (int i = 0; i < sides; i++)
                SetLinePos(i);
            Isuplins = false;
        }
        void SetLinePos(int i)
        {
            if (lines != null && points != null && lines.Count >= i + 1 && points.Count >= i + 1)
            {
                int p2 = i + 1;
                if (points.Count == p2)
                {
                    p2 = 0;
                }
                SetLine(lines[i], points[i], points[p2]);
            }
        }
        void SetLine(Image arrow, RectTransform pb, RectTransform pa)
        {
            if (arrow == null)
                return;
            arrow.transform.position = pa.position;
            arrow.transform.localRotation = Quaternion.AngleAxis(-GetAngle(pb, pa), Vector3.forward);

            var distance = Vector2.Distance(pb.anchoredPosition, pa.anchoredPosition);
            var newsizedelta = new Vector2(arrow.rectTransform.sizeDelta.x, distance);
            if (newsizedelta != arrow.rectTransform.sizeDelta)
                arrow.rectTransform.sizeDelta = newsizedelta;
        }
        float GetAngle(RectTransform pb, RectTransform pa)
        {
            var dir = pb.position - pa.position;
            var dirV2 = new Vector2(dir.x, dir.y);
            var angle = Vector2.SignedAngle(dirV2, Vector2.down);
            return angle;
        }

        public void InitLines()
        {
            ClearLines();
            if (lineSprite == null)
            {
                Debug.LogWarning("lineSprite 为空");
                return;
            }
            lines = new List<Image>();
            SpawnLines();
            SetVerticesDirty();
        }
        void ClearLines()
        {
            if (lines == null)
                return;
            foreach (Image line in lines)
            {
                if (line != null)
                    DestroyImmediate(line.gameObject);
            }
        }
        void SpawnLines()
        {
            for (int i = 0; i < sides; i++)
            {
                GameObject point = new GameObject("Line_" + i);
                point.transform.SetParent(transform);
                var rect = point.AddComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.pivot = new Vector2(0.5f, 1);
                var _Image = point.AddComponent<Image>();
                _Image.raycastTarget = false;
                _Image.sprite = lineSprite;
                _Image.SetNativeSize();
                lines.Add(_Image);
            }
        }
        #endregion

        #region points
        void SetPointsPos(int i, Vector2 pos)
        {
            if (points != null && points.Count >= i + 1)
            {
                points[i].anchoredPosition = pos;
            }
        }

        public void InitPoints()
        {
            ClearPoints();
            if (pointSprite == null)
            {
                Debug.LogWarning("pointSprite 为空");
                return;
            }
            points = new List<RectTransform>();
            SpawnPoints();
            SetVerticesDirty();
        }
        void ClearPoints()
        {
            if (points == null)
                return;
            foreach (RectTransform point in points)
            {
                if (point != null)
                    DestroyImmediate(point.gameObject);
            }
        }
        void SpawnPoints()
        {
            for (int i = 0; i < sides; i++)
            {
                GameObject point = new GameObject("Point_" + i);
                point.transform.SetParent(transform);
                var rect = point.AddComponent<RectTransform>();
                rect.localScale = Vector3.one;
                var _Image = point.AddComponent<Image>();
                _Image.raycastTarget = false;
                _Image.sprite = pointSprite;
                _Image.SetNativeSize();
                points.Add(rect);
            }
        }
        #endregion
        public override Texture mainTexture
        {
            get
            {
                return m_Texture == null ? s_WhiteTexture : m_Texture;
            }
        }
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value) return;
                m_Texture = value;
                SetVerticesDirty();
                //SetMaterialDirty();
            }
        }
        //只更改顶点的值  默认传入数量少一个方便外部传参
        public void DrawVertices(float[] _VerticesDistances)
        {
            int _len = _VerticesDistances.Length;
            int len = VerticesDistances.Length;
            if (len == _len + 1)
            {
                VerticesDistances = _VerticesDistances;//长度相同直接赋值
            }
            else if (len == _len + 1)
            {
                //长度比预制少1位 方便外部传参
                for (int i = 0; i < len - 1; i++)
                {
                    if (i > _len - 1)
                        VerticesDistances[i] = _VerticesDistances[0];
                    else
                        VerticesDistances[i] = _VerticesDistances[i];
                }
            }
            else
            {
                Debug.LogError("error  传入顶点数量与预制不符");
                VerticesDistances = _VerticesDistances;
            }
            IsUpUi = true;
        }
        public void DrawPolygon(int _sides)
        {
            sides = _sides;
            VerticesDistances = new float[_sides + 1];
            for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1; ;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = 0;
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = _rotation;
        }
        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            SetMaterialDirty();
        }
        void Update()
        {
            size = rectTransform.rect.width;
            if (rectTransform.rect.width > rectTransform.rect.height)
                size = rectTransform.rect.height;
            else
                size = rectTransform.rect.width;
            thickness = Mathf.Clamp(thickness, 0, size / 2);
            UpDateLinesPos();
            if (IsUpUi)
            {
                SetVerticesDirty();
                IsUpUi = false;
            }
        }
        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            float degrees = 360f / sides;
            int vertices = sides + 1;
            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
            }
            // last vertex is also the first!
            VerticesDistances[vertices - 1] = VerticesDistances[0];
            for (int i = 0; i < vertices; i++)
            {
                float outer = -rectTransform.pivot.x * size * VerticesDistances[i];
                float inner = -rectTransform.pivot.x * size * VerticesDistances[i] + thickness;
                float rad = Mathf.Deg2Rad * (i * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);
                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);
                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }
                prevX = pos1;
                prevY = pos2;
                vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
                SetPointsPos(i, pos1);//更新顶点位置
            }
            Isuplins = true;//同时更新连线长度
        }
    }
}
