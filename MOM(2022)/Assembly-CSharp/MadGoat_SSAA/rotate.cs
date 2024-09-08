namespace MadGoat_SSAA
{
    using System;
    using UnityEngine;

    public class rotate : MonoBehaviour
    {
        public float speed;

        private void OnGUI()
        {
            GUI.Label(new Rect(20f, (float) (Screen.height - 70), 100f, 100f), "Camera speed");
            this.speed = -GUI.HorizontalSlider(new Rect(20f, (float) (Screen.height - 50), 100f, 20f), -this.speed, 0f, 50f);
        }

        private void Update()
        {
            base.transform.Rotate(new Vector3(0f, this.speed * Time.deltaTime, 0f));
        }
    }
}

