namespace HutongGames.PlayMaker
{
    using System;
    using UnityEngine;

    public class PlayMakerCanvasRaycastFilterProxy : MonoBehaviour, ICanvasRaycastFilter
    {
        public bool RayCastingEnabled = true;

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return this.RayCastingEnabled;
        }
    }
}

