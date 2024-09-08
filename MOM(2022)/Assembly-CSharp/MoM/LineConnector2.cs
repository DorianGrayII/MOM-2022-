namespace MOM
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class LineConnector2
    {
        private const float maxArcLength = 150f;
        private const int pointsPerCurve = 40;
        public static Transform root;
        private static Canvas canvas;
        private static List<LineConnector2> lines = new List<LineConnector2>();
        private GameObject instance;
        private RectTransform rt;
        private Vector2 from;
        private Vector2 to;

        public static void Clear()
        {
            using (List<LineConnector2>.Enumerator enumerator = lines.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    UnityEngine.Object.Destroy(enumerator.Current.instance);
                }
            }
            lines.Clear();
        }

        public static LineConnector2 Connect(Vector2 v1, Vector2 v2, GameObject source)
        {
            GameObject obj2 = GameObjectUtils.Instantiate(source, root);
            LineConnector2 connector1 = new LineConnector2();
            connector1.rt = obj2.GetComponent<RectTransform>();
            connector1.instance = obj2;
            connector1.Update(v1, v2);
            return connector1;
        }

        public static LineConnector2 Connect(Vector3 v1, Vector3 v2, GameObject source)
        {
            return Connect(new Vector2(v1.x, v1.y), new Vector2(v2.x, v2.y), source);
        }

        public void Remove()
        {
            lines.Remove(this);
            UnityEngine.Object.Destroy(this.instance);
        }

        public void SetColor(UnityEngine.Color c)
        {
            this.instance.GetComponent<Image>().color = c;
        }

        public void Update(Vector2 in1, Vector2 in2)
        {
            this.from = in1;
            this.to = in2;
            Vector2 vector = in1;
            Vector2 vector2 = in2;
            if (vector.x > vector2.x)
            {
                Vector2 vector1 = vector;
                vector = vector2;
                vector2 = vector1;
            }
            if (Mathf.Abs((float) (vector.x - vector2.x)) < 3f)
            {
                vector.x = vector2.x - 4f;
            }
            if (0 != 0)
            {
                vector.y = vector2.y;
                vector2.y = vector.y;
            }
            Vector3 vector3 = new Vector3((vector2.x - vector.x) / 100f, (vector2.y - vector.y) / 100f, 1f);
            if (0 != 0)
            {
                vector3.y = -vector3.y;
            }
            this.rt.localScale = vector3;
            Vector2 vector4 = (vector2 + vector) * 0.5f;
            this.rt.localPosition = (Vector3) vector4;
        }

        public void UpdateFrom(Vector2 v)
        {
            this.Update(v, this.to);
        }

        public void UpdateTo(Vector2 v)
        {
            this.Update(this.from, v);
        }
    }
}

