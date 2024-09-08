using UnityEngine;

namespace MadGoat_SSAA
{
    public class DemoGUI : MonoBehaviour
    {
        private MadGoatSSAA ssaa;

        private bool mode;

        private bool ultra;

        private float multiplier = 100f;

        private GUIStyle s;

        private float deltaTime;

        private void Start()
        {
            this.ssaa = base.GetComponent<MadGoatSSAA>();
        }

        private void Update()
        {
            this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            GUI.contentColor = new Color(0f, 0f, 0f);
            float num = this.deltaTime * 1000f;
            float num2 = 1f / this.deltaTime;
            string text = $"{num:0.0} ms ({num2:0.} fps)";
            GUI.Label(new Rect(Screen.width - 150, 10f, 150f, 50f), text);
            if (GUI.Button(new Rect(20f, 10f, 120f, 20f), (!this.mode) ? "Switch to scaling" : "Switch to ssaa"))
            {
                this.mode = !this.mode;
            }
            if (this.mode)
            {
                GUI.Label(new Rect(20f, 50f, 100f, 20f), (int)this.multiplier + "%");
                this.multiplier = GUI.HorizontalSlider(new Rect(55f, 55f, 100f, 20f), this.multiplier, 50f, 200f);
                if (GUI.Button(new Rect(20f, 70f, 80f, 20f), "Apply"))
                {
                    this.ssaa.SetAsScale((int)this.multiplier, Filter.BICUBIC, 0.8f, 0.7f);
                }
            }
            else
            {
                if (GUI.Button(new Rect(20f, 50f, 80f, 20f), "off"))
                {
                    this.ssaa.SetAsSSAA(SSAAMode.SSAA_OFF);
                }
                if (GUI.Button(new Rect(20f, 75f, 80f, 20f), "x0.5"))
                {
                    this.ssaa.SetAsSSAA(SSAAMode.SSAA_HALF);
                }
                if (GUI.Button(new Rect(20f, 100f, 80f, 20f), "x2"))
                {
                    this.ssaa.SetAsSSAA(SSAAMode.SSAA_X2);
                }
                if (GUI.Button(new Rect(20f, 125f, 80f, 20f), "x4"))
                {
                    this.ssaa.SetAsSSAA(SSAAMode.SSAA_X4);
                }
            }
            GUI.contentColor = new Color(1f, 1f, 1f);
            this.ultra = GUI.Toggle(new Rect(20f, 150f, 150f, 20f), this.ultra, "Ultra Quality (FSSAA)");
            this.ssaa.SetUltra(this.ultra);
        }
    }
}
