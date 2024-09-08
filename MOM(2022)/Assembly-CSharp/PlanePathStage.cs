using MHUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanePathStage
{
    public bool arcanus;
    public List<Vector3i> path;
    public bool transportStage;
    public FInt mpCost;
    public bool pathFinished;

    public Vector3i GetTransportPosition()
    {
        string text1;
        if ((this.path != null) && (this.path.Count > 0))
        {
            int num = Mathf.Min(this.path.Count - 1, 1);
            return this.path[num];
        }
        if (this.path != null)
        {
            text1 = this.path.ToString();
        }
        else
        {
            List<Vector3i> path = this.path;
            text1 = null;
        }
        Debug.LogError("transportation position incorrect " + text1);
        return Vector3i.zero;
    }
}

