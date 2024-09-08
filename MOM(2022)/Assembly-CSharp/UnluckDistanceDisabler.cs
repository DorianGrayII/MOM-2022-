using UnityEngine;

public class UnluckDistanceDisabler : MonoBehaviour
{
    public int _distanceDisable = 1000;

    public Transform _distanceFrom;

    public bool _distanceFromMainCam;

    public float _disableCheckInterval = 10f;

    public float _enableCheckInterval = 1f;

    public bool _disableOnStart;

    public void Start()
    {
        if (this._distanceFromMainCam)
        {
            this._distanceFrom = Camera.main.transform;
        }
        base.InvokeRepeating("CheckDisable", this._disableCheckInterval + Random.value * this._disableCheckInterval, this._disableCheckInterval);
        base.InvokeRepeating("CheckEnable", this._enableCheckInterval + Random.value * this._enableCheckInterval, this._enableCheckInterval);
        base.Invoke("DisableOnStart", 0.01f);
    }

    public void DisableOnStart()
    {
        if (this._disableOnStart)
        {
            base.gameObject.SetActive(value: false);
        }
    }

    public void CheckDisable()
    {
        if (base.gameObject.activeInHierarchy && (base.transform.position - this._distanceFrom.position).sqrMagnitude > (float)(this._distanceDisable * this._distanceDisable))
        {
            base.gameObject.SetActive(value: false);
        }
    }

    public void CheckEnable()
    {
        if (!base.gameObject.activeInHierarchy && (base.transform.position - this._distanceFrom.position).sqrMagnitude < (float)(this._distanceDisable * this._distanceDisable))
        {
            base.gameObject.SetActive(value: true);
        }
    }
}
