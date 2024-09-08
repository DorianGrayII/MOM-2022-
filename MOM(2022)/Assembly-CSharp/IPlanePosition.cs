using MHUtils;
using UnityEngine;
using WorldCode;

public interface IPlanePosition
{
    Vector3 GetPhysicalPosition();
    WorldCode.Plane GetPlane();
    Vector3i GetPosition();
}

