namespace TheHoney
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Sampler
    {
        public Vector2 start;
        public Vector2 startLeft;
        public Vector2 startRight;
        public Vector2 end;
        public Vector2 endLeft;
        public Vector2 endRight;
        public Vector2 rectAreaMin;
        public Vector2 rectAreaMax;
        public Vector2[] uv;
        public static Sampler Create(Vector2 preStart, Vector2 start, Vector2 end, Vector2 postEnd, float halfWidth, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
        {
            Sampler sampler = new Sampler {
                start = start,
                end = end
            };
            Vector2 vector = AveragedRightAngle(preStart, start, end);
            sampler.startLeft = start + (vector * halfWidth);
            sampler.startRight = start - (vector * halfWidth);
            Vector2 vector2 = AveragedRightAngle(start, end, postEnd);
            sampler.endLeft = end + (vector2 * halfWidth);
            sampler.endRight = end - (vector2 * halfWidth);
            float[] values = new float[] { sampler.startLeft.x, sampler.startRight.x, sampler.endLeft.x, sampler.endRight.x };
            float[] singleArray2 = new float[] { sampler.startLeft.y, sampler.startRight.y, sampler.endLeft.y, sampler.endRight.y };
            sampler.rectAreaMin = new Vector2(Mathf.Min(values), Mathf.Min(singleArray2));
            float[] singleArray3 = new float[] { sampler.startLeft.x, sampler.startRight.x, sampler.endLeft.x, sampler.endRight.x };
            float[] singleArray4 = new float[] { sampler.startLeft.y, sampler.startRight.y, sampler.endLeft.y, sampler.endRight.y };
            sampler.rectAreaMax = new Vector2(Mathf.Max(singleArray3), Mathf.Max(singleArray4));
            sampler.uv = new Vector2[] { uv1, uv2, uv3, uv4 };
            return sampler;
        }

        private static Vector2 AveragedRightAngle(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 normalized = ((b - a) + (c - b)).normalized;
            return new Vector2(-normalized.y, normalized.x);
        }

        public bool IsWithin(Vector2 point)
        {
            return ((point.x >= this.rectAreaMin.x) && ((point.x <= this.rectAreaMax.x) && ((point.y >= this.rectAreaMin.y) && ((point.y <= this.rectAreaMax.y) && ((this.DotProductTest(this.startLeft, this.startRight, point - this.startLeft) >= 0f) && ((this.DotProductTest(this.startRight, this.endRight, point - this.startRight) >= 0f) && ((this.DotProductTest(this.endRight, this.endLeft, point - this.endRight) >= 0f) && (this.DotProductTest(this.endLeft, this.startLeft, point - this.endLeft) >= 0f))))))));
        }

        public float DotProductTest(Vector2 from, Vector2 to, Vector2 testPoint)
        {
            Vector2 vector = to - from;
            return Vector2.Dot(new Vector2(-vector.y, vector.x), testPoint);
        }

        public Vector2 GetPointData(Vector2 point)
        {
            Vector3 startLeft;
            Vector3 startRight;
            Vector3 endRight;
            Vector2 vector = this.endRight - this.startLeft;
            bool flag = false;
            if (Vector2.Dot(new Vector2(-vector.y, vector.x), point - this.startLeft) <= 0f)
            {
                startLeft = (Vector3) this.startLeft;
                startRight = (Vector3) this.startRight;
                endRight = (Vector3) this.endRight;
            }
            else
            {
                flag = true;
                startLeft = (Vector3) this.startLeft;
                startRight = (Vector3) this.endRight;
                endRight = (Vector3) this.endLeft;
            }
            Vector3 rhs = ((Vector3) point) - endRight;
            float magnitude = Vector3.Cross(startRight - startLeft, endRight - startLeft).magnitude;
            float num2 = Vector3.Cross(((Vector3) point) - startRight, rhs).magnitude;
            float num3 = Vector3.Cross(rhs, ((Vector3) point) - startLeft).magnitude;
            float num4 = num2 / magnitude;
            float num5 = num3 / magnitude;
            float num6 = ((magnitude - num2) - num3) / magnitude;
            return (!flag ? (((this.uv[0] * num4) + (this.uv[1] * num5)) + (this.uv[3] * num6)) : (((this.uv[0] * num4) + (this.uv[3] * num5)) + (this.uv[2] * num6)));
        }
    }
}

