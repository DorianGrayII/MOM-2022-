using UnityEngine;

public class ExampleWheelController : MonoBehaviour
{
    private static class Uniforms
    {
        internal static readonly int _MotionAmount = Shader.PropertyToID("_MotionAmount");
    }

    public float acceleration;

    public Renderer motionVectorRenderer;

    private Rigidbody m_Rigidbody;

    private void Start()
    {
        this.m_Rigidbody = base.GetComponent<Rigidbody>();
        this.m_Rigidbody.maxAngularVelocity = 100f;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.m_Rigidbody.AddRelativeTorque(new Vector3(-1f * this.acceleration, 0f, 0f), ForceMode.Acceleration);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.m_Rigidbody.AddRelativeTorque(new Vector3(1f * this.acceleration, 0f, 0f), ForceMode.Acceleration);
        }
        float value = (0f - this.m_Rigidbody.angularVelocity.x) / 100f;
        if ((bool)this.motionVectorRenderer)
        {
            this.motionVectorRenderer.material.SetFloat(Uniforms._MotionAmount, Mathf.Clamp(value, -0.25f, 0.25f));
        }
    }
}
