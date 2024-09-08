using System.Collections.Generic;
using MHUtils;
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
        if (this.path != null && this.path.Count > 0)
        {
            int index = Mathf.Min(this.path.Count - 1, 1);
            return this.path[index];
        }
        Debug.LogError("transportation position incorrect " + this.path);
        return Vector3i.zero;
    }
}
