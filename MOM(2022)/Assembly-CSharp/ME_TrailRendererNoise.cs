using System.Collections.Generic;
using UnityEngine;

public class ME_TrailRendererNoise : MonoBehaviour
{
    [Range(0.01f, 10f)]
    public float MinVertexDistance = 0.1f;

    public float VertexTime = 1f;

    public float TotalLifeTime = 3f;

    public bool SmoothCurves;

    public bool IsRibbon;

    public bool IsActive = true;

    [Range(0.001f, 10f)]
    public float Frequency = 1f;

    [Range(0.001f, 10f)]
    public float TimeScale = 0.1f;

    [Range(0.001f, 10f)]
    public float Amplitude = 1f;

    public float Gravity = 1f;

    public float TurbulenceStrength = 1f;

    public bool AutodestructWhenNotActive;

    private LineRenderer lineRenderer;

    private Transform t;

    private Vector3 prevPos;

    private List<Vector3> points = new List<Vector3>(500);

    private List<float> lifeTimes = new List<float>(500);

    private List<Vector3> velocities = new List<Vector3>(500);

    private float randomOffset;

    private List<Vector3> controlPoints = new List<Vector3>();

    private int curveCount;

    private const float MinimumSqrDistance = 0.01f;

    private const float DivisionThreshold = -0.99f;

    private const float SmoothCurvesScale = 0.5f;

    private void Start()
    {
        this.lineRenderer = base.GetComponent<LineRenderer>();
        this.lineRenderer.useWorldSpace = true;
        this.t = base.transform;
        this.prevPos = this.t.position;
        this.points.Insert(0, this.t.position);
        this.lifeTimes.Insert(0, this.VertexTime);
        this.velocities.Insert(0, Vector3.zero);
        this.randomOffset = (float)Random.Range(0, 10000000) / 1000000f;
    }

    private void OnEnable()
    {
        this.points.Clear();
        this.lifeTimes.Clear();
        this.velocities.Clear();
    }

    private void Update()
    {
        if (this.IsActive)
        {
            this.AddNewPoints();
        }
        this.UpdatetPoints();
        if (this.SmoothCurves && this.points.Count > 2)
        {
            this.UpdateLineRendererBezier();
        }
        else
        {
            this.UpdateLineRenderer();
        }
        if (this.AutodestructWhenNotActive && !this.IsActive && this.points.Count <= 1)
        {
            Object.Destroy(base.gameObject, this.TotalLifeTime);
        }
    }

    private void AddNewPoints()
    {
        if ((this.t.position - this.prevPos).magnitude > this.MinVertexDistance || (this.IsRibbon && this.points.Count == 0) || (this.IsRibbon && this.points.Count > 0 && (this.t.position - this.points[0]).magnitude > this.MinVertexDistance))
        {
            this.prevPos = this.t.position;
            this.points.Insert(0, this.t.position);
            this.lifeTimes.Insert(0, this.VertexTime);
            this.velocities.Insert(0, Vector3.zero);
        }
    }

    private void UpdatetPoints()
    {
        for (int i = 0; i < this.lifeTimes.Count; i++)
        {
            this.lifeTimes[i] -= Time.deltaTime;
            if (this.lifeTimes[i] <= 0f)
            {
                int count = this.lifeTimes.Count - i;
                this.lifeTimes.RemoveRange(i, count);
                this.points.RemoveRange(i, count);
                this.velocities.RemoveRange(i, count);
                break;
            }
            this.CalculateTurbuelence(this.points[i], this.TimeScale, this.Frequency, this.Amplitude, this.Gravity, i);
        }
    }

    private void UpdateLineRendererBezier()
    {
        if (this.SmoothCurves && this.points.Count > 2)
        {
            this.InterpolateBezier(this.points, 0.5f);
            List<Vector3> drawingPoints = this.GetDrawingPoints();
            this.lineRenderer.positionCount = drawingPoints.Count - 1;
            this.lineRenderer.SetPositions(drawingPoints.ToArray());
        }
    }

    private void UpdateLineRenderer()
    {
        this.lineRenderer.positionCount = Mathf.Clamp(this.points.Count - 1, 0, int.MaxValue);
        this.lineRenderer.SetPositions(this.points.ToArray());
    }

