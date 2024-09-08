using UnityEngine;

namespace MadGoat_SSAA
{
    public class RayCastExample : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo, 1000f))
                {
                    Debug.Log("Offsetted Hit Info: " + hitInfo.point.ToString());
                }
                if (Physics.Raycast(Camera.main.GetComponent<MadGoatSSAA>().ScreenPointToRay(Input.mousePosition), out hitInfo, 1000f))
                {
                    Debug.Log("Correct Hit Info: " + hitInfo.point.ToString());
                }
            }
        }
    }
}
