using UnityEngine;

public class FlockWaypointTrigger : MonoBehaviour
{
    public float _timer = 1f;

    public FlockChild _flockChild;

    public void Start()
    {
        if (this._flockChild == null)
        {
            this._flockChild = base.transform.parent.GetComponent<FlockChild>();
        }
        float num = Random.Range(this._timer, this._timer * 3f);
        base.InvokeRepeating("Trigger", num, num);
    }

    public void Trigger()
    {
        this._flockChild.Wander(0f);
    }
}
