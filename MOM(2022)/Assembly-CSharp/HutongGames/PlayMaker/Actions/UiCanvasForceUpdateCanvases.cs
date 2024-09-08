﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("Force all canvases to update their content.\nCode that relies on up-to-date layout or content can call this method to ensure it before executing code that relies on it.")]
    public class UiCanvasForceUpdateCanvases : FsmStateAction
    {
        public override void OnEnter()
        {
            Canvas.ForceUpdateCanvases();
            base.Finish();
        }
    }
}

