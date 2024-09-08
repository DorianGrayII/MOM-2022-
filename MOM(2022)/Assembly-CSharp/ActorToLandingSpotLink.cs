using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorToLandingSpotLink : MonoBehaviour
{
    public FlockScare flockScareScript;

    private void OnTriggerEnter(Collider c)
    {
        if (c != null)
        {
            LandingSpotController component = c.gameObject.GetComponent<LandingSpotController>();
            if (component == null)
            {
                Debug.LogError("[ERROR]Missing Landing Spot controller");
            }
            else
            {
                List<LandingSpotController> landingSpotControllers = this.flockScareScript.landingSpotControllers;
                if (landingSpotControllers == null)
                {
                    this.flockScareScript.landingSpotControllers = new List<LandingSpotController>();
                    landingSpotControllers = this.flockScareScript.landingSpotControllers;
                }
                if (!landingSpotControllers.Contains(component))
                {
                    landingSpotControllers.Add(component);
                }
            }
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c != null)
        {
            LandingSpotController component = c.gameObject.GetComponent<LandingSpotController>();
            if (component == null)
            {
                Debug.LogError("[ERROR]Missing Landing Spot controller");
            }
            else
            {
                List<LandingSpotController> landingSpotControllers = this.flockScareScript.landingSpotControllers;
                if ((landingSpotControllers != null) && landingSpotControllers.Contains(component))
                {
                    if (landingSpotControllers.Count == 1)
                    {
                        this.flockScareScript.landingSpotControllers = null;
                    }
                    else
                    {
                        landingSpotControllers.Remove(component);
                    }
                }
            }
        }
    }
}

