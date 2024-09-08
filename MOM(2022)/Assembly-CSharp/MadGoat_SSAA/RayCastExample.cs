namespace MadGoat_SSAA
{
    using System;
    using UnityEngine;

    public class RayCastExample : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f))
                {
                    Debug.Log("Offsetted Hit Info: " + hit.point.ToString());
                }
                if (Physics.Raycast(Camera.main.GetComponent<MadGoatSSAA>().ScreenPointToRay(Input.mousePosition), out hit, 1000f))
                {
                    Debug.Log("Correct Hit Info: " + hit.point.ToString());
                }
            }
        }
    }
}

