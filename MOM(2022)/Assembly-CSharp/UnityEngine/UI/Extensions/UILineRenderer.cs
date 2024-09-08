namespace UnityEngine.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Sprites;
    using UnityEngine.UI;

    [AddComponentMenu("UI/Extensions/Primitives/UILineRenderer"), RequireComponent(typeof(RectTransform))]
    public class UILineRenderer : UIPrimitiveBase
    {
        private const float MIN_MITER_JOIN = 0.2617994f;
        private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;
        private static Vector2 UV_TOP_LEFT;
        private static Vector2 UV_BOTTOM_LEFT;
        private static Vector2 UV_TOP_CENTER_LEFT;
        private static Vector2 UV_TOP_CENTER_RIGHT;
        private static Vector2 UV_BOTTOM_CENTER_LEFT;
        private static Vector2 UV_BOTTOM_CENTER_RIGHT;
        private static Vector2 UV_TOP_RIGHT;
        private static Vector2 UV_BOTTOM_RIGHT;
        private static Vector2[] startUvs;
        private static Vector2[] middleUvs;
        private static Vector2[] endUvs;
        private static Vector2[] fullUvs;
        [SerializeField, Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
        internal Vector2[] m_points;
        [SerializeField, Tooltip("Segments to be drawn\n This is a list of arrays of points")]
        internal List<Vector2[]> m_segments;
        [SerializeField, Tooltip("Thickness of the line")]
        internal float lineThickness = 2f;
        [SerializeField, Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
        internal bool relativeSize;
        [SerializeField, Tooltip("Do the points identify a single line or split pairs of lines")]
        internal bool lineList;
        [SerializeField, Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
        internal bool lineCaps;
        [SerializeField, Tooltip("scale points by this")]
        internal Vector2 scalePoints = Vector2.one;
        [SerializeField, Tooltip("Resolution of the Bezier curve, different to line Resolution")]
        internal int bezierSegmentsPerCurve = 10;
        [Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
        public JoinType LineJoins;
        [Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
        public BezierType BezierMode;
        [HideInInspector]
        public bool drivenExternally;

        private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, SegmentType type)
        {
            if (type == SegmentType.Start)
            {
                Vector2 vector = start - (((end - start).normalized * this.lineThickness) / 2f);
                return this.CreateLineSegment(vector, start, SegmentType.Start, null);
            }
            if (type == SegmentType.End)
            {
                Vector2 vector3 = end + (((end - start).normalized * this.lineThickness) / 2f);
                return this.CreateLineSegment(end, vector3, SegmentType.End, null);
            }
            Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
            return null;
        }

        private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type, UIVertex[] previousVert)
        {
            Vector2 vector = (new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness) / 2f;
            Vector2 zero = Vector2.zero;
            Vector2 vector3 = Vector2.zero;
            if (previousVert != null)
            {
                zero = new Vector2(previousVert[3].position.x, previousVert[3].position.y);
                vector3 = new Vector2(previousVert[2].position.x, previousVert[2].position.y);
            }
            else
            {
                zero = start - vector;
                vector3 = start + vector;
            }
            Vector2 vector4 = end + vector;
            Vector2 vector5 = end - vector;
            switch (type)
            {
                case SegmentType.Start:
                {
                    Vector2[] vectorArray1 = new Vector2[] { zero, vector3, vector4, vector5 };
                    return base.SetVbo(vectorArray1, startUvs);
                }
                case SegmentType.End:
                {
                    Vector2[] vectorArray2 = new Vector2[] { zero, vector3, vector4, vector5 };
                    return base.SetVbo(vectorArray2, endUvs);
                }
                case SegmentType.Full:
                {
                    Vector2[] vectorArray3 = new Vector2[] { zero, vector3, vector4, vector5 };
                    return base.SetVbo(vectorArray3, fullUvs);
                }
            }
            Vector2[] vertices = new Vector2[] { zero, vector3, vector4, vector5 };
            return base.SetVbo(vertices, middleUvs);
        }

        protected override void GeneratedUVs()
        {
            if (base.activeSprite == null)
            {
                UV_TOP_LEFT = Vector2.zero;
                UV_BOTTOM_LEFT = new Vector2(0f, 1f);
                UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
                UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
                UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
                UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
                UV_TOP_RIGHT = new Vector2(1f, 0f);
                UV_BOTTOM_RIGHT = Vector2.one;
            }
            else
            {
                Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
                Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
                UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
                UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
                UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
                UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
                UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
                UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
                UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
                UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
            }
            startUvs = new Vector2[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_CENTER_LEFT, UV_TOP_CENTER_LEFT };
            middleUvs = new Vector2[] { UV_TOP_CENTER_LEFT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_CENTER_RIGHT };
            endUvs = new Vector2[] { UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_RIGHT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
            fullUvs = new Vector2[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
        }

        public Vector2 GetClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 vector = p2 - p1;
            return (p1 + (vector * Mathf.Clamp01(Vector2.Dot(p3 - p1, vector.normalized) / vector.magnitude)));
        }

        public Vector2 GetPosition(int index, int segmentIndex)
        {
            if (segmentIndex > 0)
            {
                return this.Segments[segmentIndex - 1][index - 1];
            }
            if (this.Segments.Count <= 0)
            {
                return this.Points[index - 1];
            }
            int num = 0;
            int num2 = index;
            foreach (Vector2[] vectorArray in this.Segments)
            {
                if ((num2 - vectorArray.Length) <= 0)
                {
                    break;
                }
                num2 -= vectorArray.Length;
                num++;
            }
            return this.Segments[num][num2 - 1];
        }

        public Vector2 GetPositionBySegment(int index, int segment)
        {
            return this.Segments[segment][index - 1];
        }

        private int GetSegmentPointCount()
        {
            bool flag1;
            List<Vector2[]> segments = this.Segments;
            if (segments != null)
            {
                flag1 = segments.Count > 0;
            }
            else
            {
                List<Vector2[]> local1 = segments;
                flag1 = false;
            }
            if (!flag1)
            {
                return this.Points.Length;
            }
            int num = 0;
            foreach (Vector2[] vectorArray in this.Segments)
            {
                num += vectorArray.Length;
            }
            return num;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.m_points.Length == 0)
            {
                this.m_points = new Vector2[1];
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if ((this.m_points != null) && (this.m_points.Length != 0))
            {
                this.GeneratedUVs();
                vh.Clear();
                this.PopulateMesh(vh, this.m_points);
            }
            else if ((this.m_segments != null) && (this.m_segments.Count > 0))
            {
                this.GeneratedUVs();
                vh.Clear();
                for (int i = 0; i < this.m_segments.Count; i++)
                {
                    Vector2[] pointsToDraw = this.m_segments[i];
                    this.PopulateMesh(vh, pointsToDraw);
                }
            }
        }

        private void PopulateMesh(VertexHelper vh, Vector2[] pointsToDraw)
        {
            if ((this.BezierMode != BezierType.None) && (pointsToDraw.Length > 3))
            {
                BezierPath path = new BezierPath();
                path.SetControlPoints(pointsToDraw);
                path.SegmentsPerCurve = this.bezierSegmentsPerCurve;
                BezierType bezierMode = this.BezierMode;
                pointsToDraw = ((bezierMode == BezierType.Basic) ? path.GetDrawingPoints0() : ((bezierMode == BezierType.Improved) ? path.GetDrawingPoints1() : path.GetDrawingPoints2())).ToArray();
            }
            if (base.ImproveResolution != UnityEngine.UI.Extensions.ResolutionMode.None)
            {
                pointsToDraw = base.IncreaseResolution(pointsToDraw);
            }
            float num = !this.relativeSize ? 1f : base.rectTransform.rect.width;
            num *= this.scalePoints.x;
            float num2 = (!this.relativeSize ? 1f : base.rectTransform.rect.height) * this.scalePoints.y;
            float num3 = -base.rectTransform.pivot.x * num;
            float num4 = -base.rectTransform.pivot.y * num2;
            List<UIVertex[]> list = new List<UIVertex[]>();
            if (this.lineList)
            {
                for (int j = 1; j < pointsToDraw.Length; j += 2)
                {
                    Vector2 start = pointsToDraw[j - 1];
                    Vector2 end = pointsToDraw[j];
                    start = new Vector2((start.x * num) + num3, (start.y * num2) + num4);
                    end = new Vector2((end.x * num) + num3, (end.y * num2) + num4);
                    if (this.lineCaps)
                    {
                        list.Add(this.CreateLineCap(start, end, SegmentType.Start));
                    }
                    list.Add(this.CreateLineSegment(start, end, SegmentType.Middle, null));
                    if (this.lineCaps)
                    {
                        list.Add(this.CreateLineCap(start, end, SegmentType.End));
                    }
                }
            }
            else
            {
                for (int j = 1; j < pointsToDraw.Length; j++)
                {
                    Vector2 start = pointsToDraw[j - 1];
                    Vector2 end = pointsToDraw[j];
                    start = new Vector2((start.x * num) + num3, (start.y * num2) + num4);
                    end = new Vector2((end.x * num) + num3, (end.y * num2) + num4);
                    if (this.lineCaps && (j == 1))
                    {
                        list.Add(this.CreateLineCap(start, end, SegmentType.Start));
                    }
                    list.Add(this.CreateLineSegment(start, end, SegmentType.Middle, null));
                    if (this.lineCaps && (j == (pointsToDraw.Length - 1)))
                    {
                        list.Add(this.CreateLineCap(start, end, SegmentType.End));
                    }
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (!this.lineList && (i < (list.Count - 1)))
                {
                    Vector3 from = list[i][1].position - list[i][2].position;
                    Vector3 to = list[i + 1][2].position - list[i + 1][1].position;
                    float num8 = Vector2.Angle(from, to) * 0.01745329f;
                    float num9 = Mathf.Sign(Vector3.Cross(from.normalized, to.normalized).z);
                    float num10 = this.lineThickness / (2f * Mathf.Tan(num8 / 2f));
                    Vector3 vector7 = list[i][2].position - ((from.normalized * num10) * num9);
                    Vector3 vector8 = list[i][3].position + ((from.normalized * num10) * num9);
                    JoinType lineJoins = this.LineJoins;
                    if (lineJoins == JoinType.Miter)
                    {
                        if ((num10 >= (from.magnitude / 2f)) || ((num10 >= (to.magnitude / 2f)) || (num8 <= 0.2617994f)))
                        {
                            lineJoins = JoinType.Bevel;
                        }
                        else
                        {
                            list[i][2].position = vector7;
                            list[i][3].position = vector8;
                            list[i + 1][0].position = vector8;
                            list[i + 1][1].position = vector7;
                        }
                    }
                    if (lineJoins == JoinType.Bevel)
                    {
                        if ((num10 < (from.magnitude / 2f)) && ((num10 < (to.magnitude / 2f)) && (num8 > 0.5235988f)))
                        {
                            if (num9 < 0f)
                            {
                                list[i][2].position = vector7;
                                list[i + 1][1].position = vector7;
                            }
                            else
                            {
                                list[i][3].position = vector8;
                                list[i + 1][0].position = vector8;
                            }
                        }
                        UIVertex[] verts = new UIVertex[] { list[i][2], list[i][3], list[i + 1][0], list[i + 1][1] };
                        vh.AddUIVertexQuad(verts);
                    }
                }
                vh.AddUIVertexQuad(list[i]);
            }
            if (vh.currentVertCount > 0xfa00)
            {
                Debug.LogError("Max Verticies size is 64000, current mesh verticies count is [" + vh.currentVertCount.ToString() + "] - Cannot Draw");
                vh.Clear();
            }
        }

        protected override void ResolutionToNativeSize(float distance)
        {
            if (base.UseNativeSize)
            {
                base.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
                this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
            }
        }

        public float LineThickness
        {
            get
            {
                return this.lineThickness;
            }
            set
            {
                this.lineThickness = value;
                this.SetAllDirty();
            }
        }

        public bool RelativeSize
        {
            get
            {
                return this.relativeSize;
            }
            set
            {
                this.relativeSize = value;
                this.SetAllDirty();
            }
        }

        public bool LineList
        {
            get
            {
                return this.lineList;
            }
            set
            {
                this.lineList = value;
                this.SetAllDirty();
            }
        }

        public bool LineCaps
        {
            get
            {
                return this.lineCaps;
            }
            set
            {
                this.lineCaps = value;
                this.SetAllDirty();
            }
        }

        public int BezierSegmentsPerCurve
        {
            get
            {
                return this.bezierSegmentsPerCurve;
            }
            set
            {
                this.bezierSegmentsPerCurve = value;
            }
        }

        public Vector2[] Points
        {
            get
            {
                return this.m_points;
            }
            set
            {
                if (this.m_points != value)
                {
                    this.m_points = value;
                    this.SetAllDirty();
                }
            }
        }

        public List<Vector2[]> Segments
        {
            get
            {
                return this.m_segments;
            }
            set
            {
                this.m_segments = value;
                this.SetAllDirty();
            }
        }

        public enum BezierType
        {
            None,
            Quick,
            Basic,
            Improved
        }

        public enum JoinType
        {
            Bevel,
            Miter
        }

        private enum SegmentType
        {
            Start,
            Middle,
            End,
            Full
        }
    }
}

