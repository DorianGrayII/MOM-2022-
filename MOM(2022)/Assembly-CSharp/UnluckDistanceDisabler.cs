using System;
using UnityEngine;

public class UnluckDistanceDisabler : MonoBehaviour
{
    public int _distanceDisable = 0x3e8;
    public Transform _distanceFrom;
    public bool _distanceFromMainCam;
    public float _disableCheckInterval = 10f;
    public float _enableCheckInterval = 1f;
    public bool _disableOnStart;

    public void CheckDisable()
    {
        if (base.gameObject.activeInHierarchy && ((base.transform.position - this._distanceFrom.position).sqrMagnitude > (this._distanceDisable * this._distanceDisable)))
        {
            base.gameObject.SetActive(false);
        }
    }

    public void CheckEnable()
    {
        if (!base.gameObject.activeInHierarchy && ((base.transform.position - this._distanceFrom.position).sqrMagnitude < (this._distanceDisable * this._distanceDisable)))
        {
            base.gameObject.SetActive(true);
        }
    }

    public void DisableOnStart()
    {
        if (this._disableOnStart)
        {
            base.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        if (this._distanceFromMainCam)
        {
            this._distanceFrom = Camera.main.transform;
        }
        base.InvokeRepeating("CheckDisable", this._disableCheckInterval + (UnityEngine.Random.value * this._disableCheckInterval), this._disableCheckInterval);
        base.InvokeRepeating("CheckEnable", this._enableCheckInterval + (UnityEngine.Random.value * this._enableCheckInterval), this._enableCheckInterval);
        base.Invoke("DisableOnStart", 0.01f);
    }
}

