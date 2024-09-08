namespace MHUtils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Bezier
    {
        private Mode mode;
        private Vector3 p0;
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;
        private Vector3 b0;
        private Vector3 b1;
        private Vector3 b2;
        private Vector3 b3;

        public Bezier(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            this.p0 = v0;
            this.p1 = v1;
            this.p2 = v2;
            this.mode = Mode.Triangle;
        }

        public Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.p0 = v0;
            this.p1 = v1;
            this.p2 = v2;
            this.p3 = v3;
            this.mode = Mode.Quad;
        }

        public Vector3 GetValueAt(float t)
        {
            float num = 1f - t;
            float num2 = t * t;
            float num3 = num * num;
            if (this.mode == Mode.Triangle)
            {
                return (Vector3) (((num3 * this.p0) + (((2f * num) * t) * this.p1)) + (num2 * this.p2));
            }
            float num4 = num2 * t;
            return (Vector3) (((((num3 * num) * this.p0) + (((3f * num3) * t) * this.p1)) + (((3f * num) * num2) * this.p2)) + (num4 * this.p3));
        }

        private static List<Vector3> InterpolateSimple(List<Vector3> controlPoints, int interpolation)
        {
            List<Vector3> list = new List<Vector3>(1 + (controlPoints.Count * interpolation)) {
                controlPoints[0]
            };
            int num = 0;
            while (num < (controlPoints.Count - 1))
            {
                int num2 = 1;
                while (true)
                {
                    if (num2 >= interpolation)
                    {
                        num++;
                        break;
                    }
                    list.Add(Vector3.Lerp(controlPoints[num], controlPoints[num + 1], ((float) num2) / (interpolation + 1f)));
                    num2++;
                }
            }
            list.Add(controlPoints[controlPoints.Count - 1]);
            return list;
        }

        public static List<Vector3> InterpolateWithBezier(List<Vector3> controlPoints, int interpolation)
        {
            if (controlPoints.Count == 2)
            {
                return InterpolateSimple(controlPoints, interpolation);
            }
            if (controlPoints.Count == 3)
            {
                controlPoints.Add(controlPoints[controlPoints.Count - 1]);
            }
            List<Vector3> list = new List<Vector3>();
            int num = controlPoints.Count - 3;
            int num2 = 0;
            while (num2 <= (controlPoints.Count - 2))
            {
                int num3 = num2 - 1;
                float num4 = 0.5f;
                int num5 = num2;
                float num6 = 0f;
                if (num2 == 0)
                {
                    num3 = 0;
                    num4 = 0f;
                }
                else if (num2 > num)
                {
                    num5 = num2 - 1;
                    num6 = 0.5f;
                }
                Bezier bezier = new Bezier(controlPoints[num3], controlPoints[num3 + 1], controlPoints[num3 + 2]);
                Bezier bezier2 = new Bezier(controlPoints[num5], controlPoints[num5 + 1], controlPoints[num5 + 2]);
                int num7 = 0;
                while (true)
                {
                    if (num7 >= interpolation)
                    {
                        num2++;
                        break;
                    }
                    float t = ((float) num7) / ((float) (interpolation + 1));
                    Vector3 valueAt = bezier.GetValueAt((t * 0.5f) + num4);
                    Vector3 b = bezier2.GetValueAt((t * 0.5f) + num6);
                    list.Add(Vector3.Lerp(valueAt, b, t));
                    num7++;
                }
            }
            list.Add(controlPoints[controlPoints.Count - 1]);
            return list;
        }

        private enum Mode
        {
            Triangle,
            Quad
        }
    }
}

