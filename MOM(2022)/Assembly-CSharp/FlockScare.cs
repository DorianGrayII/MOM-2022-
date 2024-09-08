using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockScare : MonoBehaviour
{
    public List<LandingSpotController> landingSpotControllers;

    public float scareInterval = 1f;

    public float distanceToScare = 2f;

    public int checkEveryNthLandingSpot = 1;

    public int InvokeAmounts = 1;

    private int ls;

    private LandingSpotController currentController;

    private void Awake()
    {
        this.landingSpotControllers = null;
    }

    private void OnEnable()
    {
        base.StartCoroutine(this.CheckProximityToLandingSpots());
    }

    private IEnumerator CheckProximityToLandingSpots()
    {
        while (true)
        {
            if (this.landingSpotControllers != null)
            {
                foreach (LandingSpotController landingSpotController in this.landingSpotControllers)
                {
                    if (landingSpotController._activeLandingSpots > 0 && this.CheckDistanceToLandingSpot(landingSpotController))
                    {
                        landingSpotController.ScareAll();
                    }
                }
            }
            yield return new WaitForSeconds(this.scareInterval);
        }
    }

    private bool CheckDistanceToLandingSpot(LandingSpotController lc)
    {
        _ = lc.transform;
        if (lc.landingSpots.Count == 0)
        {
            return false;
        }
        this.ls = this.ls++ % lc.landingSpots.Count;
        Transform transform = lc.landingSpots[this.ls].transform;
        if (transform.GetComponent<LandingSpot>().landingChild != null && (transform.position - base.transform.position).sqrMagnitude < this.distanceToScare * this.distanceToScare)
        {
            return true;
        }
        return false;
    }
}
