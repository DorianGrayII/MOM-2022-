using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    public static List<List<Vector3>> rivers;

    public static List<List<(Vector3, Vector2)>> riversMesh;

    private void OnDrawGizmosSelected()
    {
        if (DrawGizmos.rivers != null)
        {
            Gizmos.color = Color.blue;
            foreach (List<Vector3> river in DrawGizmos.rivers)
            {
                for (int i = 0; i < river.Count - 1; i++)
                {
                    Vector3 from = river[i] + Vector3.up * 0.1f;
                    Vector3 to = river[i + 1] + Vector3.up * 0.1f;
                    Gizmos.DrawLine(from, to);
                }
            }
        }
        if (DrawGizmos.riversMesh == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        foreach (List<(Vector3, Vector2)> item in DrawGizmos.riversMesh)
        {
            for (int j = 0; j < item.Count - 2; j += 2)
            {
                Vector3 vector = item[j].Item1 + Vector3.up * 0.1f;
                Vector3 vector2 = item[j + 1].Item1 + Vector3.up * 0.1f;
                Vector3 vector3 = item[j + 2].Item1 + Vector3.up * 0.1f;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(vector, vector2);
                Gizmos.DrawLine(vector2, vector3);
                Gizmos.DrawLine(vector3, vector);
                vector = item[j + 1].Item1 + Vector3.up * 0.1f;
                vector2 = item[j + 2].Item1 + Vector3.up * 0.1f;
                vector3 = item[j + 3].Item1 + Vector3.up * 0.1f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(vector, vector2);
                Gizmos.DrawLine(vector2, vector3);
                Gizmos.DrawLine(vector3, vector);
            }
        }
    }
}
