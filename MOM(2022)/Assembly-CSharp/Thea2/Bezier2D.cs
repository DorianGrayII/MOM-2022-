using System.Collections.Generic;
using UnityEngine;

namespace Thea2
{
    public class Bezier2D
    {
        private enum Mode
        {
            Triangle = 0,
            Quad = 1
        }

        private Mode mode;

        private Vector2 p0;

        private Vector2 p1;

        private Vector2 p2;

        private Vector2 p3;

        private Vector2 b0;

        private Vector2 b1;

        private Vector2 b2;

        private Vector2 b3;

        public Bezier2D(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            this.p0 = v0;
            this.p1 = v1;
            this.p2 = v2;
            this.p3 = v3;
            this.mode = Mode.Quad;
        }

        public Bezier2D(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            this.p0 = v0;
            this.p1 = v1;
            this.p2 = v2;
            this.mode = Mode.Triangle;
        }

        public Vector2 GetValueAt(float t)
        {
            float num = 1f - t;
            float num2 = t * t;
            float num3 = num * num;
            if (this.mode == Mode.Triangle)
            {
                return num3 * this.p0 + 2f * num * t * this.p1 + num2 * this.p2;
            }
            float num4 = num3 * num;
            float num5 = num2 * t;
            return num4 * this.p0 + 3f * num3 * t * this.p1 + 3f * num * num2 * this.p2 + num5 * this.p3;
        }

        private static List<Vector2> InterpolateSimple(List<Vector2> controlPoints, int interpolation)
        {
            List<Vector2> list = new List<Vector2>(1 + controlPoints.Count * interpolation);
            list.Add(controlPoints[0]);
            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                for (int j = 1; j < interpolation; j++)
                {
                    list.Add(Vector2.Lerp(controlPoints[i], controlPoints[i + 1], (float)j / ((float)interpolation + 1f)));
                }
            }
            list.Add(controlPoints[controlPoints.Count - 1]);
            return list;
        }

        public static List<Vector2> InterpolateWithBezier(List<Vector2> controlPoints, int interpolation)
        {
            if (controlPoints.Count == 2)
            {
                return Bezier2D.InterpolateSimple(controlPoints, interpolation);
            }
            if (controlPoints.Count == 3)
            {
                controlPoints.Add(controlPoints[controlPoints.Count - 1]);
            }
            List<Vector2> list = new List<Vector2>();
            int num = controlPoints.Count - 3;
            for (int i = 0; i <= controlPoints.Count - 2; i++)
            {
                int num2 = i - 1;
                float num3 = 0.5f;
                int num4 = i;
                float num5 = 0f;
                if (i == 0)
                {
                    num2 = 0;
                    num3 = 0f;
                }
                else if (i > num)
                {
                    num4 = i - 1;
                    num5 = 0.5f;
                }
                Bezier bezier = new Bezier(controlPoints[num2], controlPoints[num2 + 1], controlPoints[num2 + 2]);
                Bezier bezier2 = new Bezier(controlPoints[num4], controlPoints[num4 + 1], controlPoints[num4 + 2]);
                for (int j = 0; j < interpolation; j++)
                {
                    float num6 = (float)j / (float)(interpolation + 1);
                    Vector2 a = bezier.GetValueAt(num6 * 0.5f + num3);
                    Vector2 b = bezier2.GetValueAt(num6 * 0.5f + num5);
                    list.Add(Vector2.Lerp(a, b, num6));
                }
            }
            list.Add(controlPoints[controlPoints.Count - 1]);
            return list;
        }
    }
}
