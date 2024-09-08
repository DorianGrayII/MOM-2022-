using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingSpotController : MonoBehaviour
{
    public bool _randomRotate = true;

    public Vector2 _autoCatchDelay = new Vector2(10f, 20f);

    public Vector2 _autoDismountDelay = new Vector2(10f, 20f);

    public float _maxBirdDistance = 20f;

    public float _minBirdDistance = 5f;

    public bool _takeClosest;

    public FlockController _flock;

    public bool _landOnStart;

    public bool _soarLand = true;

    public bool _onlyBirdsAbove;

    public float _landingSpeedModifier = 0.5f;

    public float _landingTurnSpeedModifier = 5f;

    public Transform _featherPS;

    public Transform _thisT;

    public int _activeLandingSpots;

    public float _snapLandDistance = 0.1f;

    public float _landedRotateSpeed = 0.01f;

    public float _gizmoSize = 0.2f;

    [HideInInspector]
    public List<LandingSpot> landingSpots = new List<LandingSpot>();

    public void Start()
    {
        if (this._thisT == null)
        {
            this._thisT = base.transform;
        }
        if (this._flock == null)
        {
            this._flock = (FlockController)Object.FindObjectOfType(typeof(FlockController));
            Debug.Log(this?.ToString() + " has no assigned FlockController, a random FlockController has been assigned");
        }
        if (this._landOnStart)
        {
            base.StartCoroutine(this.InstantLandOnStart(0.1f));
        }
    }

    public void ScareAll()
    {
        this.ScareAll(0f, 1f);
    }

    public void ScareAll(float minDelay, float maxDelay)
    {
        for (int i = 0; i < this.landingSpots.Count; i++)
        {
            if (this.landingSpots[i].GetComponent<LandingSpot>() != null)
            {
                this.landingSpots[i].GetComponent<LandingSpot>().Invoke("ReleaseFlockChild", Random.Range(minDelay, maxDelay));
            }
        }
    }

    public void LandAll()
    {
        for (int i = 0; i < this.landingSpots.Count; i++)
        {
            if (this.landingSpots[i].GetComponent<LandingSpot>() != null)
            {
                LandingSpot component = this.landingSpots[i].GetComponent<LandingSpot>();
                base.StartCoroutine(component.GetFlockChild(0f, 2f));
            }
        }
    }

    public IEnumerator InstantLandOnStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < this.landingSpots.Count; i++)
        {
            if (this.landingSpots[i].GetComponent<LandingSpot>() != null)
            {
                this.landingSpots[i].GetComponent<LandingSpot>().InstantLand();
            }
        }
    }

    public IEnumerator InstantLand(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < this._thisT.childCount; i++)
        {
            if (this._thisT.GetChild(i).GetComponent<LandingSpot>() != null)
            {
                this._thisT.GetChild(i).GetComponent<LandingSpot>().InstantLand();
            }
        }
    }
}
