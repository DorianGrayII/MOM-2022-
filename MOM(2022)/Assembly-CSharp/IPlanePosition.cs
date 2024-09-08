using MHUtils;
using UnityEngine;
using WorldCode;

public interface IPlanePosition
{
    Vector3i GetPosition();

    Vector3 GetPhysicalPosition();

    global::WorldCode.Plane GetPlane();
}