    private void CalculateTurbuelence(Vector3 position, float speed, float scale, float height, float gravity, int index)
    {
        float num = Time.timeSinceLevelLoad * speed + this.randomOffset;
        float x = position.x * scale + num;
        float num2 = position.y * scale + num + 10f;
        float y = position.z * scale + num + 25f;
        position.x = (Mathf.PerlinNoise(num2, y) - 0.5f) * height * Time.deltaTime;
        position.y = (Mathf.PerlinNoise(x, y) - 0.5f) * height * Time.deltaTime - gravity * Time.deltaTime;
        position.z = (Mathf.PerlinNoise(x, num2) - 0.5f) * height * Time.deltaTime;
        this.points[index] += position * this.TurbulenceStrength;
    }

    public void InterpolateBezier(List<Vector3> segmentPoints, float scale)
    {
        this.controlPoints.Clear();
        if (segmentPoints.Count < 2)
        {
            return;
        }
        for (int i = 0; i < segmentPoints.Count; i++)
        {
            if (i == 0)
            {
                Vector3 vector = segmentPoints[i];
                Vector3 vector2 = segmentPoints[i + 1] - vector;
                Vector3 item = vector + scale * vector2;
                this.controlPoints.Add(vector);
                this.controlPoints.Add(item);
            }
            else if (i == segmentPoints.Count - 1)
            {
                Vector3 vector3 = segmentPoints[i - 1];
                Vector3 vector4 = segmentPoints[i];
                Vector3 vector5 = vector4 - vector3;
                Vector3 item2 = vector4 - scale * vector5;
                this.controlPoints.Add(item2);
                this.controlPoints.Add(vector4);
            }
            else
            {
                Vector3 vector6 = segmentPoints[i - 1];
                Vector3 vector7 = segmentPoints[i];
                Vector3 vector8 = segmentPoints[i + 1];
                Vector3 normalized = (vector8 - vector6).normalized;
                Vector3 item3 = vector7 - scale * normalized * (vector7 - vector6).magnitude;
                Vector3 item4 = vector7 + scale * normalized * (vector8 - vector7).magnitude;
                this.controlPoints.Add(item3);
                this.controlPoints.Add(vector7);
                this.controlPoints.Add(item4);
            }
        }
        this.curveCount = (this.controlPoints.Count - 1) / 3;
    }

    public List<Vector3> GetDrawingPoints()
    {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < this.curveCount; i++)
        {
            List<Vector3> list2 = this.FindDrawingPoints(i);
            if (i != 0)
            {
                list2.RemoveAt(0);
            }
            list.AddRange(list2);
        }
        return list;
    }

    private List<Vector3> FindDrawingPoints(int curveIndex)
    {
        List<Vector3> list = new List<Vector3>();
        Vector3 item = this.CalculateBezierPoint(curveIndex, 0f);
        Vector3 item2 = this.CalculateBezierPoint(curveIndex, 1f);
        list.Add(item);
        list.Add(item2);
        this.FindDrawingPoints(curveIndex, 0f, 1f, list, 1);
        return list;
    }

    private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector3> pointList, int insertionIndex)
    {
        Vector3 vector = this.CalculateBezierPoint(curveIndex, t0);
        Vector3 vector2 = this.CalculateBezierPoint(curveIndex, t1);
        if ((vector - vector2).sqrMagnitude < 0.01f)
        {
            return 0;
        }
        float num = (t0 + t1) / 2f;
        Vector3 vector3 = this.CalculateBezierPoint(curveIndex, num);
        Vector3 normalized = (vector - vector3).normalized;
        Vector3 normalized2 = (vector2 - vector3).normalized;
        if (Vector3.Dot(normalized, normalized2) > -0.99f || Mathf.Abs(num - 0.5f) < 0.0001f)
        {
            int num2 = 0;
            num2 += this.FindDrawingPoints(curveIndex, t0, num, pointList, insertionIndex);
            pointList.Insert(insertionIndex + num2, vector3);
            num2++;
            return num2 + this.FindDrawingPoints(curveIndex, num, t1, pointList, insertionIndex + num2);
        }
        return 0;
    }

    public Vector3 CalculateBezierPoint(int curveIndex, float t)
    {
        int num = curveIndex * 3;
        Vector3 p = this.controlPoints[num];
        Vector3 p2 = this.controlPoints[num + 1];
        Vector3 p3 = this.controlPoints[num + 2];
        Vector3 p4 = this.controlPoints[num + 3];
        return this.CalculateBezierPoint(t, p, p2, p3, p4);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float num = 1f - t;
        float num2 = t * t;
        float num3 = num * num;
        float num4 = num3 * num;
        float num5 = num2 * t;
        return num4 * p0 + 3f * num3 * t * p1 + 3f * num * num2 * p2 + num5 * p3;
    }
}
