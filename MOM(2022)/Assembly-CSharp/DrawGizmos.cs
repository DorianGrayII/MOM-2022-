using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    public static List<List<Vector3>> rivers;
    public static List<List<ValueTuple<Vector3, Vector2>>> riversMesh;

    private void OnDrawGizmosSelected()
    {
        if (rivers != null)
        {
            Gizmos.color = Color.blue;
            foreach (List<Vector3> list in rivers)
            {
                for (int i = 0; i < (list.Count - 1); i++)
                {
                    Vector3 to = list[i + 1] + (Vector3.up * 0.1f);
                    Gizmos.DrawLine(list[i] + (Vector3.up * 0.1f), to);
                }
            }
        }
        if (riversMesh != null)
        {
            Gizmos.color = Color.green;
            foreach (List<ValueTuple<Vector3, Vector2>> list2 in riversMesh)
            {
                for (int i = 0; i < (list2.Count - 2); i += 2)
                {
                    Vector3 from = list2[i].Item1 + (Vector3.up * 0.1f);
                    Vector3 to = list2[i + 1].Item1 + (Vector3.up * 0.1f);
                    Vector3 vector4 = list2[i + 2].Item1 + (Vector3.up * 0.1f);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawLine(to, vector4);
                    Gizmos.DrawLine(vector4, from);
                    from = list2[i + 1].Item1 + (Vector3.up * 0.1f);
                    to = list2[i + 2].Item1 + (Vector3.up * 0.1f);
                    vector4 = list2[i + 3].Item1 + (Vector3.up * 0.1f);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawLine(to, vector4);
                    Gizmos.DrawLine(vector4, from);
                }
            }
        }
    }
}

