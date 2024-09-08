using System;

public interface IVisibilityChange
{
    void DestroyMarkers();
    bool IsMarkerVisible();
    bool IsModelVisible();
    void UpdateMarkers();
}

