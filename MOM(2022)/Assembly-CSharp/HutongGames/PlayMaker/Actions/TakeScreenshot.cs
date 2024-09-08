using System;
using System.IO;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Application)]
    [Tooltip("Saves a Screenshot. NOTE: Does nothing in Web Player. On Android, the resulting screenshot is available some time later.")]
    public class TakeScreenshot : FsmStateAction
    {
        public enum Destination
        {
            MyPictures = 0,
            PersistentDataPath = 1,
            CustomPath = 2
        }

        [Tooltip("Where to save the screenshot.")]
        public Destination destination;

        [Tooltip("Path used with Custom Path Destination option.")]
        public FsmString customPath;

        [RequiredField]
        public FsmString filename;

        [Tooltip("Add an auto-incremented number to the filename.")]
        public FsmBool autoNumber;

        [Tooltip("Factor by which to increase resolution.")]
        public FsmInt superSize;

        [Tooltip("Log saved file info in Unity console.")]
        public FsmBool debugLog;

        private int screenshotCount;

        public override void Reset()
        {
            this.destination = Destination.MyPictures;
            this.filename = "";
            this.autoNumber = null;
            this.superSize = null;
            this.debugLog = null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(this.filename.Value))
            {
                return;
            }
            string text;
            switch (this.destination)
            {
            case Destination.MyPictures:
                text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                break;
            case Destination.PersistentDataPath:
                text = Application.persistentDataPath;
                break;
            case Destination.CustomPath:
                text = this.customPath.Value;
                break;
            default:
                text = "";
                break;
            }
            text = text.Replace("\\", "/") + "/";
            string text2 = text + this.filename.Value + ".png";
            if (this.autoNumber.Value)
            {
                while (File.Exists(text2))
                {
                    this.screenshotCount++;
                    text2 = text + this.filename.Value + this.screenshotCount + ".png";
                }
            }
            if (this.debugLog.Value)
            {
                Debug.Log("TakeScreenshot: " + text2);
            }
            ScreenCapture.CaptureScreenshot(text2, this.superSize.Value);
            base.Finish();
        }
    }
}
