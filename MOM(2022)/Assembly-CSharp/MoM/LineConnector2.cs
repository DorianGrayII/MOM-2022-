using System.Collections.Generic;
using MHUtils;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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
            foreach (LineConnector2 line in LineConnector2.lines)
            {
                Object.Destroy(line.instance);
            }
            LineConnector2.lines.Clear();
        }

        public void Remove()
        {
            LineConnector2.lines.Remove(this);
            Object.Destroy(this.instance);
        }

        public void UpdateFrom(Vector2 v)
        {
            this.Update(v, this.to);
        }

        public void UpdateTo(Vector2 v)
        {
            this.Update(this.from, v);
        }

        public void Update(Vector2 in1, Vector2 in2)
        {
            this.from = in1;
            this.to = in2;
            Vector2 vector = in1;
            Vector2 vector2 = in2;
            if (vector.x > vector2.x)
            {
                Vector2 vector3 = vector;
                vector = vector2;
                vector2 = vector3;
            }
            if (Mathf.Abs(vector.x - vector2.x) < 3f)
            {
                vector.x = vector2.x - 4f;
            }
            /*
            if (false)
            {
                float y = vector.y;
                vector.y = vector2.y;
                vector2.y = y;
            }
            */
            Vector3 localScale = new Vector3((vector2.x - vector.x) / 100f, (vector2.y - vector.y) / 100f, 1f);
            /*
            if (false)
            {
                localScale.y = 0f - localScale.y;
            }
            */
            this.rt.localScale = localScale;
            Vector2 vector4 = (vector2 + vector) * 0.5f;
            this.rt.localPosition = vector4;
        }

        public static LineConnector2 Connect(Vector3 v1, Vector3 v2, GameObject source)
        {
            return LineConnector2.Connect(new Vector2(v1.x, v1.y), new Vector2(v2.x, v2.y), source);
        }

        public static LineConnector2 Connect(Vector2 v1, Vector2 v2, GameObject source)
        {
            GameObject gameObject = GameObjectUtils.Instantiate(source, LineConnector2.root);
            LineConnector2 lineConnector = new LineConnector2();
            lineConnector.rt = gameObject.GetComponent<RectTransform>();
            lineConnector.instance = gameObject;
            lineConnector.Update(v1, v2);
            return lineConnector;
        }

        public void SetColor(Color c)
        {
            this.instance.GetComponent<Image>().color = c;
        }
    }
}
