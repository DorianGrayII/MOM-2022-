public interface IVisibilityChange
{
    bool IsMarkerVisible();

    bool IsModelVisible();

    void UpdateMarkers();

    void DestroyMarkers();
}
