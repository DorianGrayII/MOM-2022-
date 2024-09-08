using UnityEngine;

namespace TheHoney
{
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
            Sampler result = default(Sampler);
            result.start = start;
            result.end = end;
            Vector2 vector = Sampler.AveragedRightAngle(preStart, start, end);
            result.startLeft = start + vector * halfWidth;
            result.startRight = start - vector * halfWidth;
            Vector2 vector2 = Sampler.AveragedRightAngle(start, end, postEnd);
            result.endLeft = end + vector2 * halfWidth;
            result.endRight = end - vector2 * halfWidth;
            result.rectAreaMin = new Vector2(Mathf.Min(result.startLeft.x, result.startRight.x, result.endLeft.x, result.endRight.x), Mathf.Min(result.startLeft.y, result.startRight.y, result.endLeft.y, result.endRight.y));
            result.rectAreaMax = new Vector2(Mathf.Max(result.startLeft.x, result.startRight.x, result.endLeft.x, result.endRight.x), Mathf.Max(result.startLeft.y, result.startRight.y, result.endLeft.y, result.endRight.y));
            result.uv = new Vector2[4] { uv1, uv2, uv3, uv4 };
            return result;
        }

        private static Vector2 AveragedRightAngle(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 vector = b - a;
            Vector2 vector2 = c - b;
            Vector2 normalized = (vector + vector2).normalized;
            return new Vector2(0f - normalized.y, normalized.x);
        }

        public bool IsWithin(Vector2 point)
        {
            if (point.x < this.rectAreaMin.x || point.x > this.rectAreaMax.x || point.y < this.rectAreaMin.y || point.y > this.rectAreaMax.y)
            {
                return false;
            }
            if (this.DotProductTest(this.startLeft, this.startRight, point - this.startLeft) >= 0f && this.DotProductTest(this.startRight, this.endRight, point - this.startRight) >= 0f && this.DotProductTest(this.endRight, this.endLeft, point - this.endRight) >= 0f && this.DotProductTest(this.endLeft, this.startLeft, point - this.endLeft) >= 0f)
            {
                return true;
            }
            return false;
        }

        public float DotProductTest(Vector2 from, Vector2 to, Vector2 testPoint)
        {
            Vector2 vector = to - from;
            return Vector2.Dot(new Vector2(0f - vector.y, vector.x), testPoint);
        }

        public Vector2 GetPointData(Vector2 point)
        {
            Vector2 vector = this.endRight - this.startLeft;
            Vector2 lhs = new Vector2(0f - vector.y, vector.x);
            bool flag = false;
            Vector3 vector2;
            Vector3 vector3;
            Vector3 vector4;
            if (Vector2.Dot(lhs, point - this.startLeft) > 0f)
            {
                flag = true;
                vector2 = this.startLeft;
                vector3 = this.endRight;
                vector4 = this.endLeft;
            }
            else
            {
                vector2 = this.startLeft;
                vector3 = this.startRight;
                vector4 = this.endRight;
            }
            Vector3 vector5 = point;
            Vector3 rhs = vector5 - vector2;
            Vector3 lhs2 = vector5 - vector3;
            Vector3 vector6 = vector5 - vector4;
            float magnitude = Vector3.Cross(vector3 - vector2, vector4 - vector2).magnitude;
            float magnitude2 = Vector3.Cross(lhs2, vector6).magnitude;
            float magnitude3 = Vector3.Cross(vector6, rhs).magnitude;
            float num = magnitude - magnitude2 - magnitude3;
            float num2 = magnitude2 / magnitude;
            float num3 = magnitude3 / magnitude;
            float num4 = num / magnitude;
            if (flag)
            {
                return this.uv[0] * num2 + this.uv[3] * num3 + this.uv[2] * num4;
            }
            return this.uv[0] * num2 + this.uv[1] * num3 + this.uv[3] * num4;
        }
    }
}
