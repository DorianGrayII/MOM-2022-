using UnityEngine;

namespace MadGoat_SSAA
{
    public class rotate : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            base.transform.Rotate(new Vector3(0f, this.speed * Time.deltaTime, 0f));
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(20f, Screen.height - 70, 100f, 100f), "Camera speed");
            this.speed = 0f - GUI.HorizontalSlider(new Rect(20f, Screen.height - 50, 100f, 20f), 0f - this.speed, 0f, 50f);
        }
    }
}
