using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera lookAtCamera;

    public bool lookOnlyOnAwake;

    public void Start()
    {
        if (this.lookAtCamera == null)
        {
            this.lookAtCamera = Camera.main;
        }
        if (this.lookOnlyOnAwake)
        {
            this.LookCam();
        }
    }

    public void Update()
    {
        if (!this.lookOnlyOnAwake)
        {
            this.LookCam();
        }
    }

    public void LookCam()
    {
        base.transform.LookAt(this.lookAtCamera.transform);
    }
}
