// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AutoDestroyPS
using UnityEngine;

public class AutoDestroyPS : MonoBehaviour
{
    private float timeLeft;

    private void Awake()
    {
        ParticleSystem.MainModule main = base.GetComponent<ParticleSystem>().main;
        this.timeLeft = main.startLifetimeMultiplier + main.duration;
        Object.Destroy(base.gameObject, this.timeLeft);
    }
}
