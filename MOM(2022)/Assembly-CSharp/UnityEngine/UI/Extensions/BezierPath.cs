namespace UnityEngine.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class BezierPath
    {
        public int SegmentsPerCurve = 10;
        public float MINIMUM_SQR_DISTANCE = 0.01f;
        public float DIVISION_THRESHOLD = -0.99f;
        private List<Vector2> controlPoints = new List<Vector2>();
        private int curveCount;

        public Vector2 CalculateBezierPoint(int curveIndex, float t)
        {
            int num = curveIndex * 3;
            Vector2 vector = this.controlPoints[num];
            return this.CalculateBezierPoint(t, vector, this.controlPoints[num + 1], this.controlPoints[num + 2], this.controlPoints[num + 3]);
        }

        private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float num = 1f - t;
            float num2 = t * t;
            float num3 = num * num;
            float num4 = num2 * t;
            return (Vector2) (((((num3 * num) * p0) + (((3f * num3) * t) * p1)) + (((3f * num) * num2) * p2)) + (num4 * p3));
        }

        private List<Vector2> FindDrawingPoints(int curveIndex)
        {
            List<Vector2> pointList = new List<Vector2> {
                this.CalculateBezierPoint(curveIndex, 0f),
                this.CalculateBezierPoint(curveIndex, 1f)
            };
            this.FindDrawingPoints(curveIndex, 0f, 1f, pointList, 1);
            return pointList;
        }

        private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector2> pointList, int insertionIndex)
        {
            Vector2 vector = this.CalculateBezierPoint(curveIndex, t0);
            Vector2 vector2 = this.CalculateBezierPoint(curveIndex, t1);
            if ((vector - vector2).sqrMagnitude < this.MINIMUM_SQR_DISTANCE)
            {
                return 0;
            }
            float t = (t0 + t1) / 2f;
            Vector2 item = this.CalculateBezierPoint(curveIndex, t);
            if ((Vector2.Dot((vector - item).normalized, (vector2 - item).normalized) <= this.DIVISION_THRESHOLD) && (Mathf.Abs((float) (t - 0.5f)) >= 0.0001f))
            {
                return 0;
            }
            int num2 = 0 + this.FindDrawingPoints(curveIndex, t0, t, pointList, insertionIndex);
            pointList.Insert(insertionIndex + num2, item);
            num2++;
            return (num2 + this.FindDrawingPoints(curveIndex, t, t1, pointList, insertionIndex + num2));
        }

        public List<Vector2> GetControlPoints()
        {
            return this.controlPoints;
        }

        public List<Vector2> GetDrawingPoints0()
        {
            List<Vector2> list = new List<Vector2>();
            int curveIndex = 0;
            while (curveIndex < this.curveCount)
            {
                if (curveIndex == 0)
                {
                    list.Add(this.CalculateBezierPoint(curveIndex, 0f));
                }
                int num2 = 1;
                while (true)
                {
                    if (num2 > this.SegmentsPerCurve)
                    {
                        curveIndex++;
                        break;
                    }
                    float t = ((float) num2) / ((float) this.SegmentsPerCurve);
                    list.Add(this.CalculateBezierPoint(curveIndex, t));
                    num2++;
                }
            }
            return list;
        }

        public List<Vector2> GetDrawingPoints1()
        {
            List<Vector2> list = new List<Vector2>();
            int num = 0;
            while (num < (this.controlPoints.Count - 3))
            {
                Vector2 vector = this.controlPoints[num];
                Vector2 vector2 = this.controlPoints[num + 1];
                Vector2 vector3 = this.controlPoints[num + 2];
                Vector2 vector4 = this.controlPoints[num + 3];
                if (num == 0)
                {
                    list.Add(this.CalculateBezierPoint(0f, vector, vector2, vector3, vector4));
                }
                int num2 = 1;
                while (true)
                {
                    if (num2 > this.SegmentsPerCurve)
                    {
                        num += 3;
                        break;
                    }
                    float t = ((float) num2) / ((float) this.SegmentsPerCurve);
                    list.Add(this.CalculateBezierPoint(t, vector, vector2, vector3, vector4));
                    num2++;
                }
            }
            return list;
        }

        public List<Vector2> GetDrawingPoints2()
        {
            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < this.curveCount; i++)
            {
                List<Vector2> collection = this.FindDrawingPoints(i);
                if (i != 0)
                {
                    collection.RemoveAt(0);
                }
                list.AddRange(collection);
            }
            return list;
        }

        public void Interpolate(List<Vector2> segmentPoints, float scale)
        {
            this.controlPoints.Clear();
            if (segmentPoints.Count >= 2)
            {
                for (int i = 0; i < segmentPoints.Count; i++)
                {
                    if (i == 0)
                    {
                        Vector2 item = segmentPoints[i];
                        Vector2 vector2 = segmentPoints[i + 1] - item;
                        Vector2 vector3 = item + (scale * vector2);
                        this.controlPoints.Add(item);
                        this.controlPoints.Add(vector3);
                    }
                    else if (i == (segmentPoints.Count - 1))
                    {
                        Vector2 item = segmentPoints[i];
                        Vector2 vector6 = item - segmentPoints[i - 1];
                        Vector2 vector7 = item - (scale * vector6);
                        this.controlPoints.Add(vector7);
                        this.controlPoints.Add(item);
                    }
                    else
                    {
                        Vector2 vector8 = segmentPoints[i - 1];
                        Vector2 item = segmentPoints[i];
                        Vector2 vector10 = segmentPoints[i + 1];
                        Vector2 normalized = (vector10 - vector8).normalized;
                        Vector2 vector12 = item - ((scale * normalized) * (item - vector8).magnitude);
                        Vector2 vector13 = item + ((scale * normalized) * (vector10 - item).magnitude);
                        this.controlPoints.Add(vector12);
                        this.controlPoints.Add(item);
                        this.controlPoints.Add(vector13);
                    }
                }
                this.curveCount = (this.controlPoints.Count - 1) / 3;
            }
        }

        public void SamplePoints(List<Vector2> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
        {
            if (sourcePoints.Count >= 2)
            {
                Stack<Vector2> collection = new Stack<Vector2>();
                collection.Push(sourcePoints[0]);
                Vector2 item = sourcePoints[1];
                int num = 2;
                for (num = 2; num < sourcePoints.Count; num++)
                {
                    if ((item - sourcePoints[num]).sqrMagnitude > minSqrDistance)
                    {
                        Vector2 vector5 = collection.Peek() - sourcePoints[num];
                        if (vector5.sqrMagnitude > maxSqrDistance)
                        {
                            collection.Push(item);
                        }
                    }
                    item = sourcePoints[num];
                }
                Vector2 vector2 = collection.Pop();
                Vector2 vector3 = collection.Peek();
                collection.Push(vector2 + ((vector3 - item).normalized * (((vector2 - vector3).magnitude - (item - vector2).magnitude) / 2f)));
                collection.Push(item);
                this.Interpolate(new List<Vector2>(collection), scale);
            }
        }

        public void SetControlPoints(List<Vector2> newControlPoints)
        {
            this.controlPoints.Clear();
            this.controlPoints.AddRange(newControlPoints);
            this.curveCount = (this.controlPoints.Count - 1) / 3;
        }

        public void SetControlPoints(Vector2[] newControlPoints)
        {
            this.controlPoints.Clear();
            this.controlPoints.AddRange(newControlPoints);
            this.curveCount = (this.controlPoints.Count - 1) / 3;
        }
    }
}

