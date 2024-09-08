namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.IO;
    using UnityEngine;

    [ActionCategory(ActionCategory.Application), HutongGames.PlayMaker.Tooltip("Saves a Screenshot. NOTE: Does nothing in Web Player. On Android, the resulting screenshot is available some time later.")]
    public class TakeScreenshot : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Where to save the screenshot.")]
        public Destination destination;
        [HutongGames.PlayMaker.Tooltip("Path used with Custom Path Destination option.")]
        public FsmString customPath;
        [RequiredField]
        public FsmString filename;
        [HutongGames.PlayMaker.Tooltip("Add an auto-incremented number to the filename.")]
        public FsmBool autoNumber;
        [HutongGames.PlayMaker.Tooltip("Factor by which to increase resolution.")]
        public FsmInt superSize;
        [HutongGames.PlayMaker.Tooltip("Log saved file info in Unity console.")]
        public FsmBool debugLog;
        private int screenshotCount;

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(this.filename.Value))
            {
                string folderPath;
                switch (this.destination)
                {
                    case Destination.MyPictures:
                        folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        break;

                    case Destination.PersistentDataPath:
                        folderPath = Application.persistentDataPath;
                        break;

                    case Destination.CustomPath:
                        folderPath = this.customPath.Value;
                        break;

                    default:
                        folderPath = "";
                        break;
                }
                folderPath = folderPath.Replace(@"\", "/") + "/";
                string path = folderPath + this.filename.Value + ".png";
                if (this.autoNumber.Value)
                {
                    while (File.Exists(path))
                    {
                        this.screenshotCount++;
                        path = folderPath + this.filename.Value + this.screenshotCount.ToString() + ".png";
                    }
                }
                if (this.debugLog.Value)
                {
                    Debug.Log("TakeScreenshot: " + path);
                }
                ScreenCapture.CaptureScreenshot(path, this.superSize.Value);
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.destination = Destination.MyPictures;
            this.filename = "";
            this.autoNumber = null;
            this.superSize = null;
            this.debugLog = null;
        }

        public enum Destination
        {
            MyPictures,
            PersistentDataPath,
            CustomPath
        }
    }
}

